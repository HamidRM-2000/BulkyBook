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
   public class CoverTypesController : Controller
   {
      private readonly IUnitOfWork _context;

      public CoverTypesController(IUnitOfWork context)
      {
         _context = context;
      }

      // GET: Admin/CoverTypes
      public async Task<IActionResult> Index()
      {
         Product p = new Product();
         return View(await _context.CoverTypes.GetAll());
      }

      // GET: Admin/Categories/Create
      public IActionResult Create()
      {
         ViewData["title"] = "Create New CoverType";
         CoverType cover = new CoverType();
         return View("Upsert", cover);
      }

      // POST: Admin/CoverTypes/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("CovId,Name")] CoverType coverType)
      {
         if (ModelState.IsValid)
         {
            await _context.CoverTypes.Add(coverType);
            await _context.Save();
            return RedirectToAction(nameof(Index));
         }
         return View("Upsert",coverType);
      }

      // GET: Admin/CoverTypes/Edit/5
      public async Task<IActionResult> Edit(int id = 0)
      {
         if (id == 0)
         {
            return NotFound();
         }

         var coverType = await _context.CoverTypes.GetByID(id);
         if (coverType == null)
         {
            return NotFound();
         }
         return View("Upsert",coverType);
      }

      // POST: Admin/CoverTypes/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("CovId,Name")] CoverType coverType)
      {
         if (id != coverType.CovId)
         {
            return NotFound();
         }

         if (ModelState.IsValid)
         {
            try
            {
               _context.CoverTypes.Update(coverType);
               await _context.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (!CoverTypeExists(coverType.CovId))
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
         return View("Upsert",coverType);
      }

      private bool CoverTypeExists(int id)
      {
         var cover = _context.CoverTypes.GetFirstOrDefault(e => e.CovId == id);
         return cover != null;
      }

      #region API CALLS
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
         var allCover = await _context.CoverTypes.GetAll();
         return Json(new { data = allCover });
      }

      [HttpDelete]
      public async Task<IActionResult> Delete(int id)
      {
         var Obj = await _context.CoverTypes.GetByID(id);
         if (Obj == null)
            return Json(new { success = false, message = "Something went wrong" });
         else
         {
            _context.CoverTypes.Remove(id);
            await _context.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
         }
      }
      #endregion

   }
}
