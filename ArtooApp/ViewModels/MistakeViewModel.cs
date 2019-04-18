using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class MistakeViewModel
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
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        public int? Quantity { get; set; }
    }
}
