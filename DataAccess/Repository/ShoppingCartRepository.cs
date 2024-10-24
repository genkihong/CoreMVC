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
  public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
  {
    private readonly ApplicationDbContext _db;
    public ShoppingCartRepository(ApplicationDbContext db) : base(db)
    {
      _db = db;
    }
    /// <summary>
    /// 修改資料
    /// </summary>
    /// <param name="cart"></param>
    public void Update(ShoppingCart cart)
    {
      _db.ShoppingCarts.Update(cart);
    }
  }
}
