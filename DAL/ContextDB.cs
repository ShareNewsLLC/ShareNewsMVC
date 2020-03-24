namespace DAL
{
    using DAL.Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class ContextDB : DbContext
    {
        public ContextDB()
            : base("name=ContextDB")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Post> Posts { get; set; }
        public virtual DbSet<Picture> Pictures { get; set; }
        public virtual DbSet<Avatar> Avatars { get; set; }
        public virtual DbSet<EmailCode> EmailCodes { get; set; }
    }
}