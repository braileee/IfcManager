using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSURevitApps.Core
{
    public static class CellExtensions
    {
        public static object GetCellValue(this ICell cell)
        {
            if (cell == null)
                return null;

            return cell.CellType switch
            {
                CellType.Numeric => cell.NumericCellValue,
                CellType.String => cell.StringCellValue,
                CellType.Boolean => cell.BooleanCellValue,
                CellType.Formula => cell.CachedFormulaResultType switch
                {
                    CellType.Numeric => cell.NumericCellValue,
                    CellType.String => cell.StringCellValue,
                    CellType.Boolean => cell.BooleanCellValue,
                    _ => null
                },
                _ => null
            };
        }

        public static string GetCellValueAsString(this ICell cell)
        {
            return GetCellValue(cell)?.ToString();
        }

    }
}
