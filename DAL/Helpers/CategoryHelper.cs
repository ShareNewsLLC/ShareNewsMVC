using DAL.Models;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class CategoryHelper : IRepository<Category>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<Category> GetAll()
        {
            return db.Categories;
        }

        public Category GetById(int id)
        {
            return db.Categories.SingleOrDefault(category => category.Id == id);
        }

        public void Create(Category entity)
        {
            db.Categories.Add(entity);
            db.SaveChanges();
        }

        public void Update(Category entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            db.Entry(GetById(id)).State = EntityState.Deleted;
            db.SaveChanges();
        }
    }
}
