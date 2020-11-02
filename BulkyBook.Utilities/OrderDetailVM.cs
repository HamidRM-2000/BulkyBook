using BulkyBook.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.Utilities
{
   public class OrderDetailVM
   {
      public OrderHeader OrderHeader { get; set; }
      public IEnumerable<OrderDetail> OrderDetails { get; set; }
   }
}
