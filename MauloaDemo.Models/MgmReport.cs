using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("mgm_report")]
    public class MgmReport : IValidatableObject {

        public MgmReport() {
            this.menu_cd = "Management";
            this.rep_sp = "usp_mgm_rpt_";
            this.output_type = "csv";
            this.write_header = true;
            this.sheet_num = 1;
            this.start_pos = "A1";
            this.hidden = false;
            this.Params = new List<MgmReportParam>();
        }


        [Key(), Required(), StringLength(20), Hankaku(), UpperCase()]
        public string rep_cd { get; set; }              //レポートを一意に識別するためのコード

        [Required(), StringLength(15), Hankaku()]
        public string menu_cd { get; set; }             //画面上で表示するメニューのカテゴリ(例： Reservation, Showroom, Management, ... etc.)

        public short rep_seq { get; set; }              //表示順序

        [Required(), StringLength(50)]
        public string rep_name { get; set; }            //レポートの表示名(ボタンに表示される)

        [Required(), StringLength(30), Hankaku()]
        public string rep_sp { get; set; }              //実行するストアド名

        [StringLength(200)]
        public string description { get; set; }         //レポートの説明

        [Required(), StringLength(5), Hankaku()]
        public string output_type { get; set; }         //"csv" or "excel"

        [StringLength(50)]
        public string output_name { get; set; }         //出力時のファイル名

        [StringLength(50)]
        public string excel_name { get; set; }          //Excelテンプレートのファイル名

        public short sheet_num { get; set; }            //Excelテンプレートファイル内の出力対象シートの番号。(1始まり)

        [StringLength(5), Hankaku(), UpperCase()]
        public string start_pos { get; set; }           //Excelテンプレートファイル内の出力開始位置。 (左上が"A1"。例： "A1", "B2",...)

        public bool write_header { get; set; }          //出力結果に列見出しを含めるか否か。

        public bool hidden { get; set; }                //画面にメニューボタンを表示するか否か。

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [ForeignKey("rep_cd")]
        public virtual ICollection<MgmReportParam> Params { get; set; }         //ストアドに渡すパラメータのコレクション



        [NotMapped]
        public bool is_new { get; set; }




        public void SortParams() {
            if (this.Params == null) return;

            var orderedParams = this.Params.ToArray().OrderBy(m => m.param_seq).ToList();
            this.Params = orderedParams;
        }

        public void SetDefaultValues(LoginUser user, string region_cd,short time_diff) {
            foreach (var p in this.Params) {
                p.SetDefaultValue(user, region_cd, time_diff);
            }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.rep_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.rep_cd, false)) {
                yield return new ValidationResult(string.Format("Report Code は半角英数字で入力して下さい。({0})", this.rep_cd), new[] { "rep_cd" });
            }

            if (!string.IsNullOrEmpty(this.output_type) && !"[csv][excel]".Contains(this.output_type)) {
                yield return new ValidationResult(string.Format("Output Type は csv, excel のどちらかを入力して下さい。({0})", this.output_type), new[] { "output_type" });
            }

            if (this.output_type == "excel" && this.sheet_num < 1) {
                yield return new ValidationResult(string.Format("Sheet Number は 1以上の数値のみ入力可能です。({0})", this.sheet_num), new[] { "sheet_num" });
            }

        }
    }
}

