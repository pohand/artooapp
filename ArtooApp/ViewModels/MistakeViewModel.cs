using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        public List<SelectListItem> MistakeCategoryList { get; set; }
        public int? SelectedMistakeCategoryId { get; set; }
    }
}
