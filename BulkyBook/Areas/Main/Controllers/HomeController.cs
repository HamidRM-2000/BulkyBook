using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BulkyBook.DataLayer.Models;
using BulkyBook.DataLayer.Services.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Identity;

namespace BulkyBook.Areas.Main.Controllers
{
   [Area("Main")]
   public class HomeController : Controller
   {
      private readonly ILogger<HomeController> _logger;
      private readonly UserManager<IdentityUser> _userManager;
      private readonly IUnitOfWork _context;
      public HomeController(ILogger<HomeController> logger, IUnitOfWork context, UserManager<IdentityUser> userManager)
      {
         _logger = logger;
         _context = context;
         _userManager = userManager;
      }

      public async Task<IActionResult> Index()
      {
         var claim = User.FindFirst(ClaimTypes.NameIdentifier);
         if (claim != null)
         {
            var count = await _context.ShoppingCarts.GetAll(s => s.MyUserId == claim.Value);
            HttpContext.Session.SetObj(SD.Shopping_Cart, count.Count());
         }
         var Products = await _context.Products.GetAll();
         return View(Products.Skip(1));
      }

      // GET:Admin/Products/Details/5
      public async Task<IActionResult> Details(int id = 0)
      {
         if (id == 0)
         {
            return NotFound();
         }
         var claim = User;
         var product = await _context.Products
             .GetFirstOrDefault(m => m.ProId == id, "CoverType,Category");


         if (product == null)
         {
            return NotFound();
         }
         var shoppingCart = await _context.ShoppingCarts
            .GetFirstOrDefault(s => s.ProId == product.ProId, "Product") ?? new ShoppingCart() { ProId = product.ProId, Product = product };

         return View(shoppingCart);
      }

      [HttpPost]
      [Authorize]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Details(ShoppingCart shoppingCart)
      {
         var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
         
         if (ModelState.IsValid)
         {
            var _shoppingCart = await _context.ShoppingCarts.GetFirstOrDefault(s => s.MyUser.Id == claim && s.ProId == shoppingCart.ProId);
            if (_shoppingCart == null)
            {
               _shoppingCart = new ShoppingCart()
               {
                  MyUserId = claim,
                  Count = shoppingCart.Count,
                  ProId = shoppingCart.ProId
               };
               await _context.ShoppingCarts.Add(_shoppingCart);
            }
            else
            {
               _shoppingCart.Count = shoppingCart.Count;
               _context.ShoppingCarts.Update(_shoppingCart);
            }
            await _context.Save();
            return RedirectToAction(nameof(Index));
         }

         var product = await _context.Products.GetByID(shoppingCart.ProId);

         shoppingCart = await _context.ShoppingCarts
            .GetFirstOrDefault(s => s.MyUser.Id == claim && s.ProId == shoppingCart.ProId, "Product") ?? new ShoppingCart() { ProId = shoppingCart.ProId, Product = product };


         return View(shoppingCart);
      }

      public IActionResult Privacy()
      {
         return View();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }

   }
}
