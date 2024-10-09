using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Authorization;
using Utility;


namespace MyCoreMVC.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize(Roles = SD.Role_Admin)]
  public class CategoryController : Controller
  {
    //private readonly ApplicationDbContext _db;
    //public CategoryController(ApplicationDbContext db)
    //{
    //    _db = db;
    //}
    //private readonly ICategoryRepository _productRepository;
    //public CategoryController(ICategoryRepository db)
    //{
    //    _productRepository = db; ;
    //}
    private readonly IUnitOfWork _unitOfWork;
    public CategoryController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// 取得全部資料
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
      //IEnumerable<Category> producttList = _db.Categories;
      //List<Category> producttList = _productRepository.GetAll().ToList();
      List<Category> producttList = _unitOfWork.Category.GetAll().ToList();
      return View(producttList);
    }
    /// <summary>
    /// 顯示新增資料頁
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
      return View();
    }
    /// <summary>
    /// 送出新增資料
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Category product)
    {
      if (product.Name == product.DisplayOrder.ToString())
      {
        ModelState.AddModelError("name", "名稱和數量相同!");
      }

      if (ModelState.IsValid)
      {
        //_db.Categories.Add(product);
        //_db.SaveChanges();
        //_productRepository.Add(product);
        //_productRepository.Save();
        _unitOfWork.Category.Add(product);
        _unitOfWork.Save();
        TempData["success"] = "已新增";
        return RedirectToAction("Index");
      }
      return View(product);
    }
    /// <summary>
    /// 顯示修改資料頁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Edit(int? id)
    {
      if (id == null || id == 0)
      {
        return NotFound();
      }
      //var product = _db.Categories.Find(id);
      //var productFirst = _db.Categories.FirstOrDefault(c => c.Id == id);
      //var productSingle = _db.Categories.SingleOrDefault(c => c.Id == id);
      //Category product = _productRepository.Get(c => c.Id == id);
      Category product = _unitOfWork.Category.Get(c => c.Id == id);
      if (product == null)
      {
        return NotFound();
      }
      return View(product);
    }
    /// <summary>
    /// 送出修改資料
    /// </summary>
    /// <param name="product"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Category product)
    {
      if (product.Name == product.DisplayOrder.ToString())
      {
        ModelState.AddModelError("name", "名稱和數量相同!");
      }
      if (ModelState.IsValid)
      {
        //_db.Categories.Update(product);
        //_db.SaveChanges();
        //_productRepository.Update(product);
        //_productRepository.Save();
        _unitOfWork.Category.Update(product);
        _unitOfWork.Save();
        TempData["success"] = "已修改";
        return RedirectToAction("Index");
      }
      return View(product);
    }
    /// <summary>
    /// 顯示刪除資料頁
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Delete(int? id)
    {
      if (id == null || id == 0)
      {
        return NotFound();
      }
      //var product = _db.Categories.Find(id);
      //var productFirst = _db.Categories.FirstOrDefault(c => c.Id == id);
      //var productSingle = _db.Categories.SingleOrDefault(c => c.Id == id);
      //Category product = _productRepository.Get(c => c.Id == id);
      Category product = _unitOfWork.Category.Get(c => c.Id == id);
      if (product == null)
      {
        return NotFound();
      }
      return View(product);
    }
    /// <summary>
    /// 送出刪除資料
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
      //var obj = _db.Categories.Find(id);
      //Category product = _productRepository.Get(c => c.Id == id);
      Category product = _unitOfWork.Category.Get(c => c.Id == id);
      if (product == null)
      {
        return NotFound();
      }
      //_db.Categories.Remove(obj);
      //_db.SaveChanges();
      //_productRepository.Remove(product);
      //_productRepository.Save();
      _unitOfWork.Category.Remove(product);
      _unitOfWork.Save();
      TempData["success"] = "已刪除";
      return RedirectToAction("Index");
    }
  }
}
