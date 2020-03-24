using DAL.Models;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class PictureHelper : IRepository<Picture>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<Picture> GetAll()
        {
            return db.Pictures;
        }

        public Picture GetById(int id)
        {
            return db.Pictures.SingleOrDefault(pic => pic.PostId == id);
        }

        public void Create(Picture entity)
        {
            db.Pictures.Add(entity);
            db.SaveChanges();
        }

        public void Update(Picture entity)
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
