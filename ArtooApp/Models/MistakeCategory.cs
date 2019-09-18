using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Artoo.Models
{
    public class MistakeCategory : BaseEntity
    {
        public int MistakeCategoryID { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Xin vui lòng nhập tên loại lỗi")]
        [Display(Name = "Nhóm lỗi")]
        public string Name { get; set; }
        public DateTime DateRegister { get; set; }
        [Display(Name = "Loại lỗi")]
        public int MistakeType { get; set; }
        public virtual ICollection<Mistake> Mistakes { get; set; }
    }
}
