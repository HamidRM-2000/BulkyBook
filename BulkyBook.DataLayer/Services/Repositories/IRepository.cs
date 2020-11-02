using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataLayer.Services.Repositories
{
   public interface IRepository<T> where T : class
   {
      Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> order = null, string includeProps = null);
      Task<T> GetByID(int id);
      void Remove(int id);
      void RemoveRange(IEnumerable<T> entities);
      Task Add(T entity);
      Task AddRange(IEnumerable<T> entities);
      Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null, string includeProps = null);
      void Update(T entity);
   }
}
