using DataAccess.Repository.IRepository;
using DataAcess.Data;
using Microsoft.AspNetCore.Mvc;
using Models;


namespace MyCoreMVC.Controllers
{
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _db;
        //public CategoryController(ApplicationDbContext db)
        //{
        //    _db = db;
        //}
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository db)
        {
            _categoryRepository = db;
        }
        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            //IEnumerable<Category> categoryList = _db.Categories;
            List<Category> categoryList = _categoryRepository.GetAll().ToList();
            return View(categoryList);
        }
        /// <summary>
        /// 顯示新增資料頁
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }
        //送出新增資料
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
                //_db.Categories.Add(category);
                //_db.SaveChanges();
                _categoryRepository.Add(category);
                _categoryRepository.Save();
                TempData["success"] = "已新增";
                return RedirectToAction("Index");
            }
            return View(category);
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
            //var category = _db.Categories.Find(id);
            //var categoryFirst = _db.Categories.FirstOrDefault(c => c.Id == id);
            //var categorySingle = _db.Categories.SingleOrDefault(c => c.Id == id);
            Category category = _categoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        /// <summary>
        /// 送出修改資料
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
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
                //_db.Categories.Update(category);
                //_db.SaveChanges();
                _categoryRepository.Update(category);
                _categoryRepository.Save();
                TempData["success"] = "已修改";
                return RedirectToAction("Index");
            }
            return View(category);
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
            //var category = _db.Categories.Find(id);
            //var categoryFirst = _db.Categories.FirstOrDefault(c => c.Id == id);
            //var categorySingle = _db.Categories.SingleOrDefault(c => c.Id == id);
            Category category = _categoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
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
            Category category = _categoryRepository.Get(c => c.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            //_db.Categories.Remove(obj);
            //_db.SaveChanges();
            _categoryRepository.Remove(category);
            _categoryRepository.Save();
            TempData["success"] = "已刪除";
            return RedirectToAction("Index");
        }
    }
}
