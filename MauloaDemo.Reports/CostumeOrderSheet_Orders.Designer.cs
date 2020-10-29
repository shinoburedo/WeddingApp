namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for CostumeOrderSheet_Orders.
    /// </summary>
    partial class CostumeOrderSheet_Orders {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CostumeOrderSheet_Orders));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtPhraseDate = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhrasePlace = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.lblDate = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label9 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.label3 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.crossSectionLine2 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine5 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine6 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine7 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhrasePlace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).BeginInit();
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
            this.txtPhraseDate,
            this.txtPhrasePlace,
            this.txtPhraseTitle,
            this.line3});
            this.detail.Height = 0.2083334F;
            this.detail.Name = "detail";
            // 
            // txtPhraseDate
            // 
            this.txtPhraseDate.DataField = "item_cd";
            this.txtPhraseDate.Height = 0.2F;
            this.txtPhraseDate.Left = 0.054F;
            this.txtPhraseDate.Name = "txtPhraseDate";
            this.txtPhraseDate.OutputFormat = resources.GetString("txtPhraseDate.OutputFormat");
            this.txtPhraseDate.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left; " +
    "ddo-char-set: 128";
            this.txtPhraseDate.Text = null;
            this.txtPhraseDate.Top = 0F;
            this.txtPhraseDate.Width = 1.267F;
            // 
            // txtPhrasePlace
            // 
            this.txtPhrasePlace.DataField = "item_name";
            this.txtPhrasePlace.Height = 0.2F;
            this.txtPhrasePlace.Left = 1.373F;
            this.txtPhrasePlace.Name = "txtPhrasePlace";
            this.txtPhrasePlace.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; ddo-char-set: 128";
            this.txtPhrasePlace.Text = null;
            this.txtPhrasePlace.Top = 0.008F;
            this.txtPhrasePlace.Width = 3.907F;
            // 
            // txtPhraseTitle
            // 
            this.txtPhraseTitle.DataField = "note";
            this.txtPhraseTitle.Height = 0.2F;
            this.txtPhraseTitle.Left = 5.374F;
            this.txtPhraseTitle.Name = "txtPhraseTitle";
            this.txtPhraseTitle.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; text-align: left; ddo" +
    "-char-set: 128";
            this.txtPhraseTitle.Text = null;
            this.txtPhraseTitle.Top = 0F;
            this.txtPhraseTitle.Width = 4.519F;
            // 
            // line3
            // 
            this.line3.Height = 0F;
            this.line3.Left = 0.02491665F;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0.208F;
            this.line3.Width = 9.795083F;
            this.line3.X1 = 9.82F;
            this.line3.X2 = 0.02491665F;
            this.line3.Y1 = 0.208F;
            this.line3.Y2 = 0.208F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            // 
            // reportHeader1
            // 
            this.reportHeader1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.lblDate,
            this.label2,
            this.label9,
            this.line2,
            this.label3,
            this.crossSectionLine2,
            this.crossSectionLine5,
            this.crossSectionLine6,
            this.crossSectionLine7,
            this.line1});
            this.reportHeader1.Height = 0.4270002F;
            this.reportHeader1.Name = "reportHeader1";
            // 
            // lblDate
            // 
            this.lblDate.Height = 0.1690001F;
            this.lblDate.HyperLink = null;
            this.lblDate.Left = 0.054F;
            this.lblDate.Name = "lblDate";
            this.lblDate.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.lblDate.Text = "Code";
            this.lblDate.Top = 0.258F;
            this.lblDate.Width = 0.821F;
            // 
            // label2
            // 
            this.label2.Height = 0.1690001F;
            this.label2.HyperLink = null;
            this.label2.Left = 1.321F;
            this.label2.Name = "label2";
            this.label2.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label2.Text = " Name";
            this.label2.Top = 0.258F;
            this.label2.Width = 2.012F;
            // 
            // label9
            // 
            this.label9.Height = 0.1690001F;
            this.label9.HyperLink = null;
            this.label9.Left = 5.374F;
            this.label9.Name = "label9";
            this.label9.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label9.Text = "Memo";
            this.label9.Top = 0.217F;
            this.label9.Width = 0.381F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0.02491665F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.217F;
            this.line2.Width = 9.795083F;
            this.line2.X1 = 9.82F;
            this.line2.X2 = 0.02491665F;
            this.line2.Y1 = 0.217F;
            this.line2.Y2 = 0.217F;
            // 
            // label3
            // 
            this.label3.Height = 0.1690001F;
            this.label3.HyperLink = null;
            this.label3.Left = 0F;
            this.label3.Name = "label3";
            this.label3.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label3.Text = "[Orders]";
            this.label3.Top = 4.470348E-08F;
            this.label3.Width = 2.154F;
            // 
            // crossSectionLine2
            // 
            this.crossSectionLine2.Bottom = 0.2175F;
            this.crossSectionLine2.Left = 1.321F;
            this.crossSectionLine2.LineWeight = 1F;
            this.crossSectionLine2.Name = "crossSectionLine2";
            this.crossSectionLine2.Top = 0.2175F;
            // 
            // crossSectionLine5
            // 
            this.crossSectionLine5.Bottom = 0.2175F;
            this.crossSectionLine5.Left = 0.025F;
            this.crossSectionLine5.LineWeight = 1F;
            this.crossSectionLine5.Name = "crossSectionLine5";
            this.crossSectionLine5.Top = 0.2175F;
            // 
            // crossSectionLine6
            // 
            this.crossSectionLine6.Bottom = 0.217F;
            this.crossSectionLine6.Left = 5.28F;
            this.crossSectionLine6.LineWeight = 1F;
            this.crossSectionLine6.Name = "crossSectionLine6";
            this.crossSectionLine6.Top = 0.217F;
            // 
            // crossSectionLine7
            // 
            this.crossSectionLine7.Bottom = 0.217F;
            this.crossSectionLine7.Left = 9.82F;
            this.crossSectionLine7.LineWeight = 1F;
            this.crossSectionLine7.Name = "crossSectionLine7";
            this.crossSectionLine7.Top = 0.217F;
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0.02491665F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.427F;
            this.line1.Width = 9.795083F;
            this.line1.X1 = 9.82F;
            this.line1.X2 = 0.02491665F;
            this.line1.Y1 = 0.427F;
            this.line1.Y2 = 0.427F;
            // 
            // reportFooter1
            // 
            this.reportFooter1.Height = 0F;
            this.reportFooter1.Name = "reportFooter1";
            // 
            // CostumeOrderSheet_Orders
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 9.812499F;
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
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhrasePlace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseDate;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhrasePlace;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseTitle;
        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblDate;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label9;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label3;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine2;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine5;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine6;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine7;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.Line line1;

    }
}
