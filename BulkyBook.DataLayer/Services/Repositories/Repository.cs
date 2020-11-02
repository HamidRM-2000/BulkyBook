using BulkyBook.DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataLayer.Services.Repositories
{
   public class Repository<T> : IRepository<T> where T : class
   {
      private readonly BulkyBook_DBEntities _db;
      private DbSet<T> Ts;
      public Repository(BulkyBook_DBEntities db)
      {
         _db = db;
         Ts = _db.Set<T>();
      }

      public virtual async Task Add(T entity)
      {
         await Ts.AddAsync(entity);
      }

      public virtual async Task AddRange(IEnumerable<T> entities)
      {
         await Ts.AddRangeAsync(entities);
      }

      public virtual async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> order = null, string includeProps = null)
      {
         IQueryable<T> query = Ts;
         if (filter != null)
            query = query.Where(filter);
         if (includeProps != null)
            foreach (var prop in includeProps.Split(','))
            {
               query=query.Include(prop);
            }
         //order(query);
         return await query.ToListAsync();
      }

      public virtual async Task<T> GetByID(int id)
      {
         return await Ts.FindAsync(id);
      }

      public virtual async Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProps = null)
      {
         IQueryable<T> query = Ts;
         if (filter != null)
            query = query.Where(filter);
         if (includeProps != null)
            foreach (var prop in includeProps.Split(','))
            {
               query=query.Include(prop);
            }
         return await query.FirstOrDefaultAsync();
      }

      public virtual void Remove(int id)
      {
         var entity = Ts.Find(id);
         Ts.Remove(entity);
      }

      public virtual void RemoveRange(IEnumerable<T> entities)
      {
         Ts.RemoveRange(entities);
      }

      public virtual void Update(T entity)
      {
         Ts.Update(entity);
      }
   }
}
