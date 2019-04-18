using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Models
{
    public class Inspection : BaseEntity
    {
        public int InspectionId { get; set; }
        public string  Username { get; set; }
        public DateTime DateChecked { get; set; }
        public string PassionBrandName { get; set; }
        public string IMAN { get; set; }
        public string PONumber { get; set; }
        public string OrderNumber { get; set; }
        public int OrderQuantity { get; set; }
        public int NumberChecked { get; set; }
        public string FactoryName { get; set; }
        public string Model { get; set; }
        public string  Description { get; set; }
        public int OrderType { get; set; }
        public List<InspectionMistakeDetail> MistakeLines { get; set; }
        public string Parameter { get; set; }
        public string CustomerComment { get; set; }
        public int Result { get; set; }
        public string FactoryComment { get; set; }
        public bool BookingStatus { get; set; }
        public bool InspectStatus { get; set; }
        public DateTime InspectDate { get; set; }
        public bool FinalStatus { get; set; }
        public DateTime FinalDate { get; set; }
        public DateTime Final { get; set; }
        public virtual PassionBrand PassionBrand { get; set; }
        public int PassionBrandId { get; set; }
        public string ApproveUsername { get; set; }
        public int? FactoryId { get; set; }
        public virtual Factory Factory { get; set; }
        public string UserBookingId { get; set; }
        public virtual ApplicationUser UserBooking { get; set; }
        public virtual FinalWeek FinalWeek { get; set; }
        public int? FinalWeekId { get; set; }
        public int? TechManagerId { get; set; }
        public virtual TechManager TechManager { get; set; }
        public string TechManagerName { get; set; }
        public int? ProductQuantityChecked { get; set; }
        public bool Faked { get; set; }
        public bool? IsThirdParty { get; set; }
    }
}
