﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Artoo.Models
{
    public class Email : BaseEntity
    {
        public int EmailId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }

        [Required(ErrorMessage = "Xin vui lòng nhập email")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        public DateTime DateRegister { get; set; }
    }
}
