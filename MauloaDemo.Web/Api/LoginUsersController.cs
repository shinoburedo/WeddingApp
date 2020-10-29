using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AutoMapper;
using CBAF;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Repository;
using MauloaDemo.Models;
using MauloaDemo.Web.ViewModels;
using CBAF.Attributes;

namespace MauloaDemo.Web.Api
{
    public class LoginUsersController : BaseApiController 
    {
        private LoginUserRepository db = new LoginUserRepository();

        public LoginUsersController() {
            Mapper.CreateMap<LoginUser, UserProfile>()
                    .ForMember(dest => dest.cur_password, opt => opt.Ignore())
                    .ForMember(dest => dest.new_password, opt => opt.Ignore())
                    .ForMember(dest => dest.new_password_conf, opt => opt.Ignore());
            Mapper.AssertConfigurationIsValid();
        }

        // GET api/LoginUsers
        [HttpGet]
        [Route("api/LoginUsers/GetList")]
        [ResponseType(typeof(IEnumerable<LoginUser>))]
        public async Task<IHttpActionResult> GetLoginUsers() {
            var list = await Task.Run(() => db.GetList(this._loginUser));
            return Ok(list);
        }

        [HttpPost]
        [Route("api/LoginUsers/Search")]
        [ResponseType(typeof(IEnumerable<LoginUser>))]
        public async Task<IHttpActionResult> Search(LoginUserRepository.SearchParams param) {
            var list = await db.SearchAsync(param, this._loginUser);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }


        [HttpGet]
        [Route("api/LoginUsers/{id?}")]
        [ResponseType(typeof(LoginUser))]
        public async Task<IHttpActionResult> GetLoginUser(string id = null) {
            LoginUser user = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    user = new LoginUser();
                } else {
                    user = await db.FindAsync(id);
                    if (user == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(user);
        }

        [HttpPost]
        [Route("api/LoginUsers/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(LoginUser user) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(user);
                UpperCaseHelper.Apply(user);
                LowerCaseHelper.Apply(user);
                user.last_person = this._loginUser.login_id;
                user.update_date = DateTime.UtcNow;

                //date_format, user_typeなどをセット。
                user.ValidateSave();        

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<LoginUser>(user);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                var result = await Task.Run(()=> db.SaveLoginUser(user, user.is_new, this._loginUser.login_id));
                status = "ok" + ((int)result).ToString();

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }

        // POST api/LoginUsers
        [HttpPost]
        [ResponseType(typeof(LoginUser))]
        public async Task<IHttpActionResult> PostLoginUser(LoginUser loginUser) {
            try {
                loginUser.last_person = _loginUser.login_id;
                loginUser.update_date = DateTime.UtcNow;
                await Task.Run(() => db.SaveLoginUser(loginUser, loginUser.is_new, this._loginUser.login_id));
                return Ok(new { result = "ok" });

            } catch (System.Data.Entity.Validation.DbEntityValidationException ex) {
                var msg = string.Empty;// ex.Message;
                ex.EntityValidationErrors.Each((e) => e.ValidationErrors.Each((er) => msg += er.ErrorMessage + "\n"));
                return Ok(new { result = "error", message = msg });
            } catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;   //最も内側の例外を取得。
                return Ok(new { result = "error", message = ex.Message });
            }

        }


        [HttpPost]
        [Route("api/LoginUsers/UpdateUserProfile")]
        [ResponseType(typeof(UserProfile))]
        public async Task<IHttpActionResult> UpdateUserProfile(UserProfile userProfile) {

            if (userProfile == null || !string.Equals(userProfile.login_id, _loginUser.login_id)) {
                return NotFound();
            }
            var existing = db.Find(userProfile.login_id);
            if (existing == null) {
                return NotFound();
            }

            try {
                existing.e_mail = userProfile.e_mail;
                existing.culture_name = userProfile.culture_name;
                existing.date_format = userProfile.date_format;
                existing.time_format = userProfile.time_format;
                existing.last_person = _loginUser.login_id;
                existing.update_date = DateTime.UtcNow;
                existing.ValidateSave();
                db.SetModified(existing);

                //DBを更新。
                await Task.Run(() => db.SaveChanges());

                return Ok(new { result = "ok" });

            } catch (System.Data.Entity.Validation.DbEntityValidationException ex) {
                var msg = string.Empty;// ex.Message;
                ex.EntityValidationErrors.Each((e) => e.ValidationErrors.Each((er) => msg += er.ErrorMessage + "\n"));
                return Ok(new { result = "error", message = msg });
            } catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;   //最も内側の例外を取得。
                return Ok(new { result = "error", message = ex.Message });
            }
        }

        //[HttpPost]
        //[Route("api/LoginUsers/ChangePassword")]
        //public async Task<IHttpActionResult> ChangePassword(UserProfile userProfile) {
        //    if (userProfile == null) return NotFound();

