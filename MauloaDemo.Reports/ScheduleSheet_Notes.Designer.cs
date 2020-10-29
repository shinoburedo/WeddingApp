namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for ScheduleSheet_Notes.
    /// </summary>
    partial class ScheduleSheet_Notes {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleSheet_Notes));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtNote = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.picCenter = new GrapeCity.ActiveReports.SectionReportModel.Picture();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCenter)).BeginInit();
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
            this.txtTitle,
            this.txtNote,
            this.picCenter});
            this.detail.Height = 1.739583F;
            this.detail.KeepTogether = true;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // txtTitle
            // 
            this.txtTitle.DataField = "title_jpn";
            this.txtTitle.Height = 0.273F;
            this.txtTitle.Left = 0.074F;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Style = "font-family: MS PMincho; font-size: 20.25pt; font-weight: bold; text-align: cente" +
    "r; text-decoration: none; ddo-char-set: 128";
            this.txtTitle.Text = null;
            this.txtTitle.Top = 0.9F;
            this.txtTitle.Width = 8.309F;
            // 
            // txtNote
            // 
            this.txtNote.DataField = "note_jpn";
            this.txtNote.Height = 0.253F;
            this.txtNote.Left = 0.5420001F;
            this.txtNote.LineSpacing = 0.5F;
            this.txtNote.Name = "txtNote";
            this.txtNote.Style = "font-family: MS PMincho; font-size: 9pt; font-weight: bold; ddo-char-set: 128";
            this.txtNote.Text = null;
            this.txtNote.Top = 1.256167F;
            this.txtNote.Width = 7.342F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Visible = false;
            // 
            // picCenter
            // 
            this.picCenter.Height = 0.48F;
            this.picCenter.ImageData = ((System.IO.Stream)(resources.GetObject("picCenter.ImageData")));
            this.picCenter.Left = 2.67F;
            this.picCenter.Name = "picCenter";
            this.picCenter.SizeMode = GrapeCity.ActiveReports.SectionReportModel.SizeModes.Stretch;
            this.picCenter.Top = 0.342F;
            this.picCenter.Width = 3F;
            // 
            // ScheduleSheet_Notes
            // 
            this.MasterReport = false;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.489583F;
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCenter)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTitle;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtNote;
        private GrapeCity.ActiveReports.SectionReportModel.Picture picCenter;
    }
}
