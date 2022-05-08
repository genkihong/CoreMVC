using Microsoft.AspNetCore.Mvc;
using MyCoreMVC.Data;
using MyCoreMVC.Models;

namespace MyCoreMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categoryList = _db.Categories;
            return View(categoryList);
        }
        //新增 GET
        public IActionResult Create()
        {
            return View();
        }
        //新增 POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "名稱和數量相同!");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();
                TempData["success"] = "已新增";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //修改 GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _db.Categories.Find(id);
            //var categoryFirst = _db.Categories.FirstOrDefault(c => c.Id == id);
            //var categorySingle = _db.Categories.SingleOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        //修改 POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "名稱和數量相同!");
            }
            if (ModelState.IsValid)
            {
                _db.Categories.Update(category);
                _db.SaveChanges();
                TempData["success"] = "已修改";
                return RedirectToAction("Index");
            }
            return View(category);
        }
        //刪除 GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _db.Categories.Find(id);
            //var categoryFirst = _db.Categories.FirstOrDefault(c => c.Id == id);
            //var categorySingle = _db.Categories.SingleOrDefault(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        //刪除 POST
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            _db.Categories.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "已刪除";
            return RedirectToAction("Index");
        }
    }
}
