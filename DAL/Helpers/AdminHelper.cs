using DAL.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class AdminHelper : IRepository<Admin>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<Admin> GetAll()
        {
            return db.Admins;
        }

        public Admin GetById(int id)
        {
            return db.Admins.SingleOrDefault(admin => admin.Id == id);
        }

        public void Create(Admin entity)
        {
            entity.DateJoined = DateTime.Now;
            db.Admins.Add(entity);
            db.SaveChanges();
        }

        public void Update(Admin entity)
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
