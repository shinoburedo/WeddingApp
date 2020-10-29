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
    public partial class VendorOrderSheet : GrapeCity.ActiveReports.SectionReport {

        private VendorOrderSheetInfo mInfo;

        public VendorOrderSheet(VendorOrderSheetInfo info, string dateFormat, string timeFormat) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mInfo = info;

            //Header
            this.txtVendorName.Text = info.Vendor.vendor_name;
            this.txtVendorTel.Text = info.Vendor.op_tel;
            this.txtVendorFax.Text = info.Vendor.op_fax;
            textBox2.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyTel"]);
            textBox3.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyFax"]);

            //Customer info
            if (info.Customer != null) {
                txtGroomName.Text = info.Customer.g_first + " " + info.Customer.g_last;
                txtBrideName.Text = info.Customer.b_first + " " + info.Customer.b_last;
                txtWedDate.Text = info.Customer.wed_date.HasValue ? info.Customer.wed_date.Value.ToString(dateFormat) : "";
                txtWedTime.Text = info.Customer.wed_time.HasValue ? info.Customer.wed_time.Value.ToString(timeFormat) : "";
                txtPlace.Text = info.Customer.hotel_name;
            }
            if (info.Church != null) {
                txtChurch.Text = info.Church.church_name;
            }

            var sales = mInfo.Sales;
            //サブレポート & MEMO
            string memo = sales.note;

            switch (sales.Item.ItemType.info_type) {
                case "DLV":
                    memo += String.IsNullOrEmpty(memo) ? mInfo.Sales.DeliveryInfo.note : Environment.NewLine + Environment.NewLine + mInfo.Sales.DeliveryInfo.note;
                    var sub_dlv = new VendorOrderSheet_Datail_DLV(info, dateFormat, timeFormat);
                    this.subReport.Report = sub_dlv;
                    break;
                case "MKS":
                    memo += String.IsNullOrEmpty(memo) ? mInfo.Sales.MakeInfo.note : Environment.NewLine + Environment.NewLine + mInfo.Sales.MakeInfo.note;
                    var sub_mks = new VendorOrderSheet_Datail_MKS(info, dateFormat, timeFormat);
                    this.subReport.Report = sub_mks;
                    break;
                case "RCP":
                    memo += String.IsNullOrEmpty(memo) ? mInfo.Sales.ReceptionInfo.note : Environment.NewLine + Environment.NewLine + mInfo.Sales.ReceptionInfo.note;
                    var sub_rcp = new VendorOrderSheet_Datail_RCP(info, dateFormat, timeFormat);
                    this.subReport.Report = sub_rcp;
                    break;
                case "SHO":
                case "SHT":
                    memo += String.IsNullOrEmpty(memo) ? mInfo.Sales.ShootInfo.note : Environment.NewLine + Environment.NewLine + mInfo.Sales.ShootInfo.note;
                    var sub_sht = new VendorOrderSheet_Datail_SHT(info, dateFormat, timeFormat);
                    this.subReport.Report = sub_sht;
                    break;
                case "TRN":
                    memo += String.IsNullOrEmpty(memo) ? mInfo.Sales.TransInfo.note : Environment.NewLine + Environment.NewLine + mInfo.Sales.TransInfo.note;
                    var sub_trn = new VendorOrderSheet_Datail_TRN(info, dateFormat, timeFormat);
                    this.subReport.Report = sub_trn;
                    break;
                default:
                    var sub_def = new VendorOrderSheet_Datail_OTHER(info, dateFormat, timeFormat);
                    this.subReport.Report = sub_def;
                    break;
            }
            this.txtMemo.Text = String.IsNullOrEmpty(memo) ? mInfo.Arrangement.note : memo + Environment.NewLine + Environment.NewLine + mInfo.Arrangement.note;

            //レポートタイトル
            txtTitle.Text = sales.Item.ItemType.desc_eng + " REQUEST";

            this.DataSource = mInfo;
        }

        private void detail_Format(object sender, EventArgs e) {

        }


    }
}
