using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BulkyBook.DataLayer.Models;
using BulkyBook.DataLayer.Services.UnitOfWork;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBook.Areas.Admin.Controllers
{
   [Area("Admin")]
   [Authorize]
   public class OrderController : Controller
   {
      private readonly IUnitOfWork _context;

      [BindProperty]
      public OrderDetailVM OrderVM { get; set; }

      public OrderController(IUnitOfWork context)
      {
         _context = context;
      }

      public IActionResult Index()
      {
         return View();
      }

      public async Task<IActionResult> Details(int id)
      {
         OrderVM = new OrderDetailVM()
         {
            OrderHeader = await _context.OrderHeaders.GetFirstOrDefault(u => u.Id == id,
                                             includeProps: "MyUser"),
            OrderDetails = await _context.OrderDetails.GetAll(o => o.OrderId == id, includeProps: "Product")

         };
         return View(OrderVM);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      [ActionName("Details")]
      public async Task<IActionResult> Details(string stripeToken)
      {
         OrderHeader orderHeader = await _context.OrderHeaders.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id,
                                             includeProps: "MyUser");
         //if (stripeToken != null)
         //{
         //   ////process the payment
         //   //var options = new ChargeCreateOptions
         //   //{
         //   //   Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
         //   //   Currency = "usd",
         //   //   Description = "Order ID : " + orderHeader.Id,
         //   //   Source = stripeToken
         //   //};

         //   //var service = new ChargeService();
         //   //Charge charge = service.Create(options);

         //   //if (charge.Id == null)
         //   //{
         //   //   orderHeader.PaymentStatus = SD.PaymentStatusRejected;
         //   //}
         //   else
         //   {
         //      orderHeader.TransactionId = charge.Id;
         //   }
         //   if (charge.Status.ToLower() == "succeeded")
         //   {
         //      orderHeader.PaymentStatus = SD.PaymentStatusApproved;

         //      orderHeader.PaymentDate = DateTime.Now;
         //   }

         await _context.Save();

         //}
         return RedirectToAction("Details", "Order", new { id = orderHeader.Id });
      }


      [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
      public async Task<IActionResult> StartProcessing(int id)
      {
         OrderHeader orderHeader = await _context.OrderHeaders.GetFirstOrDefault(u => u.Id == id);
         orderHeader.OrderStatus = SD.StatusInProcess;
         await _context.Save();
         return RedirectToAction("Index");
      }

      [HttpPost]
      [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
      public async Task<IActionResult> ShipOrderAsync()
      {
         OrderHeader orderHeader = await _context.OrderHeaders.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
         orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
         orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
         orderHeader.OrderStatus = SD.StatusShipped;
         orderHeader.ShippingDate = DateTime.Now;

         await _context.Save();
         return RedirectToAction("Index");
      }

      [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
      public async Task<IActionResult> CancelOrderAsync(int id)
      {
         OrderHeader orderHeader = await _context.OrderHeaders.GetFirstOrDefault(u => u.Id == id);
         if (orderHeader.PaymentStatus == SD.StatusApproved)
         {
            //var options = new RefundCreateOptions
            //{
            //   Amount = Convert.ToInt32(orderHeader.OrderTotal * 100),
            //   Reason = RefundReasons.RequestedByCustomer,
            //   Charge = orderHeader.TransactionId

            //};
            //var service = new RefundService();
            //Refund refund = service.Create(options);

            orderHeader.OrderStatus = SD.StatusRefunded;
            orderHeader.PaymentStatus = SD.StatusRefunded;
         }
         else
         {
            orderHeader.OrderStatus = SD.StatusCancelled;
            orderHeader.PaymentStatus = SD.StatusCancelled;
         }

         await _context.Save();
         return RedirectToAction("Index");
      }

      public async Task<IActionResult> UpdateOrderDetailsAsync()
      {
         var orderHEaderFromDb = await _context.OrderHeaders.GetFirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
         orderHEaderFromDb.Name = OrderVM.OrderHeader.Name;
         orderHEaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
         orderHEaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
         orderHEaderFromDb.City = OrderVM.OrderHeader.City;
         orderHEaderFromDb.State = OrderVM.OrderHeader.State;
         orderHEaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
         if (OrderVM.OrderHeader.Carrier != null)
         {
            orderHEaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
         }
         if (OrderVM.OrderHeader.TrackingNumber != null)
         {
            orderHEaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
         }

         await _context.Save();
         TempData["Error"] = "Order Details Updated Successfully.";
         return RedirectToAction("Details", "Order", new { id = orderHEaderFromDb.Id });
      }


      #region API CALLS
      [HttpGet]
      public async Task<IActionResult> GetOrderListAsync(string status)
      {
         var claim = User.FindFirst(ClaimTypes.NameIdentifier);

         IEnumerable<OrderHeader> orderHeaderList;

         if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
         {
            orderHeaderList = await _context.OrderHeaders.GetAll(includeProps: "MyUser");
         }
         else
         {
            orderHeaderList = await _context.OrderHeaders.GetAll(
                                    u => u.MyUserId == claim.Value,
                                    includeProps: "MyUser");
         }

         switch (status)
         {
            case "pending":
               orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment);
               break;
            case "inprocess":
               orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusApproved ||
                                                       o.OrderStatus == SD.StatusInProcess ||
                                                       o.OrderStatus == SD.StatusPending);
               break;
            case "completed":
               orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusShipped);
               break;
            case "rejected":
               orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusCancelled ||
                                                       o.OrderStatus == SD.StatusRefunded ||
                                                       o.OrderStatus == SD.PaymentStatusRejected);
               break;
            default:
               break;
         }

         return Json(new { data = orderHeaderList });
      }
      #endregion

   }
}
