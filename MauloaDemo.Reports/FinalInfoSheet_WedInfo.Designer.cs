namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for FinalInfoSheet_WedInfo.
    /// </summary>
    partial class FinalInfoSheet_WedInfo {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FinalInfoSheet_WedInfo));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtPhraseTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox1 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtItemName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox3 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtVendorName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.lblDate = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label9 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.label5 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.crossSectionLine3 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine4 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine5 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine6 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine7 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtPlanName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label3 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtPrice = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label23 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label4 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.crossSectionLine1 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlanName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // pageHeader
            // 
            this.pageHeader.Height = 0F;
            this.pageHeader.Name = "pageHeader";
            // 
            // detail
            // 
            this.detail.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.txtPhraseTitle,
            this.textBox1,
            this.txtItemName,
            this.textBox3,
            this.line3,
            this.txtVendorName});
            this.detail.Height = 0.2F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // txtPhraseTitle
            // 
            this.txtPhraseTitle.DataField = "item_type";
            this.txtPhraseTitle.Height = 0.2F;
            this.txtPhraseTitle.Left = 0.062F;
            this.txtPhraseTitle.Name = "txtPhraseTitle";
            this.txtPhraseTitle.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; vertical-align: middl" +
    "e; ddo-char-set: 128";
            this.txtPhraseTitle.Text = null;
            this.txtPhraseTitle.Top = 0F;
            this.txtPhraseTitle.Width = 0.791F;
            // 
            // textBox1
            // 
            this.textBox1.DataField = "item_cd";
            this.textBox1.Height = 0.2000001F;
            this.textBox1.Left = 0.744376F;
            this.textBox1.Name = "textBox1";
            this.textBox1.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; vertical-align: middl" +
    "e; ddo-char-set: 128";
            this.textBox1.Text = null;
            this.textBox1.Top = 0F;
            this.textBox1.Width = 0.947F;
            // 
            // txtItemName
            // 
            this.txtItemName.Height = 0.2F;
            this.txtItemName.Left = 1.747F;
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; vertical-align: middl" +
    "e; ddo-char-set: 128";
            this.txtItemName.Text = null;
            this.txtItemName.Top = 0F;
            this.txtItemName.Width = 4.668F;
            // 
            // textBox3
            // 
            this.textBox3.DataField = "quantity";
            this.textBox3.Height = 0.2F;
            this.textBox3.Left = 7.898F;
            this.textBox3.Name = "textBox3";
            this.textBox3.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; text-align: right; ve" +
    "rtical-align: middle; ddo-char-set: 128";
            this.textBox3.Text = null;
            this.textBox3.Top = 0F;
            this.textBox3.Width = 0.4029999F;
            // 
            // line3
            // 
            this.line3.Height = 0F;
            this.line3.Left = 0.01891732F;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0.2F;
            this.line3.Width = 8.344084F;
            this.line3.X1 = 8.363001F;
            this.line3.X2 = 0.01891732F;
            this.line3.Y1 = 0.2F;
            this.line3.Y2 = 0.2F;
            // 
            // txtVendorName
            // 
            this.txtVendorName.Height = 0.2F;
            this.txtVendorName.Left = 6.478F;
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; text-align: left; ver" +
    "tical-align: middle; ddo-char-set: 128";
            this.txtVendorName.Text = null;
            this.txtVendorName.Top = 0F;
            this.txtVendorName.Width = 1.42F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // reportHeader1
            // 
            this.reportHeader1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.label2,
            this.lblDate,
            this.label1,
            this.label9,
            this.line2,
            this.label5,
            this.crossSectionLine3,
            this.crossSectionLine4,
            this.crossSectionLine5,
            this.crossSectionLine6,
            this.crossSectionLine7,
            this.line1,
            this.txtPlanName,
            this.label3,
            this.txtPrice,
            this.label23,
            this.label4,
            this.crossSectionLine1});
            this.reportHeader1.Height = 0.7407498F;
            this.reportHeader1.Name = "reportHeader1";
            // 
            // label2
            // 
            this.label2.Height = 0.1690001F;
            this.label2.HyperLink = null;
            this.label2.Left = 0F;
            this.label2.Name = "label2";
            this.label2.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: normal; text-align: left" +
    "";
            this.label2.Text = "【Plan Items】";
            this.label2.Top = 0.341F;
            this.label2.Width = 2.154F;
            // 
            // lblDate
            // 
            this.lblDate.Height = 0.1690001F;
            this.lblDate.HyperLink = null;
            this.lblDate.Left = 0.03287452F;
            this.lblDate.Name = "lblDate";
            this.lblDate.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: center" +
    "";
            this.lblDate.Text = "Type";
            this.lblDate.Top = 0.5504998F;
            this.lblDate.Width = 0.6331255F;
            // 
            // label1
            // 
            this.label1.Height = 0.1690001F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.7142501F;
            this.label1.Name = "label1";
            this.label1.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: center" +
    "";
            this.label1.Text = "Code";
            this.label1.Top = 0.5504999F;
            this.label1.Width = 0.5140002F;
            // 
            // label9
            // 
            this.label9.Height = 0.1690001F;
            this.label9.HyperLink = null;
            this.label9.Left = 1.747F;
            this.label9.Name = "label9";
            this.label9.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label9.Text = "Name";
            this.label9.Top = 0.5500001F;
            this.label9.Width = 0.6839998F;
            // 
            // line2
            // 
            this.line2.Height = 1.192093E-07F;
            this.line2.Left = 0.03291652F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.51F;
            this.line2.Width = 8.330085F;
            this.line2.X1 = 8.363001F;
            this.line2.X2 = 0.03291652F;
            this.line2.Y1 = 0.5100001F;
            this.line2.Y2 = 0.51F;
            // 
            // label5
            // 
            this.label5.Height = 0.1690001F;
            this.label5.HyperLink = null;
            this.label5.Left = 6.478F;
            this.label5.Name = "label5";
            this.label5.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label5.Text = "Vendor";
            this.label5.Top = 0.55F;
            this.label5.Width = 0.7250004F;
            // 
            // crossSectionLine3
            // 
            this.crossSectionLine3.Bottom = 0.51F;
            this.crossSectionLine3.Left = 1.691F;
            this.crossSectionLine3.LineWeight = 1F;
            this.crossSectionLine3.Name = "crossSectionLine3";
            this.crossSectionLine3.Top = 0.5100001F;
            // 
            // crossSectionLine4
            // 
            this.crossSectionLine4.Bottom = 0.5100001F;
            this.crossSectionLine4.Left = 0.714F;
            this.crossSectionLine4.LineWeight = 1F;
            this.crossSectionLine4.Name = "crossSectionLine4";
            this.crossSectionLine4.Top = 0.5100001F;
            // 
            // crossSectionLine5
            // 
            this.crossSectionLine5.Bottom = 0.51F;
            this.crossSectionLine5.Left = 0.027F;
            this.crossSectionLine5.LineWeight = 1F;
            this.crossSectionLine5.Name = "crossSectionLine5";
            this.crossSectionLine5.Top = 0.5100001F;
            // 
            // crossSectionLine6
            // 
            this.crossSectionLine6.Bottom = 0.5100001F;
            this.crossSectionLine6.Left = 7.898F;
            this.crossSectionLine6.LineWeight = 1F;
            this.crossSectionLine6.Name = "crossSectionLine6";
            this.crossSectionLine6.Top = 0.5100001F;
            // 
            // crossSectionLine7
            // 
            this.crossSectionLine7.Bottom = 0.5100001F;
            this.crossSectionLine7.Left = 6.415F;
            this.crossSectionLine7.LineWeight = 1F;
            this.crossSectionLine7.Name = "crossSectionLine7";
            this.crossSectionLine7.Top = 0.5100001F;
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0.02691817F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.719F;
            this.line1.Width = 8.336082F;
            this.line1.X1 = 8.363001F;
            this.line1.X2 = 0.02691817F;
            this.line1.Y1 = 0.719F;
            this.line1.Y2 = 0.719F;
            // 
            // txtPlanName
            // 
            this.txtPlanName.Height = 0.341F;
            this.txtPlanName.Left = 0.39F;
            this.txtPlanName.Name = "txtPlanName";
            this.txtPlanName.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; ddo-char-set: 128";
            this.txtPlanName.Text = null;
            this.txtPlanName.Top = 0F;
            this.txtPlanName.Width = 5.573F;
            // 
            // label3
            // 
            this.label3.Height = 0.2F;
            this.label3.HyperLink = null;
            this.label3.Left = 5.963F;
            this.label3.Name = "label3";
            this.label3.Style = "font-family: MS PGothic; text-align: right";
            this.label3.Text = "Price: ";
            this.label3.Top = 0F;
            this.label3.Width = 0.4520006F;
            // 
            // txtPrice
            // 
            this.txtPrice.Height = 0.2F;
            this.txtPrice.Left = 6.478F;
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; ddo-char-set: 0";
            this.txtPrice.Text = null;
            this.txtPrice.Top = 0F;
            this.txtPrice.Width = 0.862F;
            // 
            // label23
            // 
            this.label23.Height = 0.2F;
            this.label23.HyperLink = null;
            this.label23.Left = 0.02700078F;
            this.label23.Name = "label23";
            this.label23.Style = "font-family: MS PGothic; text-align: left";
            this.label23.Text = "Plan: ";
            this.label23.Top = 0F;
            this.label23.Width = 0.3629992F;
            // 
            // label4
            // 
            this.label4.Height = 0.1690001F;
            this.label4.HyperLink = null;
            this.label4.Left = 7.960001F;
            this.label4.Name = "label4";
            this.label4.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label4.Text = "Qty";
            this.label4.Top = 0.55F;
            this.label4.Width = 0.4026237F;
            // 
            // crossSectionLine1
            // 
            this.crossSectionLine1.Bottom = 0.5100001F;
            this.crossSectionLine1.Left = 8.363001F;
            this.crossSectionLine1.LineWeight = 1F;
            this.crossSectionLine1.Name = "crossSectionLine1";
            this.crossSectionLine1.Top = 0.5100001F;
            // 
            // reportFooter1
            // 
            this.reportFooter1.Height = 0F;
            this.reportFooter1.Name = "reportFooter1";
            // 
            // FinalInfoSheet_WedInfo
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.427333F;
            this.Sections.Add(this.reportHeader1);
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.Sections.Add(this.reportFooter1);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVendorName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlanName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblDate;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label9;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label5;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine3;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine4;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine5;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine6;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine7;
        private GrapeCity.ActiveReports.SectionReportModel.Line line1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseTitle;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtItemName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox3;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPlanName;
        private GrapeCity.ActiveReports.SectionReportModel.Label label3;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPrice;
        private GrapeCity.ActiveReports.SectionReportModel.Label label23;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtVendorName;
        private GrapeCity.ActiveReports.SectionReportModel.Label label4;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine1;
    }
}