        //    try {
        //        if (string.IsNullOrEmpty(userProfile.cur_password)) {
        //            throw new Exception(this.Resource("MSG_PWDCHG_CURPWDBLANKERR"));
        //        }
        //        if (string.IsNullOrEmpty(userProfile.new_password)) {
        //            throw new Exception(this.Resource("MSG_PWDCHG_NEWPWDBLANKERR"));
        //        }

        //        if (!string.Equals(userProfile.new_password, userProfile.new_password_conf)) {
        //            throw new Exception(this.Resource("MSG_PWDCHG_NEWCONF"));
        //        }

        //        dynamic result = await Task.Run(() => {
        //            this.ApplyLoginUserCulture();       //別スレッドなのでCultureを再設定する必要がある。(エラーメッセージの多言語対応のため。)

        //            var res = db.ChangePassword(
        //                                userProfile.login_id,
        //                                userProfile.cur_password,
        //                                userProfile.new_password,
        //                                _loginUser.login_id);

        //            if (res != LoginUser.PwdChgResult.Ok) {
        //                return new { 
        //                    result = getChangePasswordErrMsg(res), 
        //                    EffToPass = "" 
        //                };
        //            }

        //            var updated = db.Find(userProfile.login_id);
        //            if (updated == null) {
        //                throw new Exception("Cannot reload the user info.");
        //            }
        //            return new {
        //                result = "ok",
        //                EffToPass = updated.eff_to_pass.HasValue ? updated.eff_to_pass.Value.ToString("yyyy/MM/dd") : ""
        //            };
        //        });
        //        return Ok(result);

        //    } catch (Exception ex) {
        //        while (ex.InnerException != null) ex = ex.InnerException;   //最も内側の例外を取得。
        //        return Ok(new { result = "error", message = ex.Message });
        //    }
        //}


        //多言語対応のエラーメッセージをリソースファイルから取得。
        private string getChangePasswordErrMsg(LoginUser.PwdChgResult result) {

            ////文字列からEnum型に変換。
            //if (!Enum.TryParse<LoginUser.PwdChgResult>(result, true, out chgRes)) {
            //    //変換出来ない場合は文字列をそのまま返す。
            //    return msg;
            //}

            var msg = string.Empty;
            switch (result) {
                case LoginUser.PwdChgResult.LengthErr:
                    var minLength = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordLengthMin"]);
                    var maxLength = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordLengthMax"]);
                    msg = this.Resource("MSG_PWDCHG_LENGTHERR", minLength, maxLength);
                    break;
                default:
                    msg = this.Resource("MSG_PWDCHG_" + Enum.GetName(typeof(LoginUser.PwdChgResult), result).ToUpper());
                    if (string.IsNullOrEmpty(msg)) {
                        msg = string.Format("Error. ({0})", result);
                    }
                    break;
            }

            return msg;
        }


        [HttpPost]
        [Route("api/LoginUsers/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string id) {
            var status = string.Empty;
            try {
                await Task.Run(() => {
                    var user = db.Find(id);
                    db.DeleteLoginUser(user, this._loginUser.login_id);
                });
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

        [HttpPost]
        [Route("api/LoginUsers/SetAutoPwd")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SetAutoPwd() {
            var status = string.Empty;
            try {
                if (!this._loginUser.IsStaff() || this._loginUser.access_level < 7) {
                    throw new InvalidOperationException("Not authorized.");
                }

                await Task.Run(() => db.SetAutoPwd(this._loginUser.login_id));
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

        [HttpPost]
        [Route("api/LoginUsers/ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(UserProfile userProfile) {
            if (userProfile == null) return NotFound();
            if (_loginUser == null) return NotFound();

            if (_loginUser.access_level <= 1) {
                throw new AccessLevelErrorException(2, _loginUser.access_level);
            }

            try {
                //スタッフユーザーが他のユーザーのパスワードをリセットする場合は現在のパスワードは不要。
                if (!_loginUser.IsStaff() || _loginUser.login_id.Equals(userProfile.login_id)) {
                    //エージェントユーザーが自分でリセットする場合またはスタッフが自分のパスワードをリセットする場合は現在のパスワードが必須。
                    if (string.IsNullOrEmpty(userProfile.cur_password)) {
                        throw new Exception(this.Resource("MSG_PWDCHG_CURPWDBLANKERR"));
                    }
                }

                dynamic result = await Task.Run(() => {
                    this.ApplyLoginUserCulture();       //別スレッドなのでCultureを再設定する必要がある。(エラーメッセージの多言語対応のため。)

                    var new_pwd = db.ResetPassword(userProfile.login_id, userProfile.cur_password, _loginUser);
                    var updated = db.Find(userProfile.login_id);
                    if (updated == null) {
                        throw new Exception("Cannot reload the user info.");
                    }
                    return new {
                        result = "ok",
                        new_password = new_pwd,
                        EffToPass = updated.eff_to_pass.HasValue ? updated.eff_to_pass.Value.ToString("yyyy/MM/dd") : ""
                    };
                });
                return Ok(result);

            } catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;   //最も内側の例外を取得。
                return Ok(new { result = "error", new_password =  "",  message = ex.Message });
            }
        }





        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
