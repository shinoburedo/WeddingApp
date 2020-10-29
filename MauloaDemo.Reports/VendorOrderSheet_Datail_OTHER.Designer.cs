namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    partial class VendorOrderSheet_Datail_OTHER {
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(VendorOrderSheet_Datail_OTHER));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.label12 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.shape2 = new GrapeCity.ActiveReports.SectionReportModel.Shape();
            this.txtItemName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            ((System.ComponentModel.ISupportInitialize)(this.label12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).BeginInit();
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
            this.txtItemName});
            this.detail.Height = 0.559361F;
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
            this.shape2.Height = 0.27F;
            this.shape2.Left = 0F;
            this.shape2.Name = "shape2";
            this.shape2.RoundingRadius = new GrapeCity.ActiveReports.Controls.CornersRadius(2F);
            this.shape2.Style = GrapeCity.ActiveReports.SectionReportModel.ShapeType.RoundRect;
            this.shape2.Top = 0.2F;
            this.shape2.Width = 8.363501F;
            // 
            // txtItemName
            // 
            this.txtItemName.Height = 0.2909999F;
            this.txtItemName.Left = 0.05599989F;
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Style = "vertical-align: middle";
            this.txtItemName.Text = null;
            this.txtItemName.Top = 0.179F;
            this.txtItemName.Width = 8.162998F;
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
            // VendorOrderSheet_Datail_OTHER
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
            ((System.ComponentModel.ISupportInitialize)(this.txtItemName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label12;
        private GrapeCity.ActiveReports.SectionReportModel.Shape shape2;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtItemName;
    }
}
