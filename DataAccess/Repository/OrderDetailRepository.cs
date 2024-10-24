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
  public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
  {
    private readonly ApplicationDbContext _db;
    public OrderDetailRepository(ApplicationDbContext db) : base(db)
    {
      _db = db;
    }
    /// <summary>
    /// 修改資料
    /// </summary>
    /// <param name="orderDetail"></param>
    public void Update(OrderDetail orderDetail)
    {
      _db.OrderDetails.Update(orderDetail);
    }
  }
}
