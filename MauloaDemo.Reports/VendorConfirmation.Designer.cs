namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    partial class VendorConfirmation
    {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(VendorConfirmation));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            this.groupHeader1 = new GrapeCity.ActiveReports.SectionReportModel.GroupHeader();
            this.subReport = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.label3 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtVendorName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label4 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtVendorTel = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label5 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtVendorFax = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label7 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.textBox2 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label8 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.textBox3 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.groupFooter1 = new GrapeCity.ActiveReports.SectionReportModel.GroupFooter();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTtl = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label6 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorTel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorFax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTtl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Height = 0F;
            this.pageHeader.Name = "pageHeader";
            this.pageHeader.Visible = false;
            this.pageHeader.Format += new System.EventHandler(this.pageHeader_Format);
            // 
            // detail
            // 
            this.detail.Height = 0F;
            this.detail.Name = "detail";
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
            // groupHeader1
            // 
            this.groupHeader1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.subReport,
            this.label3,
            this.txtVendorName,
            this.label4,
            this.txtVendorTel,
            this.label5,
            this.txtVendorFax,
            this.label7,
            this.textBox2,
            this.label8,
            this.textBox3,
            this.txtTitle,
            this.label6});
            this.groupHeader1.DataField = "report_group_key";
            this.groupHeader1.Height = 2.115167F;
            this.groupHeader1.KeepTogether = true;
            this.groupHeader1.Name = "groupHeader1";
            this.groupHeader1.Format += new System.EventHandler(this.groupHeader1_Format);
            // 
            // subReport
            // 
            this.subReport.CloseBorder = false;
            this.subReport.Height = 0.531F;
            this.subReport.Left = 0.07837486F;
            this.subReport.Name = "subReport";
            this.subReport.Report = null;
            this.subReport.ReportName = "subReport";
            this.subReport.Top = 1.511F;
            this.subReport.Width = 10.916F;
            // 
            // label3
            // 
            this.label3.Height = 0.2F;
            this.label3.HyperLink = null;
            this.label3.Left = 0.3293748F;
            this.label3.Name = "label3";
            this.label3.Style = "text-align: right";
            this.label3.Text = "TO:";
            this.label3.Top = 0F;
            this.label3.Width = 0.3329999F;
            // 
            // txtVendorName
            // 
            this.txtVendorName.Height = 0.2F;
            this.txtVendorName.Left = 0.7283748F;
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.Style = "font-size: 12pt; ddo-char-set: 128";
            this.txtVendorName.Text = null;
            this.txtVendorName.Top = 0F;
            this.txtVendorName.Width = 3.74F;
            // 
            // label4
            // 
            this.label4.Height = 0.2F;
            this.label4.HyperLink = null;
            this.label4.Left = 0.3288749F;
            this.label4.Name = "label4";
            this.label4.Style = "text-align: right; text-justify: auto";
            this.label4.Text = "TEL:";
            this.label4.Top = 0.3129999F;
            this.label4.Width = 0.3329999F;
            // 
            // txtVendorTel
            // 
            this.txtVendorTel.Height = 0.2F;
            this.txtVendorTel.Left = 0.7278748F;
            this.txtVendorTel.Name = "txtVendorTel";
            this.txtVendorTel.Style = "font-size: 9.75pt; ddo-char-set: 128";
            this.txtVendorTel.Text = null;
            this.txtVendorTel.Top = 0.3129999F;
            this.txtVendorTel.Width = 1.542F;
            // 
            // label5
            // 
            this.label5.Height = 0.2F;
            this.label5.HyperLink = null;
            this.label5.Left = 0.3288749F;
            this.label5.Name = "label5";
            this.label5.Style = "text-align: right";
            this.label5.Text = "FAX:";
            this.label5.Top = 0.5680001F;
            this.label5.Width = 0.3329999F;
            // 
            // txtVendorFax
            // 
            this.txtVendorFax.Height = 0.2F;
            this.txtVendorFax.Left = 0.7278748F;
            this.txtVendorFax.Name = "txtVendorFax";
            this.txtVendorFax.Style = "font-size: 9.75pt; ddo-char-set: 128";
            this.txtVendorFax.Text = null;
            this.txtVendorFax.Top = 0.5680001F;
            this.txtVendorFax.Width = 1.542F;
            // 
            // label7
            // 
            this.label7.Height = 0.2F;
            this.label7.HyperLink = null;
            this.label7.Left = 9.234875F;
            this.label7.Name = "label7";
            this.label7.Style = "text-align: right; text-justify: auto";
            this.label7.Text = "TEL:";
            this.label7.Top = 0.6669999F;
            this.label7.Width = 0.3329999F;
            // 
            // textBox2
            // 
            this.textBox2.Height = 0.2F;
            this.textBox2.Left = 9.633875F;
            this.textBox2.Name = "textBox2";
            this.textBox2.Style = "font-size: 9.75pt; ddo-char-set: 128";
            this.textBox2.Text = "926-8600";
            this.textBox2.Top = 0.6669999F;
            this.textBox2.Width = 1.058F;
            // 
            // label8
            // 
            this.label8.Height = 0.2F;
            this.label8.HyperLink = null;
            this.label8.Left = 9.234875F;
            this.label8.Name = "label8";
            this.label8.Style = "text-align: right";
            this.label8.Text = "FAX:";
            this.label8.Top = 0.9220001F;
            this.label8.Width = 0.3329999F;
            // 
            // textBox3
            // 
            this.textBox3.Height = 0.2F;
            this.textBox3.Left = 9.633875F;
            this.textBox3.Name = "textBox3";
            this.textBox3.Style = "font-size: 9.75pt; ddo-char-set: 128";
            this.textBox3.Text = "926-8603";
            this.textBox3.Top = 0.9220001F;
            this.textBox3.Width = 1.058F;
            // 
            // txtTitle
            // 
            this.txtTitle.Height = 0.377F;
            this.txtTitle.Left = 2.417875F;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Style = "font-family: Times New Roman; font-size: 20.25pt; font-style: italic; font-weight" +
    ": bold; text-align: center; text-decoration: underline; ddo-char-set: 0";
            this.txtTitle.Text = "Confirmation List";
            this.txtTitle.Top = 1.009F;
            this.txtTitle.Width = 5.995999F;
            // 
            // groupFooter1
            // 
            this.groupFooter1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.label1,
            this.txtTtl});
            this.groupFooter1.Height = 0.3229167F;
            this.groupFooter1.Name = "groupFooter1";
            this.groupFooter1.NewPage = GrapeCity.ActiveReports.SectionReportModel.NewPage.After;
            // 
            // label1
            // 
            this.label1.Height = 0.2F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.1040001F;
            this.label1.Name = "label1";
            this.label1.Style = "text-align: right";
            this.label1.Text = "TTL:";
            this.label1.Top = 0.061F;
            this.label1.Width = 0.3329999F;
            // 
            // txtTtl
            // 
            this.txtTtl.CountNullValues = true;
            this.txtTtl.DataField = "report_group_key";
            this.txtTtl.Height = 0.2F;
            this.txtTtl.Left = 0.5029998F;
            this.txtTtl.Name = "txtTtl";
            this.txtTtl.Style = "font-size: 9.75pt; ddo-char-set: 128";
            this.txtTtl.SummaryFunc = GrapeCity.ActiveReports.SectionReportModel.SummaryFunc.Count;
            this.txtTtl.SummaryGroup = "groupHeader1";
            this.txtTtl.SummaryRunning = GrapeCity.ActiveReports.SectionReportModel.SummaryRunning.Group;
            this.txtTtl.SummaryType = GrapeCity.ActiveReports.SectionReportModel.SummaryType.SubTotal;
            this.txtTtl.Text = null;
            this.txtTtl.Top = 0.061F;
            this.txtTtl.Width = 1.052F;
            // 
            // label6
            // 
            this.label6.Height = 0.4670002F;
            this.label6.HyperLink = null;
            this.label6.Left = 8.266001F;
            this.label6.Name = "label6";
            this.label6.Style = "font-family: Times New Roman; font-size: 24pt; font-style: italic; font-weight: b" +
    "old; text-align: center; ddo-char-set: 0";
            this.label6.Text = "Mauloa Demo";
            this.label6.Top = 0.101F;
            this.label6.Width = 2.426F;
            // 
            // VendorConfirmation
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0.4F;
            this.PageSettings.Margins.Left = 0.4F;
            this.PageSettings.Margins.Right = 0.4F;
            this.PageSettings.Margins.Top = 0.4F;
            this.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Landscape;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 11.07275F;
            this.Sections.Add(this.reportHeader1);
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.groupHeader1);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.groupFooter1);
            this.Sections.Add(this.pageFooter);
            this.Sections.Add(this.reportFooter1);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; font-size: 10pt; " +
            "color: Black; font-family: \"MS PGothic\"; ddo-char-set: 186", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; ddo-char-set: 186", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: italic; font-family: \"MS PGothic\"" +
            "; ddo-char-set: 186", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 186", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorTel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorFax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTtl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.GroupHeader groupHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.GroupFooter groupFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTtl;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subReport;
        private GrapeCity.ActiveReports.SectionReportModel.Label label3;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtVendorName;
        private GrapeCity.ActiveReports.SectionReportModel.Label label4;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtVendorTel;
        private GrapeCity.ActiveReports.SectionReportModel.Label label5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtVendorFax;
        private GrapeCity.ActiveReports.SectionReportModel.Label label7;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label8;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox3;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTitle;
        private GrapeCity.ActiveReports.SectionReportModel.Label label6;
    }
}
