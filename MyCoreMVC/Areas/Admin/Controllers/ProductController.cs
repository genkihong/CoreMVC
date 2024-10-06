using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models.ViewModel;
using System.IO;
using System;

namespace MyCoreMVC.Areas.Admin.Controllers
{
  [Area("Admin")]
  public class ProductController : Controller
  {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
      _unitOfWork = unitOfWork;
      _webHostEnvironment = webHostEnvironment;
    }
    /// <summary>
    /// 取得全部資料
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
      // 具名引數 https://learn.microsoft.com/zh-tw/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments
      List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
      return View(productList);
    }
    /// <summary>
    /// 新增/修改 Get
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Upsert(int? id)
    {
      ProductViewModel productViewModel = new()
      {
        CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
        {
          Text = c.Name,
          Value = c.Id.ToString()
        })
      };
      //新增
      if (id == null || id == 0)
      {
        productViewModel.Product = new Product();
        return View(productViewModel);
      }
      //修改
      productViewModel.Product = _unitOfWork.Product.Get(p => p.Id == id);
      return View(productViewModel);
    }
    /// <summary>
    /// 新增/修改 Post
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductViewModel productViewModel, IFormFile? uploadImage)
    {
      ModelState.Remove("CategoryList");
      ModelState.Remove("Product.ImageUrl");

      if (ModelState.IsValid)
      {
        // 上傳圖片
        if (uploadImage != null)
        {
          // wwwroot path
          string rootPath = _webHostEnvironment.WebRootPath;
          string filePath = Path.Combine(rootPath, @"images\product");// wwwroot\images\product\
          // 上傳檔案為圖片格式
          var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
          // 取得圖片副檔名
          var fileExtension = Path.GetExtension(uploadImage.FileName).ToLower();

          // 驗證圖片格式
          if (!allowedExtensions.Contains(fileExtension))
          {
            throw new InvalidOperationException($"只能上傳{string.Join(",", allowedExtensions)}");
          }

          // 圖片檔名 = Guid + 副檔名
          string fileName = Guid.NewGuid().ToString() + fileExtension;
          // 上傳圖片路徑
          string uploadPath = Path.Combine(filePath, fileName);

          // 編輯時刪除舊圖片(新增時 ImageUrl 為 null)
          if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))
          {
            // 已存在的圖片路徑
            string oldImagePath = Path.Combine(rootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
              // 刪除圖片
              System.IO.File.Delete(oldImagePath);
            }
          }

          // 上傳圖片至 wwwroot/images/product
          using (var fileStream = new FileStream(uploadPath, FileMode.Create))
          {
            await uploadImage.CopyToAsync(fileStream);
          }
          // 儲存圖片路徑
          productViewModel.Product.ImageUrl = $@"\images\product\{fileName}";
        }

        if (productViewModel.Product.Id == 0)
        {
          //新增
          _unitOfWork.Product.Add(productViewModel.Product);
          TempData["success"] = "新增成功";
        }
        else
        {
          //修改
          _unitOfWork.Product.Update(productViewModel.Product);
          TempData["success"] = "修改成功";
        }

        _unitOfWork.Save();
        return RedirectToAction("Index");
      }
      //驗證失敗時，下拉選單
      productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
      {
        Text = c.Name,
        Value = c.Id.ToString()
      });
      return View(productViewModel);
    }
    /// <summary>
    /// 顯示新增資料頁
    /// </summary>
    /// <returns></returns>
    public IActionResult Create()
    {
      //IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll()
      //  .Select(c => new SelectListItem
      //  {
      //    Text = c.Name,
      //    Value = c.Id.ToString()
      //  });

      ProductViewModel productViewModel = new()
      {
        CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
        {
          Text = c.Name,
          Value = c.Id.ToString()
        }),
        Product = new Product()
      };
      return View(productViewModel);
    }
    /// <summary>
    /// 送出新增資料
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ProductViewModel productViewModel)
    {
      ModelState.Remove("CategoryList");

      if (ModelState.IsValid)
      {
        _unitOfWork.Product.Add(productViewModel.Product);
        _unitOfWork.Save();
        TempData["success"] = "已新增";
        return RedirectToAction("Index");
      }

      productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
      {
        Text = c.Name,
        Value = c.Id.ToString()
      });
      return View(productViewModel);
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

      Product product = _unitOfWork.Product.Get(p => p.Id == id);

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
    public IActionResult Edit(Product product)
    {
      if (ModelState.IsValid)
      {
        _unitOfWork.Product.Update(product);
        _unitOfWork.Save();
        TempData["success"] = "已修改";

        return RedirectToAction("Index");
      }
      ViewBag.CategoryList = new SelectList(_unitOfWork.Category.GetAll(), "Id", "CategoryName");
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

      Product product = _unitOfWork.Product.Get(p => p.Id == id);

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
      Product product = _unitOfWork.Product.Get(p => p.Id == id);

      if (product == null)
      {
        return NotFound();
      }

      _unitOfWork.Product.Remove(product);
      _unitOfWork.Save();
      TempData["success"] = "已刪除";
      return RedirectToAction("Index");
    }
    #region API
    /// <summary>
    /// 取得所有產品(DataTable)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetAll()
    {
      List<Product> productList = _unitOfWork.Product
                                  .GetAll(includeProperties: "Category")
                                  .ToList();
      return Json(new
      {
        data = productList
      });
    }
    /// <summary>
    /// 刪除產品
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    public IActionResult DeleteProduct(int? id)
    {
      if (!id.HasValue)
      {
        return Json(new
        {
          success = false,
          message = "無法刪除"
        });
      }

      var product = _unitOfWork.Product.Get(p => p.Id == id);

      if (product == null)
      {
        return Json(new
        {
          success = false,
          message = "無法刪除"
        });
      }

      // 已存在的圖片路徑
      //var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product", product.ImageUrl);
      string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

      if (System.IO.File.Exists(imagePath))
      {
        // 刪除圖片
        System.IO.File.Delete(imagePath);
      }

      _unitOfWork.Product.Remove(product);
      _unitOfWork.Save();

      return Json(new
      {
        success = true,
        message = "圖片刪除成功"
      });
    }
    #endregion
  }
}
