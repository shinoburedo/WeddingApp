using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("customer")]
    public class Customer : IValidatableObject {

        [Key(), Hankaku()]
        public string c_num { get; set; }

        [Hankaku(), UpperCase()]
        public string agent_cd { get; set; }

        [Required(), Hankaku(), UpperCase()]
        public string sub_agent_cd { get; set; }


        [Required(), Hankaku(), UpperCase()]
        public string g_last { get; set; }

        [Hankaku(), UpperCase()]
        public string g_first { get; set; }

        [Hankaku(), UpperCase()]
        public string b_last { get; set; }

        [Hankaku(), UpperCase()]
        public string b_first { get; set; }

        public string g_last_kana { get; set; }
        public string g_first_kana { get; set; }
        public string b_last_kana { get; set; }
        public string b_first_kana { get; set; }
        public string g_last_kanji { get; set; }
        public string g_first_kanji { get; set; }
        public string b_last_kanji { get; set; }
        public string b_first_kanji { get; set; }

        [Required(), StringLength(3), Hankaku(), UpperCase()]
        public string area_cd { get; set; }

        [StringLength(20), Hankaku()]
        public string tour_cd { get; set; }

        [Hankaku(), UpperCase()]
        public string church_cd { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? wed_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? wed_time { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? htl_pick { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? bf_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? bf_time { get; set; }

        public string bf_place { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? ft_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? ft_time { get; set; }

        public string ft_place { get; set; }

        [UpperCase]
        public string in_flight { get; set; }

        [UpperCase]
        public string in_dep { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? in_dep_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? in_dep_time { get; set; }

        [UpperCase]
        public string in_arr { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? in_arr_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? in_arr_time { get; set; }

        [UpperCase]
        public string out_flight { get; set; }

        [UpperCase]
        public string out_dep { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? out_dep_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? out_dep_time { get; set; }

        [UpperCase]
        public string out_arr { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? out_arr_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? out_arr_time { get; set; }

        [Hankaku(), UpperCase()]
        public string hotel_cd { get; set; }

        [NotMapped]
        public string hotel_name { get; set; }

        public string room_number { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? checkin_date { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? checkout_date { get; set; }

        public string note { get; set; }

        public string staff_note { get; set; }

        public short attend_count { get; set; }

        [StringLength(100)]
        public string attend_name { get; set; }

        public string attend_memo { get; set; }

        public bool cust_cxl { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? cxl_date { get; set; }

        [Hankaku(), LowerCase()]
        public string cxl_by { get; set; }

        [DataType(DataType.DateTime), 
         DisplayFormat(DataFormatString = "MM/dd/yyyy HH:mm:ss"), 
         DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [IgnoreChangeDiff]
        public DateTime create_date { get; set; }

        [IgnoreChangeDiff]
        public string create_by { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy HH:mm:ss")]
        public DateTime? final_date { get; set; }

        [Hankaku(), LowerCase()]
        public string final_by { get; set; }

        [Required(), Hankaku(), LowerCase()]
        [IgnoreChangeDiff]
        public string last_person { get; set; }

        [DataType(DataType.DateTime), 
         DisplayFormat(DataFormatString = "MM/dd/yyyy HH:mm:ss"),
         DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        /// 排他制御のための更新日時比較で使うための値。ユーザーのTime Zoneによる補正処理からは除外される。
        [NotMapped]
        [IgnoreChangeDiff]
        public DateTime update_date_stamp { get; set; }


        [NotMapped]
        public bool is_sunset { get; set; }

        [NotMapped]
        public bool noDupCheck { get; set; }


        [NotMapped, JsonIgnore]
        public string GroomName {
            get {
                return TypeHelper.GetStrTrim(this.g_last) 
                    + (string.IsNullOrEmpty(this.g_first) ? "" : " " + TypeHelper.GetStrTrim(this.g_first));
            }
        }

        [NotMapped, JsonIgnore]
        public string GroomNameKanji {
            get {
                return TypeHelper.GetStrTrim(this.g_last_kanji) 
                    + (string.IsNullOrEmpty(this.g_first_kanji) ? "" : " " + TypeHelper.GetStrTrim(this.g_first_kanji));
            }
        }

        [NotMapped, JsonIgnore]
        public string BrideName {
            get {
                return TypeHelper.GetStrTrim(this.b_last) 
                    + (string.IsNullOrEmpty(this.b_first) ? "" : " " + TypeHelper.GetStrTrim(this.b_first));
            }
        }

        [NotMapped, JsonIgnore]
        public string BrideNameKanji {
            get {
                return TypeHelper.GetStrTrim(this.b_last_kanji)
                    + (string.IsNullOrEmpty(this.b_first_kanji) ? "" : " " + TypeHelper.GetStrTrim(this.b_first_kanji));
            }
        }

        [NotMapped, JsonIgnore]
        public string CustomerNames {
            get {
                return this.GroomName 
                    + (string.IsNullOrEmpty(this.BrideName) ? "" : " / " + this.BrideName);
            }
        }

        [NotMapped, JsonIgnore]
        public string CustomerNamesKanji {
            get {
                return this.GroomNameKanji 
                    + (string.IsNullOrEmpty(this.BrideNameKanji) ? "" : " / " + this.BrideNameKanji);
            }
        }


        [NotMapped]
        public string wed_time_s {
            get {
                if (is_sunset) {
                    return "Sunset";
                } else { 
                    return TypeHelper.TimeHHmm(this.wed_time);
                }
            }
            set {
                var SunsetBlockTime = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SunsetBlockTime"]);
                if (string.Equals(value, "Sunset")) {
                    value = SunsetBlockTime;
                }
                this.wed_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string htl_pick_s {
            get {
                return TypeHelper.TimeHHmm(this.htl_pick);
            }
            set {
                this.htl_pick = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string bf_time_s {
            get {
                return TypeHelper.TimeHHmm(this.bf_time);
            }
            set {
                this.bf_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string ft_time_s {
            get {
                return TypeHelper.TimeHHmm(this.ft_time);
            }
            set {
                this.ft_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string in_dep_time_s {
            get {
                return TypeHelper.TimeHHmm(this.in_dep_time);
            }
            set {
                this.in_dep_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string in_arr_time_s {
            get {
                return TypeHelper.TimeHHmm(this.in_arr_time);
            }
            set {
                this.in_arr_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string out_dep_time_s {
            get {
                return TypeHelper.TimeHHmm(this.out_dep_time);
            }
            set {
                this.out_dep_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        [NotMapped]
        public string out_arr_time_s {
            get {
                return TypeHelper.TimeHHmm(this.out_arr_time);
            }
            set {
                this.out_arr_time = TypeHelper.GetDateTimeOrNull(value);
            }
        }

        public class ChangedRows {
            public ICollection<string> c_num { get; set; }
            public ICollection<bool> dup_check_done { get; set; }
        }

        public class Note {
            public string c_num { get; set; }
            public string note { get; set; }
            public bool edit_all { get; set; }
        }

        public void ValidateSave() {
            if (string.IsNullOrEmpty(this.g_last)) {
                throw new Exception("Please input a last name for the groom.");
            }
            if (string.IsNullOrEmpty(this.area_cd)) {
                throw new Exception("Please select an area.");
            }
            if (string.IsNullOrEmpty(this.sub_agent_cd)) {
                throw new Exception("Please select an agent code.");
            }
            if (string.IsNullOrEmpty(this.last_person)) {
                throw new Exception("User id is required in order to save customer.");
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }

            if (!string.IsNullOrEmpty(this.agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.agent_cd, false)) {
                yield return new ValidationResult(string.Format("agent_cd は半角英数字で入力してください。({0})", this.agent_cd), new[] { "agent_cd" });
            }

            if (!string.IsNullOrEmpty(this.sub_agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.sub_agent_cd, false)) {
                yield return new ValidationResult(string.Format("sub_agent_cd は半角英数字で入力してください。({0})", this.sub_agent_cd), new[] { "sub_agent_cd" });
            }

            if (!string.IsNullOrEmpty(this.g_last) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.g_last, true)) {
                yield return new ValidationResult(string.Format("g_last は半角英数字で入力してください。({0})", this.g_last), new[] { "g_last" });
            }

            if (!string.IsNullOrEmpty(this.g_first) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.g_first, true)) {
                yield return new ValidationResult(string.Format("g_first は半角英数字で入力してください。({0})", this.g_first), new[] { "g_first" });
            }

            if (!string.IsNullOrEmpty(this.b_last) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.b_last, true)) {
                yield return new ValidationResult(string.Format("b_last は半角英数字で入力してください。({0})", this.b_last), new[] { "b_last" });
            }

            if (!string.IsNullOrEmpty(this.b_first) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.b_first, true)) {
                yield return new ValidationResult(string.Format("b_first は半角英数字で入力してください。({0})", this.b_first), new[] { "b_first" });
            }
        }
    }
}
