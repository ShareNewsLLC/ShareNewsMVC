using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC.Models
{
    public class PasswordChangeModel
    {
        [Key]
        public int Id { get; set; }
        [MinLength(6)]
        public string OldPassword { get; set; }
        [MinLength(6)]
        public string NewPassword { get; set; }
        [MinLength(6)]
        public string ConfirmPassword { get; set; }
    }
}