namespace MauloaDemo.Reports {
    /// <summary>
    /// Summary description for ScheduleSheet.
    /// </summary>
    partial class ScheduleSheet {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScheduleSheet));
            this.pageHeader = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.txtDescription = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseDate = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhraseTime = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtPhrasePlace = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.pageFooter = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            this.reportHeader1 = new GrapeCity.ActiveReports.SectionReportModel.ReportHeader();
            this.txtCNum = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtWedDateTime = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtChurchName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label6 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.txtTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtGroomName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.lblDate = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label1 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.textBox4 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox5 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox6 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.label2 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.label9 = new GrapeCity.ActiveReports.SectionReportModel.Label();
            this.line1 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.picTop = new GrapeCity.ActiveReports.SectionReportModel.Picture();
            this.pageBreak1 = new GrapeCity.ActiveReports.SectionReportModel.PageBreak();
            this.picCenter1 = new GrapeCity.ActiveReports.SectionReportModel.Picture();
            this.txtScheduleDetailTitle = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.txtBrideName = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.textBox1 = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.reportFooter1 = new GrapeCity.ActiveReports.SectionReportModel.ReportFooter();
            this.txtWeddingNote = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            this.line2 = new GrapeCity.ActiveReports.SectionReportModel.Line();
            this.subNotes = new GrapeCity.ActiveReports.SectionReportModel.SubReport();
            this.txtHotel = new GrapeCity.ActiveReports.SectionReportModel.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhrasePlace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWedDateTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChurchName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGroomName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCenter1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScheduleDetailTitle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBrideName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeddingNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHotel)).BeginInit();
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
            this.txtDescription,
            this.txtPhraseTitle,
            this.txtPhraseDate,
            this.txtPhraseTime,
            this.txtPhrasePlace});
            this.detail.Height = 0.24F;
            this.detail.Name = "detail";
            this.detail.Format += new System.EventHandler(this.detail_Format);
            // 
            // txtDescription
            // 
            this.txtDescription.DataField = "description";
            this.txtDescription.Height = 0.2F;
            this.txtDescription.Left = 6.611001F;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Style = "font-family: MS PMincho; font-size: 9pt; ddo-char-set: 128";
            this.txtDescription.Text = null;
            this.txtDescription.Top = 0F;
            this.txtDescription.Width = 1.888999F;
            // 
            // txtPhraseTitle
            // 
            this.txtPhraseTitle.DataField = "title";
            this.txtPhraseTitle.Height = 0.2F;
            this.txtPhraseTitle.Left = 4.438F;
            this.txtPhraseTitle.Name = "txtPhraseTitle";
            this.txtPhraseTitle.Style = "font-family: MS PMincho; font-size: 9pt; font-weight: normal; ddo-char-set: 128";
            this.txtPhraseTitle.Text = null;
            this.txtPhraseTitle.Top = 0F;
            this.txtPhraseTitle.Width = 2.173F;
            // 
            // txtPhraseDate
            // 
            this.txtPhraseDate.DataField = "date";
            this.txtPhraseDate.Height = 0.2F;
            this.txtPhraseDate.Left = 0.07399988F;
            this.txtPhraseDate.Name = "txtPhraseDate";
            this.txtPhraseDate.OutputFormat = resources.GetString("txtPhraseDate.OutputFormat");
            this.txtPhraseDate.Style = "font-family: MS PMincho; font-size: 9pt; font-weight: normal; text-align: center;" +
    " ddo-char-set: 128";
            this.txtPhraseDate.Text = "12/31 (Mon)";
            this.txtPhraseDate.Top = 0F;
            this.txtPhraseDate.Width = 0.8210002F;
            // 
            // txtPhraseTime
            // 
            this.txtPhraseTime.DataField = "time";
            this.txtPhraseTime.Height = 0.2F;
            this.txtPhraseTime.Left = 0.895F;
            this.txtPhraseTime.Name = "txtPhraseTime";
            this.txtPhraseTime.Style = "font-family: MS PMincho; font-size: 9pt; font-weight: normal; text-align: center;" +
    " ddo-char-set: 128";
            this.txtPhraseTime.Text = "12:59";
            this.txtPhraseTime.Top = 0F;
            this.txtPhraseTime.Width = 0.543F;
            // 
            // txtPhrasePlace
            // 
            this.txtPhrasePlace.DataField = "place";
            this.txtPhrasePlace.Height = 0.2F;
            this.txtPhrasePlace.Left = 1.438F;
            this.txtPhrasePlace.Name = "txtPhrasePlace";
            this.txtPhrasePlace.Style = "font-family: MS PMincho; font-size: 9pt; font-weight: normal; ddo-char-set: 128";
            this.txtPhrasePlace.Text = null;
            this.txtPhrasePlace.Top = 0F;
            this.txtPhrasePlace.Width = 3F;
            // 
            // pageFooter
            // 
            this.pageFooter.Height = 0F;
            this.pageFooter.Name = "pageFooter";
            this.pageFooter.Visible = false;
            this.pageFooter.Format += new System.EventHandler(this.pageFooter_Format);
            // 
            // reportHeader1
            // 
            this.reportHeader1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.txtCNum,
            this.txtWedDateTime,
            this.txtChurchName,
            this.label6,
            this.txtTitle,
            this.txtGroomName,
            this.lblDate,
            this.label1,
            this.textBox4,
            this.textBox5,
            this.textBox6,
            this.label2,
            this.label9,
            this.line1,
            this.picTop,
            this.pageBreak1,
            this.picCenter1,
            this.txtScheduleDetailTitle,
            this.txtBrideName,
            this.textBox1});
            this.reportHeader1.Height = 10.88633F;
            this.reportHeader1.Name = "reportHeader1";
            this.reportHeader1.Format += new System.EventHandler(this.reportHeader1_Format);
            // 
            // txtCNum
            // 
            this.txtCNum.Height = 0.27F;
            this.txtCNum.Left = 7.016F;
            this.txtCNum.Name = "txtCNum";
            this.txtCNum.Style = "color: #A38F4D; font-size: 11.25pt; font-weight: bold; ddo-char-set: 128";
            this.txtCNum.Text = null;
            this.txtCNum.Top = 6.576F;
            this.txtCNum.Visible = false;
            this.txtCNum.Width = 1.001F;
            // 
            // txtWedDateTime
            // 
            this.txtWedDateTime.Height = 0.27F;
            this.txtWedDateTime.Left = 0.9980005F;
            this.txtWedDateTime.Name = "txtWedDateTime";
            this.txtWedDateTime.Style = "color: #A38F4D; font-family: MS PMincho; font-size: 15.75pt; font-weight: bold; t" +
    "ext-align: center; ddo-char-set: 128";
            this.txtWedDateTime.Text = "2015年 7月 5日 (日) 撮影";
            this.txtWedDateTime.Top = 5.935F;
            this.txtWedDateTime.Width = 6.466F;
            // 
            // txtChurchName
            // 
            this.txtChurchName.Height = 0.27F;
            this.txtChurchName.Left = 0.9980005F;
            this.txtChurchName.Name = "txtChurchName";
            this.txtChurchName.Style = "color: #A38F4D; font-family: MS PMincho; font-size: 15.75pt; font-weight: bold; t" +
    "ext-align: center; ddo-char-set: 128";
            this.txtChurchName.Text = "セントラルユニオン教会 中聖堂";
            this.txtChurchName.Top = 6.28F;
            this.txtChurchName.Width = 6.466F;
            // 
            // label6
            // 
            this.label6.Height = 0.4670002F;
            this.label6.HyperLink = null;
            this.label6.Left = 3.147F;
            this.label6.Name = "label6";
            this.label6.Style = "font-family: Times New Roman; font-size: 24pt; font-style: italic; font-weight: b" +
    "old; text-align: center; ddo-char-set: 0";
            this.label6.Text = "Mauloa Demo";
            this.label6.Top = 8.085F;
            this.label6.Width = 2.426F;
            // 
            // txtTitle
            // 
            this.txtTitle.CanGrow = false;
            this.txtTitle.Height = 0.9360001F;
            this.txtTitle.Left = 0.317F;
            this.txtTitle.MultiLine = false;
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Style = resources.GetString("txtTitle.Style");
            this.txtTitle.Text = "ワンランク上のビーチフォトデータプラン : ワイマナロビーチ＆ワイキキストリート";
            this.txtTitle.Top = 3.285F;
            this.txtTitle.Width = 7.793003F;
            // 
            // txtGroomName
            // 
            this.txtGroomName.Height = 0.3850003F;
            this.txtGroomName.Left = 0.9980005F;
            this.txtGroomName.Name = "txtGroomName";
            this.txtGroomName.Style = "color: #A38F4D; font-family: MS PMincho; font-size: 24pt; font-weight: bold; text" +
    "-align: center; ddo-char-set: 128";
            this.txtGroomName.Text = "MR. YAMADA TAKASHI";
            this.txtGroomName.Top = 4.375F;
            this.txtGroomName.Width = 6.466F;
            // 
            // lblDate
            // 
            this.lblDate.Height = 0.2390001F;
            this.lblDate.HyperLink = null;
            this.lblDate.Left = 0.07500051F;
            this.lblDate.Name = "lblDate";
            this.lblDate.Style = "font-family: MS PMincho; font-size: 10.125pt; font-weight: bold; text-align: cent" +
    "er";
            this.lblDate.Text = "日付";
            this.lblDate.Top = 10.687F;
            this.lblDate.Width = 0.821F;
            // 
            // label1
            // 
            this.label1.Height = 0.2390001F;
            this.label1.HyperLink = null;
            this.label1.Left = 0.9250005F;
            this.label1.Name = "label1";
            this.label1.Style = "font-family: MS PMincho; font-size: 10.125pt; font-weight: bold; text-align: cent" +
    "er";
            this.label1.Text = "時間";
            this.label1.Top = 10.687F;
            this.label1.Width = 0.5140001F;
            // 
            // textBox4
            // 
            this.textBox4.Height = 0.5409998F;
            this.textBox4.Left = 0.056F;
            this.textBox4.LineSpacing = 5F;
            this.textBox4.Name = "textBox4";
            this.textBox4.Style = "font-family: MS PMincho; font-size: 12pt; font-style: normal; font-weight: bold; " +
    "text-align: center; text-decoration: none; ddo-char-set: 128";
            this.textBox4.Text = "お申込みありがとうございます。\r\nハワイご到着後、撮影のご案内をいたしますので、下記までお電話下さい。";
            this.textBox4.Top = 7.134F;
            this.textBox4.Width = 8.440001F;
            // 
            // textBox5
            // 
            this.textBox5.Height = 0.2410053F;
            this.textBox5.Left = 3.49F;
            this.textBox5.Name = "textBox5";
            this.textBox5.Style = "font-family: MS PMincho; font-size: 12pt; font-style: normal; font-weight: bold; " +
    "text-align: left; text-decoration: none; ddo-char-set: 128";
            this.textBox5.Text = "電話： (808) 926-8600";
            this.textBox5.Top = 8.647F;
            this.textBox5.Width = 1.672F;
            // 
            // textBox6
            // 
            this.textBox6.Height = 0.2410055F;
            this.textBox6.Left = 3.36F;
            this.textBox6.Name = "textBox6";
            this.textBox6.Style = "font-family: MS PMincho; font-size: 12pt; font-style: normal; font-weight: bold; " +
    "text-align: left; text-decoration: none; ddo-char-set: 128";
            this.textBox6.Text = "（営業時間： 9:00～17:00）";
            this.textBox6.Top = 8.928F;
            this.textBox6.Width = 1.912F;
            // 
            // label2
            // 
            this.label2.Height = 0.2390001F;
            this.label2.HyperLink = null;
            this.label2.Left = 1.439F;
            this.label2.Name = "label2";
            this.label2.Style = "font-family: MS PMincho; font-size: 10.125pt; font-weight: bold; text-align: left" +
    "";
            this.label2.Text = "  場所";
            this.label2.Top = 10.687F;
            this.label2.Width = 2.999F;
            // 
            // label9
            // 
            this.label9.Height = 0.2390001F;
            this.label9.HyperLink = null;
            this.label9.Left = 4.438F;
            this.label9.Name = "label9";
            this.label9.Style = "font-family: MS PMincho; font-size: 10.125pt; font-weight: bold; text-align: left" +
    "";
            this.label9.Text = "  スケジュール";
            this.label9.Top = 10.687F;
            this.label9.Width = 2.173001F;
            // 
            // line1
            // 
            this.line1.Height = 0F;
            this.line1.Left = 0.07391717F;
            this.line1.LineWeight = 1F;
            this.line1.Name = "line1";
            this.line1.Top = 10.856F;
            this.line1.Width = 8.367083F;
            this.line1.X1 = 8.441F;
            this.line1.X2 = 0.07391717F;
            this.line1.Y1 = 10.856F;
            this.line1.Y2 = 10.856F;
            // 
            // picTop
            // 
            this.picTop.Height = 2.957F;
            this.picTop.ImageData = ((System.IO.Stream)(resources.GetObject("picTop.ImageData")));
            this.picTop.Left = 1.522F;
            this.picTop.Name = "picTop";
            this.picTop.SizeMode = GrapeCity.ActiveReports.SectionReportModel.SizeModes.Stretch;
            this.picTop.Top = 0.1830001F;
            this.picTop.Width = 5.494F;
            // 
            // pageBreak1
            // 
            this.pageBreak1.Height = 0.01F;
            this.pageBreak1.Left = 0F;
            this.pageBreak1.Name = "pageBreak1";
            this.pageBreak1.Size = new System.Drawing.SizeF(6.5F, 0.01F);
            this.pageBreak1.Top = 9.305F;
            this.pageBreak1.Width = 6.5F;
            // 
            // picCenter1
            // 
            this.picCenter1.Height = 0.5F;
            this.picCenter1.ImageData = ((System.IO.Stream)(resources.GetObject("picCenter1.ImageData")));
            this.picCenter1.Left = 2.825F;
            this.picCenter1.Name = "picCenter1";
            this.picCenter1.SizeMode = GrapeCity.ActiveReports.SectionReportModel.SizeModes.Stretch;
            this.picCenter1.Top = 9.517F;
            this.picCenter1.Width = 2.865F;
            // 
            // txtScheduleDetailTitle
            // 
            this.txtScheduleDetailTitle.Height = 0.273F;
            this.txtScheduleDetailTitle.Left = 0.07400024F;
            this.txtScheduleDetailTitle.Name = "txtScheduleDetailTitle";
            this.txtScheduleDetailTitle.Style = "font-family: MS PMincho; font-size: 20.25pt; font-weight: bold; text-align: cente" +
    "r; text-decoration: none; ddo-char-set: 128";
            this.txtScheduleDetailTitle.Text = "スケジュールのご案内";
            this.txtScheduleDetailTitle.Top = 10.124F;
            this.txtScheduleDetailTitle.Width = 8.366F;
            // 
            // txtBrideName
            // 
            this.txtBrideName.Height = 0.3850003F;
            this.txtBrideName.Left = 0.9980005F;
            this.txtBrideName.Name = "txtBrideName";
            this.txtBrideName.Style = "color: #A38F4D; font-family: MS PMincho; font-size: 24pt; font-weight: bold; text" +
    "-align: center; ddo-char-set: 128";
            this.txtBrideName.Text = "MS. HAYASHI YASUE";
            this.txtBrideName.Top = 5.186F;
            this.txtBrideName.Width = 6.466F;
            // 
            // textBox1
            // 
            this.textBox1.Height = 0.3850003F;
            this.textBox1.Left = 0.9970005F;
            this.textBox1.Name = "textBox1";
            this.textBox1.Style = "color: #A38F4D; font-family: MS PMincho; font-size: 24pt; font-weight: bold; text" +
    "-align: center; ddo-char-set: 128";
            this.textBox1.Text = "＆";
            this.textBox1.Top = 4.789F;
            this.textBox1.Width = 6.466F;
            // 
            // reportFooter1
            // 
            this.reportFooter1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.txtWeddingNote,
            this.line2,
            this.subNotes,
            this.txtHotel});
            this.reportFooter1.Height = 1.28F;
            this.reportFooter1.Name = "reportFooter1";
            this.reportFooter1.Format += new System.EventHandler(this.reportFooter1_Format);
            // 
            // txtWeddingNote
            // 
            this.txtWeddingNote.CanShrink = true;
            this.txtWeddingNote.Height = 0.231F;
            this.txtWeddingNote.Left = 0.565F;
            this.txtWeddingNote.Name = "txtWeddingNote";
            this.txtWeddingNote.Style = "font-family: MS PMincho; font-size: 10.125pt; font-style: normal; font-weight: bo" +
    "ld; text-align: left; text-decoration: none; ddo-char-set: 128";
            this.txtWeddingNote.Text = "* 結婚指輪をお持ちの方は、当日お忘れにならないようにお持ち下さい。";
            this.txtWeddingNote.Top = 0.457F;
            this.txtWeddingNote.Width = 7.875F;
            // 
            // line2
            // 
            this.line2.Height = 0F;
            this.line2.Left = 0.05591726F;
            this.line2.LineWeight = 1F;
            this.line2.Name = "line2";
            this.line2.Top = 0.05F;
            this.line2.Width = 8.384083F;
            this.line2.X1 = 8.44F;
            this.line2.X2 = 0.05591726F;
            this.line2.Y1 = 0.05F;
            this.line2.Y2 = 0.05F;
            // 
            // subNotes
            // 
            this.subNotes.CloseBorder = false;
            this.subNotes.Height = 0.4790001F;
            this.subNotes.Left = 0F;
            this.subNotes.Name = "subNotes";
            this.subNotes.Report = null;
            this.subNotes.ReportName = "subNotes";
            this.subNotes.Top = 0.758F;
            this.subNotes.Width = 8.5F;
            // 
            // txtHotel
            // 
            this.txtHotel.CanShrink = true;
            this.txtHotel.Height = 0.231F;
            this.txtHotel.Left = 0.565F;
            this.txtHotel.Name = "txtHotel";
            this.txtHotel.Style = "font-family: MS PMincho; font-size: 10.125pt; font-style: normal; font-weight: bo" +
    "ld; text-align: left; text-decoration: none; ddo-char-set: 128";
            this.txtHotel.Text = "ご滞在ホテル：";
            this.txtHotel.Top = 0.09500001F;
            this.txtHotel.Width = 7.875F;
            // 
            // ScheduleSheet
            // 
            this.MasterReport = false;
            this.PageSettings.Margins.Bottom = 0.3F;
            this.PageSettings.Margins.Left = 0.4F;
            this.PageSettings.Margins.Right = 0.4F;
            this.PageSettings.Margins.Top = 0.4F;
            this.PageSettings.Orientation = GrapeCity.ActiveReports.Document.Section.PageOrientation.Portrait;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 8.5F;
            this.Sections.Add(this.reportHeader1);
            this.Sections.Add(this.pageHeader);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter);
            this.Sections.Add(this.reportFooter1);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-style: normal; text-decoration: none; font-weight: normal; font-size: 10pt; " +
            "color: Black; font-family: \"MS PGothic\"; ddo-char-set: 128", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold; ddo-char-set: 128", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 14pt; font-weight: bold; font-style: italic; font-family: \"MS PGothic\"" +
            "; ddo-char-set: 128", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold; ddo-char-set: 128", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.txtDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhraseTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPhrasePlace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWedDateTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChurchName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGroomName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.label9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCenter1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScheduleDetailTitle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBrideName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeddingNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHotel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.ReportHeader reportHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.ReportFooter reportFooter1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtCNum;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtWedDateTime;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtChurchName;
        private GrapeCity.ActiveReports.SectionReportModel.Label label6;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtTitle;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtGroomName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtDescription;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseTitle;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseDate;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhraseTime;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtPhrasePlace;
        private GrapeCity.ActiveReports.SectionReportModel.Label lblDate;
        private GrapeCity.ActiveReports.SectionReportModel.Label label1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox4;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtWeddingNote;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox5;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox6;
        private GrapeCity.ActiveReports.SectionReportModel.Label label2;
        private GrapeCity.ActiveReports.SectionReportModel.Label label9;
        private GrapeCity.ActiveReports.SectionReportModel.Line line1;
        private GrapeCity.ActiveReports.SectionReportModel.Line line2;
        private GrapeCity.ActiveReports.SectionReportModel.SubReport subNotes;
        private GrapeCity.ActiveReports.SectionReportModel.Picture picTop;
        private GrapeCity.ActiveReports.SectionReportModel.PageBreak pageBreak1;
        private GrapeCity.ActiveReports.SectionReportModel.Picture picCenter1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtScheduleDetailTitle;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtBrideName;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox textBox1;
        private GrapeCity.ActiveReports.SectionReportModel.TextBox txtHotel;
    }
}
