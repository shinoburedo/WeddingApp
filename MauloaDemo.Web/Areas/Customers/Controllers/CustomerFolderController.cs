using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;
using MauloaDemo.Web.Controllers;
using CBAF;
using System.Configuration;

namespace MauloaDemo.Web.Areas.Customers.Controllers
{
    [AccessLevelFilter(2)]
    public class CustomerFolderController : BaseController
    {
        //
        // GET: /Reservation/CustomerFolder/

        public JsonResult GetGridData(string c_num) {
            //カスタマー毎のフォルダーのファイル一覧を取得。
            string sPath = GetCustomerFolderPath(c_num);
            var dw = new FileList(sPath);
            var list = dw.GetFileList(UserHelper.LoginUser);
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Upload(string c_num, IEnumerable<HttpPostedFileBase> files) {
            try {
                if (files != null) {
                    string sFolder = GetCustomerFolderPath(c_num);
                    var d = new System.IO.DirectoryInfo(sFolder);
                    if (!d.Exists) {
                        d.Create();
                    }

                    foreach (var file in files) {
                        // Some browsers send file names with full path. We are only interested in the file name.
                        var fileName = System.IO.Path.GetFileName(file.FileName);
                        var physicalPath = System.IO.Path.Combine(sFolder, fileName);

                        var fi = new System.IO.FileInfo(physicalPath);
                        if (fi.Exists) {
                            throw new Exception("The same filename already exists.");
                        }

                        file.SaveAs(physicalPath);

                        //変更ログを保存。
                        var log = new LogChangeRepository();
                        var json_str = Newtonsoft.Json.JsonConvert.SerializeObject(new FileInfo() { filename = fileName });
                        log.InsertLog(this.CurrentLoginUser.login_id, c_num, "file", null, null, "I", json_str);
                    }
                }

                return Content("");
            } catch (Exception ex) {
                //return new HttpStatusCodeResult(500, ex.Message);
                //throw ex;
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(string c_num, string[] fileNames) {
            var hash = new Dictionary<string, object>();
            try {
                if (fileNames != null) {
                    string sFolder = GetCustomerFolderPath(c_num);

                    foreach (var fullName in fileNames) {
                        var fileName = System.IO.Path.GetFileName(fullName);
                        var physicalPath = System.IO.Path.Combine(sFolder, fileName);

                        var dw = new FileList(physicalPath);
                        dw.DeleteFile(physicalPath);

                        //変更ログを保存。
                        var log = new LogChangeRepository();
                        var json_str = Newtonsoft.Json.JsonConvert.SerializeObject(new FileInfo() { filename = fileName });
                        log.InsertLog(this.CurrentLoginUser.login_id, c_num, "file", null, null, "D", json_str);
                    }
                }
                hash.Add("Result", "success");
            } catch (Exception ex) {
                hash.Add("Result", "error");
                hash.Add("Message", ex.Message);
            }
            return Json(hash);
        }


        public ActionResult Download(string c_num, string filename) {
            string sPath = GetCustomerFolderPath(c_num);
            sPath += filename;

            //ファイルの拡張子から対応するMIME識別子を得る。
            var mime = System.Web.MimeMapping.GetMimeMapping(filename);

            if ("application/pdf".Equals(mime)) {
                //PDFの場合は直接ブラウザ内で開く。
                return File(sPath, mime);
            } else {
                //それ以外の場合は同じファイル名でダウンロードする。
                return File(sPath, mime, filename);
            }
        }

        private string GetCustomerFolderPath(string c_num) {
            string sPath = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["CustomerFolderPath"]);
            if (string.IsNullOrWhiteSpace(sPath)) sPath = "\\CustomerFolder\\";

            sPath = sPath.Replace("%REGIONCD%", this.CurrentRegionInfo.CurrentDestination);

            if (!sPath.EndsWith("\\")) sPath += "\\";
            sPath += c_num + "\\";
            return sPath;
        }

        [HttpPost]
        public ActionResult Count(string c_num) {
            var hash = new Dictionary<string, object>();
            try {
                //カスタマー毎のフォルダーのファイル一覧を取得。
                string sPath = GetCustomerFolderPath(c_num);
                var dw = new FileList(sPath);
                var list = dw.GetFileList(UserHelper.LoginUser);
                hash.Add("Result", "success");
                hash.Add("Count", list.Count());
            } catch (Exception ex) {
                hash.Add("Result", "error");
                hash.Add("Message", ex.Message);
            }
            return Json(hash);
        }

    }
}