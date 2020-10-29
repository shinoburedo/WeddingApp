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
    public partial class VendorOrderSheet_Datail_RCP : GrapeCity.ActiveReports.SectionReport {
        private VendorOrderSheetInfo mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public VendorOrderSheet_Datail_RCP(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
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
                txtPartyDate.Text = mInfo.Sales.ReceptionInfo.party_date.HasValue ? mInfo.Sales.ReceptionInfo.party_date.Value.ToString(mDateFormat) : "";
                txtPartyTime.Text = mInfo.Sales.ReceptionInfo.party_time.HasValue ? mInfo.Sales.ReceptionInfo.party_time.Value.ToString(mTimeFormat) : "";
                txtRestCd.Text = mInfo.Sales.ReceptionInfo.rest_cd;
            }
            if (mInfo.Item != null) {
                txtItemName.Text = mInfo.Item.item_name;
            }
        }


    }
}
