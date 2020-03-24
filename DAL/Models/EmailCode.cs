using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class EmailCode
    {
        [Key]
        public int          Id { get; set; }
        public int          AuthorId { get; set; }
        [StringLength(450)]
        [DataType(DataType.EmailAddress)]
        public string       Email { get; set; }
        public int          ConfirmationNumber { get; set; }
        public bool         isExpired { get; set; }
    }
}
