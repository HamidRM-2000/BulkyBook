using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class ShoppingCart
   {
      [Key]
      public int Id { get; set; }

      public string MyUserId { get; set; }

      [ForeignKey("MyUserId")]
      public MyUser MyUser { get; set; }

      public int ProId { get; set; }
      [ForeignKey("ProId")]
      public Product Product { get; set; }

      [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
      public int Count { get; set; }

      [NotMapped]
      public double Price { get; set; }

   }
}
