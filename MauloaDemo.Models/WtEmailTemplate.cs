//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Configuration;
//using System.Linq;
//using Newtonsoft.Json;
//using WatabeWedding.Utilities;
//using WtbApi.Data.DTO.All;
//using WtbApi.Data.Models.Destination;

//namespace MauloaDemo.Models {

//    [Table("wt_email_template")]
//    public class WtEmailTemplate : IValidatableObject {

//        //各メールtemplate_cd
//        [NotMapped]
//        public const string TEMP_CD_ACCT_NEW = "Acct_New_Confm"; //Account作成用URL通知
//        [NotMapped]
//        public const string TEMP_CD_ACCT_NEW_MOBILE = "Acct_New_Confm_Mobil"; //Account作成用URL通知(Mobile App用)
//        [NotMapped]
//        public const string TEMP_CD_ACCT_DUP = "ACCT_DUP_EMAIL"; //Account作成Email重複
//        [NotMapped]
//        public const string TEMP_CD_ACCT_REREGIST = "ACCT_REREGIST"; //PasswordReset用URL通知
//        [NotMapped]
//        public const string TEMP_CD_ACCT_NEW_PTN = "Acct_New_Confm_Partn"; //Partner用Account作成用URL通知
//        [NotMapped]
//        public const string TEMP_CD_ACCT_PASS_CHG = "Acct_PassChg"; //AccountのPassword変更通知
//        [NotMapped]
//        public const string TEMP_CD_ACCT_EML_CHG = "ACCT_EMLADDRCHG"; //AccountのEmail変更通知
//        [NotMapped]
//        public const string TEMP_CD_ORD_COMP_NTC = "Ord_Comp_Ntc"; //Order完了通知
//        [NotMapped]
//        public const string TEMP_CD_ORD_RQCXL_NTC = "Ord_RQCxl_Ntc"; //キャンセル完了通知

//        [Key, Required, StringLength(20), Hankaku, UpperCase]
//        public string template_cd { get; set; }

//        [Required, StringLength(200), EmailAddress]
//        public string from_addr { get; set; }

//        [Required, StringLength(200), EmailAddress]
//        public string from_addr_en { get; set; }

//        [StringLength(100)]
//        public string subject {get; set;}

//        [StringLength(100)]
//        public string subject_en { get; set; }

//        [Required]
//        public string body { get; set; }

//        [Required]
//        public string body_en { get; set; }

//        [Required, StringLength(15), Hankaku]
//        public string create_by { get; set; }

//        public DateTime create_date { get; set; }

//        [Required, StringLength(15)]
//        public string update_by { get; set; }

//        public DateTime update_date { get; set; }


//        [NotMapped]
//        public bool is_new { get; set; }

//        public WtEmailTemplate() {
//            this.is_new = true;
//        }

//        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
//            if (!string.IsNullOrEmpty(template_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(template_cd)) {
//                yield return new ValidationResult(string.Format("template_cdは半角文字で入力して下さい。('{0}')", template_cd), new[] { "template_cd" });
//            }
//        }

//        /// <summary>
//        /// テンプレートからEmailの内容を生成する。
//        /// </summary>
//        /// <param name="lang">対象言語（'J'または'E'）</param>
//        /// <param name="props">メール生成に必要な情報が入ったオブジェクトを渡す。
//        /// 例）c_num : カスタマー番号 (文字列)
//        ///     account_temp : アカウント新規作成時の一時登録情報（CAccountTempのインスタンス）
//        ///     account: アカウント情報 (CAccountのインスタンス)
//        ///     customer: カスタマー情報（WtbApi.Data.Models.Destination.Customerのインスタンス）
//        ///     booking_list: オプション申込情報（MauloaDemo.Models.WtBookingのインスタンス）のリスト
//        /// </param>
//        /// <returns></returns>
//        public WtEmailQueue GenerateEmail(bool is_japan, Dictionary<string, object> props) {
//            var generated_from_addr = TypeHelper.GetStr(is_japan ? this.from_addr : this.from_addr_en);
//            var generated_subject = TypeHelper.GetStr(is_japan ? this.subject : this.subject_en);
//            var generated_body = StringHelper.CutBefore(TypeHelper.GetStr(is_japan ? this.body : this.body_en), "%END%");
//            var additional_template = StringHelper.CutAfter(TypeHelper.GetStr(is_japan ? this.body : this.body_en), "%END%");

//            var c_num = "";
//            if (props.ContainsKey("c_num")) { 
//                c_num = TypeHelper.GetStr(props["c_num"]);
//                generated_subject = ReplaceCNum(generated_subject, c_num);
//                generated_body = ReplaceCNum(generated_body, c_num);
//            }

