using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("mgm_report_param")]
    public class MgmReportParam : IValidatableObject {

        public MgmReportParam(){
            this.param_type = "text";
            this.field_length = 5;
            this.decimal_length = 0;
            this.required = false;
            this.default_value = "";
            this.list_cd = "";
        }

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int param_id { get; set; }

        [Required(), StringLength(20), Hankaku(), UpperCase()]
        public string rep_cd { get; set; }

        public short param_seq { get; set; }

        [Required(), StringLength(50), Hankaku()]
        public string param_name { get; set; }

        [Required(), StringLength(10), Hankaku()]
        public string param_type { get; set; }

        [StringLength(20)]
        public string caption { get; set; }

        [StringLength(10)]
        public string prefix { get; set; }

        [StringLength(10)]
        public string suffix { get; set; }

        public bool break_after { get; set; }

        public short field_length { get; set; }

        public byte decimal_length { get; set; }

        public bool required { get; set; }

        [StringLength(20)]
        public string default_value { get; set; }

        [StringLength(50), Hankaku()]
        public string list_cd { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        //レポート実行時に入力された値を格納するフィールド。
        [NotMapped]
        public string input_value { get; set; }


        public void SetDefaultValue(LoginUser user, string region_cd, short time_diff) {
            //パラメータ名は常に小文字で比較する。
            this.param_name = this.param_name.ToLower();

            if (string.IsNullOrEmpty(this.default_value)) {
                this.input_value = string.Empty;
                return;
            }

            var date_fmt = string.IsNullOrEmpty(user.date_format) ? "MM/dd/yyyy" : user.date_format;
            if (this.field_length == 8) date_fmt = date_fmt.Replace("yyyy", "yy");
            var time_fmt = string.IsNullOrEmpty(user.time_format) ? "HH:mm" : user.time_format;
            var today = DateTime.UtcNow.AddHours(time_diff).Date;

            var def_val = defValueWithoutNumber();

            switch (def_val) { 
                case "$TODAY":
                    this.input_value = today.ToString(date_fmt);
                    break;
                case "$YESTERDAY":
                    this.input_value = today.AddDays(-1).ToString(date_fmt);
                    break;
                case "$TOMORROW":
                    this.input_value = today.AddDays(1).ToString(date_fmt);
                    break;
                case "$WEEKAGO":
                    this.input_value = today.AddDays(-7).ToString(date_fmt);
                    break;
                case "$MONTHAGO":
                    this.input_value = today.AddMonths(-1).ToString(date_fmt);
                    break;
                case "$YEARAGO":
                    this.input_value = today.AddYears(-1).ToString(date_fmt);
                    break;
                case "$WEEKLATER":
                    this.input_value = today.AddDays(7).ToString(date_fmt);
                    break;
                case "$MONTHLATER":
                    this.input_value = today.AddMonths(1).ToString(date_fmt);
                    break;
                case "$YEARLATER":
                    this.input_value = today.AddYears(1).ToString(date_fmt);
                    break;
                case "$STARTOFMONTH":
                    this.input_value = new DateTime(today.Year, today.Month, 1).ToString(date_fmt);
                    break;
                case "$ENDOFMONTH":
                    this.input_value = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1).ToString(date_fmt);
                    break;
                case "$STARTOFLASTMONTH":
                    this.input_value = new DateTime(today.Year, today.Month, 1).AddMonths(-1).ToString(date_fmt);
                    break;
                case "$ENDOFLASTMONTH":
                    this.input_value = new DateTime(today.Year, today.Month, 1).AddDays(-1).ToString(date_fmt);
                    break;
                case "$ADDDAYS":
                    var days = extractNumber();
                    this.input_value = today.AddDays(days).ToString(date_fmt);
                    break;
                case "$ADDMONTHS":
                    var months = extractNumber();
                    this.input_value = today.AddMonths(months).ToString(date_fmt);
                    break;
                case "$ADDYEARS":
                    var years = extractNumber();
                    this.input_value = today.AddYears(years).ToString(date_fmt);
                    break;

                case "$USERID":
                    this.input_value = user.login_id;
                    break;
                case "$USERAREA":
                    this.input_value = user.area_cd;
                    break;
                //case "$DEFAULTAREA":
                //    this.input_value = RegionConfig.GetDefaultAreaCd(region_cd);
                //    break;

                default:
                    if ("check".Equals(this.param_type)) {
                        this.input_value = TypeHelper.GetBool(this.default_value) ? "1" : "0";
                    } else {
                        this.input_value = this.default_value;
                    }
                    break;
            }

        }

        private string defValueWithoutNumber() {
            var def_val = string.IsNullOrEmpty(this.default_value) ? "" : this.default_value.ToUpper();
            var ix = def_val.IndexOfAny("([|:".ToCharArray());
            if (ix < 0) return def_val;
            var str = def_val.Substring(0, ix);
            return TypeHelper.GetStrTrim(str);
        }

        private int extractNumber() {
            var def_val = string.IsNullOrEmpty(this.default_value) ? "" : this.default_value.ToUpper();
            var ix = def_val.IndexOfAny("([|:".ToCharArray());
            if (ix < 0) return 0;
            var str_num = def_val.Substring(ix + 1).Replace(")","").Replace("]","");
            var num = TypeHelper.GetInt(str_num);
            return num;
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.rep_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.rep_cd, false)) {
                yield return new ValidationResult(string.Format("Report Code は半角英数字で入力してください。({0})", this.rep_cd), new[] { "rep_cd" });
            }

            if (!string.IsNullOrEmpty(this.param_name) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.param_name, false)) {
                yield return new ValidationResult(string.Format("Parameter Name は半角英数字で入力してください。({0})", this.param_name), new[] { "param_name" });
            }

            if (!string.IsNullOrEmpty(this.param_type) && !"[text][number][date][check][radio][combo][hidden]".Contains(this.param_type)) {
                yield return new ValidationResult(string.Format("Param Type は text, number, date, check, radio, combo, hidden のいずれかを入力して下さい。({0})", this.param_type), new[] { "param_type" });
            }

            if (this.field_length < 0) {
                yield return new ValidationResult(string.Format("Field Length は 0以上の数値のみ入力可能です。({0})", this.field_length), new[] { "field_length" });
            }

            if (this.decimal_length >= 18 || this.decimal_length < 0) {
                yield return new ValidationResult(string.Format("Decimal Length は 0以上18未満の数値のみ入力可能です。({0})", this.decimal_length), new[] { "decimal_length" });
            }

            if (this.decimal_length > this.field_length) {
                yield return new ValidationResult(string.Format("Decimal Length に Field Length より大きな値をセットする事は出来ません。(Field Length:{0}, Decimal Length:{1})", this.field_length, this.decimal_length), new[] { "decimal_length" });
            }

            if (string.Equals("combo", this.param_type) && string.IsNullOrEmpty(this.list_cd)){
                yield return new ValidationResult("list_cd is required when Param Type is 'combo'.", new[] { "list_cd" });
            }

            if (!string.IsNullOrEmpty(this.list_cd) && !this.list_cd.StartsWith("[")) {
                //list_cdは固定リストでない場合のみ大文字で保存。固定リストの場合は入力された通りに保存。
                this.list_cd = this.list_cd.ToUpper();
            }
        }
    }

    public class MgmReportParamComparer : IComparer<MgmReportParam> {
        public int Compare(MgmReportParam x, MgmReportParam y) {
            if (string.Equals(x.rep_cd, y.rep_cd)) {
                return x.param_seq - y.param_seq;
            } else {
                return string.Compare(x.rep_cd, y.rep_cd);
            }
        }
    }
}
