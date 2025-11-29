using System.Linq.Expressions;
using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public interface IUnitRepository<T> : IEnumerable<T> where T : Identifiable
    {
        T? Find(Guid id);
        //Task<T> Get(Expression<Func<T, bool>> filter, string[] includes = null);
        //Task<IEnumerable<T>> GetAll(string[] includes = null, int count = 20);
        //Task<IEnumerable<T>?> GetAll(Expression<Func<T, bool>> filter, string[] includes = null);
        void Add(T entity);
        bool Remove(T entity);
        void Update(T entity);
    }
}
