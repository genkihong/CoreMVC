using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
	/// <summary>
	/// 泛型介面，定義共用的方法
	/// where T : class 型別參數必須是Reference type，也就是預設為Pass by reference的型別
    /// 像string, object，以及class, interface, delegate宣告的型別
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IRepository<T> where T : class
    {
        //T - Category
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T, bool>> filter);
        void Add(T entity);        
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}
