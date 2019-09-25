using System;
using System.ComponentModel.DataAnnotations;

namespace Artoo.Models
{
    public class Mistake : BaseEntity
    {
        public int MistakeId { get; set; }

        [Required(ErrorMessage = "Xin vui lòng nhập tên lỗi")]
        [Display(Name = "Lỗi")]
        public string Name { get; set; }

        [Display(Name = "Loại lỗi")]
        public int ManualType { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }
        public bool Status { get; set; }
        public DateTime DateRegister { get; set; }
        public string ImageUrl { get; set; }
        public int? MistakeCategoryID { get; set; }
        public virtual MistakeCategory MistakeCategory { get; set; }
    }
}
