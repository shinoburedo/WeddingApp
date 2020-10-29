using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    public partial class VendorConfirmation_Datail_OTHER : GrapeCity.ActiveReports.SectionReport
    {

        List<VendorConfirmationReport> mInfo;

        public VendorConfirmation_Datail_OTHER(List<VendorConfirmationReport> info, string dateFormat, string timeFormat)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            txtwed_date.OutputFormat = dateFormat;
            txtwed_time.OutputFormat = timeFormat;

            mInfo = info;
            this.DataSource = info;
        }

        private void detail_Format(object sender, EventArgs e)
        {
            if (mInfo == null || mInfo.Count == 0) return;
            txtg_name.Text = TypeHelper.GetStr(this.Fields["g_first"].Value) + " " + TypeHelper.GetStr(this.Fields["g_last"].Value);
            txtb_name.Text = TypeHelper.GetStr(this.Fields["b_first"].Value) + " " + TypeHelper.GetStr(this.Fields["b_last"].Value);
        }

    }
}
