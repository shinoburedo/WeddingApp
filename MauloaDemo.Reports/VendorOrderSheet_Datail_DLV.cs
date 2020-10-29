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
    public partial class VendorOrderSheet_Datail_DLV : GrapeCity.ActiveReports.SectionReport {
        private VendorOrderSheetInfo mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public VendorOrderSheet_Datail_DLV(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
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
            if (mInfo.Item != null) {
                txtItemName.Text = mInfo.Item.item_name;
            }

            if (mInfo.Sales != null) {
                txtDlvDate.Text = mInfo.Sales.DeliveryInfo.delivery_date.HasValue ? mInfo.Sales.DeliveryInfo.delivery_date.Value.ToString(mDateFormat) : "";
                txtDlvTime.Text = mInfo.Sales.DeliveryInfo.delivery_time.HasValue ? mInfo.Sales.DeliveryInfo.delivery_time.Value.ToString(mTimeFormat) : "";
                txtDlvPlace.Text = mInfo.Sales.DeliveryInfo.delivery_place;
            }
        }


    }
}
