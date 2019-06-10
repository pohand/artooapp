using Artoo.Helpers;
using Artoo.IRepositories;
using Artoo.IServices;
using Artoo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Artoo.Services
{
    public class InspectionImportService : IInspectionImportService
    {
        #region Variables
        private readonly IFileProvider _fileProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IPassionBrandRepository _passionBrandRepository;
        private readonly IInspectionRepository _inspectionRepository;
        private readonly IFactoryRepository _factoryRepository;
        private readonly IFinalWeekRepository _finalWeekRepository;
        private readonly ITechManagerRepository _techManagerRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        #endregion
        public InspectionImportService(AppDbContext appDbContext,
            IFileProvider fileProvider,
            IPassionBrandRepository passionBrandRepository,
            IInspectionRepository inspectionRepository,
            IFactoryRepository factoryRepository,
            IFinalWeekRepository finalWeekRepository,
            ITechManagerRepository techManagerRepository,
            UserManager<ApplicationUser> userManager)
        {
            _fileProvider = fileProvider;
            _appDbContext = appDbContext;
            _passionBrandRepository = passionBrandRepository;
            _inspectionRepository = inspectionRepository;
            _factoryRepository = factoryRepository;
            _finalWeekRepository = finalWeekRepository;
            _techManagerRepository = techManagerRepository;
            _userManager = userManager;
        }

        public bool ImportItem(ApplicationUser username, ExcelPackage package, List<Inspection> inspectionList, int j)
        {
            ExcelHandlingHelper excelHandling = new ExcelHandlingHelper();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[j];
            //read every column in each worksheet
            if (workSheet.Dimension != null)
            {
                int totalRows = workSheet.Dimension.Rows;

                var passionBrands = _passionBrandRepository.PassionBrands;
                var factories = _factoryRepository.Factories;
                var finalWeeks = _finalWeekRepository.FinalWeeks;
                var techManagers = _techManagerRepository.TechManagers;
                for (int i = 2; i <= totalRows; i++)
                {
                    if (workSheet.Cells[i, 1].Value == null)
                    {
                        continue;
                    }
                    var inspection = new Inspection();
                    inspection.FactoryName = excelHandling.AssignCell(i, workSheet, "Factory");
                    inspection.OrderNumber = excelHandling.AssignCell(i, workSheet, "Order number");
                    inspection.IMAN = excelHandling.AssignCell(i, workSheet, "Iman");
                    inspection.Model = excelHandling.AssignCell(i, workSheet, "Model");
                    inspection.PassionBrandName = excelHandling.AssignCell(i, workSheet, "Passion brand");
                    inspection.Description = excelHandling.AssignCell(i, workSheet, "Description");
                    inspection.OrderQuantity = excelHandling.AssignCell(i, workSheet, "Ord Q'ty") != null ? int.Parse(excelHandling.AssignCell(i, workSheet, "Ord Q'ty").Trim()) : 0;
                    inspection.OrderType = excelHandling.AssignCell(i, workSheet, "Implantation") == null ? 0 : (excelHandling.AssignCell(i, workSheet, "Implantation").Trim().ToUpper() == "YES" ? 1 : 0);
                    inspection.TechManagerName = excelHandling.AssignCell(i, workSheet, "KĨ THUẬT TRƯỞNG");

                    //Add finalweek into db, finalweek is based on Final column in excel file. It must be unique. If it is existing, get existing id. 
                    double finalDate;
                    int weekOfYear = 0;
                    var importHelper = new ImportHelper();
                    if (double.TryParse(excelHandling.AssignCell(i, workSheet, "Final"), out finalDate))
                    {
                        inspection.FinalDate = importHelper.FromOADate(finalDate);
                        weekOfYear = importHelper.GetIso8601WeekOfYear(inspection.FinalDate);
                    }

                    if (weekOfYear != 0)
                    {
                        //the format standard is Year + -W + WeekNumberOfYear, for example: 2018-W14
                        var finalWeekName = inspection.FinalDate.Year.ToString() + "-W" + weekOfYear;
                        var existingFinalWeek = finalWeeks.FirstOrDefault(x => x.Name == finalWeekName);
                        //if finalweek is existing, get the existing FinalWeekID and assign it to inspection
                        if (existingFinalWeek != null)
                        {
                            inspection.FinalWeekId = existingFinalWeek.FinalWeekId;
                        }
                        //if finalweek is NOT existing, create new one and assign this to inspection
                        else
                        {
                            var newFinalWeek = new FinalWeek()
                            {
                                Week = weekOfYear,
                                Name = finalWeekName,
                                Year = Int32.Parse(inspection.FinalDate.Year.ToString()),
                                Description = inspection.FinalDate.Year.ToString() + "-W" + weekOfYear,
                                FinalWeekDay = inspection.FinalDate,
                                DateRegister = DateTime.Now
                            };

                            int finalWeekId = _finalWeekRepository.CreateFinalWeek(newFinalWeek);
                            inspection.FinalWeekId = finalWeekId;
                        }
                    }

                    //Add passionBranch into db, passionBranch is based on Passion Branch column in excel file. It must be unique. If it is existing, get existing id.
                    var brand = passionBrands.FirstOrDefault(x => x.Name.Contains(inspection.PassionBrandName));
                    //var brand = passionBrands.FirstOrDefault(x => x.Name.IndexOf(inspection.PassionBrandName, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (brand != null)
                    {
                        inspection.PassionBrandId = brand.PassionBrandId;
                    }
                    else
                    {
                        var passionBrand = new PassionBrand()
                        {
                            Name = inspection.PassionBrandName
                        };
                        inspection.PassionBrandId = _passionBrandRepository.CreatePassionBrand(passionBrand);
                    }

                    //Add factory into db, factory is based on Factory column in excel file. It must be unique. If it is existing, get existing id.
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

                    //Add techmanager into db, techmanager is based on Kĩ Thuật Trưởng column in excel file. It must be unique. If it is existing, get existing id.
                    //var techManager = techManagers.FirstOrDefault(x => x.Name.Contains(inspection.TechManagerName));
                    //if (techManager != null)
                    //{
                    //    inspection.TechManagerId = techManager.TechManagerId;
                    //}
                    //else
                    //{
                    //    var newTechManager = new TechManager()
                    //    {
                    //        Name = inspection.TechManagerName
                    //    };


                    //    inspection.TechManagerId = _techManagerRepository.CreateTechManager(newTechManager);
                    //}
                    inspection.TenantId = username.TenantId;
                    inspectionList.Add(inspection);                    
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ImportItemByGarmexTenant(ApplicationUser username, ExcelPackage package, List<Inspection> inspectionList, int j)
        {
            ExcelHandlingHelper excelHandling = new ExcelHandlingHelper();
            ExcelWorksheet workSheet = package.Workbook.Worksheets[j];
            //read every column in each worksheet
            if (workSheet.Dimension != null)
            {
                int totalRows = workSheet.Dimension.Rows;

                var passionBrands = _passionBrandRepository.PassionBrands;
                var factories = _factoryRepository.Factories;
                var finalWeeks = _finalWeekRepository.FinalWeeks;
                var techManagers = _techManagerRepository.TechManagers;
                for (int i = 2; i <= totalRows; i++)
                {
                    if (workSheet.Cells[i, 1].Value == null)
                    {
                        continue;
                    }
                    var inspection = new Inspection();
                    inspection.FactoryName = excelHandling.AssignCell(i, workSheet, "Factory");
                    inspection.OrderNumber = excelHandling.AssignCell(i, workSheet, "Order number");
                    inspection.IMAN = excelHandling.AssignCell(i, workSheet, "Iman");
                    inspection.Model = excelHandling.AssignCell(i, workSheet, "Model");
                    inspection.PassionBrandName = excelHandling.AssignCell(i, workSheet, "Passion brand");
                    inspection.Description = excelHandling.AssignCell(i, workSheet, "Description");
                    inspection.OrderQuantity = excelHandling.AssignCell(i, workSheet, "Ord Q'ty") != null ? int.Parse(excelHandling.AssignCell(i, workSheet, "Ord Q'ty").Trim()) : 0;
                    inspection.OrderType = excelHandling.AssignCell(i, workSheet, "Implantation") == null ? 0 : (excelHandling.AssignCell(i, workSheet, "Implantation").Trim().ToUpper() == "YES" ? 1 : 0);

                    //Add finalweek into db, finalweek is based on Final column in excel file. It must be unique. If it is existing, get existing id. 
                    double finalDate;
                    int weekOfYear = 0;
                    var importHelper = new ImportHelper();
                    if (double.TryParse(excelHandling.AssignCell(i, workSheet, "Final"), out finalDate))
                    {
                        inspection.FinalDate = importHelper.FromOADate(finalDate);
                        weekOfYear = importHelper.GetIso8601WeekOfYear(inspection.FinalDate);
                    }

                    if (weekOfYear != 0)
                    {
                        //the format standard is Year + -W + WeekNumberOfYear, for example: 2018-W14
                        var finalWeekName = inspection.FinalDate.Year.ToString() + "-W" + weekOfYear;
                        var existingFinalWeek = finalWeeks.FirstOrDefault(x => x.Name == finalWeekName);
                        //if finalweek is existing, get the existing FinalWeekID and assign it to inspection
                        if (existingFinalWeek != null)
                        {
                            inspection.FinalWeekId = existingFinalWeek.FinalWeekId;
                        }
                        //if finalweek is NOT existing, create new one and assign this to inspection
                        else
                        {
                            var newFinalWeek = new FinalWeek()
                            {
                                Week = weekOfYear,
                                Name = finalWeekName,
                                Year = Int32.Parse(inspection.FinalDate.Year.ToString()),
                                Description = inspection.FinalDate.Year.ToString() + "-W" + weekOfYear,
                                FinalWeekDay = inspection.FinalDate,
                                DateRegister = DateTime.Now
                            };

                            int finalWeekId = _finalWeekRepository.CreateFinalWeek(newFinalWeek);
                            inspection.FinalWeekId = finalWeekId;
                        }
                    }

                    //Add passionBranch into db, passionBranch is based on Passion Branch column in excel file. It must be unique. If it is existing, get existing id.
                    var brand = passionBrands.FirstOrDefault(x => x.Name.Contains(inspection.PassionBrandName));
                    //var brand = passionBrands.FirstOrDefault(x => x.Name.IndexOf(inspection.PassionBrandName, StringComparison.OrdinalIgnoreCase) >= 0);
                    if (brand != null)
                    {
                        inspection.PassionBrandId = brand.PassionBrandId;
                    }
                    else
                    {
                        var passionBrand = new PassionBrand()
                        {
                            Name = inspection.PassionBrandName
                        };
                        inspection.PassionBrandId = _passionBrandRepository.CreatePassionBrand(passionBrand);
                    }

                    //Add factory into db, factory is based on Factory column in excel file. It must be unique. If it is existing, get existing id.
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

                    inspection.TenantId = username.TenantId;
                    inspectionList.Add(inspection);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
