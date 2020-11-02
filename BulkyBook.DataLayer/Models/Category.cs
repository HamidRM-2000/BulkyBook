using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class Category
   {
      [Key]
      public int CatId { get; set; }
      [Display(Name ="Category Name")]
      [Required(ErrorMessage ="Please Enter {0}")]
      [MaxLength(70)]
      public string CatName { get; set; }

      public virtual List<Product> Products { get; set; }

      public Category()
      {

      }
   }
}
