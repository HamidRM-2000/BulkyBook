using BulkyBook.DataLayer.Context;
using BulkyBook.DataLayer.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataLayer.Services.UnitOfWork
{
   public class UnitOfWork : IUnitOfWork
   {
      BulkyBook_DBEntities _db;
      public UnitOfWork(BulkyBook_DBEntities db)
      {
         _db = db;
         Categories = new CategoryRepository(_db);
         CoverTypes = new CoverTypeRepository(_db);
         Products = new ProductRepository(_db);
         Companies = new CompanyRepository(_db);
         MyUsers = new MyUserRepository(_db);
         ShoppingCarts = new ShoppingCartRepository(_db);
         OrderDetails = new OrderDetailRepository(_db);
         OrderHeaders = new OrderHeaderRepository(_db);
      }

      public ICategoryRepository Categories { get; }

      public ICoverTypeRepository CoverTypes { get; }

      public int MyProperty { get; set; }

      public IProductRepository Products { get; }

      public ICompanyRepository Companies { get; }

      public IMyUserRepository MyUsers { get; }

      public IOrderDetailRepository OrderDetails { get; }

      public IOrderHeaderRepository OrderHeaders { get; }

      public IShoppingCartRepository ShoppingCarts { get; }

      public void Dispose()
      {
         _db.DisposeAsync();
      }

      public async Task Save()
      {
         await _db.SaveChangesAsync();
      }
   }
}
