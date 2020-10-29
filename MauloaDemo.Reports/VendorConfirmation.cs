using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF;
using System.Linq;
using System.Configuration;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    public partial class VendorConfirmation : GrapeCity.ActiveReports.SectionReport {

        private List<VendorConfirmationReport> mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public VendorConfirmation(List<VendorConfirmationReport> list, string dateFormat, string timeFormat) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = list;
            
            mInfo = list;
            mDateFormat = dateFormat;
            mTimeFormat = timeFormat;
        }

        private void pageHeader_Format(object sender, EventArgs e){
            if (mInfo == null || mInfo.Count == 0) return;
            var info = mInfo.First();

            //Header
            this.txtVendorName.Text = info.vendor_name;
            this.txtVendorTel.Text = info.op_tel;
            this.txtVendorFax.Text = info.op_fax;
        }

        private void groupHeader1_Format(object sender, EventArgs e) {
            if (mInfo == null || mInfo.Count == 0) return;

            textBox2.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyTel"]);
            textBox3.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyFax"]);
            var vendor_cd = TypeHelper.GetStrTrim(this.Fields["vendor_cd"].Value);
            var info_type = TypeHelper.GetStrTrim(this.Fields["info_type"].Value);
            if (info_type == "") info_type = null;

            var sub_list = mInfo.Where(m => m.vendor_cd == vendor_cd 
                                         && m.info_type == info_type)
                                .ToList();

            //info_typeに応じてサブレポートを切り替える
            switch (info_type){
                case "DLV":
                    var sub_dlv = new VendorConfirmation_Datail_DLV(sub_list, mDateFormat, mTimeFormat);
                    this.subReport.Report = sub_dlv;
                    break;
                case "MKS":
                    var sub_mks = new VendorConfirmation_Datail_MKS(sub_list, mDateFormat, mTimeFormat);
                    this.subReport.Report = sub_mks;
                    break;
                case "RCP":
                    var sub_rcp = new VendorConfirmation_Datail_RCP(sub_list, mDateFormat, mTimeFormat);
                    this.subReport.Report = sub_rcp;
                    break;
                case "SHO":
                case "SHT":
                    var sub_sht = new VendorConfirmation_Datail_SHT(sub_list, mDateFormat, mTimeFormat);
                    this.subReport.Report = sub_sht;
                    break;
                case "TRN":
                    var sub_trn = new VendorConfirmation_Datail_TRN(sub_list, mDateFormat, mTimeFormat);
                    this.subReport.Report = sub_trn;
                    break;
                default:
                    var sub_def = new VendorConfirmation_Datail_OTHER(sub_list, mDateFormat, mTimeFormat);
                    this.subReport.Report = sub_def;
                    break;
            }
        }

    }
}
