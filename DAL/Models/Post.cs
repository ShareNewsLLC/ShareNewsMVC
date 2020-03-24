using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Post
    {
        [Key]
        public int          Id { get; set; }
        public int          CategoryId { get; set; }
        public string       Title { get; set; }
        public int          AuthorId { get; set; }
        public DateTime     DateCreated { get; set; }
        public DateTime     LastModified { get; set; }
        public List<string> Hashtags { get; set; }
        public string       Article { get; set; }
        public bool         isActive { get; set; }
    }
}
