using System;
using System.Collections.Generic;
using System.Text;
using BulkyBook.DataLayer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataLayer.Context
{
   public class BulkyBook_DBEntities : IdentityDbContext
   {
      public BulkyBook_DBEntities(DbContextOptions<BulkyBook_DBEntities> options)
          :base(options)
      {
      }
      public DbSet<Category> Categories { get; set; }
      public DbSet<CoverType> CoverTypes { get; set; }
      public DbSet<Product> Products { get; set; }
      public DbSet<MyUser> MyUsers { get; set; }
      public DbSet<Company> Companies { get; set; }
      public DbSet<ShoppingCart> ShoppingCarts  { get; set; }
      public DbSet<OrderDetail> OrderDetails { get; set; }
      public DbSet<OrderHeader> OrderHeaders  { get; set; }

   }
}
