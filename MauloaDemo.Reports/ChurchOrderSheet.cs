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
    /// Summary description for SectionReport1.
    /// </summary>
    public partial class ChurchOrderSheet : GrapeCity.ActiveReports.SectionReport {

        private ChurchOrderSheetInfo mInfo;

        public ChurchOrderSheet(ChurchOrderSheetInfo info) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = info;
            this.DataSource = mInfo;
        }

        private void detail_Format(object sender, EventArgs e) {
            txtWedDate.Text = mInfo.WedInfo.req_wed_date.ToString("MM/dd/yyyy");
            txtWedTime.Text = mInfo.WedInfo.req_wed_time.ToString("HH:mm");
            txtChurch.Text = mInfo.Church.church_name;
            txtGroomName.Text = mInfo.Customer.GroomName;
            txtBrideName.Text = mInfo.Customer.BrideName;
            txtOption.Text = string.Empty;
            txtMemo.Text = string.Empty;
        }

        private void pageHeader_Format(object sender, EventArgs e) {
            textBox2.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyTel"]);
            textBox3.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyFax"]);
            txtChurchName.Text = mInfo.Church.church_name;
            txtChurchTel.Text = mInfo.Church.op_tel;
            txtChurchFax.Text = mInfo.Church.op_fax;
        }
    }
}
