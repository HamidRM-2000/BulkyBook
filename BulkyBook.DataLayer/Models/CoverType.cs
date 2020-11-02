using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class CoverType
   {
      [Key]
      public int CovId { get; set; }
      [Display(Name ="Cover Type Name")]
      [Required(ErrorMessage ="Please Enter {0}")]
      [MaxLength(70)]
      public string Name { get; set; }

      public virtual List<Product> Products { get; set; }

      public CoverType()
      {

      }
   }
}
