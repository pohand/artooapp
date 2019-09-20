using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Artoo.Models;
using Artoo.Models.FileInputModel;
using Artoo.IRepositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using Microsoft.AspNetCore.Identity;
using Artoo.Infrastructure;
using Artoo.IServices;
using Artoo.Common;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Artoo.Controllers
{
    [Authorize]
    [ServiceFilter(typeof(TenantAttribute))]
    public class InspectionImportController : Controller
    {
        private readonly IFileProvider _fileProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IPassionBrandRepository _passionBrandRepository;
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IFactoryRepository _factoryRepository;
        private readonly IFinalWeekRepository _finalWeekRepository;
        private readonly ITechManagerRepository _techManagerRepository;
        private readonly UserManager<ApplicationUser> _userManager;      

        private readonly IInspectionImportService _inspectionImportService;
        private Tenant _tenant;

        public InspectionImportController(AppDbContext appDbContext,
            IFileProvider fileProvider,
            IPassionBrandRepository passionBrandRepository,
            IInspectionRepository inspectionRepository,
            IFactoryRepository factoryRepository,
            IFinalWeekRepository finalWeekRepository,
            ITechManagerRepository techManagerRepository,
            IInspectionImportService inspectionImportService,
            UserManager<ApplicationUser> userManager)
        {
            _fileProvider = fileProvider;
            _appDbContext = appDbContext;
            _passionBrandRepository = passionBrandRepository;
            _inspectionRepository = inspectionRepository;
            _factoryRepository = factoryRepository;
            _finalWeekRepository = finalWeekRepository;
            _techManagerRepository = techManagerRepository;
            _inspectionImportService = inspectionImportService;
            _userManager = userManager;
            //_tenant = new Tenant();
        }
        public IActionResult Index()
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            var model = new FilesViewModel();
            var physicalLocation = _fileProvider.GetDirectoryContents("FileUploads" + "\\" + _tenant.HostName);
            foreach (var item in physicalLocation)
            {
                model.Files.Add(
                    new FileDetails { Name = item.Name, Path = item.PhysicalPath });
            }
            return View(model);
        }


        public void UpdateFactory()
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            var dt = DateTime.Now;
            foreach (string path in Directory.EnumerateFiles(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\FileUploads" + "\\" + _tenant.HostName), "*", SearchOption.AllDirectories))
            {
                FileInfo fileInfo = new FileInfo(path);

                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    List<Inspection> inspectionList = new List<Inspection>();
                    int workSheetTotal = package.Workbook.Worksheets.Count;
                    try
                    {
                        for (int j = 1; j <= workSheetTotal; j++)
                        {
                            ExcelWorksheet workSheet = package.Workbook.Worksheets[j];
                            if (workSheet.Dimension != null)
                            {
                                int totalRows = workSheet.Dimension.Rows;

                                var factories = _factoryRepository.Factories;
                                for (int i = 2; i <= totalRows; i++)
                                {
                                    var inspection = new Inspection();
                                    inspection.FactoryName = AssignCell(i, workSheet, "Factory");
                                    inspection.OrderNumber = AssignCell(i, workSheet, "Order number");

                                    var factory = factories.FirstOrDefault(x => x.Name.Contains(inspection.FactoryName));
                                    if (factory != null)
                                    {
                                        inspection.FactoryId = factory.FactoryId;
                                    }
                                    else
                                    {
                                        var newFactory = new Factory()
                                        {
                                            Name = inspection.FactoryName
                                        };

                                        inspection.FactoryId = _factoryRepository.CreateFactory(newFactory);
                                    }
                                    inspectionList.Add(inspection);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                    _inspectionRepository.UpdateFactorty(inspectionList);

                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            var username = await _userManager.GetUserAsync(User);
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var dt = DateTime.Now;

            //repare path for file
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\FileUploads" + "\\" + _tenant.HostName,
                        DateTime.Now.ToString("d").Replace("/", "-") + "-" + GetRandomNumber(0, 10) + "-" + file.GetFilename());

            //input excel file to system in order read it
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            FileInfo fileInfo = new FileInfo(path);

            //check whether the  file is excel or not
            try
            {
                ExcelPackage package = new ExcelPackage(fileInfo);
            }
            catch
            {
                //if file is not excel file, delete it
                System.IO.File.Delete(path);
                TempData["Message"] = "Please upload excel file picture with *.xslx";
                return RedirectToAction("Index");
            }

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                List<Inspection> inspectionList = new List<Inspection>();
                //count how many worksheet in excel file
                int workSheetTotal = package.Workbook.Worksheets.Count;
                try
                {
                    //loop all worksheet in excel file
                    for (int j = 1; j <= workSheetTotal; j++)
                    {
                        if (_tenant.TenantId == (int)TenantEnum.garmex)
                        {
                            _inspectionImportService.ImportItemByGarmexTenant(username, package, inspectionList, j);
                        }
                        else
                        {
                            _inspectionImportService.ImportItem(username, package, inspectionList, j);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                _inspectionRepository.InsertList(inspectionList);

                return RedirectToAction("Index");
            }
        }
                
        public IActionResult RemoveDB(string filename)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot\\FileUploads" + "\\" + _tenant.HostName, filename);
            FileInfo fileInfo = new FileInfo(path);

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                int workSheetTotal = package.Workbook.Worksheets.Count;
                try
                {
                    List<string> OrderNumberList = new List<string>();
                    for (int j = 1; j <= workSheetTotal; j++)
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[j];
                        if (workSheet.Dimension != null)
                        {
                            int totalRows = workSheet.Dimension.Rows;

                            for (int i = 2; i <= totalRows; i++)
                            {
                                OrderNumberList.Add(AssignCell(i, workSheet, "Order number"));
                            }
                        }
                    }

                    _inspectionRepository.DeleteInspectByOrderNumbers(OrderNumberList);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                return RedirectToAction("Index");
            }
        }

        private string AssignCell(int position, ExcelWorksheet ws, string columnName)
        {
            if (GetColumnByName(ws, columnName) != 0)
            {
                var cell = ws.Cells[position, GetColumnByName(ws, columnName)].Value;
                return cell == null ? null : cell.ToString();
            }
            return null;
        }

        private int GetColumnByName(ExcelWorksheet ws, string columnName)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            var cell = ws.Cells["1:1"].Any(c => c.Value.ToString() == columnName);
            if (cell)
            {
                return ws.Cells["1:1"].First(c => c.Value.ToString() == columnName).Start.Column;
            }
            return 0;
        }

        private static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            lock (getrandom) // synchronize
            {
                return getrandom.Next(min, max);
            }
        }
        public IActionResult Files()
        {
            var model = new FilesViewModel();
            foreach (var item in _fileProvider.GetDirectoryContents(""))
            {
                model.Files.Add(
                    new FileDetails { Name = item.Name, Path = item.PhysicalPath });
            }
            return View(model);
        }

        public async Task<IActionResult> Download(string filename)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot\\FileUploads" + "\\" + _tenant.HostName, filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }

        public IActionResult Delete(string filename)
        {
            if (RouteData != null)
            {
                _tenant = (Tenant)RouteData.Values.SingleOrDefault(r => r.Key == "tenant").Value;
            }

            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot\\FileUploads" + "\\" + _tenant.HostName, filename);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            return RedirectToAction("Index");
        }

        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                //{".txt", "text/plain"},
                //{".pdf", "application/pdf"},
                //{".doc", "application/vnd.ms-word"},
                //{".docx", "application/vnd.ms-word"},
                //{".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
                //{".png", "image/png"},
                //{".jpg", "image/jpeg"},
                //{".jpeg", "image/jpeg"},
                //{".gif", "image/gif"},
                //{".csv", "text/csv"}
            };
        }
    }
}
