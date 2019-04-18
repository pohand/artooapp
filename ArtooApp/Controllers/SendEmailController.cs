using Artoo.Common;
using Artoo.Helpers;
using Artoo.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Controllers
{
    public class SendEmailController : Controller
    {
        private readonly IEmailRepository _emailRepository;
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IMistakeRepository _mistakeRepository;
        public SendEmailController(IInspectionRepository inspectionRepository, 
            IMistakeRepository mistakeRepository, 
            IEmailRepository emailRepository)
        {
            _inspectionRepository = inspectionRepository;
            _mistakeRepository = mistakeRepository;
            _emailRepository = emailRepository;
        }

        public async Task<IActionResult> SendInsepectionEmail(int id)
        {
            var inspection = _inspectionRepository.GetInspectionById(id);
            var manualMistakeString = string.Empty;
            var deviceMistakeString = string.Empty;
            var manualMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.ManualMistake).ToList();
            if (manualMistakeList != null)
            {
                manualMistakeString = "<ul>";

                foreach (var mistake in manualMistakeList)
                {
                    manualMistakeString += "<li>" + mistake.Name + "(" + mistake.Quantity + ")";
                    manualMistakeString += "<br /></ li>";
                }
                manualMistakeString += "</ul>";
            }

            var deviceMistakeList = _mistakeRepository.GetMistakeDetailByInspectionId(inspection.InspectionId).Where(x => x.ManualType == (int)MistakeEnum.DeviceMistake).ToList();
            if (deviceMistakeList.Count > 0)
            {
                deviceMistakeString = "<ul>";

                foreach (var mistake in deviceMistakeList)
                {
                    deviceMistakeString += "<li>" + mistake.Name + "(" + mistake.Quantity + ")";
                    deviceMistakeString += "<br /></li>";
                }
                deviceMistakeString += "</ul>";
            }
            var mailbody = $@"Gửi anh chị quản lý chất lượng và quản lý xưởng,
                            Đây là kết quả Final Inspection do QPL đã thực hiện.<br />
                            Nhà máy vui lòng xác nhận bằng cách reply mail này.
                            <div style='text-align:center;font-size:14px;font-family:arial;font-weight:bold'>
                            <table style='font-family:arial;font-size:14px;border-radius:2px;margin:50px 0 25px;margin-left:20px;margin-right:20px;min-width:400px;border:1px solid #eee' cellspacing='0' cellpadding='10' border='0'>
                             <tbody>
                              <tr style='background:#fbfbfb'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Người kiểm</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.Username}</td>
                              </tr>
                              <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Ngày kiểm</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.InspectDate}</td>
                              </tr>
                              <tr style='background:#fbfbfb'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Passion Brand</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.PassionBrandName}</td>
                              </tr>
                               <tr style='background:#f9f9f9'><td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>IMAN</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.IMAN}</td>
                              </tr>
                              <tr style='background:#fbfbfb'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Model</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.Model}</td>
                              </tr>
                              <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Số PO</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.OrderNumber}</td>
                              </tr>
                              <tr style='background:#fbfbfb'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Số lượng kiểm</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.NumberChecked}</td>
                              </tr>
                              <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Xưởng sản xuất</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.FactoryName}</td>
                              </tr>
                              <tr style='background:#fbfbfb'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Loại đơn hàng</td>
                               <td style='border-bottom:1px solid #eee'>{((OrderType)inspection.OrderType).ToString()}</td>
                              </tr>
                              <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Kiểm tra bằng tay và mắt</td>
                               <td style='border-bottom:1px solid #eee'>{manualMistakeString}</td>
                              </tr>
                              <tr style='background:#fbfbfb'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Kiểm tra bằng thiết bị </td>
                               <td style='border-bottom:1px solid #eee'>{deviceMistakeString}</td>
                              </tr>
                              <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Thông số</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.Parameter}</td>
                              </tr>		
                                    <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Khách hàng góp ý</td>
                               <td style='border-bottom:1px solid #eee'>{inspection.CustomerComment}</td>
                              </tr>
                              <tr style='background:#f9f9f9'>
                               <td style='font-weight:bold;border-bottom:1px solid #eee;width:420px'>Result</td>
                               <td style='border-bottom:1px solid #eee'>{((InspectionResultEnum)inspection.Result).ToString()}</td>
                              </tr>		
                             </tbody>
                            </table></div>";

            EmailSender sender = new EmailSender();
            var subject = $@"[{((OrderType)inspection.OrderType).ToString()}] Kết quả kiểm Final: {inspection.PassionBrandName} - [{((InspectionResultEnum)inspection.Result).ToString()}] - IMAN: {inspection.IMAN} - Model: {inspection.Model} - PO: {inspection.OrderNumber}";
            var mailList = _emailRepository.GetEmailByBrandResult(inspection.PassionBrandId, (InspectionResultEnum)inspection.Result).ToList();
            if(mailList != null)
            {
                await sender.SendEmailAsync(mailList.Select(x => x.EmailAddress).ToList(), subject, mailbody);
            }            
            return RedirectToAction("Report", "Inspection");
        }
    }
}
