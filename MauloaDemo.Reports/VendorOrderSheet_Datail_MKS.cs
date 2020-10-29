using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    public partial class VendorOrderSheet_Datail_MKS : GrapeCity.ActiveReports.SectionReport {
        private VendorOrderSheetInfo mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public VendorOrderSheet_Datail_MKS(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = info;
            mDateFormat = dateFormat;
            mTimeFormat = timeFormat;
            this.DataSource = info;
        }

        private void detail_Format(object sender, EventArgs e) {
            if (mInfo.Sales != null) {
                txtMakeTime.Text = mInfo.Sales.MakeInfo.make_time.HasValue ? mInfo.Sales.MakeInfo.make_time.Value.ToString(mTimeFormat) : "";
                txtMakePlace.Text = mInfo.Sales.MakeInfo.make_place;
                txtMakeInTime.Text = mInfo.Sales.MakeInfo.make_in_time.HasValue ? mInfo.Sales.MakeInfo.make_in_time.Value.ToString(mTimeFormat) : "";
            }
            if (mInfo.Item != null) {
                textBox11.Text = mInfo.Item.item_name;
            }
        }


    }
}