//            if (props.ContainsKey("account_temp")) {
//                var accountTemp = props["account_temp"] as CAccountTemp;
//                generated_subject = ReplaceAccountTempInfo(generated_subject, is_japan, accountTemp);
//                generated_body = ReplaceAccountTempInfo(generated_body, is_japan, accountTemp);
//            }

//            if (props.ContainsKey("account")) { 
//               var account = props["account"] as CAccount;
//               generated_subject = ReplaceAccountInfo(generated_subject, is_japan, account);
//               generated_body = ReplaceAccountInfo(generated_body, is_japan, account);
//            }

//            if (props.ContainsKey("customer_info")) {
//                var customer_info = props["customer_info"] as CAccountInfo;
//                generated_subject = ReplaceCustomerInfo(generated_subject, is_japan, customer_info);
//                generated_body = ReplaceCustomerInfo(generated_body, is_japan, customer_info);
//            }

//            if (props.ContainsKey("customer")) {
//                var customer = props["customer"] as Customer;
//                generated_subject = ReplaceCustomer(generated_subject, is_japan, customer);
//                generated_body = ReplaceCustomer(generated_body, is_japan, customer);
//            }

//            if (props.ContainsKey("jpn_info")) {
//                Customer customer = null; 
//                if (props.ContainsKey("customer")) {
//                    customer = props["customer"] as Customer;
//                }
//                var jpn_info = props["jpn_info"] as JpnInfo;
//                generated_subject = ReplaceJpnInfo(generated_subject, is_japan, jpn_info, customer);
//                generated_body = ReplaceJpnInfo(generated_body, is_japan, jpn_info, customer);
//            }

//            if (props.ContainsKey("payment_info")) {
//                var payment_info = props["payment_info"] as PaymentInfo;
//                generated_subject = ReplacePaymentInfo(generated_subject, is_japan, payment_info);
//                generated_body = ReplacePaymentInfo(generated_body, is_japan, payment_info);
//            }

//            if (props.ContainsKey("booking")) {
//                var booking = props["booking"] as WtBooking;
//                generated_subject = ReplaceBooking(generated_subject, is_japan, booking, true);
//                generated_body = ReplaceBooking(generated_body, is_japan, booking);
//            }

//            if (props.ContainsKey("booking_list")) {
//                var booking_list = props["booking_list"] as List<WtBooking>;
//                generated_subject = ReplaceBookingList(generated_subject, additional_template, is_japan, booking_list, "", true);
//                generated_body = ReplaceBookingList(generated_body, additional_template, is_japan, booking_list, "");
//            }

//            if (props.ContainsKey("RQ_booking_list")) {
//                var booking_list = props["RQ_booking_list"] as List<WtBooking>;
//                generated_subject = ReplaceBookingList(generated_subject, additional_template, is_japan, booking_list, "RQ_", true);
//                generated_body = ReplaceBookingList(generated_body, additional_template, is_japan, booking_list, "RQ_");
//            }

//            if (props.ContainsKey("OK_booking_list")) {
//                var booking_list = props["OK_booking_list"] as List<WtBooking>;
//                generated_subject = ReplaceBookingList(generated_subject, additional_template, is_japan, booking_list, "OK_", true);
//                generated_body = ReplaceBookingList(generated_body, additional_template, is_japan, booking_list, "OK_");
//            }

//            if (props.ContainsKey("service_date")) {
//                var service_date = props["service_date"] as DateTime?;
//                generated_subject = ReplaceServiceDate(generated_subject, is_japan, service_date, null);
//                generated_body = ReplaceServiceDate(generated_body, is_japan, service_date, null);
//            }

//            if (props.ContainsKey("service_time")) {
//                var service_time = props["service_time"] as DateTime?;
//                generated_subject = ReplaceServiceDate(generated_subject, is_japan, null, service_time);
//                generated_body = ReplaceServiceDate(generated_body, is_japan, null, service_time);
//            }

//            //各種URLを設定ファイルから反映。
//            generated_subject = ReplaceUrls(generated_subject, is_japan);
//            generated_body = ReplaceUrls(generated_body, is_japan);

//            if (props.ContainsKey("reregisterUrl")) {
//                var reregister_url_key = props["reregisterUrl"] as string;
//                generated_subject = ReplaceReregisterUrl(generated_subject, reregister_url_key);
//                generated_body = ReplaceReregisterUrl(generated_body, reregister_url_key);
//            }

