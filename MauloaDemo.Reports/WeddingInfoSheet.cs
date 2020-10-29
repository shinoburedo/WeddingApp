using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for FinalInfoSheet.
    /// </summary>
    public partial class WeddingInfoSheet : GrapeCity.ActiveReports.SectionReport {

        private bool mEnglish = false;
        private int mCount = 0;

        public WeddingInfoSheet(FinalInfoSheetInfo data, string dateFormat, string timeFormat, bool english, LoginUser user) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            mEnglish = english;
            mCount = data.list.Count;
            this.DataSource = data.list;
            txtPrintDate.Text = DateTime.Now.ToString(dateFormat + " " + timeFormat);
            txtPrintBy.Text = english ? user.UserName : user.UserNameJpn;
            txtGName.Text = data.Customer.g_last + " " + data.Customer.g_first;
            txtGNameJpn.Text = data.Customer.g_last_kanji + " " + data.Customer.g_first_kanji;
            txtBName.Text = data.Customer.b_last + " " + data.Customer.b_first;
            txtBNameJpn.Text = data.Customer.b_last_kanji + " " + data.Customer.b_first_kanji;
            txtAgentName.Text = data.agent_name;
            txtWeddingDate.Text = data.Customer.wed_date.HasValue ? data.Customer.wed_date.Value.ToString(dateFormat) : "";
            txtWeddingTime.Text = data.Customer.wed_time_s;
            txtChurch.Text = data.church_name;
            txtHotelName.Text = data.Customer.hotel_name;
            txtRoom.Text = data.Customer.room_number;
            txtCheckIn.Text = data.Customer.checkin_date.HasValue ? data.Customer.checkin_date.Value.ToString(dateFormat) : "";
            txtCheckOut.Text = data.Customer.checkout_date.HasValue ? data.Customer.checkout_date.Value.ToString(dateFormat) : "";
            txtMemo.Text = data.Customer.note;
            txtAttCount.Text = data.Customer.attend_count.ToString();
            txtAttName.Text = data.Customer.attend_name;
            txtAttMemo.Text = data.Customer.attend_memo;
            if (data.PhotoPlanItems != null && data.PhotoPlanItems.Count > 0) {
                var sub = new WeddingInfoSheet_WedInfo(data.PhotoPlan, data.PhotoPlanItems, english);
                this.subReport3.Report = sub;
                this.subReport3.Visible = true;
            } else {
                this.subReport3.Visible = false;
            }
            if (data.WedPlanItems != null && data.WedPlanItems.Count > 0) {
                var sub = new WeddingInfoSheet_WedInfo(data.WedPlan, data.WedPlanItems, english);
                this.subReport4.Report = sub;
                this.subReport4.Visible = true;
            } else {
                this.subReport4.Visible = false;
            }
            if (data.AddressInfos != null && data.AddressInfos.Count > 0) {
                var sub = new FinalInfoSheet_AddressInfo(data.AddressInfos);
                this.subReport1.Report = sub;
            }
            if (data.CosInfos != null && data.CosInfos.Count > 0) {
                var sub = new FinalInfoSheet_CostumeInfo(data.CosInfos);
                this.subReport2.Report = sub;
            }
            //Schedule info
            if (data.Schedule != null && data.Schedule.Phrases.Count > 0) {
                var sub = new CostumeOrderSheet_Schedule(data.Schedule, mEnglish);
                this.subReport5.Report = sub;
            }

        }

        private void reportHeader1_Format(object sender, EventArgs e) {
            if (!string.IsNullOrEmpty(txtBNameJpn.Text.Trim())) {
                txtBNameJpn.Text = txtBNameJpn.Text + " 様";
            }
            if (!string.IsNullOrEmpty(txtGNameJpn.Text.Trim())) {
                txtGNameJpn.Text = txtGNameJpn.Text + " 様";
            }
        }

        //private void groupHeader1_Format(object sender, EventArgs e) {
        //    if (TypeHelper.GetBool(this.Fields["is_option"].Value)) {
        //        groupHeader1.Visible = true;
        //    } else {
        //        groupHeader1.Visible = false;
        //    }
        //}

        private void detail_Format(object sender, EventArgs e) {
            if (mCount == 0) {
                return;
            }
            var item_name = mEnglish ? TypeHelper.GetStrTrim(this.Fields["item_name"].Value) : TypeHelper.GetStrTrim(this.Fields["item_name_jpn"].Value);
            txtItemName.Text = item_name;

        }

        private void groupHeader1_Format(object sender, EventArgs e) {
            if (mCount == 0) {
                groupHeader1.Visible = false;
                groupFooter1.Visible = false;
            }
        }

    }
}
