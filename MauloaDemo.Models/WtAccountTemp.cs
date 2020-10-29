//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Configuration;
//using Newtonsoft.Json;
//using WatabeWedding.Utilities;
//using WtbApi.Data.Repository.All;

//namespace MauloaDemo.Models {

//    [Serializable]
//    [Table("wt_account_temp")]
//    public class CAccountTemp {

//        [Key]
//        public int temp_id { get; set; }

//        [Hankaku]
//        public string email { get; set; }

//        [NotMapped]
//        public string email_val { get; set; }

//        public string url_key { get; set; }

//        public DateTime eff_date { get; set; }

//        public DateTime? account_regist_date { get; set; }

//        public int primary_id { get; set; }

//        [NotMapped]
//        public string primary_id_name { get; set; }

//        public string last_person { get; set; }

//        public DateTime update_date { get; set; }


//        //[NotMapped]
//        //public string app_path { get; set; }

//        //[NotMapped]
//        //public string referrer_url { get; set; }



//        //url_keyから登録の確認時にアクセスするURLを編集して返す。
//        public string GetTempURL() {
//            var base_url = TypeHelper.GetStr(ConfigurationManager.AppSettings["BaseURL"]);
//            var loginPath = TypeHelper.GetStr(ConfigurationManager.AppSettings["LoginPath"]);
//            if (string.IsNullOrEmpty(loginPath)) loginPath = "/login";

//            var tempUrl =  base_url + loginPath + "/Register?key=" + this.url_key;
//            return tempUrl;
//        }

//        public string TempCode { 
//            get {
//                if (string.IsNullOrEmpty(this.url_key)) return "";

//                //URLの6文字目から6文字分を取得。        
//                var chars = String.Concat(this.url_key.Skip(5).Take(6));

//                //各文字のアスキーコードを10で割った余り(必ず0～9になる)を連結してコードとする。
//                var code = String.Concat(chars
//                                    .Select(c => (int)c)
//                                    .Select(i => i % 10));

//                //"0"で始まる場合は先頭の"0"を"9"に置き換える。
//                //  クライアント側の入力画面で<input type="number">が使える様にするため。
//                if (code.StartsWith("0")) {
//                    code = "9" + code.Substring(1);
//                }

//                return code;
//            } 
//        }

//        public void ValidateSave(string lang) {
//            var repository = new CAccountTempRepository();

//            var exists = repository.Context.CAccountTemps.Any(m => m.temp_id == this.temp_id);
//            if (exists) {
//                throw new Exception("temp_id already exists.");
//            }
//            if (string.IsNullOrEmpty(this.email)) {
//                if (lang == "J") {
//                    throw new ArgumentNullException(String.Empty, "Eメールアドレスを入力してください。");
//                } else {
//                    throw new ArgumentNullException(String.Empty, "Please enter Email address.");
//                }
//            }
//            if (!System.Text.RegularExpressions.Regex.IsMatch(
//                    this.email,
//                    @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z",
//                    System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
//                if (lang == "J") {
//                    throw new ArgumentNullException(String.Empty, "Eメールアドレスを正しく入力してください。");
//                } else {
//                    throw new ArgumentNullException(String.Empty, "Please enter correct Email address.");
//                }
//            }
//            if (this.email.Length > 200) {
//                if (lang == "J") {
//                    throw new ArgumentNullException(String.Empty, "Eメールアドレスは200文字以内で入力してください。");
//                } else {
//                    throw new ArgumentNullException(String.Empty, "Please enter your Email address in 200 characters or less.");
//                }
//            }
//            if (this.email != this.email_val) {
//                if (lang == "J") {
//                    throw new ArgumentNullException(String.Empty, "Eメールアドレスが正しく入力されているか確認してください。");
//                } else {
//                    throw new ArgumentNullException(String.Empty, "Please check that your e-mail addresses match.");
//                }
//            }

//            var exists_email = repository.Context.CAccounts.Any(m => m.email == this.email);
//            if (exists_email) {
//                //このメッセージは実際には表示されない
//                throw new ArgumentException("Email already exists.", "email_dup");
//            }
//        }

//    }
//}