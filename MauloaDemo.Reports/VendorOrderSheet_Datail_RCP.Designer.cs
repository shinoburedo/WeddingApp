namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    partial class VendorOrderSheet_Datail_RCP {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(VendorOrderSheet_Datail_RCP));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.label12 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.shape2 = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.txtRestCd = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line10 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtPartyDate = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPartyTime = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line9 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtItemName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label4 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label5 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            ((System.ComponentModel.ISupportInitialize)(this.label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestCd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartyDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartyTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).BeginInit();
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
            this.label12,
            this.shape2,
            this.txtRestCd,
            this.line3,
            this.line10,
            this.txtPartyDate,
            this.txtPartyTime,
            this.line2,
            this.line9,
            this.txtItemName,
            this.label1,
            this.label4,
            this.label5});
            this.detail.Height = 1.434361F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // label12
            // 
            this.label12.Height = 0.2F;
            this.label12.HyperLink = null;
            this.label12.Left = 0.0560001F;
            this.label12.Name = "label12";
            this.label12.Style = "text-align: left";
            this.label12.Text = "<ORDER>";
            this.label12.Top = 0F;
            this.label12.Width = 1.198F;
            // 
            // shape2
            // 
            this.shape2.Height = 1.187F;
            this.shape2.Left = 0F;
            this.shape2.Name = "shape2";
            this.shape2.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(2F);
            this.shape2.Style = GrapeCity.ActiveReports.SectionReportModel.ShapeType.RoundRect;
            this.shape2.Top = 0.2F;
            this.shape2.Width = 8.363501F;
            // 
            // txtRestCd
            // 
            this.txtRestCd.Height = 0.323F;
            this.txtRestCd.Left = 2.406F;
            this.txtRestCd.Name = "txtRestCd";
            this.txtRestCd.Style = "font-family: MS Gothic; vertical-align: middle";
            this.txtRestCd.Text = null;
            this.txtRestCd.Top = 1.043F;
            this.txtRestCd.Width = 5.956998F;
            // 
            // line3
            // 
            this.line3.Height = 0F;
            this.line3.Left = 0F;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0.47F;
            this.line3.Width = 8.363499F;
            this.line3.X1 = 0F;
            this.line3.X2 = 8.363499F;
            this.line3.Y1 = 0.47F;
            this.line3.Y2 = 0.47F;
            // 
            // line10
            // 
            this.line10.Height = 0.9169999F;
            this.line10.Left = 2.323F;
            this.line10.LineWeight = 1F;
            this.line10.Name = "line10";
            this.line10.Top = 0.47F;
            this.line10.Width = 0.0004999638F;
            this.line10.X1 = 2.3235F;
            this.line10.X2 = 2.323F;
            this.line10.Y1 = 1.387F;
            this.line10.Y2 = 0.47F;
            // 
            // txtPartyDate
            // 
            this.txtPartyDate.Height = 0.27F;
            this.txtPartyDate.Left = 2.406F;
            this.txtPartyDate.Name = "txtPartyDate";
            this.txtPartyDate.Style = "vertical-align: middle";
            this.txtPartyDate.Text = null;
            this.txtPartyDate.Top = 0.47F;
            this.txtPartyDate.Width = 5.956998F;
            // 
            // txtPartyTime
            // 
            this.txtPartyTime.Height = 0.303F;
            this.txtPartyTime.Left = 2.406F;
            this.txtPartyTime.Name = "txtPartyTime";
            this.txtPartyTime.Style = "font-family: MS Gothic; vertical-align: middle";
            this.txtPartyTime.Text = null;
            this.txtPartyTime.Top = 0.74F;
            this.txtPartyTime.Width = 5.956998F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.773F;
            this.line2.Width = 8.363499F;
            this.line2.X1 = 0F;
            this.line2.X2 = 8.363499F;
            this.line2.Y1 = 0.773F;
            this.line2.Y2 = 0.773F;
            // 
            // line9
            // 
            this.line9.Height = 0F;
            this.line9.Left = 0F;
            this.line9.LineWeight = 1F;
            this.line9.Name = "line9";
            this.line9.Top = 1.096F;
            this.line9.Width = 8.363499F;
            this.line9.X1 = 0F;
            this.line9.X2 = 8.363499F;
            this.line9.Y1 = 1.096F;
            this.line9.Y2 = 1.096F;
            // 
            // txtItemName
            // 
            this.txtItemName.Height = 0.2909999F;
            this.txtItemName.Left = 0.0559999F;
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Style = "vertical-align: middle";
            this.txtItemName.Text = null;
            this.txtItemName.Top = 0.179F;
            this.txtItemName.Width = 8.162998F;
            // 
            // label1
            // 
            this.label1.Height = 0.27F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.05599973F;
            this.label1.Name = "label1";
            this.label1.Style = "text-align: left; vertical-align: middle";
            this.label1.Text = "PARTY DATE";
            this.label1.Top = 0.47F;
            this.label1.Width = 2.206F;
            // 
            // label4
            // 
            this.label4.Height = 0.303F;
            this.label4.HyperLink = null;
            this.label4.Left = 0.05599973F;
            this.label4.Name = "label4";
            this.label4.Style = "text-align: left; vertical-align: middle";
            this.label4.Text = "PARTY TIME";
            this.label4.Top = 0.74F;
            this.label4.Width = 2.206F;
            // 
            // label5
            // 
            this.label5.Height = 0.323F;
            this.label5.HyperLink = null;
            this.label5.Left = 0.05599973F;
            this.label5.Name = "label5";
            this.label5.Style = "text-align: left; vertical-align: middle";
            this.label5.Text = "REST";
            this.label5.Top = 1.043F;
            this.label5.Width = 2.206F;
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
            // VendorOrderSheet_Datail_RCP
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0.4F;
            this.PageSettings.Margins.Left = 0.4F;
            this.PageSettings.Margins.Right = 0.4F;
            this.PageSettings.Margins.Top = 0.4F;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.501666F;
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
            ((System.ComponentModel.ISupportInitialize)(this.label12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRestCd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartyDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPartyTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label12;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shape2;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtRestCd;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.Line line10;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPartyDate;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPartyTime;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.Line line9;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label4;
        private GrapeCity.ActiveReports.SectionReportModel.Label label5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtItemName;
    }
}
