using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF;

namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for CostumeOrderSheet_Orders.
    /// </summary>
    public partial class CostumeOrderSheet_Orders : GrapeCity.ActiveReports.SectionReport {

        public CostumeOrderSheet_Orders(List<SalesListItem> wedInfo) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            this.DataSource = wedInfo;
        }

    }
}
