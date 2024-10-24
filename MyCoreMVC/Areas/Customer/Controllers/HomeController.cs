using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Models;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Utility;
using Microsoft.AspNetCore.Http;

namespace MyCoreMVC.Areas.Customer.Controllers
{
  [Area("Customer")]
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
      _logger = logger;
      _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// 首頁
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
      var claimsIdentity = (ClaimsIdentity)User.Identity;
      var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

      if (claim != null)
      {
        // 購物車總數量
        var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).Count();
        // 寫入 Session
        HttpContext.Session.SetInt32(SD.SessionCart, count);
      }

      IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category,ProductImages");
      return View(productList);
    }
    /// <summary>
    /// 商品明細
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    public IActionResult Detail(int productId)
    {
      ShoppingCart cart = new()
      {
        Product = _unitOfWork.Product.Get(p => p.Id == productId, includeProperties: "Category,ProductImages"),
        Quantity = 1,
        ProductId = productId
      };

      return View(cart);
    }
    /// <summary>
    /// 加入/更新購物車
    /// </summary>
    /// <param name="cart"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public IActionResult Detail(ShoppingCart cart)
    {
      var claimsIdentity = (ClaimsIdentity)User.Identity;
      var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

      ShoppingCart shoppingCart = _unitOfWork.ShoppingCart.Get(s => s.ApplicationUserId == userId && s.ProductId == cart.ProductId);

      if (shoppingCart == null)
      {
        // 加入購物車
        cart.ApplicationUserId = userId;
        _unitOfWork.ShoppingCart.Add(cart);
        _unitOfWork.Save();
        // 購物車總數量
        var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId).Count();
        // 寫入 Session
        HttpContext.Session.SetInt32(SD.SessionCart, count);
      }
      else
      {
        // 更新購物車
        shoppingCart.Quantity += cart.Quantity;
        _unitOfWork.ShoppingCart.Update(shoppingCart);
        _unitOfWork.Save();
      }

      TempData["success"] = "已加入購物車";
      return RedirectToAction(nameof(Index));
    }
    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}