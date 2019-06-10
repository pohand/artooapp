using OfficeOpenXml;
using System;
using System.Linq;

namespace Artoo.Helpers
{
    public class ExcelHandlingHelper
    {
        public string AssignCell(int position, ExcelWorksheet ws, string columnName)
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
    }
}
