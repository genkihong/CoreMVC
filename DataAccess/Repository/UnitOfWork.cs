using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
  public class UnitOfWork : IUnitOfWork
  {
    private readonly ApplicationDbContext _db;
    public ICategoryRepository Category { get; private set; }
    public IProductRepository Product { get; private set; }
    public IProductImageRepository ProductImage { get; private set; }
    public ICompanyRepository Company { get; private set; }
    public IShoppingCartRepository ShoppingCart { get; private set; }
    public IApplicationUserRepository ApplicationUser { get; private set; }
    public IOrderRepository Order { get; private set; }
    public IOrderDetailRepository OrderDetail { get; private set; }
    public UnitOfWork(ApplicationDbContext db)
    {
      _db = db;
      Category = new CategoryRepository(_db);
      Product = new ProductRepository(_db);
      ProductImage = new ProductImageRepository(_db);
      Company = new CompanyRepository(_db);
      ShoppingCart = new ShoppingCartRepository(_db);
      ApplicationUser = new ApplicationUserRepository(_db);
      Order = new OrderRepository(_db);
      OrderDetail = new OrderDetailRepository(_db);
    }
    /// <summary>
    /// 寫入資料庫
    /// </summary>
    public void Save()
    {
      _db.SaveChanges();
    }
  }
}
