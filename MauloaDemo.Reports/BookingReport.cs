using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using CBAF;
using System.Configuration;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for BookingReport.
    /// </summary>
    public partial class BookingReport : GrapeCity.ActiveReports.SectionReport {

        private List<MauloaDemo.Models.Combined.BookingReport.Sales> mInfo;
        private string mDateFormat;
        private string mTimeFormat;

        public BookingReport(List<MauloaDemo.Models.Combined.BookingReport.Sales> list, MauloaDemo.Models.Combined.BookingReport.SearchParam param, 
            string dateFormat, string timeFormat) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = list;
            
            mInfo = list;
            mDateFormat = dateFormat;
            mTimeFormat = timeFormat;

            txtAgent.Text = param.agent_cd;
            txtChurch.Text = param.church_cd;
            txtItemCd.Text = param.item_cd;
            txtItemType.Text = param.item_type;
            txtVendor.Text = param.vendor_cd;
            txtWedDateFrom.Text = param.date_from.HasValue ? param.date_from.Value.ToString(dateFormat) : "";
            txtWedDateTo.Text = param.date_to.HasValue ? param.date_to.Value.ToString(dateFormat) : "";
            chkArrangeCxl.Checked = param.include_sales_cxl;
            chkCustomerCxl.Checked = param.include_cust_cxl;
            chkFinal.Checked = param.not_finalized_only;

            textBox2.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyTel"]);
            textBox3.Text = TypeHelper.GetStr(ConfigurationManager.AppSettings["CompanyFax"]);

            txtWedDate.OutputFormat = dateFormat;
            txtWedTime.OutputFormat = timeFormat;
        }

        private void pageHeader_Format(object sender, EventArgs e){
        }

        private void detail_Format(object sender, EventArgs e) {
            if (mInfo.Count > 0) {
                var g_last = this.Fields["g_last"].Value;
                var g_first = this.Fields["g_first"].Value;
                var b_last = this.Fields["b_last"].Value;
                var b_first = this.Fields["b_first"].Value;

                txtGroomName.Text = string.Format("{0}, {1}", g_last, g_first);
                txtBrideName.Text = string.Format("{0}, {1}", b_last, b_first);

            }

        }

    }
}
