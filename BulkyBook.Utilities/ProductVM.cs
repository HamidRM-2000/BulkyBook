using BulkyBook.DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.Utilities
{
   public class ProductVM
   {
      public ProductVM(Product product)
      {
         Product = product;
      }
      public Product Product { get; set; }
      public IEnumerable<SelectListItem> Categories { get; set; }
      public IEnumerable<SelectListItem> CoverTypes { get; set; }
      public void FillCategory(IEnumerable<Category> list)
      {
         Categories = list.Select(i => new SelectListItem
         {
            Text = i.CatName,
            Value = i.CatId.ToString()
         });
      }
      public void FillCoverType(IEnumerable<CoverType> list)
      {
         CoverTypes = list.Select(i => new SelectListItem
         {
            Text = i.Name,
            Value = i.CovId.ToString()
         });
      }
   }
}
