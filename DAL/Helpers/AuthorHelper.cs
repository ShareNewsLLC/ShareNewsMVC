using DAL.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class AuthorHelper : IRepository<Author>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<Author> GetAll()
        {
            return db.Authors;
        }

        public Author GetById(int id)
        {
            return db.Authors.SingleOrDefault(author => author.Id == id);
        }

        public void Create(Author entity)
        {
            entity.DateJoined = DateTime.Now;
            db.Authors.Add(entity);
            db.SaveChanges();
        }

        public void Update(Author entity)
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
