using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using BulkyBook.DataLayer.Models;
using BulkyBook.DataLayer.Services.UnitOfWork;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Areas.Main.Controllers
{
   [Area("Main")]
   [Authorize]
   public class CartController : Controller
   {
      private readonly IUnitOfWork _context;
      private readonly UserManager<IdentityUser> _userManager;

      public CartController(IUnitOfWork context, UserManager<IdentityUser> userManager)
      {
         _context = context;
         _userManager = userManager;
      }

      [BindProperty]
      public ShoppingCartVM ShoppingCartVM { get; set; }


      public async Task<IActionResult> Index()
      {
         var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
         ShoppingCartVM = new ShoppingCartVM()
         {
            OrderHeader = new OrderHeader()
            {
               MyUser = await _context.MyUsers.GetFirstOrDefault(i => i.Id == claim, includeProps: "Company")
            },
            ShoppingCarts = await _context.ShoppingCarts.GetAll(s => s.MyUserId == claim, includeProps: "Product")
         };
         foreach (var item in ShoppingCartVM.ShoppingCarts)
         {
            item.Price = SD.CalcPrice(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
            ShoppingCartVM.OrderHeader.OrderTotal += (item.Price * item.Count);
         };

         return View(ShoppingCartVM);
      }

      [HttpPost]
      public async Task<IActionResult> Plus(int id)
      {
         var shoppcart = await _context.ShoppingCarts.GetFirstOrDefault(s => s.Id == id, includeProps: "Product");

         shoppcart.Count++;

         await _context.Save();

         return RedirectToAction(nameof(Index));
      }

      [HttpPost]
      public async Task<IActionResult> Minus(int count, int id)
      {
         var shoppcart = await _context.ShoppingCarts.GetFirstOrDefault(s => s.Id == id, includeProps: "Product");
         if (count == 1)
            return RedirectToAction("Delete", new { id });
         shoppcart.Count--;

         await _context.Save();

         return RedirectToAction(nameof(Index));
      }

      public async Task<IActionResult> Delete(int id)
      {
         var shoppcart = await _context.ShoppingCarts.GetFirstOrDefault(s => s.Id == id);
         if (shoppcart != null)
         {
            _context.ShoppingCarts.Remove(id);
            await _context.Save();
         }
         return RedirectToAction(nameof(Index));

      }

      public async Task<IActionResult> Summary()
      {
         var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
         ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
         {
            OrderHeader = new OrderHeader()
            {
               MyUser = await _context.MyUsers.GetFirstOrDefault(i => i.Id == claim, includeProps: "Company")
            },
            ShoppingCarts = await _context.ShoppingCarts.GetAll(s => s.MyUserId == claim, includeProps: "Product")
         };
         foreach (var item in shoppingCartVM.ShoppingCarts)
         {
            item.Price = SD.CalcPrice(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
            shoppingCartVM.OrderHeader.OrderTotal += (item.Price * item.Count);
         };

         return View(shoppingCartVM);
      }

      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Summary(string stripeToken)
      {
         var claim = User.FindFirst(ClaimTypes.NameIdentifier).Value;
         ShoppingCartVM.OrderHeader.MyUser = await _context.MyUsers
                                                         .GetFirstOrDefault(c => c.Id == claim,
                                                                 includeProps: "Company");

         ShoppingCartVM.ShoppingCarts = await _context.ShoppingCarts
                                     .GetAll(c => c.MyUserId == claim,
                                     includeProps: "Product");

         ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
         ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
         ShoppingCartVM.OrderHeader.MyUserId = claim;
         ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

         await _context.OrderHeaders.Add(ShoppingCartVM.OrderHeader);
         await _context.Save();

         List<OrderDetail> orderDetailsList = new List<OrderDetail>();
         foreach (var item in ShoppingCartVM.ShoppingCarts)
         {
            item.Price = SD.CalcPrice(item.Count, item.Product.Price,
                item.Product.Price50, item.Product.Price100);
            OrderDetail orderDetails = new OrderDetail()
            {
               ProId = item.ProId,
               OrderId = ShoppingCartVM.OrderHeader.Id,
               Price = item.Price,
               Count = item.Count
            };
            ShoppingCartVM.OrderHeader.OrderTotal += orderDetails.Count * orderDetails.Price;
            await _context.OrderDetails.Add(orderDetails);

         }

         _context.ShoppingCarts.RemoveRange(ShoppingCartVM.ShoppingCarts);
         await _context.Save();
         HttpContext.Session.SetObj(SD.Shopping_Cart, 0);

         if (stripeToken == null)
         {
            //order will be created for delayed payment for authroized company
            ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
         }
         else
         {
            //process the payment
            //var options = new ChargeCreateOptions
            //{
            //   Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal * 100),
            //   Currency = "usd",
            //   Description = "Order ID : " + ShoppingCartVM.OrderHeader.Id,
            //   Source = stripeToken
            //};

            //var service = new ChargeService();
            //Charge charge = service.Create(options);

            //if (charge.BalanceTransactionId == null)
            //{
            //   ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            //}
            //else
            //{
            //   ShoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
            //}
            //if (charge.Status.ToLower() == "succeeded")
            //{
            //   ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
            //   ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            //   ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
            //}
         }
         _context.OrderHeaders.Update(ShoppingCartVM.OrderHeader);
         await _context.Save();

         return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });

      }
      public IActionResult OrderConfirmation(int id)
      {
         ViewData["Id"] = id;
         return View();
      }
   }
}
