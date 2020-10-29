using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models.Combined;
using MauloaDemo.Models;
using CBAF;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for DailyMovement.
    /// </summary>
    public partial class DailyMovement : GrapeCity.ActiveReports.SectionReport {

        private bool mEnglish = false;
        private int mCount = 0;
        private string mDateFormat = "";
        private string mTimeFormat = "";

        public DailyMovement(List<MauloaDemo.Models.Combined.DailyMovement> data, string dateFormat, string timeFormat, bool english, LoginUser user) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mEnglish = english;
            mCount = data.Count;
            mDateFormat = dateFormat;
            mTimeFormat = timeFormat;
            this.DataSource = data;
            txtPrintDate.Text = DateTime.Now.ToString(dateFormat + " " + timeFormat);
            DateTime? wed_date = null;
            foreach (var detail in data) {
                if (detail.wed_date.HasValue) {
                     wed_date = detail.wed_date.Value;
                    break;
                }
            }
            if (wed_date.HasValue) {
                txtTitle.Text = "Daily Movement " + wed_date.Value.ToString(dateFormat);
            }
        }

        private void groupHeader1_Format(object sender, EventArgs e) {
            if (mCount == 0) {
                return;
            }
            var checkin_date = TypeHelper.GetDateTimeOrNull(this.Fields["checkin_date"].Value);
            var checkout_date = TypeHelper.GetDateTimeOrNull(this.Fields["checkout_date"].Value);
            txtCheckIn.Text = checkin_date.HasValue ? checkin_date.Value.ToString(mDateFormat) : "";
            txtCheckOut.Text = checkout_date.HasValue ? checkout_date.Value.ToString(mDateFormat) : "";

        }
    }
}
