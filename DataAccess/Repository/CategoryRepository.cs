using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
  public class CategoryRepository : Repository<Category>, ICategoryRepository
  {
    private readonly ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
      _db = db;
    }
    /// <summary>
    /// 修改資料
    /// </summary>
    /// <param name="category"></param>
    public void Update(Category category)
    {
      _db.Categories.Update(category);
    }
  }
}
