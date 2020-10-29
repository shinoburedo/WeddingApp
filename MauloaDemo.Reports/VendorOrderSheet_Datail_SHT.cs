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
    public partial class VendorOrderSheet_Datail_SHT : GrapeCity.ActiveReports.SectionReport {
        private VendorOrderSheetInfo mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public VendorOrderSheet_Datail_SHT(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
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
                txtShootDate.Text = mInfo.Sales.ShootInfo.shoot_date.HasValue ? mInfo.Sales.ShootInfo.shoot_date.Value.ToString(mDateFormat) : "";
                txtShootTime.Text = mInfo.Sales.ShootInfo.shoot_time.HasValue ? mInfo.Sales.ShootInfo.shoot_time.Value.ToString(mTimeFormat) : "";
                txtShootPlace.Text = mInfo.Sales.ShootInfo.shoot_place;
                txtItemName.Text = mInfo.Sales.ShootInfo.note;
            }
            if (mInfo.Item != null) {
                txtItemName.Text = mInfo.Item.item_name;
            }
        }


    }
}
