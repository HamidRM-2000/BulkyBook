using BulkyBook.DataLayer.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataLayer.Services.UnitOfWork
{
   public interface IUnitOfWork:IDisposable
   {
      ICategoryRepository Categories { get; }
      ICoverTypeRepository CoverTypes { get;}
      IProductRepository Products { get; }
      ICompanyRepository Companies { get; }
      IMyUserRepository MyUsers { get; }
      IShoppingCartRepository ShoppingCarts { get; }
      IOrderHeaderRepository OrderHeaders { get; }
      IOrderDetailRepository OrderDetails { get; }


      Task Save();
   }
}
