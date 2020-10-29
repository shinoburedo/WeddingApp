using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF;
using System.Configuration;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for ScheduleSheet.
    /// </summary>
    public partial class ScheduleSheet : GrapeCity.ActiveReports.SectionReport {

        private ScheduleSheetInfo mInfo;
        private DateTime _old_date = DateTime.MinValue;
        private string _old_time = string.Empty;
        private string _old_place = string.Empty;


        public ScheduleSheet(ScheduleSheetInfo scheduleSheetInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = scheduleSheetInfo;
            this.DataSource = mInfo.Phrases;
        }


        private void reportHeader1_Format(object sender, EventArgs e) {
            try {
                var title = "『" + mInfo.WedInfo.item_name_jpn + "』";

                //TextBoxの幅に合わせて文字列が収まる様にフォントサイズを調整する。
                txtTitle.AdjustFontSizeToFitWidth(this.CurrentPage, title);

                txtCNum.Text = mInfo.Customer.c_num;

                var groomName = TypeHelper.GetStrTrim(mInfo.Customer.GroomName);
                txtGroomName.Text = "MR. " + groomName;
                var brideName = TypeHelper.GetStrTrim(mInfo.Customer.BrideName);
                txtBrideName.Text = "MS. " + brideName;

                txtWedDateTime.Text = mInfo.WedInfo.req_wed_date.ToString("yyyy年 M月 d日") + "　" + mInfo.WedInfo.req_wed_time_s;
                txtWedDateTime.Text += "　" + (mInfo.WedInfo.isPhoto ? "撮影" : "挙式");

                txtChurchName.Text = mInfo.Church.church_name_jpn;
                txtChurchName.Visible = mInfo.WedInfo.isWedding;

                label6.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyName"]);
                textBox5.Text = "TEL : " + TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyTel"]);
                textBox6.Text = "（営業時間:" + TypeHelper.GetStr(ConfigurationManager.AppSettings["BusinessHours"]) + "）";

            } catch (Exception) {
                //Ignore for now.
            }
        }

        private void detail_Format(object sender, EventArgs e) {
            var cur_date =  TypeHelper.GetDateTime(this.Fields["date"].Value);
            var cur_time = TypeHelper.GetStr(this.Fields["time"].Value);
            var cur_place = TypeHelper.GetStr(this.Fields["place"].Value);

            //日付が前と同じなら表示しない。
            if (!DateTime.Equals(cur_date, _old_date)) {
                _old_date = cur_date;
                txtPhraseDate.Visible = true;
            } else {
                txtPhraseDate.Visible = false;
            }

            //時間が前と同じなら表示しない。
            if (!string.Equals(cur_time, _old_time)) {
                _old_time = cur_time;
                txtPhraseTime.Visible = true;
            } else {
                txtPhraseTime.Visible = false;
            }

            ////場所が前と同じなら表示しない。
            //if (!string.Equals(cur_place, _old_place)) {
            //    _old_place = cur_place;
            //    txtPhrasePlace.Visible = true;
            //} else {
            //    txtPhrasePlace.Visible = false;
            //}
        }

        private void pageFooter_Format(object sender, EventArgs e) {
        }

        private void reportFooter1_Format(object sender, EventArgs e) {
            txtHotel.Visible = !string.IsNullOrEmpty(mInfo.Customer.hotel_name);
            txtHotel.Text = "ご滞在ホテル： " + mInfo.Customer.hotel_name;

            txtWeddingNote.Visible = mInfo.WedInfo.isWedding;

            subNotes.Report = new ScheduleSheet_Notes();
            subNotes.Report.DataSource = mInfo.Notes;
        }

    }
}
