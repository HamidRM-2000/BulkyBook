using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class OrderDetail
   {
      [Key]
      public int Id { get; set; }

      [Required]
      public int OrderId { get; set; }
      [ForeignKey("OrderId")]
      public OrderHeader OrderHeader { get; set; }


      [Required]
      public int ProId { get; set; }
      [ForeignKey("ProId")]
      public Product Product { get; set; }

      public int Count { get; set; }
      public double Price { get; set; }
   }
}
