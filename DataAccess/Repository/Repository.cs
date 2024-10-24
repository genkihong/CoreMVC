using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repository
{
  /// <summary>
  /// 泛型類別實作泛型介面
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class Repository<T> : IRepository<T> where T : class
  {
    private readonly ApplicationDbContext _db;
    internal DbSet<T> dbSet;
    public Repository(ApplicationDbContext db)
    {
      _db = db;
      //_db.Categories == dbSet
      dbSet = _db.Set<T>();
      _db.Products.Include(p => p.Category);
    }
    /// <summary>
    /// 新增資料
    /// </summary>
    /// <param name="entity"></param>
    public void Add(T entity)
    {
      dbSet.Add(entity);
    }
    /// <summary>
    /// 取得全部資料
    /// </summary>
    /// <returns></returns>
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
    {
      IQueryable<T> query = dbSet;

      if (filter != null)
      {
        query = query.Where(filter);
      }

      if (!string.IsNullOrEmpty(includeProperties))
      {
        // 以字元','做為分隔符號切割字串，StringSplitOptions.RemoveEmptyEntries 為不回傳空字串
        string[] properties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var includeProp in properties )
        {
          query = query.Include(includeProp);
        }
      }

      return query.ToList();
    }
    /// <summary>
    /// 取得一筆資料
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
    {
      IQueryable<T> query;

      if (tracked)
      {
        query = dbSet;
      }
      else
      {
        query = dbSet.AsNoTracking();
      }

      query = query.Where(filter);

      if (!string.IsNullOrEmpty(includeProperties))
      {
        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
          query = query.Include(includeProp);
        }
      }

      return query.FirstOrDefault();
    }
    /// <summary>
    /// 刪除一筆資料
    /// </summary>
    /// <param name="entity"></param>
    public void Remove(T entity)
    {
      dbSet.Remove(entity);
    }
    /// <summary>
    /// 刪除全部資料
    /// </summary>
    /// <param name="entities"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void RemoveRange(IEnumerable<T> entities)
    {
      dbSet.RemoveRange(entities);
    }
  }
}
