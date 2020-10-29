using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Reflection;
using CBAF;
using MauloaDemo.Web.Helpers;


namespace MauloaDemo.Web.Helpers {

    public class CSVResult : ContentResult  {

        public List<object> Data { get; set; }
        public DataTable dataTable { get; set; }
        public string Filename { get; set; }
        public string[] Columns { get; set; }
        public string[] Titles { get; set; }

        /// <summary>
        /// オブジェクトのリストをCSV形式で出力する。
        /// </summary>
        /// <param name="list">任意の型のオブジェクトのリスト</param>
        /// <param name="columns">出力対象のフィールド/プロパティ名称の配列 (nullの場合は全フィールド/プロパティが対象)</param>
        /// <param name="titles">出力対象のフィールド/プロパティに対応する見出しの配列 (nullの場合はフィールド名/プロパティ名をそのまま出力する。)</param>
        /// <param name="filename">出力ファイル名</param>
        public CSVResult(List<object> list, string[] columns, string[] titles, string filename = null) {
            this.Data = list;
            this.Columns = columns;
            this.Titles = titles;
            this.Filename = filename;
        }

        public CSVResult(DataTable dt, bool add_header, string filename = null) {
            this.dataTable = dt;
            this.Filename = filename;
        }

        public override void ExecuteResult(ControllerContext context) {

            if (context == null) {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;
            response.ClearHeaders();
            response.AppendHeader("Cache-Control", "private, no-cache, no-store, must-revalidate, max-stale=0, post-check=0, pre-check=0");
            response.AppendHeader("Pragma", "no-cache");
            response.AppendHeader("Expires", "-1");
            HttpCachePolicyBase cache = response.Cache;
            cache.SetCacheability(HttpCacheability.NoCache);

            if (string.IsNullOrEmpty(this.Filename)) {
                this.Filename = "data.csv";
            } 
            response.ContentType = "application/octet-stream";
            response.AddHeader("Content-Disposition", "attachment;filename=" + this.Filename);

            if (this.ContentEncoding == null) {
                this.ContentEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");
                //this.ContentEncoding = System.Text.Encoding.UTF8;
            }
            response.ContentEncoding = this.ContentEncoding;

            if (this.Data != null) {
                //見出し行を出力。
                OutputHeader(response);

                //各行のデータを出力。
                foreach (object item in this.Data) {
                    OutputLine(response, item);
                }
            } else {
                //見出し行を出力。
                OutputHeaderFromDataTable(response);

                //各行のデータを出力。
                foreach (DataRow dr in this.dataTable.Rows) {
                    OutputLineFromDataTable(response, dr);
                }
            }

        }


        private void OutputHeader(HttpResponseBase response) {
            if (this.Data.Count == 0) return;

            var target = this.Data[0];
            string str = ObjectReflectionHelper.BuildCSVHeaderStringFromObject(target, this.Columns, this.Titles);
            str += Environment.NewLine;
            response.BinaryWrite(this.ContentEncoding.GetBytes(str));
        }

        private void OutputLine(HttpResponseBase response, object item) {
            string str = ObjectReflectionHelper.BuildCSVStringFromObject(item, this.Columns);
            str += Environment.NewLine;
            response.BinaryWrite(this.ContentEncoding.GetBytes(str));
        }

        private void OutputHeaderFromDataTable(HttpResponseBase response) {
            if (this.dataTable.Rows.Count == 0) return;

            var sOut = new StringBuilder();
            int i = 0;
            foreach (DataColumn c in this.dataTable.Columns) {
                if (i > 0) sOut.Append(",");
                sOut.Append(c.ColumnName);
                i++;
            }
            sOut.AppendLine();
            response.BinaryWrite(this.ContentEncoding.GetBytes(sOut.ToString()));
        }

        private void OutputLineFromDataTable(HttpResponseBase response, DataRow dr) {
            var sOut = new StringBuilder();
            int i = 0;
            foreach (DataColumn c in this.dataTable.Columns) {
                if (i > 0) sOut.Append(",");
                object oValue = dr[c];
                string str = "";

                if (c.DataType.Equals(typeof(DateTime)) || c.DataType.Equals(typeof(DateTime?))) {
                    str = ObjectReflectionHelper.GetDateStringForCSV(oValue, c.ColumnName);
                } else {
                    str = TypeHelper.GetStrTrim(oValue);
                }

                //半角カンマは全角カンマに変換する。
                str = str.Replace(",", "，");

                sOut.Append(str);
                i++;
            }
            sOut.AppendLine();
            response.BinaryWrite(this.ContentEncoding.GetBytes(sOut.ToString()));
        }



    }
}