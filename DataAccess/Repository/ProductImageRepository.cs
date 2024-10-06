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
  public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
  {
    private readonly ApplicationDbContext _db;
    public ProductImageRepository(ApplicationDbContext db) : base(db)
    {
      _db = db;
    }
    /// <summary>
    /// 修改資料
    /// </summary>
    /// <param name="product"></param>
    public void Update(ProductImage productImg)
    {
      _db.ProductImages.Update(productImg);
    }
  }
}
