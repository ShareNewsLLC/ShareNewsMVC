using System.Linq;

namespace DAL.Helpers
{
    public interface IRepository<T> where T : class
    {

        IQueryable<T> GetAll();

        T GetById(int id);

        void Create(T entity);

        void Update(T entity);

        void Delete(int id);

    }
}
