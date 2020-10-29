namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    partial class VendorOrderSheet_Datail_DLV {
        private GrapeCity.ActiveReports.SectionReportModel.PageHeader pageHeader;
        private GrapeCity.ActiveReports.SectionReportModel.Detail detail;
        private GrapeCity.ActiveReports.SectionReportModel.PageFooter pageFooter;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
            }
            base.Dispose(disposing);
        }

        #region ActiveReport Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(VendorOrderSheet_Datail_DLV));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.shape2 = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.txtItemName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox4 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox5 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox6 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line4 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line10 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.textBox7 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox8 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label12 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line9 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.textBox9 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox10 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox11 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.shape3 = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.txtDlvDate = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label3 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.line11 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.label7 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtDlvPlace = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line14 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line15 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtDlvTime = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDlvDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDlvPlace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDlvTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Height = 0F;
            this.pageHeader.Name = "pageHeader";
            this.pageHeader.Visible = false;
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.shape2,
            this.txtItemName,
            this.textBox4,
            this.textBox5,
            this.textBox6,
            this.line3,
            this.line4,
            this.line10,
            this.textBox7,
            this.textBox8,
            this.label12,
            this.line2,
            this.line9,
            this.textBox9,
            this.textBox10,
            this.textBox11,
            this.shape3,
            this.txtDlvDate,
            this.label3,
            this.line11,
            this.label7,
            this.txtDlvPlace,
            this.line14,
            this.line15,
            this.line1,
            this.label1,
            this.txtDlvTime});
            this.detail.Height = 2.871861F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // shape2
            // 
            this.shape2.Height = 1.51F;
            this.shape2.Left = 0.001999997F;
            this.shape2.Name = "shape2";
            this.shape2.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(2F);
            this.shape2.Style = GrapeCity.ActiveReports.SectionReportModel.ShapeType.RoundRect;
            this.shape2.Top = 1.269F;
            this.shape2.Width = 8.363501F;
            // 
            // txtItemName
            // 
            this.txtItemName.DataField = "Item.item_name";
            this.txtItemName.Height = 0.2619999F;
            this.txtItemName.Left = 0.05800001F;
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Style = "font-family: MS Gothic; vertical-align: middle";
            this.txtItemName.Text = null;
            this.txtItemName.Top = 1.269F;
            this.txtItemName.Width = 8.3075F;
            // 
            // textBox4
            // 
            this.textBox4.Height = 0.2F;
            this.textBox4.Left = 4.2315F;
            this.textBox4.Name = "textBox4";
            this.textBox4.Style = "vertical-align: middle";
            this.textBox4.Text = null;
            this.textBox4.Top = 2.227F;
            this.textBox4.Width = 4.134F;
            // 
            // textBox5
            // 
            this.textBox5.Height = 0.2F;
            this.textBox5.Left = 0.05800001F;
            this.textBox5.Name = "textBox5";
            this.textBox5.Style = "vertical-align: middle";
            this.textBox5.Text = null;
            this.textBox5.Top = 1.621F;
            this.textBox5.Width = 4.1735F;
            // 
            // textBox6
            // 
            this.textBox6.Height = 0.2F;
            this.textBox6.Left = 0.05800001F;
            this.textBox6.Name = "textBox6";
            this.textBox6.Style = "vertical-align: middle";
            this.textBox6.Text = null;
            this.textBox6.Top = 1.912F;
            this.textBox6.Width = 4.1725F;
            // 
            // line3
            // 
            this.line3.Height = 0F;
            this.line3.Left = 0.001999997F;
            this.line3.LineStyle = GrapeCity.ActiveReports.SectionReportModel.LineStyle.Dash;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 1.862F;
            this.line3.Width = 8.363499F;
            this.line3.X1 = 0.001999997F;
            this.line3.X2 = 8.365499F;
            this.line3.Y1 = 1.862F;
            this.line3.Y2 = 1.862F;
            // 
            // line4
            // 
            this.line4.Height = 0F;
            this.line4.Left = 0.001999997F;
            this.line4.LineStyle = GrapeCity.ActiveReports.SectionReportModel.LineStyle.Dash;
            this.line4.LineWeight = 1F;
            this.line4.Name = "line4";
            this.line4.Top = 1.586F;
            this.line4.Width = 8.363499F;
            this.line4.X1 = 0.001999997F;
            this.line4.X2 = 8.365499F;
            this.line4.Y1 = 1.586F;
            this.line4.Y2 = 1.586F;
            // 
            // line10
            // 
            this.line10.Height = 1.187F;
            this.line10.Left = 4.1575F;
            this.line10.LineStyle = GrapeCity.ActiveReports.SectionReportModel.LineStyle.Dash;
            this.line10.LineWeight = 1F;
            this.line10.Name = "line10";
            this.line10.Top = 1.592F;
            this.line10.Width = 0F;
            this.line10.X1 = 4.1575F;
            this.line10.X2 = 4.1575F;
            this.line10.Y1 = 2.779F;
            this.line10.Y2 = 1.592F;
            // 
            // textBox7
            // 
            this.textBox7.Height = 0.2F;
            this.textBox7.Left = 4.2315F;
            this.textBox7.Name = "textBox7";
            this.textBox7.Style = "vertical-align: middle";
            this.textBox7.Text = null;
            this.textBox7.Top = 1.621F;
            this.textBox7.Width = 4.134F;
            // 
            // textBox8
            // 
            this.textBox8.Height = 0.2F;
            this.textBox8.Left = 4.2315F;
            this.textBox8.Name = "textBox8";
            this.textBox8.Style = "vertical-align: middle";
            this.textBox8.Text = null;
            this.textBox8.Top = 1.912F;
            this.textBox8.Width = 4.134F;
            // 
            // label12
            // 
            this.label12.Height = 0.2F;
            this.label12.HyperLink = null;
            this.label12.Left = 0.0004997179F;
            this.label12.Name = "label12";
            this.label12.Style = "text-align: left";
            this.label12.Text = "<ORDER>";
            this.label12.Top = 0F;
            this.label12.Width = 1.198F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0.001999997F;
            this.line2.LineStyle = GrapeCity.ActiveReports.SectionReportModel.LineStyle.Dash;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 2.165F;
            this.line2.Width = 8.363499F;
            this.line2.X1 = 0.001999997F;
            this.line2.X2 = 8.365499F;
            this.line2.Y1 = 2.165F;
            this.line2.Y2 = 2.165F;
            // 
            // line9
            // 
            this.line9.Height = 0F;
            this.line9.Left = 0.001999997F;
            this.line9.LineStyle = GrapeCity.ActiveReports.SectionReportModel.LineStyle.Dash;
            this.line9.LineWeight = 1F;
            this.line9.Name = "line9";
            this.line9.Top = 2.488F;
            this.line9.Width = 8.363499F;
            this.line9.X1 = 0.001999997F;
            this.line9.X2 = 8.365499F;
            this.line9.Y1 = 2.488F;
            this.line9.Y2 = 2.488F;
            // 
            // textBox9
            // 
            this.textBox9.Height = 0.2F;
            this.textBox9.Left = 0.05700012F;
            this.textBox9.Name = "textBox9";
            this.textBox9.Style = "vertical-align: middle";
            this.textBox9.Text = null;
            this.textBox9.Top = 2.227F;
            this.textBox9.Width = 4.1735F;
            // 
            // textBox10
            // 
            this.textBox10.Height = 0.2F;
            this.textBox10.Left = 0.05700002F;
            this.textBox10.Name = "textBox10";
            this.textBox10.Style = "vertical-align: middle";
            this.textBox10.Text = null;
            this.textBox10.Top = 2.527F;
            this.textBox10.Width = 4.1735F;
            // 
            // textBox11
            // 
            this.textBox11.Height = 0.2F;
            this.textBox11.Left = 4.2305F;
            this.textBox11.Name = "textBox11";
            this.textBox11.Style = "vertical-align: middle";
            this.textBox11.Text = null;
            this.textBox11.Top = 2.527F;
            this.textBox11.Width = 4.134999F;
            // 
            // shape3
            // 
            this.shape3.Height = 0.6610001F;
            this.shape3.Left = 0.0004997179F;
            this.shape3.Name = "shape3";
            this.shape3.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(2F);
            this.shape3.Style = GrapeCity.ActiveReports.SectionReportModel.ShapeType.RoundRect;
            this.shape3.Top = 0.2279999F;
            this.shape3.Width = 8.3645F;
            // 
            // txtDlvDate
            // 
            this.txtDlvDate.Height = 0.317F;
            this.txtDlvDate.Left = 1.426499F;
            this.txtDlvDate.Name = "txtDlvDate";
            this.txtDlvDate.Style = "vertical-align: middle";
            this.txtDlvDate.Text = null;
            this.txtDlvDate.Top = 0.228F;
            this.txtDlvDate.Width = 2.7305F;
            // 
            // label3
            // 
            this.label3.Height = 0.317F;
            this.label3.HyperLink = null;
            this.label3.Left = 0.05649968F;
            this.label3.Name = "label3";
            this.label3.Style = "text-align: left; vertical-align: middle";
            this.label3.Text = "DELIVERY DATE";
            this.label3.Top = 0.228F;
            this.label3.Width = 1.198F;
            // 
            // line11
            // 
            this.line11.Height = 0.6610003F;
            this.line11.Left = 1.326499F;
            this.line11.LineWeight = 1F;
            this.line11.Name = "line11";
            this.line11.Top = 0.2279999F;
            this.line11.Width = 0.001001F;
            this.line11.X1 = 1.326499F;
            this.line11.X2 = 1.3275F;
            this.line11.Y1 = 0.2279999F;
            this.line11.Y2 = 0.8890001F;
            // 
            // label7
            // 
            this.label7.Height = 0.344F;
            this.label7.HyperLink = null;
            this.label7.Left = 0.05649986F;
            this.label7.Name = "label7";
            this.label7.Style = "text-align: left; vertical-align: middle";
            this.label7.Text = "DELIVERY PLACE";
            this.label7.Top = 0.545F;
            this.label7.Width = 1.198F;
            // 
            // txtDlvPlace
            // 
            this.txtDlvPlace.Height = 0.344F;
            this.txtDlvPlace.Left = 1.4265F;
            this.txtDlvPlace.Name = "txtDlvPlace";
            this.txtDlvPlace.Style = "vertical-align: middle";
            this.txtDlvPlace.Text = null;
            this.txtDlvPlace.Top = 0.545F;
            this.txtDlvPlace.Width = 6.886001F;
            // 
            // line14
            // 
            this.line14.Height = 0.3170001F;
            this.line14.Left = 5.5125F;
            this.line14.LineWeight = 1F;
            this.line14.Name = "line14";
            this.line14.Top = 0.2279999F;
            this.line14.Width = 0F;
            this.line14.X1 = 5.5125F;
            this.line14.X2 = 5.5125F;
            this.line14.Y1 = 0.545F;
            this.line14.Y2 = 0.2279999F;
            // 
            // line15
            // 
            this.line15.Height = 0.3170001F;
            this.line15.Left = 4.1565F;
            this.line15.LineWeight = 1F;
            this.line15.Name = "line15";
            this.line15.Top = 0.2279999F;
            this.line15.Width = 0.0002512932F;
            this.line15.X1 = 4.1565F;
            this.line15.X2 = 4.156751F;
            this.line15.Y1 = 0.545F;
            this.line15.Y2 = 0.2279999F;
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0.002499722F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.5450001F;
            this.line1.Width = 8.363499F;
            this.line1.X1 = 0.002499722F;
            this.line1.X2 = 8.365998F;
            this.line1.Y1 = 0.5450001F;
            this.line1.Y2 = 0.5450001F;
            // 
            // label1
            // 
            this.label1.Height = 0.317F;
            this.label1.HyperLink = null;
            this.label1.Left = 4.2305F;
            this.label1.Name = "label1";
            this.label1.Style = "text-align: left; vertical-align: middle";
            this.label1.Text = "DELIVERY TIME";
            this.label1.Top = 0.228F;
            this.label1.Width = 1.198F;
            // 
            // txtDlvTime
            // 
            this.txtDlvTime.Height = 0.317F;
            this.txtDlvTime.Left = 5.5825F;
            this.txtDlvTime.Name = "txtDlvTime";
            this.txtDlvTime.Style = "vertical-align: middle";
            this.txtDlvTime.Text = null;
            this.txtDlvTime.Top = 0.228F;
            this.txtDlvTime.Width = 2.7305F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Visible = false;
            // 
            // reportHeader1
            // 
            this.reportHeader1.Height = 0F;
            this.reportHeader1.Name = "reportHeader1";
            // 
            // reportFooter1
            // 
            this.reportFooter1.Height = 0F;
            this.reportFooter1.Name = "reportFooter1";
            // 
            // VendorOrderSheet_Datail_DLV
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0.4F;
            this.PageSettings.Margins.Left = 0.4F;
            this.PageSettings.Margins.Right = 0.4F;
            this.PageSettings.Margins.Top = 0.4F;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.5F;
            this.Sections.Add(this.reportHeader1);
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.Sections.Add(this.reportFooter1);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; font-size: 10pt; " +
            "color: Black; font-family: \"MS PGothic\"; ddo-char-set: 186", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; ddo-char-set: 186", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: italic; font-family: \"MS PGothic\"" +
            "; ddo-char-set: 186", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 186", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDlvDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDlvPlace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDlvTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shape2;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtItemName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox4;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox6;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.Line line4;
        private GrapeCity.ActiveReports.SectionReportModel.Line line10;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox7;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox8;
        private GrapeCity.ActiveReports.SectionReportModel.Label label12;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.Line line9;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox9;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox10;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox11;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shape3;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtDlvDate;
        private GrapeCity.ActiveReports.SectionReportModel.Label label3;
        private GrapeCity.ActiveReports.SectionReportModel.Line line11;
        private GrapeCity.ActiveReports.SectionReportModel.Label label7;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtDlvPlace;
        private GrapeCity.ActiveReports.SectionReportModel.Line line14;
        private GrapeCity.ActiveReports.SectionReportModel.Line line15;
        private GrapeCity.ActiveReports.SectionReportModel.Line line1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtDlvTime;
    }
}
