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
    public partial class VendorOrderSheet_Datail_OTHER : GrapeCity.ActiveReports.SectionReport {

        private VendorOrderSheetInfo mInfo;

        public VendorOrderSheet_Datail_OTHER(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = info;
            this.DataSource = info;
        }

        private void detail_Format(object sender, EventArgs e) {
            if (mInfo.Item != null) {
                txtItemName.Text = mInfo.Item.item_name;
            }
        }


    }
}
