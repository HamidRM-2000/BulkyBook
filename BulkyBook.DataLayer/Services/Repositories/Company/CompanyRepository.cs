using BulkyBook.DataLayer.Context;
using BulkyBook.DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataLayer.Services.Repositories
{
   public class CompanyRepository : Repository<Company>, ICompanyRepository
   {
      private readonly BulkyBook_DBEntities _db;
      public CompanyRepository(BulkyBook_DBEntities db) : base(db)
      {
         _db = db;
      }
      public override Task<IEnumerable<Company>> GetAll(Expression<Func<Company, bool>> filter = null, Func<IQueryable<Company>, IOrderedQueryable<Company>> order = null, string includeProps = null)
      {
         return base.GetAll(filter = i => i.ComId != 1, order, includeProps);
      }
   }
}
