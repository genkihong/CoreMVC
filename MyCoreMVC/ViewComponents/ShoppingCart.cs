using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Utility;

namespace MyCoreMVC.ViewComponents
{
  public class ShoppingCart : ViewComponent
  {
    private readonly IUnitOfWork _unitOfWork;
    public ShoppingCart(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
      var claimsIdentity = (ClaimsIdentity)User.Identity;
      var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
      // 登入
      if (claim != null)
      {
        // 登入時無 Session
        if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
        {
          // 購物車總數量
          var count = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == claim.Value).Count();
          // 寫入 Session
          HttpContext.Session.SetInt32(SD.SessionCart, count);
        }

        return View(HttpContext.Session.GetInt32(SD.SessionCart));
      }
      // 登出時清空 Session
      HttpContext.Session.Clear();
      return View(0);
    }
  }
}
