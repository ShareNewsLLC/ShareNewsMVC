using DAL.Models;
using System.Data.Entity;
using System.Linq;

namespace DAL.Helpers
{
    public class EmailCodeHelper : IRepository<EmailCode>
    {
        private ContextDB db = new ContextDB();

        public IQueryable<EmailCode> GetAll()
        {
            return db.EmailCodes;
        }

        public EmailCode GetById(int id)
        {
            return db.EmailCodes.SingleOrDefault(code => code.Id == id);
        }

        public void Create(EmailCode entity)
        {
            db.EmailCodes.Add(entity);
            db.SaveChanges();
        }

        public void Update(EmailCode entity)
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
