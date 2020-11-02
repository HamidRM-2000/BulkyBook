using BulkyBook.DataLayer.Context;
using BulkyBook.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BulkyBook.DataLayer.Services.Repositories
{
   public class CategoryRepository : Repository<Category>, ICategoryRepository
   {
      private readonly BulkyBook_DBEntities _db;
      public CategoryRepository(BulkyBook_DBEntities db) : base(db)
      {
         _db = db;
      }
   }
}
