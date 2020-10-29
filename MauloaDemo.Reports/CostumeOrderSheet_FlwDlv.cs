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
    public partial class CostumeOrderSheet_FlwDlv: GrapeCity.ActiveReports.SectionReport {

        private DateTime _old_date = DateTime.MinValue;
        private string _old_time = string.Empty;


        public CostumeOrderSheet_FlwDlv(List<CostumeOrderSheetDeliveryInfo> scheduleSheetInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.DataSource = scheduleSheetInfo;
        }

        private void detail_Format(object sender, EventArgs e) {
            var cur_date = TypeHelper.GetDateTime(this.Fields["delivery_date"].Value);
            var cur_time = TypeHelper.GetDateTime(this.Fields["delivery_time"].Value).ToString("HH:mm");

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
                txtPhraseTime.Text = cur_time;
            } else {
                txtPhraseTime.Visible = false;
            }

        }
    }
}
