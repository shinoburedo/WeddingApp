namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for VendorOrderSheet.
    /// </summary>
    partial class VendorConfirmation_Datail_MKS
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(VendorConfirmation_Datail_MKS));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtwed_date = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtwed_time = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtchurch_name = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtg_name = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtb_name = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtitem_name = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtmake_date = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtmake_in_time = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtmake_note = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.txtmake_place = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label3 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label6 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label7 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label8 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label9 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label10 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.crossSectionLine1 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine2 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine3 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine4 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine5 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine6 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine7 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine8 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.crossSectionLine9 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.line3 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.crossSectionLine10 = new GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine();
            this.label4 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            ((System.ComponentModel.ISupportInitialize)(this.txtwed_date)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtwed_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtchurch_name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtg_name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtb_name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtitem_name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_date)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_in_time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_note)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_place)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).BeginInit();
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
            this.txtwed_date,
            this.txtwed_time,
            this.txtchurch_name,
            this.txtg_name,
            this.txtb_name,
            this.txtitem_name,
            this.txtmake_date,
            this.txtmake_in_time,
            this.txtmake_note,
            this.line1,
            this.txtmake_place});
            this.detail.Height = 0.2981944F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // txtwed_date
            // 
            this.txtwed_date.DataField = "wed_date";
            this.txtwed_date.Height = 0.2909999F;
            this.txtwed_date.Left = 0.056F;
            this.txtwed_date.Name = "txtwed_date";
            this.txtwed_date.Style = "vertical-align: middle";
            this.txtwed_date.Text = null;
            this.txtwed_date.Top = 0F;
            this.txtwed_date.Width = 0.7480001F;
            // 
            // txtwed_time
            // 
            this.txtwed_time.DataField = "wed_time";
            this.txtwed_time.Height = 0.2909999F;
            this.txtwed_time.Left = 0.804F;
            this.txtwed_time.Name = "txtwed_time";
            this.txtwed_time.Style = "text-align: center; vertical-align: middle";
            this.txtwed_time.Text = null;
            this.txtwed_time.Top = 0F;
            this.txtwed_time.Width = 0.4250001F;
            // 
            // txtchurch_name
            // 
            this.txtchurch_name.DataField = "church_name";
            this.txtchurch_name.Height = 0.2909999F;
            this.txtchurch_name.Left = 1.281F;
            this.txtchurch_name.Name = "txtchurch_name";
            this.txtchurch_name.Style = "vertical-align: middle";
            this.txtchurch_name.Text = null;
            this.txtchurch_name.Top = 0F;
            this.txtchurch_name.Width = 1.413F;
            // 
            // txtg_name
            // 
            this.txtg_name.Height = 0.2909999F;
            this.txtg_name.Left = 2.794F;
            this.txtg_name.Name = "txtg_name";
            this.txtg_name.Style = "vertical-align: middle";
            this.txtg_name.Text = null;
            this.txtg_name.Top = -5.820766E-11F;
            this.txtg_name.Width = 1.747F;
            // 
            // txtb_name
            // 
            this.txtb_name.Height = 0.2909999F;
            this.txtb_name.Left = 4.614F;
            this.txtb_name.Name = "txtb_name";
            this.txtb_name.Style = "vertical-align: middle";
            this.txtb_name.Text = null;
            this.txtb_name.Top = 0F;
            this.txtb_name.Width = 1.059F;
            // 
            // txtitem_name
            // 
            this.txtitem_name.DataField = "item_name";
            this.txtitem_name.Height = 0.2909999F;
            this.txtitem_name.Left = 5.725F;
            this.txtitem_name.Name = "txtitem_name";
            this.txtitem_name.Style = "vertical-align: middle";
            this.txtitem_name.Text = null;
            this.txtitem_name.Top = 0F;
            this.txtitem_name.Width = 1.239F;
            // 
            // txtmake_date
            // 
            this.txtmake_date.DataField = "make_date";
            this.txtmake_date.Height = 0.2909999F;
            this.txtmake_date.Left = 7.027F;
            this.txtmake_date.Name = "txtmake_date";
            this.txtmake_date.Style = "vertical-align: middle";
            this.txtmake_date.Text = null;
            this.txtmake_date.Top = 0F;
            this.txtmake_date.Width = 0.7880001F;
            // 
            // txtmake_in_time
            // 
            this.txtmake_in_time.DataField = "make_in_time";
            this.txtmake_in_time.Height = 0.2909999F;
            this.txtmake_in_time.Left = 7.815001F;
            this.txtmake_in_time.Name = "txtmake_in_time";
            this.txtmake_in_time.Style = "vertical-align: middle";
            this.txtmake_in_time.Text = null;
            this.txtmake_in_time.Top = 0F;
            this.txtmake_in_time.Width = 0.6360001F;
            // 
            // txtmake_note
            // 
            this.txtmake_note.CanGrow = false;
            this.txtmake_note.DataField = "make_note";
            this.txtmake_note.Height = 0.2909999F;
            this.txtmake_note.Left = 9.962001F;
            this.txtmake_note.Name = "txtmake_note";
            this.txtmake_note.Style = "vertical-align: middle";
            this.txtmake_note.Text = null;
            this.txtmake_note.Top = 0F;
            this.txtmake_note.Width = 0.934F;
            // 
            // line1
            // 
            this.line1.Height = 0.0002499819F;
            this.line1.Left = 0.067F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 0.298F;
            this.line1.Width = 10.83925F;
            this.line1.X1 = 0.067F;
            this.line1.X2 = 10.90625F;
            this.line1.Y1 = 0.298F;
            this.line1.Y2 = 0.29825F;
            // 
            // txtmake_place
            // 
            this.txtmake_place.DataField = "make_place";
            this.txtmake_place.Height = 0.2909999F;
            this.txtmake_place.Left = 8.523001F;
            this.txtmake_place.Name = "txtmake_place";
            this.txtmake_place.Style = "vertical-align: middle";
            this.txtmake_place.Text = null;
            this.txtmake_place.Top = 0.007F;
            this.txtmake_place.Width = 1.386F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Visible = false;
            // 
            // reportHeader1
            // 
            this.reportHeader1.CanGrow = false;
            this.reportHeader1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.label1,
            this.label2,
            this.label3,
            this.label6,
            this.label7,
            this.label8,
            this.label9,
            this.label10,
            this.crossSectionLine1,
            this.crossSectionLine2,
            this.crossSectionLine3,
            this.crossSectionLine4,
            this.crossSectionLine5,
            this.crossSectionLine6,
            this.crossSectionLine7,
            this.crossSectionLine8,
            this.crossSectionLine9,
            this.line2,
            this.line3,
            this.crossSectionLine10,
            this.label4});
            this.reportHeader1.Height = 0.3020833F;
            this.reportHeader1.Name = "reportHeader1";
            // 
            // label1
            // 
            this.label1.Height = 0.27F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.119F;
            this.label1.Name = "label1";
            this.label1.Style = "text-align: left; vertical-align: middle";
            this.label1.Text = "DATE";
            this.label1.Top = 0.03150004F;
            this.label1.Width = 0.5391681F;
            // 
            // label2
            // 
            this.label2.Height = 0.27F;
            this.label2.HyperLink = null;
            this.label2.Left = 0.8151682F;
            this.label2.Name = "label2";
            this.label2.Style = "text-align: left; vertical-align: middle";
            this.label2.Text = "TIME";
            this.label2.Top = 0.03150004F;
            this.label2.Width = 0.4138318F;
            // 
            // label3
            // 
            this.label3.Height = 0.27F;
            this.label3.HyperLink = null;
            this.label3.Left = 1.292F;
            this.label3.Name = "label3";
            this.label3.Style = "text-align: left; vertical-align: middle";
            this.label3.Text = "CHURCH OR BEACH";
            this.label3.Top = 0.032F;
            this.label3.Width = 1.402F;
            // 
            // label6
            // 
            this.label6.Height = 0.27F;
            this.label6.HyperLink = null;
            this.label6.Left = 2.794F;
            this.label6.Name = "label6";
            this.label6.Style = "text-align: left; vertical-align: middle";
            this.label6.Text = "GROOM NAME";
            this.label6.Top = 0.032F;
            this.label6.Width = 1.747F;
            // 
            // label7
            // 
            this.label7.Height = 0.27F;
            this.label7.HyperLink = null;
            this.label7.Left = 4.614F;
            this.label7.Name = "label7";
            this.label7.Style = "text-align: left; vertical-align: middle";
            this.label7.Text = "BRIDE NAME";
            this.label7.Top = 0.031F;
            this.label7.Width = 1.059F;
            // 
            // label8
            // 
            this.label8.Height = 0.27F;
            this.label8.HyperLink = null;
            this.label8.Left = 5.725F;
            this.label8.Name = "label8";
            this.label8.Style = "text-align: left; vertical-align: middle";
            this.label8.Text = "ORDER";
            this.label8.Top = 0.032F;
            this.label8.Width = 1.239F;
            // 
            // label9
            // 
            this.label9.Height = 0.27F;
            this.label9.HyperLink = null;
            this.label9.Left = 7.027F;
            this.label9.Name = "label9";
            this.label9.Style = "text-align: left; vertical-align: middle";
            this.label9.Text = "MAKE IN";
            this.label9.Top = 0.031F;
            this.label9.Width = 1.132169F;
            // 
            // label10
            // 
            this.label10.Height = 0.27F;
            this.label10.HyperLink = null;
            this.label10.Left = 9.962001F;
            this.label10.Name = "label10";
            this.label10.Style = "text-align: left; vertical-align: middle";
            this.label10.Text = "MEMO";
            this.label10.Top = 0.03150004F;
            this.label10.Width = 0.9451675F;
            // 
            // crossSectionLine1
            // 
            this.crossSectionLine1.Bottom = 0F;
            this.crossSectionLine1.Left = 0.8150001F;
            this.crossSectionLine1.LineWeight = 1F;
            this.crossSectionLine1.Name = "crossSectionLine1";
            this.crossSectionLine1.Top = 0.032F;
            // 
            // crossSectionLine2
            // 
            this.crossSectionLine2.Bottom = 0F;
            this.crossSectionLine2.Left = 2.694F;
            this.crossSectionLine2.LineWeight = 1F;
            this.crossSectionLine2.Name = "crossSectionLine2";
            this.crossSectionLine2.Top = 0.032F;
            // 
            // crossSectionLine3
            // 
            this.crossSectionLine3.Bottom = 0F;
            this.crossSectionLine3.Left = 1.229F;
            this.crossSectionLine3.LineWeight = 1F;
            this.crossSectionLine3.Name = "crossSectionLine3";
            this.crossSectionLine3.Top = 0.032F;
            // 
            // crossSectionLine4
            // 
            this.crossSectionLine4.Bottom = 0F;
            this.crossSectionLine4.Left = 4.541F;
            this.crossSectionLine4.LineWeight = 1F;
            this.crossSectionLine4.Name = "crossSectionLine4";
            this.crossSectionLine4.Top = 0.032F;
            // 
            // crossSectionLine5
            // 
            this.crossSectionLine5.Bottom = 0F;
            this.crossSectionLine5.Left = 5.673F;
            this.crossSectionLine5.LineWeight = 1F;
            this.crossSectionLine5.Name = "crossSectionLine5";
            this.crossSectionLine5.Top = 0.032F;
            // 
            // crossSectionLine6
            // 
            this.crossSectionLine6.Bottom = 0F;
            this.crossSectionLine6.Left = 6.964F;
            this.crossSectionLine6.LineWeight = 1F;
            this.crossSectionLine6.Name = "crossSectionLine6";
            this.crossSectionLine6.Top = 0.032F;
            // 
            // crossSectionLine7
            // 
            this.crossSectionLine7.Bottom = 0F;
            this.crossSectionLine7.Left = 9.909F;
            this.crossSectionLine7.LineWeight = 1F;
            this.crossSectionLine7.Name = "crossSectionLine7";
            this.crossSectionLine7.Top = 0.032F;
            // 
            // crossSectionLine8
            // 
            this.crossSectionLine8.Bottom = 6.332994E-08F;
            this.crossSectionLine8.Left = 0.06699997F;
            this.crossSectionLine8.LineWeight = 1F;
            this.crossSectionLine8.Name = "crossSectionLine8";
            this.crossSectionLine8.Top = 0.03200006F;
            // 
            // crossSectionLine9
            // 
            this.crossSectionLine9.Bottom = 6.332994E-08F;
            this.crossSectionLine9.Left = 10.896F;
            this.crossSectionLine9.LineWeight = 1F;
            this.crossSectionLine9.Name = "crossSectionLine9";
            this.crossSectionLine9.Top = 0.03200006F;
            // 
            // line2
            // 
            this.line2.Height = 0.0002498925F;
            this.line2.Left = 0.066F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.03200001F;
            this.line2.Width = 10.83925F;
            this.line2.X1 = 0.066F;
            this.line2.X2 = 10.90525F;
            this.line2.Y1 = 0.03200001F;
            this.line2.Y2 = 0.0322499F;
            // 
            // line3
            // 
            this.line3.Height = 0.0002498925F;
            this.line3.Left = 0.066F;
            this.line3.LineWeight = 1F;
            this.line3.Name = "line3";
            this.line3.Top = 0.301F;
            this.line3.Width = 10.83925F;
            this.line3.X1 = 0.066F;
            this.line3.X2 = 10.90525F;
            this.line3.Y1 = 0.301F;
            this.line3.Y2 = 0.3012499F;
            // 
            // crossSectionLine10
            // 
            this.crossSectionLine10.Bottom = 0F;
            this.crossSectionLine10.Left = 8.451F;
            this.crossSectionLine10.LineWeight = 1F;
            this.crossSectionLine10.Name = "crossSectionLine10";
            this.crossSectionLine10.Top = 0.032F;
            // 
            // label4
            // 
            this.label4.Height = 0.27F;
            this.label4.HyperLink = null;
            this.label4.Left = 8.523001F;
            this.label4.Name = "label4";
            this.label4.Style = "text-align: left; vertical-align: middle";
            this.label4.Text = "HOTEL";
            this.label4.Top = 0.032F;
            this.label4.Width = 1.132169F;
            // 
            // reportFooter1
            // 
            this.reportFooter1.Height = 0F;
            this.reportFooter1.Name = "reportFooter1";
            // 
            // VendorConfirmation_Datail_MKS
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0.4F;
            this.PageSettings.Margins.Left = 0.4F;
            this.PageSettings.Margins.Right = 0.4F;
            this.PageSettings.Margins.Top = 0.4F;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 10.97042F;
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
            ((System.ComponentModel.ISupportInitialize)(this.txtwed_date)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtwed_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtchurch_name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtg_name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtb_name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtitem_name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_date)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_in_time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_note)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtmake_place)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtwed_date;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtwed_time;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtchurch_name;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtg_name;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtb_name;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtitem_name;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtmake_date;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtmake_in_time;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtmake_note;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label3;
        private GrapeCity.ActiveReports.SectionReportModel.Label label6;
        private GrapeCity.ActiveReports.SectionReportModel.Label label7;
        private GrapeCity.ActiveReports.SectionReportModel.Label label8;
        private GrapeCity.ActiveReports.SectionReportModel.Label label9;
        private GrapeCity.ActiveReports.SectionReportModel.Label label10;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine1;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine2;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine3;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine4;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine5;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine6;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine7;
        private GrapeCity.ActiveReports.SectionReportModel.Line line1;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine8;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine9;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.Line line3;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtmake_place;
        private GrapeCity.ActiveReports.SectionReportModel.CrossSectionLine crossSectionLine10;
        private GrapeCity.ActiveReports.SectionReportModel.Label label4;
    }
}
