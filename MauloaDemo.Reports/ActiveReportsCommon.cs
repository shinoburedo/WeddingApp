using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Reports {

    public static class ActiveReportsCommon {

        /// TextBoxの幅に合わせて文字列が収まる様にフォントサイズを調整する。
        /// 
        /// 本来はTextBoxのShrinkToFitプロパティをtrueにすれば良いはずだが、
        /// やってみたところ今ひとつ文字が途切れたりするケースがあるので自前で調整する。
        /// 
        /// 参考URL：http://arhelp.grapecity.com/groups/topic/how-can-i-adjust-the-font-size-of-textbox-autmaticly/
        ///
        public static void AdjustFontSizeToFitWidth(
                                this GrapeCity.ActiveReports.SectionReportModel.TextBox txt,
                                GrapeCity.ActiveReports.Document.Section.Page page, 
                                string s) {
            var sz =  page.MeasureText(s, txt.Font);
            float k = (float)(txt.Width / (sz.Width + sz.Width * 0.075));
            var f = new System.Drawing.Font(txt.Font.FontFamily.Name, k * txt.Font.Size);
            txt.Font = f;
            txt.Text = s;
        }


    }
}
