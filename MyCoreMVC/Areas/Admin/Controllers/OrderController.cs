using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Models;
using Models.ViewModel;
using System.Security.Claims;
using Utility;

namespace MyCoreMVC.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize]
  public class OrderController : Controller
  {
    private readonly IUnitOfWork _unitOfWork;
    public OrderController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// 取得所有訂單
    /// </summary>
    /// <returns></returns>
    public IActionResult Index(string status)
    {
      IEnumerable<Order> orderList;

      if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
      {
        orderList = _unitOfWork.Order.GetAll(includeProperties: "ApplicationUser");
      }
      else
      {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        orderList = _unitOfWork.Order.GetAll(o => o.ApplicationUserId == userId, includeProperties: "ApplicationUser");
      }
      //延遲付款
      if (status == "pending")
      {
        orderList = orderList.Where(o => o.OrderStatus == SD.PaymentStatusDelayedPayment);
      }
      //出貨中
      if (status == "inprocess")
      {
        orderList = orderList.Where(o => o.OrderStatus == SD.StatusInProcess);
      }
      //已出貨
      if (status == "completed")
      {
        orderList = orderList.Where(o => o.OrderStatus == SD.StatusShipped);
      }
      //已成立
      if (status == "approved")
      {
        orderList = orderList.Where(o => o.OrderStatus == SD.StatusApproved);
      }

      return View(orderList);
    }
    /// <summary>
    /// 訂單明細
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public IActionResult Detail(int orderId)
    {
      OrderViewModel orderViewModel = new()
      {
        Order = _unitOfWork.Order.Get(o => o.Id == orderId, includeProperties: "ApplicationUser"),
        OrderDetailList = _unitOfWork.OrderDetail.GetAll(od => od.OrderId == orderId, includeProperties: "Product")
      };

      return View(orderViewModel);
    }
    /// <summary>
    /// 更新訂單明細
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public IActionResult UpdateOrderDetail(OrderViewModel orderViewModel)
    {
      var order = _unitOfWork.Order.Get(u => u.Id == orderViewModel.Order.Id);
      order.Name = orderViewModel.Order.Name;
      order.PhoneNumber = orderViewModel.Order.PhoneNumber;
      order.Address = orderViewModel.Order.Address;
      order.City = orderViewModel.Order.City;
      order.Zone = orderViewModel.Order.Zone;
      order.PostalCode = orderViewModel.Order.PostalCode;

      if (!string.IsNullOrEmpty(orderViewModel.Order.Carrier))
      {
        order.Carrier = orderViewModel.Order.Carrier;
      }

      if (!string.IsNullOrEmpty(orderViewModel.Order.TrackingNumber))
      {
        order.Carrier = orderViewModel.Order.TrackingNumber;
      }

      _unitOfWork.Order.Update(order);
      _unitOfWork.Save();
      TempData["Success"] = "Order Details Updated Successfully.";

      return RedirectToAction(nameof(Detail), new { orderId = order.Id });
    }
    /// <summary>
    /// 更新訂單狀態/付款狀態
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public IActionResult StartProcessing(OrderViewModel orderViewModel)
    {
      _unitOfWork.Order.UpdateStatus(orderViewModel.Order.Id, SD.StatusInProcess);
      _unitOfWork.Save();
      TempData["Success"] = "Order Details Updated Successfully.";
      return RedirectToAction(nameof(Detail), new { orderId = orderViewModel.Order.Id });
    }
    /// <summary>
    /// 更新出貨編號/出貨日期
    /// </summary>
    /// <param name="orderViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public IActionResult ShipOrder(OrderViewModel orderViewModel)
    {
      var order = _unitOfWork.Order.Get(o => o.Id == orderViewModel.Order.Id);
      order.TrackingNumber = orderViewModel.Order.TrackingNumber;
      order.Carrier = orderViewModel.Order.Carrier;
      order.OrderStatus = SD.StatusShipped;
      order.ShippingDate = DateTime.Now;

      if (order.PaymentStatus == SD.PaymentStatusDelayedPayment)
      {
        order.PaymentDueDate = DateTime.Now.AddDays(30);// DateOnly.FromDateTime(DateTime.Now.AddDays(30))
      }

      _unitOfWork.Order.Update(order);
      _unitOfWork.Save();
      TempData["Success"] = "Order Shipped Successfully.";
      return RedirectToAction(nameof(Detail), new { orderId = orderViewModel.Order.Id });
    }
    /// <summary>
    /// 取消訂單
    /// </summary>
    /// <param name="orderViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
    public IActionResult CancelOrder(OrderViewModel orderViewModel)
    {
      var order = _unitOfWork.Order.Get(o => o.Id == orderViewModel.Order.Id);

      if (order.PaymentStatus == SD.PaymentStatusApproved)
      {
        //var options = new RefundCreateOptions
        //{
        //  Reason = RefundReasons.RequestedByCustomer,
        //  PaymentIntent = orderHeader.PaymentIntentId
        //};

        //var service = new RefundService();
        //Refund refund = service.Create(options);

        _unitOfWork.Order.UpdateStatus(order.Id, SD.StatusCancelled, SD.StatusRefunded);
      }
      else
      {
        _unitOfWork.Order.UpdateStatus(order.Id, SD.StatusCancelled, SD.StatusCancelled);
      }

      _unitOfWork.Save();
      TempData["Success"] = "Order Cancelled Successfully.";
      return RedirectToAction(nameof(Detail), new { orderId = orderViewModel.Order.Id });
    }
    /// <summary>
    /// 付款
    /// </summary>
    /// <param name="orderViewModel"></param>
    /// <returns></returns>
    [ActionName("Detail")]
    [HttpPost]
    public IActionResult Details_PAY_NOW(OrderViewModel orderViewModel)
    {
      orderViewModel.Order = _unitOfWork.Order.Get(u => u.Id == orderViewModel.Order.Id, includeProperties: "ApplicationUser");
      orderViewModel.OrderDetailList = _unitOfWork.OrderDetail.GetAll(u => u.OrderId == orderViewModel.Order.Id, includeProperties: "Product");

      #region 介接金流
      //var domain = Request.Scheme + "://" + Request.Host.Value + "/";
      //var options = new SessionCreateOptions
      //{
      //SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderId={orderViewModel.Order.Id}",
      //  CancelUrl = domain + $"admin/order/details?orderId={orderViewModel.Order.Id}",
      //  LineItems = new List<SessionLineItemOptions>(),
      //  Mode = "payment",
      //};

      //foreach (var item in orderViewModel.OrderDetail)
      //{
      //  var sessionLineItem = new SessionLineItemOptions
      //  {
      //    PriceData = new SessionLineItemPriceDataOptions
      //    {
      //      UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
      //      Currency = "usd",
      //      ProductData = new SessionLineItemPriceDataProductDataOptions
      //      {
      //        Name = item.Product.Title
      //      }
      //    },
      //    Quantity = item.Count
      //  };
      //  options.LineItems.Add(sessionLineItem);
      //}


      //var service = new SessionService();
      //Session session = service.Create(options);
      //_unitOfWork.Order.UpdateStripePaymentID(orderViewModel.Order.Id, session.Id, session.PaymentIntentId);
      //_unitOfWork.Save();
      //Response.Headers.Add("Location", session.Url);
      #endregion
      return new StatusCodeResult(303);
    }
    /// <summary>
    /// 確認付款
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public IActionResult PaymentConfirmation(int orderId)
    {
      Order order = _unitOfWork.Order.Get(u => u.Id == orderId);
      if (order.PaymentStatus == SD.PaymentStatusDelayedPayment)
      {
        //this is an order by company

        //var service = new SessionService();
        //Session session = service.Get(order.SessionId);

        //if (session.PaymentStatus.ToLower() == "paid")
        //{
        //  _unitOfWork.Order.UpdateStripePaymentID(orderId, session.Id, session.PaymentIntentId);
        //  _unitOfWork.Order.UpdateStatus(orderId, order.OrderStatus, SD.PaymentStatusApproved);
        //  _unitOfWork.Save();
        //}
      }
      return View(orderId);
    }
    #region API
    /// <summary>
    /// 取得所有訂單
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetAll(string status)
    {
      List<Order> orderList = _unitOfWork.Order.GetAll(includeProperties: "ApplicationUser").ToList();

      if (orderList.Count == 0)
      {
        return Json(new
        {
          success = false,
          message = "無資料"
        });
      }

      return Json(new
      {
        success = true,
        data = orderList
      });
    }
    #endregion
  }
}