//            var email = new WtEmailQueue() { 
//                c_num = c_num,
//                template_cd = this.template_cd,
//                from_addr = generated_from_addr, 
//                subject =  generated_subject ,
//                body = generated_body,
//                create_date = RegionConfig.GetJapanNow(),
//                status = WtEmailQueue.Status.New,
//                send_date = null,
//                send_count = 0
//            };
//            return email;
//        }

//        private string ReplaceCNum(string str, string c_num) {
//            return str.Replace("%CNUM%", c_num);
//        }

//        private string ReplaceAccountTempInfo(string str, bool is_jpn, CAccountTemp accountTemp) {
//            if (accountTemp == null) return str;

//            var date_format = is_jpn ? "yyyy/MM/dd" : "MM/dd/yyyy";

//            var url = accountTemp.GetTempURL();
//            str = str.Replace("%TEMP_URL%", url);

//            var code = accountTemp.TempCode;
//            str = str.Replace("%TEMP_CODE%", code);

//            str = str.Replace("%TEMP_EMAIL%", accountTemp.email);
//            str = str.Replace("%TEMP_EFF_DATE%", accountTemp.eff_date.ToString(date_format));
//            str = str.Replace("%PRIMARY_ID_NAME%", accountTemp.primary_id_name);
//            return str;
//        }

//        private string ReplaceAccountInfo(string str, bool is_jpn, CAccount account) {
//            if (account == null) return str;

//            str = str.Replace("%J_LAST%", account.e_last_name);
//            str = str.Replace("%J_FIRST%", account.e_first_name);
//            str = str.Replace("%E_LAST%", account.e_last_name);
//            str = str.Replace("%E_FIRST%", account.e_first_name);
//            str = str.Replace("%EMAIL%", account.email);
//            str = str.Replace("%EMAIL_OLD%", account.email_old);
//            return str;
//        }

//        private string ReplaceCustomerInfo(string str, bool is_jpn, CAccountInfo cust) {
//            if (cust == null) return str;

//            str = str.Replace("%CUSTOMER_NAME_J%", ConcatNames(true, cust.last, cust.first, cust.last_kanji, cust.first_kanji));
//            str = str.Replace("%CUSTOMER_NAME_E%", ConcatNames(false, cust.last, cust.first, null, null));

//            if (cust.is_groom) {
//                str = str.Replace("%GROOM_NAME_E%", ConcatNames(false, cust.last, cust.first, null, null));
//                str = str.Replace("%BRIDE_NAME_E%", ConcatNames(false, cust.p_last, cust.p_first, null, null));
//                str = str.Replace("%GROOM_NAME_J%", ConcatNames(true, cust.last, cust.first, cust.last_kanji, cust.first_kanji));
//                str = str.Replace("%BRIDE_NAME_J%", ConcatNames(true, cust.p_last, cust.p_first, cust.p_last_kanji, cust.p_first_kanji));
//            } else {
//                str = str.Replace("%GROOM_NAME_E%", ConcatNames(false, cust.p_last, cust.p_first, null, null));
//                str = str.Replace("%BRIDE_NAME_E%", ConcatNames(false, cust.last, cust.first, null, null));
//                str = str.Replace("%GROOM_NAME_J%", ConcatNames(true, cust.p_last, cust.p_first, cust.p_last_kanji, cust.p_first_kanji));
//                str = str.Replace("%BRIDE_NAME_J%", ConcatNames(true, cust.last, cust.first, cust.last_kanji, cust.first_kanji));
//            }
//            return str;
//        }

//        /// <summary>
//        /// カスタマー氏名を連結する。
//        /// 日本語(is_japan=true)の場合、漢字氏名がnullならローマ字氏名を返す。
//        /// </summary>
//        /// <param name="is_japan"></param>
//        /// <param name="last"></param>
//        /// <param name="first"></param>
//        /// <param name="last_j"></param>
//        /// <param name="first_j"></param>
//        /// <returns></returns>
//        private string ConcatNames(bool is_japan, string last, string first, string last_j, string first_j) {
//            string s;
//            if (is_japan && !string.IsNullOrWhiteSpace(last_j) && !string.IsNullOrWhiteSpace(first_j)) {
//                s = last_j + " " + first_j;
//            } else {
//                s = first + " " + last;
//            }
//            return s;
//        }

//        private string ReplacePaymentInfo(string str, bool is_jpn, PaymentInfo payment_info) {
//            if (payment_info == null) return str;

//            return str;
//        }

