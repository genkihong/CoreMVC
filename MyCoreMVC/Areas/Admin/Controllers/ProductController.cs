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
using Microsoft.AspNetCore.Authorization;
using Utility;

namespace MyCoreMVC.Areas.Admin.Controllers
{
  [Area("Admin")]
  [Authorize(Roles = SD.Role_Admin)]
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
    /// 取得全部商品
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
      // 具名引數 https://learn.microsoft.com/zh-tw/dotnet/csharp/programming-guide/classes-and-structs/named-and-optional-arguments
      // List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
      return View();
    }
    /// <summary>
    /// 新增/修改商品 Get
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
      productViewModel.Product = _unitOfWork.Product.Get(p => p.Id == id, includeProperties: "ProductImages");
      return View(productViewModel);
    }
    /// <summary>
    /// 新增/修改商品 Post
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(ProductViewModel productViewModel)
    {
      //ModelState.Remove("CategoryList");
      ModelState.Remove("Product.Images");

      if (ModelState.IsValid)
      {
        if (productViewModel.Product.Id == 0)
        {// 新增
          _unitOfWork.Product.Add(productViewModel.Product);
          TempData["success"] = "新增成功";
        }
        else
        {// 修改          
          _unitOfWork.Product.Update(productViewModel.Product);
          TempData["success"] = "修改成功";
        }

        _unitOfWork.Save();

        await UploadImage(productViewModel);// 上傳圖片

        _unitOfWork.Product.Update(productViewModel.Product);
        _unitOfWork.Save();

        return RedirectToAction("Index");
      }

      //驗證失敗時，商品類別下拉選單
      productViewModel.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
      {
        Text = c.Name,
        Value = c.Id.ToString()
      });

      return View(productViewModel);
    }
    /// <summary>
    /// 刪除圖片
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    public IActionResult DeleteImage(int imageId)
    {
      var image = _unitOfWork.ProductImage.Get(p => p.Id == imageId);

      if (image != null)
      {
        if (!string.IsNullOrEmpty(image.ImageUrl))
        {
          // 已存在的圖片路徑
          //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), image.ImageUrl);
          string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));

          if (System.IO.File.Exists(imagePath))
          {
            // 刪除圖片
            System.IO.File.Delete(imagePath);
          }
        }
        _unitOfWork.ProductImage.Remove(image);
        _unitOfWork.Save();
        TempData["success"] = "刪除圖片成功";
      }

      return RedirectToAction(nameof(Upsert), new { id = image.ProductId });
    }
    /// <summary>
    /// 上傳圖片
    /// </summary>
    /// <param name="productViewModel"></param>
    /// <returns></returns>
    private async Task UploadImage(ProductViewModel model)
    {
      if (model.Product.Images != null)
      {
        foreach (IFormFile file in model.Product.Images)
        {
          // string rootPath = Directory.GetCurrentDirectory();
          // string filePath = Path.Combine(rootPath, "wwwroot/images/product");
          // wwwroot path
          string rootPath = _webHostEnvironment.WebRootPath;
          string productPath = $"images/products/product-{model.Product.Id}";
          string filePath = Path.Combine(rootPath, productPath);

          // 判斷圖片資料夾是否存在
          if (!Directory.Exists(filePath))
          {
            // 建立圖片資料夾
            Directory.CreateDirectory(filePath);
          }
          // 取得圖片副檔名
          var fileExtension = Path.GetExtension(file.FileName).ToLower();
          #region 驗證圖片格式
          // 上傳檔案為圖片格式
          //var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

          // 驗證圖片格式
          //if (!allowedExtensions.Contains(fileExtension))
          //{
          //  ModelState.AddModelError("Image", "僅允許上傳圖片格式 (jpg, jpeg, png, gif, bmp)。");
          //  return View(productViewModel);// 回到表單顯示錯誤訊息
          //}
          #endregion
          // 圖片檔名 = Guid + 副檔名
          string fileName = Guid.NewGuid().ToString() + fileExtension;// 356944f2-34ea-4d0c-9fef-81323c762682.jpg
          // 上傳圖片路徑
          string uploadPath = Path.Combine(filePath, fileName);
          // 上傳圖片至 wwwroot/images/product
          using (var fileStream = new FileStream(uploadPath, FileMode.Create))
          {
            await file.CopyToAsync(fileStream);
          }

          ProductImage productImage = new ProductImage()
          {
            ImageUrl = $"/{productPath}/{fileName}",
            ProductId = model.Product.Id,
          };

          model.Product.ProductImages.Add(productImage);         
        }
        #region
        // 編輯時刪除舊圖片(新增時 ImageUrl 為 null)
        //if (!string.IsNullOrEmpty(productViewModel.Product.ImageUrl))
        //{
        //  // 已存在的圖片路徑
        //  string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product", productViewModel.Product.ImageUrl);
        //  //string oldImagePath = Path.Combine(rootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));

        //  if (System.IO.File.Exists(oldImagePath))
        //  {
        //    // 刪除圖片
        //    System.IO.File.Delete(oldImagePath);
        //  }
        //}

        // 儲存圖片路徑
        //productViewModel.Product.ImageUrl = fileName;
        //productViewModel.Product.ImageUrl = $@"\images\product\{fileName}";
        #endregion
      }
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
    public IActionResult DeleteGet(int? id)
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
    /// 取得所有商品(DataTable)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetAll()
    {
      List<Product> productList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
      return Json(new
      {
        data = productList
      });
    }
    /// <summary>
    /// 刪除商品
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    public IActionResult Delete(int? id)
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

      #region 刪除已存在的圖片
      string rootPath = Directory.GetCurrentDirectory();
      string productPath = $"images/products/product-{id}";
      string filePath = Path.Combine(rootPath, productPath);

      if (Directory.Exists(filePath))
      {
        string[] imagePaths = Directory.GetFiles(filePath);
        foreach (var path in imagePaths)
        {
          // 刪除圖片資料夾內所有圖片
          System.IO.File.Delete(path);
        }
        // 刪除圖片資料夾
        Directory.Delete(filePath);
      }
      // 已存在的圖片路徑
      //string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/product", product.ImageUrl);
      //string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));

      //if (System.IO.File.Exists(imagePath))
      //{
      //  // 刪除圖片
      //  System.IO.File.Delete(imagePath);
      //}
      #endregion
      _unitOfWork.Product.Remove(product);
      _unitOfWork.Save();

      return Json(new
      {
        success = true,
        message = "刪除成功"
      });
    }
    #endregion
  }
}
