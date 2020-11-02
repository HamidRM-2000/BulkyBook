using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BulkyBook.DataLayer.Models
{
   public class MyUser:IdentityUser
   {
      [AllowNull]
      public int ComId { get; set; }

      [Required]
      [MaxLength(200)]
      public string Name { get; set; }
      
      [Required]
      [MaxLength(100)]
      public string City { get; set; }

      [Required]  
      [MaxLength(200)]
      public string State { get; set; }

      [Required]
      [MaxLength(200)]
      public string PostalCode { get; set; }

      [NotMapped]
      [MaxLength(200)]
      public string Role { get; set; }

      [ForeignKey("ComId")]
      public virtual Company Company { get; set; }

      public MyUser()
      {

      }
   }
}
