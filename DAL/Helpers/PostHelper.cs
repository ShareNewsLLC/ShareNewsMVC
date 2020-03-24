using DAL.Models;
using System;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class PostHelper : IRepository<Post>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<Post> GetAll()
        {
            return db.Posts;
        }

        public Post GetById(int id)
        {
            return db.Posts.SingleOrDefault(post => post.Id == id);
        }

        public void Create(Post entity)
        {
            entity.DateCreated = DateTime.Now;
            entity.LastModified = DateTime.Now;
            entity.isActive = false;

            db.Posts.Add(entity);
            db.SaveChanges();
        }

        public void Update(Post entity)
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
