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
    public partial class VendorOrderSheet_Datail_TRN : GrapeCity.ActiveReports.SectionReport {
        private VendorOrderSheetInfo mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public VendorOrderSheet_Datail_TRN(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
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
                var pickupDate = mInfo.Sales.TransInfo.pickup_date.HasValue ? mInfo.Sales.TransInfo.pickup_date.Value.ToString(mDateFormat) : "";
                var pickupTime = mInfo.Sales.TransInfo.pickup_time.HasValue ? mInfo.Sales.TransInfo.pickup_time.Value.ToString(mTimeFormat) : "";
                var dropoffTime = mInfo.Sales.TransInfo.dropoff_time.HasValue ? mInfo.Sales.TransInfo.dropoff_time.Value.ToString(mTimeFormat) : "";
                txtPickUpDate.Text = pickupDate + " " + pickupTime;
                txtPickUpPlace.Text = mInfo.Sales.TransInfo.pickup_hotel_name + " " + mInfo.Sales.TransInfo.pickup_place;
                txtDropoffDate.Text = pickupDate + " " + dropoffTime;
                txtDropOffPlace.Text = mInfo.Sales.TransInfo.dropoff_hotel_name + " " + mInfo.Sales.TransInfo.dropoff_place;
            }
            if (mInfo.Item != null) {
                txtItemName.Text = mInfo.Item.item_name;
            }
        }


    }
}
