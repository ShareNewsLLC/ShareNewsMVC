using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Avatar
    {
        [Key]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public byte[] Source { get; set; }
    }
}
