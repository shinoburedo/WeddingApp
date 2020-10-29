using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Threading;
using System.Globalization;
using System.Data.Entity.Validation;
using MauloaDemo.Repository;
using MauloaDemo.Models;

namespace MauloaDemo.Customer.Api {

    public class BaseApiController : ApiController {

        public const string DATE_FORMAT = "ddd MMM dd yyyy HH:mm:ss";       //これ以外のフォーマットだとカルチャーが変わると上手く行かないケースがある。
        public const string DATE_FORMAT_JPN = "yyyy/MM/dd HH:mm:ss";        //カルチャーが日本語(ja-JP)の場合はこれでないと上手く行かない！
        protected LoginUser _loginUser = null;

        protected override void Initialize(System.Web.Http.Controllers.HttpControllerContext controllerContext) {
            base.Initialize(controllerContext);

            if (User == null) return;
            if (User.Identity == null) return;
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(User.Identity.Name)) {
                return;
            } 

            var userRepository = new LoginUserRepository();
            _loginUser = userRepository.Find(User.Identity.Name);
            ApplyLoginUserCulture();
        }

        protected void ApplyLoginUserCulture() {
            if (_loginUser == null) return;
            if (string.IsNullOrEmpty(_loginUser.culture_name)) return;

            //ユーザーの言語・カルチャー設定をスレッド全体に反映する。(これによってActiveReportsにも反映される。)
            var ci = CultureInfo.CreateSpecificCulture(_loginUser.culture_name);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }

        /// <summary>
        /// Save時のモデルバインディングの検証エラーからエラーメッセージの文字列を生成する。
        /// </summary>
        /// <returns></returns>
        protected string buildModelStateErrorMsg() {
            if (ModelState == null) return string.Empty;

            var errorMsg = "Validation error:\n";
            foreach (var value in ModelState.Values) {
                foreach (var err in value.Errors) {
                    errorMsg += string.Format("\n{0}", err.ErrorMessage);
                }
            }
            return errorMsg;
        }

        /// <summary>
        /// EntityFrameworkのモデル検証エラーからエラーメッセージの文字列を生成する。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private string buildDbEntityValidationErrorMsg(DbEntityValidationException ex) {
            if (ex == null) return string.Empty;

            var errorMsg = "Validation error:\n";
            foreach (var err in ex.EntityValidationErrors) {
                foreach (var error in err.ValidationErrors) {
                    errorMsg += string.Format("\n{0}", error.ErrorMessage);
                }
            }
            return errorMsg;
        }

        /// <summary>
        /// Save/Delete時の例外からエラーメッセージの文字列を生成する。
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected string buildExceptionErrorMsg(Exception ex) {
            if (ex == null) return string.Empty;

            if (ex.GetType().IsSubclassOf(typeof(DbEntityValidationException))) {
                return buildDbEntityValidationErrorMsg(ex as DbEntityValidationException);
            }

            string errorMsg = string.Empty;
            while (ex.InnerException != null) { ex = ex.InnerException; }
            errorMsg = ex.Message;

            return errorMsg;
        }

        //ModelStateから全てのエラーメッセージを結合して取得する。
        protected string GetModelStateErrors(string separator = "\n") {
            return string.Join(separator, ModelState.SelectMany(p => p.Value.Errors).Select(p => p.ErrorMessage));
        }

    }
}