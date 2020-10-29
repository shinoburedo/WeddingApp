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
    public partial class WeddingInfoSheet_WedInfo : GrapeCity.ActiveReports.SectionReport {

        private bool mEnglish;

        public WeddingInfoSheet_WedInfo(WedInfo info, List<SalesListItem> list, bool english) {
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

        }
    }
}
