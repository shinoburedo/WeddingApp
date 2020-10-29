using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using Newtonsoft.Json;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Serializable]
    [Table("login_user")]
    public class LoginUser : IValidatableObject {

        public enum PwdChgResult {
            Ok = 0,
            CurPwdBlankErr,
            NewPwdBlankErr,
            InvalidCharErr,
            LengthErr,
            SameAsUIDErr,
            NumericCharErr,
            InvalidOldPass,
            NoChangeErr,
            AlphabetCharErr,
            SequentialErr,
            RepeatedErr,
            UnknownErr
        }

        public const string USERTYPE_STAFF = "STF";
        public const string USERTYPE_AGENT = "AGT";

        [Key(), Required(), StringLength(15), Hankaku()]
        public string login_id { get; set; }

        public string password { get; set; }

        public string region_cd { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string area_cd { get; set; }

        [Required(), StringLength(6), Hankaku(), UpperCase()]
        public string sub_agent_cd { get; set; }

        [StringLength(100)]
        public string busho { get; set; }

        [StringLength(100)]
        public string section { get; set; }

        [StringLength(100)]
        public string company { get; set; }

        [StringLength(30), Hankaku(), UpperCase()]
        public string e_last_name { get; set; }

        [StringLength(30), Hankaku(), UpperCase()]
        public string e_first_name { get; set; }

        [StringLength(30)]
        public string j_last_name { get; set; }

        [StringLength(30)]
        public string j_first_name { get; set; }

        public int access_level { get; set; }

        public int access_count { get; set; }

        [Required(), StringLength(3), Hankaku(), UpperCase()]
        public string user_type { get; set; }

        [StringLength(50), Hankaku(), LowerCase()]
        public string e_mail { get; set; }

        [StringLength(20), Hankaku()]
        public string phone { get; set; }

        [JsonIgnore]
        public string key { get; set; }

        [JsonIgnore]
        public string pwd_hash { get; set; }

        [JsonIgnore]
        public string old_password { get; set; }

        [DataType(DataType.Date)]
        public DateTime? eff_from_pass { get; set; }

        [DataType(DataType.Date)]
        public DateTime? eff_to_pass { get; set; }

        public bool locked { get; set; }

        [Required(), StringLength(20), Hankaku()]
        public string culture_name { get; set; }

        [StringLength(20)]
        public string date_format { get; set; }

        [StringLength(10)]
        public string time_format { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [DataType(DataType.DateTime)]
        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public string agent_cd { get; set; }                //Find()内でAgentParentテーブルから取得。
        [NotMapped]
        public bool staff_required { get; set; }            //Find()内でAgentテーブルから取得。
        [NotMapped]
        public bool branch_staff_required { get; set; }     //Find()内でAgentテーブルから取得。

        [NotMapped]
        public int time_zone { get; set; }

        [NotMapped]
        public bool is_new { get; set; }

        [NotMapped]
        public LoginUserToken Token { get; set; }

        [NotMapped]
        public bool HasChildAgents { get; set; }            //Find()内でセット。

        public LoginUser() {
            this.region_cd = "HWI";
            this.area_cd = "HNL";
            this.access_level = 1;
            this.culture_name = "en-US";
            this.date_format = "MM/dd/yyyy";
            this.time_format = "HH:mm";
            this.time_zone = -10;
            this.user_type = "AGT";
        }

        [NotMapped]
        public string Language { 
            get {
                if (this.culture_name.StartsWith("en")) return "en";
                if (this.culture_name.StartsWith("ja")) return "ja";
                return string.Empty;
            }
        }

        [NotMapped]
        public string UserName {
            get {
                string s = this.e_first_name + " " + this.e_last_name;
                return s.Trim();
            }
        }

        [NotMapped]
        public string UserNameJpn {
            get {
                string s = this.j_last_name + " " + this.j_first_name;
                return s.Trim();
            }
        }

        public bool IsStaff() {
            return USERTYPE_STAFF.Equals(this.user_type);
        }

        public bool IsAgent() {
            return USERTYPE_AGENT.Equals(this.user_type);
        }

        //public bool CanViewMasters() {
        //    return this.IsStaff();
        //}

        //public bool CanViewReports() {
        //    return this.IsStaff();
        //}

        //public bool CanViewAccounting() {
        //    return this.IsStaff();
        //}


        public string GetDateAndTimeFormat() {
            return this.date_format + " " + this.time_format;
        }

        public string GetMonthAndDateOnlyFormat() {
            var s = this.date_format;
            s = s.Replace("yyyy", "")
                 .Replace("MM", "M")
                 .Replace("dd", "d");
            if (s.StartsWith("/")) s = s.Substring(1);
            if (s.EndsWith("/")) s = s.Substring(0, s.Length - 1);

            return s;
        }

        public void ValidateSave() {

            if (string.IsNullOrEmpty(this.login_id)) {
                throw new Exception("Login Id is required.");
            }

            if (string.IsNullOrEmpty(this.culture_name)) {
                this.culture_name = "en-US";
            }

            if (string.IsNullOrEmpty(this.last_person)) {
                throw new Exception("Last person is required.");
            }

            // 文字列型プロパティの値を変換する。
            UpperCaseHelper.Apply(this);
            HankakuHelper.Apply(this);
            LowerCaseHelper.Apply(this);

            //*** CultureNameとDateFormatは別々に設定可能にしたので下記は不要。
            //if ("ja-JP".Equals(this.culture_name)) {
            //    this.date_format = "yyyy/MM/dd";
            //    this.time_format = "HH:mm";
            //} else {
            //    this.date_format = "MM/dd/yyyy";
            //    this.time_format = "HH:mm";
            //}

            if (AgentParent.GetOwnSubAgentCd().Equals(this.sub_agent_cd)) {
                this.user_type = USERTYPE_STAFF;
            } else {
                this.user_type = USERTYPE_AGENT;
            }
        }


        public PwdChgResult ValidateChangePassword(string cur_password, string new_password) {
            PwdChgResult rtn = PwdChgResult.Ok;

            if (string.IsNullOrEmpty(cur_password)) {
                return PwdChgResult.CurPwdBlankErr;
            }

            if (string.IsNullOrEmpty(new_password)) {
                return PwdChgResult.NewPwdBlankErr;
            }

            //現在と同じパスワードは不可。
            if (string.Equals(cur_password, new_password)) {
                return PwdChgResult.NoChangeErr;
            }

            //パスワードの長さをチェック。
            var minLength = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordLengthMin"]);
            var maxLength = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordLengthMax"]);
            if (new_password.Length < minLength || new_password.Length > maxLength) {
                return PwdChgResult.LengthErr;
            }

            //login_idまたはユーザー名を含むパスワードは不可。
            if (new_password.ToUpper().Contains(this.login_id.ToUpper())
                || (!string.IsNullOrEmpty(this.e_last_name) && new_password.ToUpper().Contains(TypeHelper.GetStrTrim(this.e_last_name).ToUpper()))
                || (!string.IsNullOrEmpty(this.e_first_name) && new_password.ToUpper().Contains(TypeHelper.GetStrTrim(this.e_first_name).ToUpper()))) {
                return PwdChgResult.SameAsUIDErr;
            }

            //全角文字、使用禁止文字(;"'\ )が含まれていたらNG。
            if (!CheckPasswordCharSigns(new_password)) {
                return PwdChgResult.InvalidCharErr;
            }

            //数字が１文字以上含まれていなければNG。
            if (!CheckPasswordCharNumeric(new_password)) {
                return PwdChgResult.NumericCharErr;
            }

            //アルファベットが2文字以上含まれていなければNG
            //かつ大文字と小文字の両方を含んでいなければNG
            if (!CheckPasswordCharAlphabet(new_password)) {
                return PwdChgResult.AlphabetCharErr;
            }

            //3文字以上の連続した数字（ABC1234, X9876X 等）はNG
            //3文字以上の連続したアルファベットはNG
            if (!CheckPasswordSequential(new_password)) {
                return PwdChgResult.SequentialErr;
            }

            //3文字以上の連続した同一文字はNG
            if (!CheckPasswordRepeated(new_password)) {
                return PwdChgResult.RepeatedErr;
            }

            return rtn;
        }

        private bool CheckPasswordCharSigns(string pwd) {

            //半角文字以外が含まれていたらNG。
            if (!HankakuZenkaku.isHankaku(pwd)) return false;

            //使用不可の文字が含まれていたらNG。(セミコロン、ダブルクォート、シングルクォート、バックスラッシュ、空白)
            if (pwd.IndexOfAny(";\"'\\ ".ToCharArray()) >= 0) return false;

            return true;
        }

        private bool CheckPasswordCharNumeric(string pwd) {

            //ひとつでも数字があればOK
            foreach (char c in pwd.ToCharArray()) {
                if (c >= '0' && c <= '9') {
                    return true;
                }
            }

            //ひとつも無ければNG
            return false;
        }

        private bool CheckPasswordCharAlphabet(string pwd) {
            int big = 0;
            int small = 0;

            //アルファベット(大文字)の数を数える。
            foreach (char c in pwd.ToCharArray()) {
                if (c >= 'A' && c <= 'Z') big++;
            }

            //アルファベット(小文字)の数を数える。
            foreach (char c in pwd.ToCharArray()) {
                if (c >= 'a' && c <= 'z') small++;
            }

            //大文字と小文字がともに１文字以上ずつあればOK。
            return (big >= 1 && small >= 1);
        }

        private bool CheckPasswordSequential(string pwd) {
            const int MAX_SERIES = 3;

            //MAX_SERIES文字以上の連続(昇順)はNG
            int cnt = 1;
            for (int i = 1; i <= pwd.Length - 1; i++) {
                char c0 = Convert.ToChar(pwd.Substring(i - 1, 1));
                char c1 = Convert.ToChar(pwd.Substring(i, 1));

                if (char.IsLetterOrDigit(c0)) {
                    if (c1 == c0 + 1) {
                        cnt++;
                        if (cnt >= MAX_SERIES) return false;
                    } else {
                        cnt = 1;
                    }
                }
            }

            //MAX_SERIES文字以上の連続(降順)はNG
            cnt = 1;
            for (int i = 1; i <= pwd.Length - 1; i++) {
                char c0 = Convert.ToChar(pwd.Substring(i - 1, 1));
                char c1 = Convert.ToChar(pwd.Substring(i, 1));

                if (char.IsLetterOrDigit(c0)) {
                    if (c1 == c0 - 1) {
                        cnt++;
                        if (cnt >= MAX_SERIES) return false;
                    } else {
                        cnt = 1;
                    }
                }
            }

            return true;
        }

        //3文字以上の連続同一文字はNG
        private bool CheckPasswordRepeated(string pwd) {
            const int MAX_SAME = 3;

            int cnt = 1;
            for (int i = 1; i <= pwd.Length - 1; i++) {
                char c0 = Convert.ToChar(pwd.Substring(i - 1, 1));
                char c1 = Convert.ToChar(pwd.Substring(i, 1));

                if (c1.Equals(c0)) {
                    cnt++;
                    if (cnt >= MAX_SAME) return false;
                } else {
                    cnt = 1;
                }
            }

            return true;
        }


        public static string GenerateRandomPassword(short length = 8) {
            var available = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789%$#@!";
            var s = string.Empty;
            var rnd = new Random();

            for (short i = 0; i < length; i++) {
                s += available[rnd.Next(available.Length)];
            }
            return s;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.login_id) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.login_id, false)) {
                yield return new ValidationResult(string.Format("login_id は半角英数字で入力してください。({0})", this.login_id), new[] { "login_id" });
            }
        }

    }
}
