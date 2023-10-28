using System.Linq.Expressions;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        T Add(T entity);
        T Update(T entity);
        //void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        T Get(Expression<Func<T, bool>> expression);
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
