using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class Product
   {
      [Key]
      public int ProId { get; set; }

      [Display(Name ="Category")]
      [Required]
      public int CatId { get; set; }

      [Display(Name = "CoverType")]
      [Required]
      public int CovId { get; set; }

      [Required]
      [MaxLength(150)]
      public string Title { get; set; }

      [Required]
      [MaxLength(200)]
      public string Author { get; set; }

      [Required]
      [DataType(DataType.MultilineText)]
      public string Description { get; set; }

      [Required]
      [MaxLength(50)]
      public string ISBN { get; set; }

      public string Image { get; set; }

      [Required]
      [Range(1,10000)]
      public double PriceList { get; set; }

      [Required]
      [Range(1, 10000)]
      public double Price { get; set; }

      [Required]
      [Range(1, 10000)]
      public double Price50 { get; set; }

      [Required]
      [Range(1, 10000)]
      public double Price100 { get; set; }

      [ForeignKey("CatId")]
      public virtual Category Category { get; set; }
      
      [ForeignKey("CovId")]
      public virtual CoverType CoverType { get; set; }
      public Product()
      {

      }
   }
}
