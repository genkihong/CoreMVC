using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModel;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;
using Utility;

namespace MyCoreMVC.Areas.Customer.Controllers
{
  [Area("Customer")]
  [Authorize]
  public class CartController : Controller
  {
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public ShoppingCartViewModel ShoppingCartViewModel { get; set; }
    public CartController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// 購物車頁
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
      var claimsIdentity = (ClaimsIdentity)User.Identity;
      var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

      ShoppingCartViewModel = new()
      {
        Order = new Order(),
        ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, includeProperties: "Product")
      };
      // 所有商品圖片
      IEnumerable<ProductImage> productImages = _unitOfWork.ProductImage.GetAll();

      foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
      {
        cart.Product.ProductImages = productImages.Where(p => p.ProductId == cart.ProductId).ToList();
        cart.Price = GetPrice(cart);
        ShoppingCartViewModel.Order.OrderTotal += cart.SubTotal;//cart.Price * cart.Quantity;
      }

      return View(ShoppingCartViewModel);
    }
    /// <summary>
    /// 訂單明細
    /// </summary>
    /// <returns></returns>
    public IActionResult Summary()
    {
      var claimsIdentity = (ClaimsIdentity)User.Identity;
      var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

      ShoppingCartViewModel = new()
      {
        Order = new(),
        ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, includeProperties: "Product")
      };

      ShoppingCartViewModel.Order.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

      ShoppingCartViewModel.Order.Name = ShoppingCartViewModel.Order.ApplicationUser.Name;
      ShoppingCartViewModel.Order.PhoneNumber = ShoppingCartViewModel.Order.ApplicationUser.PhoneNumber;
      ShoppingCartViewModel.Order.City = ShoppingCartViewModel.Order.ApplicationUser.City;
      ShoppingCartViewModel.Order.Zone = ShoppingCartViewModel.Order.ApplicationUser.Zone;
      ShoppingCartViewModel.Order.PostalCode = ShoppingCartViewModel.Order.ApplicationUser.PostalCode;
      ShoppingCartViewModel.Order.Address = ShoppingCartViewModel.Order.ApplicationUser.Address;

      foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
      {
        cart.Price = GetPrice(cart);
        ShoppingCartViewModel.Order.OrderTotal += cart.SubTotal;
      }

      return View(ShoppingCartViewModel);
    }
    /// <summary>
    /// 新增訂單/訂單明細
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [ActionName("Summary")]
    public IActionResult PostSummary()
    {
      if (ModelState.IsValid)
      {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartViewModel.Order.ApplicationUserId = userId;
        ShoppingCartViewModel.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, includeProperties: "Product");

        #region 新增 Order
        ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
        // 計算總金額
        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
          cart.Price = GetPrice(cart);
          ShoppingCartViewModel.Order.OrderTotal += cart.SubTotal;
        }
        // 訂單狀態
        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
          // customer
          ShoppingCartViewModel.Order.OrderStatus = SD.StatusApproved;//SD.StatusPending;
          ShoppingCartViewModel.Order.PaymentStatus = SD.PaymentStatusApproved;//SD.PaymentStatusPending;
        }
        else
        {
          // company user
          ShoppingCartViewModel.Order.OrderStatus = SD.StatusApproved;
          ShoppingCartViewModel.Order.PaymentStatus = SD.PaymentStatusDelayedPayment;
        }

        _unitOfWork.Order.Add(ShoppingCartViewModel.Order);
        _unitOfWork.Save();
        #endregion
        #region 新增 OrderDetail
        foreach (var cart in ShoppingCartViewModel.ShoppingCartList)
        {
          OrderDetail orderDetail = new()
          {
            OrderId = ShoppingCartViewModel.Order.Id,
            ProductId = cart.ProductId,
            Quantity = cart.Quantity,
            Price = cart.Price,
          };
          _unitOfWork.OrderDetail.Add(orderDetail);
        }
        _unitOfWork.Save();
        #endregion
        // 介接第三方金流
        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
          //stripe logic
          //var domain = Request.Scheme + "://" + Request.Host.Value + "/";
          //var options = new SessionCreateOptions
          //{
          //  SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
          //  CancelUrl = domain + "customer/cart/index",
          //  LineItems = new List<SessionLineItemOptions>(),
          //  Mode = "payment",
          //};

          //foreach (var item in ShoppingCartViewModel.ShoppingCartList)
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
          //_unitOfWork.Order.UpdateStripePaymentID(ShoppingCartViewModel.Order.Id, session.Id, session.PaymentIntentId);
          //_unitOfWork.Save();
          //Response.Headers.Add("Location", session.Url);
          //return new StatusCodeResult(303);
        }
        return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.Order.Id });
      }
      return View(ShoppingCartViewModel);
    }
    /// <summary>
    /// 確認訂購
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult OrderConfirmation(int id)
    {
      Order order = _unitOfWork.Order.Get(u => u.Id == id, includeProperties: "ApplicationUser");
      if (order.PaymentStatus != SD.PaymentStatusDelayedPayment)
      {
        //  //this is an order by customer

        //  var service = new SessionService();
        //  Session session = service.Get(order.SessionId);

        //  if (session.PaymentStatus.ToLower() == "paid")
        //  {
        //    _unitOfWork.Order.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
        //    _unitOfWork.Order.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
        //    _unitOfWork.Save();
        //  }
        HttpContext.Session.Clear();
      }

      //_emailSender.SendEmailAsync(order.ApplicationUser.Email, "New Order - Bulky Book",
      //    $"<p>New Order Created - {order.Id}</p>");

      List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == order.ApplicationUserId).ToList();

      _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
      _unitOfWork.Save();
      return View(id);
    }
    /// <summary>
    /// 增加購物車數量
    /// </summary>
    /// <param name="cartId"></param>
    /// <returns></returns>
    public IActionResult Plus(int cartId)
    {
      ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId);
      shoppingCart.Quantity += 1;
      _unitOfWork.ShoppingCart.Update(shoppingCart);
      _unitOfWork.Save();
      return RedirectToAction(nameof(Index));
    }
    /// <summary>
    /// 減少購物車數量
    /// </summary>
    /// <param name="cartId"></param>
    /// <returns></returns>
    public IActionResult Minus(int cartId)
    {
      ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId, tracked: true);

      if (shoppingCart.Quantity > 1)
      {
        shoppingCart.Quantity -= 1;
        _unitOfWork.ShoppingCart.Update(shoppingCart);
      }
      else
      {
        // 購物車總數量 - 1
        var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).Count() - 1;
        // 寫入 Session
        HttpContext.Session.SetInt32(SD.SessionCart, count);
        _unitOfWork.ShoppingCart.Remove(shoppingCart);
      }

      _unitOfWork.Save();
      return RedirectToAction(nameof(Index));
    }
    /// <summary>
    /// 刪除購物車
    /// </summary>
    /// <param name="cartId"></param>
    /// <returns></returns>
    public IActionResult Remove(int cartId)
    {
      ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(s => s.Id == cartId, tracked: true);
      // 購物車總數量 - 1
      var count = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == shoppingCart.ApplicationUserId).Count() - 1;
      // 寫入 Session
      HttpContext.Session.SetInt32(SD.SessionCart, count);

      _unitOfWork.ShoppingCart.Remove(shoppingCart);
      _unitOfWork.Save();
      return RedirectToAction(nameof(Index));
    }
    /// <summary>
    /// 依購買數量取得價格
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    private int GetPrice(ShoppingCart cart)
    {
      if (cart.Quantity <= 50)
      {
        return cart.Product.Price;
      }

      if (cart.Quantity <= 100)
      {
        return cart.Product.Price50;
      }

      return cart.Product.Price100;
    }
  }
}
