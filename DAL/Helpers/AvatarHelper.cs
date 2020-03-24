using DAL.Models;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class AvatarHelper : IRepository<Avatar>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<Avatar> GetAll()
        {
            return db.Avatars;
        }

        public Avatar GetById(int id)
        {
            return db.Avatars.SingleOrDefault(ava => ava.AuthorId == id);
        }

        public void Create(Avatar entity)
        {
            db.Avatars.Add(entity);
            db.SaveChanges();
        }

        public void Update(Avatar entity)
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
