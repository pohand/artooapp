using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DinkToPdf;
using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using Artoo.IRepositories;
using Artoo.Common;
using DinkToPdf.Contracts;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    public class PdfController : Controller
    {
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IMistakeRepository _mistakeRepository;
        private IConverter _converter;

        public PdfController(IInspectionRepository inspectionRepository, IMistakeRepository mistakeRepository,
            IConverter converter)
        {
            _inspectionRepository = inspectionRepository;
            _mistakeRepository = mistakeRepository;
            _converter = converter;
        }

        [HttpGet]
        public IActionResult PrintPdf(int id)
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
            var document = $@"<div style='text-align:center;font-size:24px;font-family:arial;font-weight:bold'>
                            <table style='font-family:arial;font-size:24px;border-radius:2px;margin:50px 0 25px;margin-left:20px;margin-right:20px;min-width:400px;border:1px solid #eee' cellspacing='0' cellpadding='10' border='0'>
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
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\lib\\libwkhtmltox.dll");

            CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(path);

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4Plus,
                    },

                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = document,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    }
                }
            };

            byte[] pdf = _converter.Convert(doc);


            return new FileContentResult(pdf, "application/pdf");
        }

        public IActionResult Index()
        {
            return View();
        }

        internal class CustomAssemblyLoadContext : AssemblyLoadContext
        {
            public IntPtr LoadUnmanagedLibrary(string absolutePath)
            {
                return LoadUnmanagedDll(absolutePath);
            }
            protected override IntPtr LoadUnmanagedDll(String unmanagedDllName)
            {
                return LoadUnmanagedDllFromPath(unmanagedDllName);
            }

            protected override Assembly Load(AssemblyName assemblyName)
            {
                throw new NotImplementedException();
            }
        }
    }
}
