using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Author
    {
        [Key]
        public int      Id { get; set; }
        [StringLength(450)]
        [DataType(DataType.EmailAddress)]
        [Index(IsUnique = true)]
        public string   Email { get; set; }
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string   Password { get; set; }
        public DateTime DateJoined { get; set; }
        public bool     isAnonymous { get; set; }
        public bool     isVerified { get; set; }
        public bool     isEmailConfirmed { get; set; }
        public string   FullName { get; set; }
    }
}