//        private string ReplaceBookingList(string str, string additional_template, bool is_jpn, List<WtBooking> booking_list, string prefix, bool is_subject = false) {
//            if (booking_list == null) return str;

//            var header = ""; 
//            var details = "";
//            var footer = "";
//            var cur_fmt = "#,0" + (is_jpn ? "円" : " Yen");

//            if (booking_list.Count > 0) {
//                if (booking_list[0].price_type == 1 || booking_list[0].price_type == 2) {
//                    cur_fmt = RegionConfig.GetCurrencyFormatWithSymbol(booking_list[0].region_cd);
//                }

//                header = StringHelper.CutBetween(additional_template, "%" + prefix + "ITEMS_HEADER_START%", "%" + prefix + "ITEMS_HEADER_END%");
//                footer = StringHelper.CutBetween(additional_template, "%" + prefix + "ITEMS_FOOTER_START%", "%" + prefix + "ITEMS_FOOTER_END%");
//                var detail_template = StringHelper.CutBetween(additional_template, "%" + prefix + "ITEMS_START%", "%" + prefix + "ITEMS_END%");
//                if (!string.IsNullOrEmpty(detail_template)) {
//                    //明細行をループ処理
//                    foreach (var booking in booking_list) {
//                        details += ReplaceBooking(detail_template, is_jpn, booking, is_subject);
//                    }
//                }
//            } 
//            str = str.Replace("%" + prefix + "ITEMS%", header + details + footer);

//            //合計金額
//            var total_amount = booking_list.Sum(m => m.total);
//            str = str.Replace("%" + prefix + "TOTAL%", total_amount.ToString(cur_fmt));

//            var order_num = TypeHelper.GetStr(booking_list.Select(i => i.order_num).FirstOrDefault());
//            var order_date = RegionConfig.GetJapanToday();
//            var date_format = is_jpn ? "yyyy/MM/dd" : "MM/dd/yyyy";

//            //オーダー番号
//            str = str.Replace("%" + prefix + "ORDER_NUM%", order_num);

//            //商品点数
//            str = str.Replace("%" + prefix + "ORDER_COUNT%", booking_list.Count.ToString());            

//            //オーダー日
//            var str_order_date = order_date.ToString(date_format) + (is_jpn ? "　(日本時間)" : " (Japan Standard Time)");
//            str = str.Replace("%" + prefix + "ORDER_DATE%", str_order_date);

//            return str;
//        }

//        private string ReplaceBooking(string str, bool is_jpn, WtBooking booking, bool is_subject = false) {
//            if (booking == null) return str;

//            var date_format = is_jpn ? "yyyy/MM/dd" : "MM/dd/yyyy";

//            str = str.Replace("%ITEM_CD%", booking.trf_item_cd);

//            str = str.Replace("%SERVICE_DATE%", TypeHelper.GetDateTime(booking.service_date).ToString(date_format));
//            var service_time = TypeHelper.GetDateTime(booking.service_time).ToString("HH:mm");
//            str = str.Replace("%SERVICE_TIME%", service_time);

//            //金額の書式
//            var cur_fmt = "#,0" + (is_jpn ? "円" : " Yen");
//            if (booking.price_type == 1 || booking.price_type == 2) {
//                cur_fmt = RegionConfig.GetCurrencyFormatWithSymbol(booking.region_cd);
//            }

//            //単価
//            str = str.Replace("%PRICE%", TypeHelper.GetDecimal(booking.price_charge).ToString(cur_fmt));

//            //数量
//            str = str.Replace("%QUANTITY%", booking.quantity.ToString());

//            //行の合計金額
//            str = str.Replace("%AMOUNT%", booking.total.ToString(cur_fmt));

//            //キャンセルチャージ額
//            var cxl_charge = TypeHelper.GetDecimal(booking.cxl_charge);
//            str = str.Replace("%CXL_CHARGE%", cxl_charge.ToString(cur_fmt));

//            //リファンド額(合計金額-キャンセルチャージ額)
//            var refund = booking.total - cxl_charge;
//            str = str.Replace("%REFUND%", refund.ToString(cur_fmt));

//            if (is_subject) {
//                //Subjectの場合は、桁数100文字を超えるとテーブルInsertできないのでTrimする
//                var len_item_name = string.IsNullOrEmpty(booking.trf_item_name) ? 0 : booking.trf_item_name.Length;
//                if (str.Length - "%ITEM_NAME%".Length + len_item_name > 100) {
//                    var new_len_item_name = 100 - str.Length + "%ITEM_NAME%".Length - 3;
//                    var item_name = booking.trf_item_name.Substring(0, new_len_item_name) + "...";
//                    str = str.Replace("%ITEM_NAME%", item_name);
//                } else {
//                    str = str.Replace("%ITEM_NAME%", booking.trf_item_name);
//                }
//            } else {
//                str = str.Replace("%ITEM_NAME%", booking.trf_item_name);
//            }

