using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using CBAF;
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for CostumeOrderSheet_Schedule.
    /// </summary>
    public partial class CostumeOrderSheet_Schedule : GrapeCity.ActiveReports.SectionReport {

        private ScheduleSheetInfo mInfo;
        private DateTime _old_date = DateTime.MinValue;
        private string _old_time = string.Empty;
        private string _old_place = string.Empty;
        private bool mEnglish;

        public CostumeOrderSheet_Schedule(ScheduleSheetInfo scheduleSheetInfo, bool english = true) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = scheduleSheetInfo;
            this.DataSource = mInfo.Phrases;
            mEnglish = english;
        }

        private void detail_Format(object sender, EventArgs e) {
            var cur_date = TypeHelper.GetDateTime(this.Fields["date"].Value);
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

            //場所が前と同じなら表示しない。
            if (!string.Equals(cur_place, _old_place)) {
                _old_place = cur_place;
                txtPhrasePlace.Visible = true;
            } else {
                txtPhrasePlace.Visible = false;
            }

            var title = mEnglish ? string.IsNullOrEmpty(TypeHelper.GetStr(this.Fields["title_eng"].Value)) ? TypeHelper.GetStr(this.Fields["title"].Value) : TypeHelper.GetStr(this.Fields["title_eng"].Value) : TypeHelper.GetStr(this.Fields["title"].Value);
            txtPhraseTitle.Text = title;
            var place = mEnglish ? string.IsNullOrEmpty(TypeHelper.GetStr(this.Fields["place_eng"].Value)) ? TypeHelper.GetStr(this.Fields["place"].Value) : TypeHelper.GetStr(this.Fields["place_eng"].Value) : TypeHelper.GetStr(this.Fields["place"].Value);
            txtPhrasePlace.Text = place;
        }
    }
}
