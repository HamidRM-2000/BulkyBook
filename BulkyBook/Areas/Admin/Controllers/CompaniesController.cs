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
using BulkyBook.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace BulkyBook.Areas.Admin.Controllers
{
   [Area("Admin")]
   [Authorize(Roles = SD.Role_Admin +","+SD.Role_Employee)]
   public class CompaniesController : Controller
   {
      private readonly IUnitOfWork _context;

      public CompaniesController(IUnitOfWork context)
      {
         _context = context;
      }

      // GET: Admin/Companies
      public async Task<IActionResult> Index()
      {
         return View(await _context.Companies.GetAll());
      }

      // GET: Admin/Companies/Create
      public IActionResult Create()
      {
         ViewData["title"] = "Create New company";
         Company company = new Company();
         return View("Upsert",company);
      }

      // POST: Admin/Companies/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("ComId,Name,StreetAddress,City,State,PostalCard,PhoneNumber,IsAuthorizedCompany")] Company company)
      {
         if (ModelState.IsValid)
         {
            await _context.Companies.Add(company);
            await _context.Save();
            return RedirectToAction(nameof(Index));
         }
         return View(company);
      }

      // GET: Admin/Companies/Edit/5
      public async Task<IActionResult> Edit(int id=0)
      {
         if (id == 0)
         {
            return NotFound();
         }

         var company = await _context.Companies.GetByID(id);
         if (company == null)
         {
            return NotFound();
         }
         ViewData["title"] = "Edit company";
         return View("Upsert",company);
      }

      // POST: Admin/Companies/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("ComId,Name,StreetAddress,City,State,PostalCard,PhoneNumber,IsAuthorizedCompany")] Company company)
      {
         if (id != company.ComId)
         {
            return NotFound();
         }

         if (ModelState.IsValid)
         {
            try
            {
               _context.Companies.Update(company);
               await _context.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (_context.Companies.GetByID(company.ComId) == null)
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


      private async Task<bool> companyExists(int id)
      {
         var Cat = await _context.Companies.GetFirstOrDefault(e => e.ComId == id);
         return Cat != null;
      }

      #region API CALLS
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
         var allCats = await _context.Companies.GetAll();
         return Json(new { data = allCats });
      }
      [HttpDelete]
      public async Task<IActionResult> Delete(int id)
      {
         var Obj = await _context.Companies.GetByID(id);
         if (Obj == null)
            return Json(new { success = false, message = "Something went wrong" });
         else
         {
            _context.Companies.Remove(id);
            await _context.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
         }
      }
      #endregion
   }
}