//            return str;
//        }

//        private string ReplaceServiceDate(string str, bool is_jpn, DateTime? service_date, DateTime? service_time) {
//            var date_format = is_jpn ? "yyyy/MM/dd" : "MM/dd/yyyy";
//            str = str.Replace("%SERVICE_DATE%", TypeHelper.GetDateTime(service_date).ToString(date_format));
//            var str_service_time = TypeHelper.GetDateTime(service_time).ToString("HH:mm");
//            str = str.Replace("%SERVICE_TIME%", str_service_time);
//            return str;
//        }


//        private string ReplaceCustomer(string str, bool is_jpn, Customer customer) {
//            if (customer == null) return str;
//            str = str.Replace("%GROOM_NAME_E%", customer.g_first + " " + customer.g_last);
//            str = str.Replace("%BRIDE_NAME_E%", customer.b_first + " " + customer.b_last);

//            var missingCols =customer.GetMissingColumnsForMyWedding();
//            if (missingCols != null && missingCols.Count > 0) {
//                str = str.Replace("%MISSING_COLS%", missingCols.Aggregate("", (a, b) => {
//                    return string.IsNullOrEmpty(a) ? b : string.Format("{0}, {1}", a, b);
//                }));
//            }

//            return str;
//        }

//        private string ReplaceJpnInfo(string str, bool is_jpn, JpnInfo jpnInfo, Customer customer) {
//            if (jpnInfo == null) return str;

//            if (customer != null) {
//                str = str.Replace("%GROOM_NAME_J%", ConcatNames(true, customer.g_last, customer.g_first, jpnInfo.g_last_kanji, jpnInfo.g_first_kanji));
//                str = str.Replace("%BRIDE_NAME_J%", ConcatNames(true, customer.b_last, customer.b_first, jpnInfo.b_last_kanji, jpnInfo.b_first_kanji));
//            } else {
//                str = str.Replace("%GROOM_NAME_J%", jpnInfo.g_last_kanji + " " + jpnInfo.g_first_kanji);
//                str = str.Replace("%BRIDE_NAME_J%", jpnInfo.b_last_kanji + " " + jpnInfo.b_first_kanji);
//            }

//            return str;
//        }

//        private string ReplaceUrls(string str, bool is_jpn) {
//            //MyWeddingURLの編集
//            var url = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BaseURL"]) + "/MyWedding";
//            str = str.Replace("%MYWEDDING_URL%", url);

//            //LoginURLの編集
//            url = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BaseURL"])
//                + TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["LoginPath"]);
//            str = str.Replace("%LOGIN_URL%", url);

//            //ReminderUrlの編集
//            url = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BaseURL"]) 
//                + TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["LoginPath"]) 
//                + "/reregister";
//            str = str.Replace("%REMINDER_URL%", url);

//            //RQがOKになった後自動CXLされるまでの時間
//            var timeLimit = TypeHelper.GetInt(ConfigurationManager.AppSettings["NotPaidAutoCancelTimeLimit"]);
//            str = str.Replace("%AUTOCXL_LIMIT%", timeLimit.ToString());

//            //カスタマーサービスの連絡先
//            str = ReplaceCustomerServiceInfo(str, is_jpn);

//            return str;

//        }


//        private string ReplaceReregisterUrl(string str, string url_key) {
//            //ReregisterUrlの編集
//            var url = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BaseURL"])
//                    + TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["LoginPath"])
//                    +  "/ResetPassword?key=" 
//                    + url_key;
//            str = str.Replace("%REREGISTER_URL%", url);
//            return str;
//        }

//        private string ReplaceCustomerServiceInfo(string str, bool is_jpn) {
//            var customer_service_info = "***************************************************************************\r\n";
//            if (is_jpn) {
//                customer_service_info += TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["CompanyName"]) + "\r\n";
//                customer_service_info += TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SiteName"]) + "\r\n";
//            } else {
//                customer_service_info += TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["CompanyNameEng"]) + "\r\n";
//                customer_service_info += TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SiteNameEng"]) + "\r\n";
//            }
//            customer_service_info += "***************************************************************************\r\n";

//            str = str.Replace("%CUSTOMER_SERVICE_INFO%", customer_service_info);
//            return str;
//        }
//    }
//}
