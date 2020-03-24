using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Category
    {
        [Key]
        public int      Id { get; set; }
        [StringLength(450)]
        [Index(IsUnique = true)]
        public string   Name { get; set; }
        public bool     isActive { get; set; }
    }
}
