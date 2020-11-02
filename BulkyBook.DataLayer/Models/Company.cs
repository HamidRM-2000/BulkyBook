using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class Company
   {
      [Key]
      public int ComId { get; set; }

      [Required]
      [MaxLength(150)]
      public string Name { get; set; }
      
      [Required]
      public string StreetAddress { get; set; }

      [Required]
      [MaxLength(100)]
      public string City { get; set; }

      [Required]
      [MaxLength(200)]
      public string State { get; set; }

      [Required]
      [MaxLength(200)]
      public string PostalCard { get; set; }

      [Required]
      [MaxLength(50)]
      public string PhoneNumber { get; set; }

      [Required]
      public bool IsAuthorizedCompany { get; set; }

      public virtual List<MyUser> MyUsers { get; set; }

      public Company()
      {

      }
   }
}
