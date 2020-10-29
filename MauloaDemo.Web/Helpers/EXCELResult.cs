using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Reflection;
using System.IO;
using CBAF;
using MauloaDemo.Web.Helpers;

namespace MauloaDemo.Web.Helpers {

    public class EXCELResult : ContentResult  {

        public MemoryStream Data { get; set; }
        public string Filename { get; set; }

        /// <summary>
        /// MemoryStreamをEXCEL形式で出力する。
        /// </summary>
        /// <param name="book">ExcelのIWorkbookのMemoryStream</param>
        /// <param name="filename">出力ファイル名</param>
        public EXCELResult(MemoryStream book, string filename = null) {
            this.Data = book;
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
                this.Filename = "output.xls";
            }
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", "attachment;filename=" + this.Filename);

            if (this.ContentEncoding == null) {
                this.ContentEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");
                //this.ContentEncoding = System.Text.Encoding.UTF8;
            }
            response.ContentEncoding = this.ContentEncoding;

            response.BinaryWrite(Data.GetBuffer());
        }

    }
}

