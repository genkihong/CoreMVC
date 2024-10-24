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
  public class OrderRepository : Repository<Order>, IOrderRepository
  {
    private readonly ApplicationDbContext _db;
    public OrderRepository(ApplicationDbContext db) : base(db)
    {
      _db = db;
    }
    /// <summary>
    /// 更新訂單
    /// </summary>
    /// <param name="order"></param>
    public void Update(Order order)
    {
      _db.Orders.Update(order);
    }
    /// <summary>
    /// 更新訂單狀態/付款狀態
    /// </summary>
    /// <param name="id"></param>
    /// <param name="orderStatus"></param>
    /// <param name="paymentStatus"></param>
    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
      var order = _db.Orders.FirstOrDefault(u => u.Id == id);

      if (order != null)
      {
        order.OrderStatus = orderStatus;

        if (!string.IsNullOrEmpty(paymentStatus))
        {
          order.PaymentStatus = paymentStatus;
        }
      }
    }
    /// <summary>
    /// 更新付款編號/付款日期
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sessionId"></param>
    /// <param name="paymentIntentId"></param>
    public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
    {
      var order = _db.Orders.FirstOrDefault(u => u.Id == id);

      if (!string.IsNullOrEmpty(sessionId))
      {
        order.SessionId = sessionId;
      }

      if (!string.IsNullOrEmpty(paymentIntentId))
      {
        order.PaymentIntentId = paymentIntentId;
        order.PaymentDate = DateTime.Now;
      }
    }
  }
}
