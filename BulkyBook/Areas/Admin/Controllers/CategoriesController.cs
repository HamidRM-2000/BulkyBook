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
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utilities;

namespace BulkyBook.Areas.Admin.Controllers
{
   [Area("Admin")]
   [Authorize(Roles = SD.Role_Admin)]
   public class CategoriesController : Controller
   {
      private readonly IUnitOfWork _context;

      public CategoriesController(IUnitOfWork context)
      {
         _context = context;
      }

      // GET: Admin/Categories
      public async Task<IActionResult> Index()
      {
         return View(await _context.Categories.GetAll());
      }

      // GET: Admin/Categories/Create
      public IActionResult Create()
      {
         ViewData["title"] = "Create New Category";
         Category category = new Category();
         return View("Upsert",category);
      }

      // POST: Admin/Categories/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("CatId,CatName")] Category category)
      {
         if (ModelState.IsValid)
         {
            await _context.Categories.Add(category);
            await _context.Save();
            return RedirectToAction(nameof(Index));
         }
         return View(category);
      }

      // GET: Admin/Categories/Edit/5
      public async Task<IActionResult> Edit(int id=0)
      {
         if (id == 0)
         {
            return NotFound();
         }

         var category = await _context.Categories.GetByID(id);
         if (category == null)
         {
            return NotFound();
         }
         ViewData["title"] = "Edit Category";
         return View("Upsert",category);
      }

      // POST: Admin/Categories/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("CatId,CatName")] Category category)
      {
         if (id != category.CatId)
         {
            return NotFound();
         }

         if (ModelState.IsValid)
         {
            try
            {
               _context.Categories.Update(category);
               await _context.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (_context.Categories.GetByID(category.CatId) == null)
               {
                  return NotFound();
               }
               else
               {
                  throw;
               }
            }
            return RedirectToAction(nameof(Index));
         }
            return RedirectToAction(nameof(Index));
      }


      private async Task<bool> CategoryExists(int id)
      {
         var Cat=await _context.Categories.GetFirstOrDefault(e => e.CatId == id);
         return Cat != null;
      }

      #region API CALLS
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
         var allCats = await _context.Categories.GetAll();
         return Json(new { data = allCats });
      }
      [HttpDelete]
      public async Task<IActionResult> Delete(int id)
      {
         var Obj = await _context.Categories.GetByID(id);
         if (Obj == null)
            return Json(new { success = false, message = "Something went wrong" });
         else
         {
            _context.Categories.Remove(id);
            await _context.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
         }
      }
      #endregion
   }
}
