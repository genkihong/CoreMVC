using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Models;
using DataAccess.Repository.IRepository;

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
      IEnumerable<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category");
      return View(productList);
    }
    /// <summary>
    /// 產品明細
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Details(int? id)
    {
      if(!id.HasValue)
      {
        return NotFound();
      }

      Product product = _unitOfWork.Product.Get(p => p.Id == id, includeProperties: "Category");
      return View(product);
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