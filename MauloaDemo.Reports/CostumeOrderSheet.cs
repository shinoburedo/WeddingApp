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
    /// Summary description for VendorOrderSheet.
    /// </summary>
    public partial class CostumeOrderSheet : GrapeCity.ActiveReports.SectionReport {

        private CostumeOrderSheetInfo mInfo;

        public CostumeOrderSheet(CostumeOrderSheetInfo info, string dateFormat, string timeFormat, ScheduleSheetInfo scheduleSheetInfo, List<SalesListItem> wedInfo, List<CosInfo> cosInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = info;

            //Header
            //this.txtVendorName.Text = info.Vendor.vendor_name;
            //this.txtVendorTel.Text = info.Vendor.op_tel;
            //this.txtVendorFax.Text = info.Vendor.op_fax;
            textBox2.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyTel"]);
            textBox3.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyFax"]);

            //Customer info
            if (info.Customer != null) {
                txtGroomName.Text = info.Customer.g_last + " " + info.Customer.g_first;
                txtBrideName.Text = info.Customer.b_last + " " + info.Customer.b_first;
                txtWedDate.Text = info.Customer.wed_date.HasValue ? info.Customer.wed_date.Value.ToString(dateFormat) : "";
                txtWedTime.Text = info.Customer.wed_time.HasValue ? info.Customer.wed_time.Value.ToString(timeFormat) : "";
                txtRoom.Text = info.Customer.room_number;
            }
            txtChurch.Text = info.church_name;
            txtHotel.Text = info.hotel_name;
            txtAgent.Text = info.agent_name;
            txtPlan.Text = info.plan_name;

            //Costume info
            //if (info.CosInfo != null)
            //{
            //    var type_name = "";
            //    switch (info.CosInfo.pax_type)
            //    {
            //        case "G":
            //            type_name = "Groom";
            //            break;
            //        case "B":
            //            type_name = "Bride";
            //            break;
            //        case "A":
            //            type_name = "Attendance";
            //            break;
            //    }
            //    var order = "Type: " + type_name;
            //    order += String.IsNullOrEmpty(info.CosInfo.height) ? "" : " , Height: " + info.CosInfo.height;
            //    order += String.IsNullOrEmpty(info.CosInfo.chest) ? "" : " , Chest: " + info.CosInfo.chest;
            //    order += String.IsNullOrEmpty(info.CosInfo.waist) ? "" : " , Waist: " + info.CosInfo.waist;
            //    order += String.IsNullOrEmpty(info.CosInfo.cloth_size) ? "" : " , Cloth_size: " + info.CosInfo.cloth_size;
            //    order += String.IsNullOrEmpty(info.CosInfo.shoe_size) ? "" : " , Shoe_size: " + info.CosInfo.shoe_size;
            //    txtOrder.Text = order;
            //    txtMemo.Text = info.CosInfo.note;
            //}

            //Orders
            if (wedInfo != null && wedInfo.Count > 0) {
                var sub = new CostumeOrderSheet_Orders(wedInfo);
                this.subReport2.Report = sub;
            }

            //Schedule info
            if (scheduleSheetInfo != null && scheduleSheetInfo.Phrases.Count > 0) {
                var sub = new CostumeOrderSheet_Schedule(scheduleSheetInfo);
                this.subReport1.Report = sub;
            }

            //CosInfo
            if (cosInfo != null && cosInfo.Count > 0) {
                var sub = new CostumeOrderSheet_CosInfos(cosInfo);
                this.subReport3.Report = sub;
            }

            ////Hair Make
            //if (info.MksVendorList != null && info.MksVendorList.Count > 0) {
            //    var make = "";
            //    foreach (var vendor in info.MksVendorList) {
            //        make += string.IsNullOrEmpty(make) ? vendor.vendor_name : ", " + vendor.vendor_name;
            //    }
            //    txtHairMake.Text = make;
            //}

            ////Flower Delivery
            //if (info.FlwDlvList != null && info.FlwDlvList.Count > 0) {
            //    var sub = new CostumeOrderSheet_FlwDlv(info.FlwDlvList);
            //    this.subReport2.Report = sub;
            //}

            //Vendor Info
            if (info.CosVendor != null) {
                txtVendorName.Text = info.CosVendor.vendor_name;
                txtVendorTel.Text = info.CosVendor.op_tel;
                txtVendorFax.Text = info.CosVendor.op_fax;
            }

            this.DataSource = mInfo;
        }


    }
}
