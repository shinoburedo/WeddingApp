namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for CostumeOrderSheet_Schedule.
    /// </summary>
    partial class CostumeOrderSheet_Schedule {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CostumeOrderSheet_Schedule));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtDescription = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseDate = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseTime = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhrasePlace = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.lblDate = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label9 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.crossSectionLine1 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine2 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine3 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine4 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine5 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.label3 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhrasePlace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
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
            this.txtDescription,
            this.txtPhraseTitle,
            this.txtPhraseDate,
            this.txtPhraseTime,
            this.txtPhrasePlace});
            this.detail.Height = 0.2F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // txtDescription
            // 
            this.txtDescription.DataField = "description";
            this.txtDescription.Height = 0.2F;
            this.txtDescription.Left = 5.77F;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Style = "font-family: MS PGothic; font-size: 9pt; ddo-char-set: 128";
            this.txtDescription.Text = null;
            this.txtDescription.Top = 0F;
            this.txtDescription.Width = 2.681F;
            // 
            // txtPhraseTitle
            // 
            this.txtPhraseTitle.Height = 0.2F;
            this.txtPhraseTitle.Left = 5.378F;
            this.txtPhraseTitle.Name = "txtPhraseTitle";
            this.txtPhraseTitle.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; ddo-char-set: 128";
            this.txtPhraseTitle.Text = null;
            this.txtPhraseTitle.Top = 0F;
            this.txtPhraseTitle.Width = 4.442F;
            // 
            // txtPhraseDate
            // 
            this.txtPhraseDate.DataField = "date";
            this.txtPhraseDate.Height = 0.2F;
            this.txtPhraseDate.Left = 0.02500033F;
            this.txtPhraseDate.Name = "txtPhraseDate";
            this.txtPhraseDate.OutputFormat = resources.GetString("txtPhraseDate.OutputFormat");
            this.txtPhraseDate.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: center" +
    "; ddo-char-set: 128";
            this.txtPhraseDate.Text = "12/31 (Mon)";
            this.txtPhraseDate.Top = 0F;
            this.txtPhraseDate.Width = 0.821F;
            // 
            // txtPhraseTime
            // 
            this.txtPhraseTime.DataField = "time";
            this.txtPhraseTime.Height = 0.2F;
            this.txtPhraseTime.Left = 0.8460006F;
            this.txtPhraseTime.Name = "txtPhraseTime";
            this.txtPhraseTime.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: center" +
    "; ddo-char-set: 128";
            this.txtPhraseTime.Text = "12:59";
            this.txtPhraseTime.Top = 0F;
            this.txtPhraseTime.Width = 0.543F;
            // 
            // txtPhrasePlace
            // 
            this.txtPhrasePlace.Height = 0.2F;
            this.txtPhrasePlace.Left = 1.462F;
            this.txtPhrasePlace.Name = "txtPhrasePlace";
            this.txtPhrasePlace.Style = "font-family: MS PGothic; font-size: 9pt; font-weight: bold; ddo-char-set: 128";
            this.txtPhrasePlace.Text = null;
            this.txtPhrasePlace.Top = 0F;
            this.txtPhrasePlace.Width = 3.801F;
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
            this.label1,
            this.label2,
            this.label9,
            this.line1,
            this.line2,
            this.crossSectionLine1,
            this.crossSectionLine2,
            this.crossSectionLine3,
            this.crossSectionLine4,
            this.crossSectionLine5,
            this.label3});
            this.reportHeader1.Height = 0.4270833F;
            this.reportHeader1.Name = "reportHeader1";
            // 
            // lblDate
            // 
            this.lblDate.Height = 0.1690001F;
            this.lblDate.HyperLink = null;
            this.lblDate.Left = 0.02500006F;
            this.lblDate.Name = "lblDate";
            this.lblDate.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: center" +
    "";
            this.lblDate.Text = "Date";
            this.lblDate.Top = 0.2579999F;
            this.lblDate.Width = 0.821F;
            // 
            // label1
            // 
            this.label1.Height = 0.1690001F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.8750001F;
            this.label1.Name = "label1";
            this.label1.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: center" +
    "";
            this.label1.Text = "Time";
            this.label1.Top = 0.2579999F;
            this.label1.Width = 0.5140002F;
            // 
            // label2
            // 
            this.label2.Height = 0.1690001F;
            this.label2.HyperLink = null;
            this.label2.Left = 1.389F;
            this.label2.Name = "label2";
            this.label2.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label2.Text = "  Place";
            this.label2.Top = 0.2579999F;
            this.label2.Width = 2.114F;
            // 
            // label9
            // 
            this.label9.Height = 0.1690001F;
            this.label9.HyperLink = null;
            this.label9.Left = 5.378F;
            this.label9.Name = "label9";
            this.label9.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label9.Text = "Schedule";
            this.label9.Top = 0.258F;
            this.label9.Width = 2.173F;
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0.02591848F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.427F;
            this.line1.Width = 9.794082F;
            this.line1.X1 = 9.82F;
            this.line1.X2 = 0.02591848F;
            this.line1.Y1 = 0.427F;
            this.line1.Y2 = 0.427F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0.02391815F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.217F;
            this.line2.Width = 9.796082F;
            this.line2.X1 = 9.82F;
            this.line2.X2 = 0.02391815F;
            this.line2.Y1 = 0.217F;
            this.line2.Y2 = 0.217F;
            // 
            // crossSectionLine1
            // 
            this.crossSectionLine1.Bottom = 0.2175833F;
            this.crossSectionLine1.Left = 9.82F;
            this.crossSectionLine1.LineWeight = 1F;
            this.crossSectionLine1.Name = "crossSectionLine1";
            this.crossSectionLine1.Top = 0.228F;
            // 
            // crossSectionLine2
            // 
            this.crossSectionLine2.Bottom = 0.217F;
            this.crossSectionLine2.Left = 5.326F;
            this.crossSectionLine2.LineWeight = 1F;
            this.crossSectionLine2.Name = "crossSectionLine2";
            this.crossSectionLine2.Top = 0.217F;
            // 
            // crossSectionLine3
            // 
            this.crossSectionLine3.Bottom = 0.2174999F;
            this.crossSectionLine3.Left = 1.389125F;
            this.crossSectionLine3.LineWeight = 1F;
            this.crossSectionLine3.Name = "crossSectionLine3";
            this.crossSectionLine3.Top = 0.2174999F;
            // 
            // crossSectionLine4
            // 
            this.crossSectionLine4.Bottom = 0.2174999F;
            this.crossSectionLine4.Left = 0.8751246F;
            this.crossSectionLine4.LineWeight = 1F;
            this.crossSectionLine4.Name = "crossSectionLine4";
            this.crossSectionLine4.Top = 0.2174999F;
            // 
            // crossSectionLine5
            // 
            this.crossSectionLine5.Bottom = 0.2174999F;
            this.crossSectionLine5.Left = 0.02412453F;
            this.crossSectionLine5.LineWeight = 1F;
            this.crossSectionLine5.Name = "crossSectionLine5";
            this.crossSectionLine5.Top = 0.2174999F;
            // 
            // label3
            // 
            this.label3.Height = 0.1690001F;
            this.label3.HyperLink = null;
            this.label3.Left = 0F;
            this.label3.Name = "label3";
            this.label3.Style = "font-family: MS PGothic; font-size: 9.75pt; font-weight: bold; text-align: left";
            this.label3.Text = "[Schedule]";
            this.label3.Top = 0F;
            this.label3.Width = 2.154F;
            // 
            // reportFooter1
            // 
            this.reportFooter1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.line3});
            this.reportFooter1.Height = 0F;
            this.reportFooter1.Name = "reportFooter1";
            // 
            // line3
            // 
            this.line3.Height = 0F;
            this.line3.Left = 0.02591753F;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0F;
            this.line3.Width = 9.794083F;
            this.line3.X1 = 9.82F;
            this.line3.X2 = 0.02591753F;
            this.line3.Y1 = 0F;
            this.line3.Y2 = 0F;
            // 
            // CostumeOrderSheet_Schedule
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 9.864587F;
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
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhrasePlace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblDate;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label9;
        private GrapeCity.ActiveReports.SectionReportModel.Line line1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtDescription;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseTitle;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseDate;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseTime;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhrasePlace;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine1;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine2;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine3;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine4;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine5;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.Label label3;
    }
}
