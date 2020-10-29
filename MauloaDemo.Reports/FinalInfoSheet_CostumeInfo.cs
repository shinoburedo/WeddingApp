using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for FinalInfoSheet_CostumeInfo.
    /// </summary>
    public partial class FinalInfoSheet_CostumeInfo : GrapeCity.ActiveReports.SectionReport {

        public FinalInfoSheet_CostumeInfo(List<CosInfo> list) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = list;
        }
    }
}
