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
        /// 取得一筆資料
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public T Get(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            return query.FirstOrDefault();
        }
        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAll()
        {
            IQueryable<T> query = dbSet;
            return query.ToList();
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
