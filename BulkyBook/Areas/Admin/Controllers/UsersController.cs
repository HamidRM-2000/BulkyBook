using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BulkyBook.DataLayer.Context;
using BulkyBook.DataLayer.Models;
using BulkyBook.DataLayer.Services.UnitOfWork;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utilities;

namespace BulkyBook.Areas.Admin.Controllers
{
   [Area("Admin")]
   [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
   public class UsersController : Controller
   {
      private readonly IUnitOfWork _context;

      public UsersController(IUnitOfWork context)
      {
         _context = context;
      }

      // GET: Admin/Users
      public IActionResult Index()
      {
         return View();
      }

      // GET: Admin/Users/Add
      public IActionResult Add()
      {
         return Redirect("/identity/account/Register");
      }

      #region API CALLS
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
         var allUsers = await _context.MyUsers.GetAll(includeProps:"Company");

         return Json(new { data = allUsers });
      }

      [HttpPost]
      public async Task<IActionResult> LockUnlock([FromBody] string id)
      {
         var objFromDb =await _context.MyUsers.GetFirstOrDefault(u => u.Id == id);
         if (objFromDb == null)
         {
            return Json(new { success = false, message = "Error while Locking/Unlocking" });
         }
         if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
         {
            //user is currently locked, we will unlock them
            objFromDb.LockoutEnd = DateTime.Now;
         }
         else
         {
            objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
         }
         await _context.Save();
         return Json(new { success = true, message = "Operation Successful." });
      }
      #endregion
   }
}
