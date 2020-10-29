using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF;
using MauloaDemo.Repository;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for FinalInfoSheet_WedInfo.
    /// </summary>
    public partial class FinalInfoSheet_WedInfo : GrapeCity.ActiveReports.SectionReport {

        private bool mEnglish;

        public FinalInfoSheet_WedInfo(WedInfo info, List<SalesListItem> list, bool english) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = list;
            mEnglish = english;
            txtPlanName.Text = english ? info.item_name : info.item_name_jpn;
            txtPrice.Text = info.amount.ToString("N2");
        }

        private void detail_Format(object sender, EventArgs e) {
            var item_name = mEnglish ? TypeHelper.GetStrTrim(this.Fields["item_name"].Value) : TypeHelper.GetStrTrim(this.Fields["item_name_jpn"].Value);
            txtItemName.Text = item_name;

            var op_seq = TypeHelper.GetInt(this.Fields["op_seq"].Value);
            var arrangementRepository = new ArrangementRepository();
            var vendor_list = arrangementRepository.GetVendorByOpSeq(op_seq);
            var vendor_name_value = "";
            foreach (var vendor in vendor_list) {
                var vendor_name = mEnglish ? vendor.vendor_name : vendor.vendor_name;
                vendor_name_value += string.IsNullOrEmpty(vendor_name_value) ? vendor_name : ", " + vendor_name;
            }
            txtVendorName.Text = vendor_name_value;

        }
    }
}
