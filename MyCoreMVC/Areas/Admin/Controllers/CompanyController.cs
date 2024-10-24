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
  public class CompanyController : Controller
  {
    private readonly IUnitOfWork _unitOfWork;
    public CompanyController(IUnitOfWork unitOfWork)
    {
      _unitOfWork = unitOfWork;
    }
    /// <summary>
    /// 取得全部資料
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
      List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
      return View(companyList);
    }
    /// <summary>
    /// 新增/修改 Get
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IActionResult Upsert(int? id)
    {
      //新增
      if (id == null || id == 0)
      {
        return View(new Company());
      }
      //修改
      Company company = _unitOfWork.Company.Get(c => c.Id == id);
      return View(company);
    }
    /// <summary>
    /// 新增/修改 Post
    /// </summary>
    /// <param name="company"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Upsert(Company company)
    {
      if (ModelState.IsValid)
      {
        if (company.Id == 0)
        {
          //新增
          _unitOfWork.Company.Add(company);
          TempData["success"] = "新增成功";
        }
        else
        {
          //修改
          _unitOfWork.Company.Update(company);
          TempData["success"] = "修改成功";
        }

        _unitOfWork.Save();
        return RedirectToAction("Index");
      }
      return View(company);
    }
    #region API
    /// <summary>
    /// 取得所有公司(DataTable)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult GetAll()
    {
      List<Company> companyList = _unitOfWork.Company.GetAll().ToList();
      return Json(new
      {
        data = companyList
      });
    }
    /// <summary>
    /// 刪除公司
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

      var company = _unitOfWork.Company.Get(c => c.Id == id);

      if (company == null)
      {
        return Json(new
        {
          success = false,
          message = "無法刪除"
        });
      }

      _unitOfWork.Company.Remove(company);
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
