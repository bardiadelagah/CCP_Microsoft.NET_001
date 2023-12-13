using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        //T - Category
        IEnumerable<T> GetAll();
        T Get(Expression<Func<T, bool>> filter); //T GetFirstOrDefault();
        void Add(T entity);
        // void Update(T entity);
        void Remove(T entity); //Delete(T entity)
        void Remove(IEnumerable<T> entity); //eleteRange(IEnumerable<T> entity)

    }
}
