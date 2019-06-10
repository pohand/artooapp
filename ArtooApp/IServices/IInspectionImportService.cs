using Artoo.Models;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace Artoo.IServices
{
    public interface IInspectionImportService
    {
        bool ImportItem(ApplicationUser username, ExcelPackage package, List<Inspection> inspectionList, int j);
        bool ImportItemByGarmexTenant(ApplicationUser username, ExcelPackage package, List<Inspection> inspectionList, int j);
    }
}
