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
  public class ProductRepository : Repository<Product>, IProductRepository
  {
    private readonly ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db)
    {
      _db = db;
    }
    /// <summary>
    /// 修改資料
    /// </summary>
    /// <param name="product"></param>
    public void Update(Product product)
    {
      //_db.Products.Update(product);
      var result = _db.Products.FirstOrDefault(p => p.Id == product.Id);
      if (result != null)
      {
        result.Title = product.Title;
        result.Description = product.Description;
        result.Author = product.Author;
        result.ISBN = product.ISBN;
        result.ListPrice = product.ListPrice;
        result.Price = product.Price;
        result.Price50 = product.Price50;
        result.Price100 = product.Price100;
        result.CategoryId = product.CategoryId;
        if (product.ImageUrl != null)
        {
          result.ImageUrl = product.ImageUrl;
        }
      }
    }
  }
}
