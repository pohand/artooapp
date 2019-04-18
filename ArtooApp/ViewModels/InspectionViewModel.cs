using Artoo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.ViewModels
{
    public class InspectionViewModel
    {
        public int InspectionId { get; set; }
        public string Username { get; set; }
        public DateTime DateChecked { get; set; }
        public string PassionBrandName { get; set; }
        public string IMAN { get; set; }
        public string PONumber { get; set; }
        public string OrderNumber { get; set; }
        public int OrderQuantity { get; set; }

        [Required(ErrorMessage = "Please select số lượng kiểm tra")]
        [Display(Name = "Số lượng kiểm")]
        public int NumberChecked { get; set; }
        public string FactoryName { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public bool InspectStatus { get; set; }
        public DateTime InspectDate { get; set; }
        public bool FinalStatus { get; set; }
        public string FinalDate { get; set; }
        public DateTime Final { get; set; }
        public int OrderType { get; set; }
        public List<SelectListItem> ManualMistake { get; set; }
        public List<SelectListItem> DeviceMistake { get; set; }

        [Required(ErrorMessage = "Please select thông số")]
        [Display(Name = "Thông số")]
        public string Parameter { get; set; }

        [Required(ErrorMessage = "Please select Result")]
        [Display(Name = "Result")]
        public int Result { get; set; }
        public string OrderTypeName { get; set; }
        public string CustomerComment { get; set; }
        public string FactoryComment { get; set; }
        public List<int> ManualMistakeIds { get; set; }
        public List<int> DeviceMistakeIds { get; set; }
        public List<SelectListItem> PassionBrands { get; set; }
        public int PassionBrandId { get; set; }

        public List<MistakeViewModel> ManualMistakeList { get; set; }
        public List<MistakeViewModel> DeviceMistakeList { get; set; }
        public bool BookingStatus { get; set; }
        public string ResultString { get; set; }
        public string ApproveUsername { get; set; }
        public string ManualMistakeString { get; set; }
        public string DeviceMistakeString { get; set; }
        public virtual FinalWeek FinalWeek { get; set; }
        public int? FinalWeekId { get; set; }
        public int ItemIndex { get; set; }
        public string TechManagerName { get; set; }
        public int? ProductQuantityChecked { get; set; }

        public string UserBookingId { get; set; }
        public string UserBooking { get; set; }

        public Boolean IsThirdParty { get; set; }
        public string ThirdPartyDate { get; set; }
    }
}
