using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using CBAF;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for CostumeOrderSheet_CosInfos.
    /// </summary>
    public partial class CostumeOrderSheet_CosInfos : GrapeCity.ActiveReports.SectionReport {

        public CostumeOrderSheet_CosInfos(List<CosInfo> cosInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = cosInfo;
        }

        private void detail_Format(object sender, EventArgs e) {
            var type = TypeHelper.GetStr(this.Fields["pax_type"].Value);
            var type_name = "";
            switch (type) {
                case "G":
                    type_name = "Groom";
                    break;
                case "B":
                    type_name = "Bride";
                    break;
                case "A":
                    type_name = "Attendance";
                    break;
            }
            txtPaxType.Text = type_name;
        }
    }
}
