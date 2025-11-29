using UEFASwissFormatSelector.Models;

namespace UEFASwissFormatSelector.Services
{
    public class MockUnitRepository<T> : List<T>, IUnitRepository<T> where T : Identifiable
    {
        public T? Find(Guid id)
        {
            return this.Where( e => e.Id == id).FirstOrDefault();
        }

        //public Task<IEnumerable<T>?> GetAll(Expression<Func<T, bool>> filter, string[] includes = null)
        //{
        //    var func = filter as Func<T, bool>; 
        //    return this.Where<T>(func).FirstOrDefault();
        //}

        public void Update(T entity)
        {
            var dbEntity = Find(entity.Id);
            if (dbEntity != null)
            {
                (this as List<T>)![this.ToList().IndexOf(dbEntity)] = entity;
            }
        }        
    }
}
