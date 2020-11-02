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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BulkyBook.Areas.Admin.Controllers
{
   [Area("Admin")]
   [Authorize(Roles = SD.Role_Admin)]
   public class ProductsController : Controller
   {
      private readonly IUnitOfWork _context;
      private readonly IWebHostEnvironment _hostEnvironment;

      public ProductsController(IUnitOfWork context, IWebHostEnvironment hostEnvironment)
      {
         _context = context;
         _hostEnvironment = hostEnvironment;
      }

      // GET: Admin/Products
      public async Task<IActionResult> Index()
      {
         return View(await _context.Products.GetAll());
      }

      // GET: Admin/Categories/Create
      public async Task<IActionResult> Create()
      {
         ViewData["title"] = "Create New Product";
         ProductVM pro = new ProductVM(new Product());
         pro.FillCategory(await _context.Categories.GetAll());
         pro.FillCoverType(await _context.CoverTypes.GetAll());
         return View("Upsert", pro);
      }

      // POST: Admin/Products/Create
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create([Bind("ProId,Title,Author,Description,ISBN,PriceList,Price,Price50,Price100,CovId,CatId")] Product product)
      {
         var a = product;
         if (ModelState.IsValid)
         {
            var img = HttpContext.Request.Form.Files[0];
            string webRootPath = _hostEnvironment.WebRootPath;
            if (img != null)
            {
               string fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
               var route = Path.Combine(webRootPath, "Images\\Products");
               using (var filesStreams = new FileStream(Path.Combine(route, fileName), FileMode.Create))
                  img.CopyTo(filesStreams);
               product.Image = fileName;
            }
            await _context.Products.Add(product);
            await _context.Save();
            return RedirectToAction(nameof(Index));
         }
         ProductVM productVM = new ProductVM(product);
         productVM.FillCategory(await _context.Categories.GetAll());
         productVM.FillCoverType(await _context.CoverTypes.GetAll());
         return View("Upsert", productVM);
      }

      // GET: Admin/Products/Edit/
      public async Task<IActionResult> Edit(int id = 0)
      {
         if (id == 0)
         {
            return NotFound();
         }

         var product = await _context.Products.GetByID(id);
         if (product == null)
         {
            return NotFound();
         }
         ProductVM productVM = new ProductVM(product);
         productVM.FillCategory(await _context.Categories.GetAll());
         productVM.FillCoverType(await _context.CoverTypes.GetAll());
         ViewData["title"] = $"Edit Product";
         return View("Upsert", productVM);
      }

      // POST: Admin/Products/Edit/5
      // To protect from overposting attacks, enable the specific properties you want to bind to, for 
      // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, [Bind("ProId,Title,Author,Description,ISBN,Image,PriceList,Price,Price50,Price100,CovId,CatId")] Product product)
      {
         if (id != product.ProId)
         {
            return NotFound();
         }

         var Image = HttpContext.Request.Form.Files;
         string webRootPath = _hostEnvironment.WebRootPath;

         if (ModelState.IsValid)
         {
            try
            {
               if (Image.Count > 0)
               {

                  var imagePath = Path.Combine(webRootPath, product.Image.TrimStart('\\'));
                  if (System.IO.File.Exists(imagePath))
                  {
                     System.IO.File.Delete(imagePath);
                  }

                  string fileName = Guid.NewGuid().ToString() + Path.GetExtension(Image[0].FileName);
                  var route = Path.Combine(webRootPath, "Images\\Products");
                  using (var filesStreams = new FileStream(Path.Combine(route, fileName), FileMode.Create))
                     Image[0].CopyTo(filesStreams);
                  product.Image = fileName;

               }
               _context.Products.Update(product);
               await _context.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
               if (!ProductExists(product.ProId))
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
         ProductVM productVM = new ProductVM(product);
         productVM.FillCategory(await _context.Categories.GetAll());
         productVM.FillCoverType(await _context.CoverTypes.GetAll());
         return View("Upsert", productVM);
      }

      private bool ProductExists(int id)
      {
         var cover = _context.Products.GetFirstOrDefault(e => e.ProId == id);
         return cover != null;
      }

      #region API CALLS
      [HttpGet]
      public async Task<IActionResult> GetAll()
      {
         var allCover = await _context.Products.GetAll();
         return Json(new { data = allCover });
      }

      [HttpDelete]
      public async Task<IActionResult> Delete(int id)
      {
         var Obj = await _context.Products.GetByID(id);
         if (Obj == null)
            return Json(new { success = false, message = "Something went wrong" });
         else
         {
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, Obj.Image.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
               System.IO.File.Delete(imagePath);
            }
            _context.Products.Remove(id);
            await _context.Save();
            return Json(new { success = true, message = "Deleted Successfully" });
         }
      }
      #endregion

   }
}
