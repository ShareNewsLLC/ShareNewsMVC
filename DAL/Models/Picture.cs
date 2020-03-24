using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Picture
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public byte[] Source { get; set; }
    }
}
