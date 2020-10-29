using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for FinalInfoSheet_AddressInfo.
    /// </summary>
    public partial class FinalInfoSheet_AddressInfo : GrapeCity.ActiveReports.SectionReport {

        public FinalInfoSheet_AddressInfo(List<AddressInfo> list) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = list;

        }
    }
}
