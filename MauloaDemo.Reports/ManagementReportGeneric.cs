using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Data;
using System.Configuration;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using CBAF;
using MauloaDemo.Repository;
using MauloaDemo.Models;

namespace MauloaDemo.Reports {
    class ManagementReportGeneric {
    }
}


namespace WatabeWedding.CatWeb.Reports {
    public class ManagementReportGeneric {

        //Excelテンプレートファイルのパスを取得する
        public static string GetTemplatePath(string region_cd) {
            string path = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["ManagementReportTemplatePath"]);
            path = path.Replace("%ROOT%", RegionConfig.GetApplicationRootPath());
            path = path.Replace("%REGIONCD%", region_cd);

            if (string.IsNullOrEmpty(path)) {
                path = RegionConfig.GetApplicationRootPath();
            }

            if (!path.EndsWith("\\")) {
                path += "\\";
            }
            return path;
        }

        public static MemoryStream CreateExcel(string excelPath, int sheet_num, string start_pos, bool add_header, DataTable dataTable) {
            var book = ReadXLS(excelPath, sheet_num, start_pos, add_header, dataTable);
            MemoryStream ms = new MemoryStream();
            book.Write(ms);
            return ms;
        }

        private static IWorkbook ReadXLS(string path, int sheet_num, string start_pos, bool add_header, DataTable dataTable) {

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                var book = new HSSFWorkbook(fs);

                ISheet sheet = null;
                try {
                    //書き込み対象のシートを取得。（1が最初のシート, 2が2番目のシート, 3が3番目のシート...）
                    sheet = book.GetSheetAt(sheet_num - 1);
                    if (sheet == null) throw new Exception();
                } catch (Exception) {
                    throw new ArgumentOutOfRangeException("sheet_num", string.Format("Specified sheet ({0}) cannot be found in Excel template. ({1})", sheet_num, path));
                }

                int start_row = 1;
                int start_col = 1;
                if (!string.IsNullOrEmpty(start_pos)) {
                    try {
                        var cr = new NPOI.SS.Util.CellReference(start_pos);
                        start_row = cr.Row;
                        start_col = cr.Col;
                    } catch (Exception) {
                        //Ignore errors.
                    }
                }

                int rowIndex = start_row;

                if (add_header) {
                    //ヘッダー行を出力。
                    CreateHeaderRow(sheet, dataTable, rowIndex++, start_col);
                }

                //データをSheetに書き込む。
                foreach (DataRow dr in dataTable.Rows) {
                    CreateRow(sheet, dr, rowIndex, start_col);
                    rowIndex++;
                }

                return book;
            }
        }

        // ヘッダー行を作成する
        private static void CreateHeaderRow(ISheet sheet, DataTable dataTable, int rowIndex, int start_col) {
            var row = sheet.CreateRow(rowIndex);

            for (int colIndex = 0; colIndex < dataTable.Columns.Count; colIndex++) {
                var cell = row.CreateCell(colIndex + start_col);
                cell.SetCellValue(dataTable.Columns[colIndex].ColumnName);
            }
        }

        // 1行分のデータをExcelシートに書き込む。
        private static void CreateRow(ISheet sheet, DataRow dr, int rowIndex, int start_col) {

            // 行を作る。
            var row = sheet.CreateRow(rowIndex);

            // 列を作る。
            for (int colIndex = 0; colIndex < dr.Table.Columns.Count; colIndex++) {
                var cell = row.CreateCell(colIndex + start_col);
                object value = dr[colIndex];
                if (value.GetType().Equals(typeof(Nullable<int>))) {
                    cell.SetCellValue(TypeHelper.GetInt(value));
                } else if (value.GetType().Equals(typeof(Nullable<short>))) {
                    cell.SetCellValue(TypeHelper.GetShort(value));
                } else if (value.GetType().Equals(typeof(Nullable<decimal>)) || value.GetType().Equals(typeof(Nullable<double>))) {
                    cell.SetCellType(CellType.Numeric);
                    //var format = book.CreateDataFormat();
                    //idCell.CellStyle.DataFormat = format.GetFormat("#,##0.###");

                    //idCell.SetCellValue(TypeHelper.GetStrTrim(m.Value));
                    var DoubleValue = (double)TypeHelper.GetDecimal(value);
                    cell.SetCellValue(DoubleValue);
                    //ICellStyle cellStyle = book.CreateCellStyle();
                    //cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00E+00");
                    //idCell.CellStyle = cellStyle;

                } else if (value.GetType().Equals(typeof(string)) || value.GetType().IsValueType) {
                    cell.SetCellValue(TypeHelper.GetStrTrim(value));
                }
            }
        }

    }
}
