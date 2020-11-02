using BulkyBook.DataLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.Utilities
{
   public class ShoppingCartVM
   {
      public ShoppingCartVM()
      {
      }
      public OrderHeader OrderHeader { get; set; }
      public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
   }
}
