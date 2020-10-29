using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Repository;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Export.Pdf.Section;

namespace MauloaDemo.Customer {

    public static class PDFExportHelper {

        ///// <summary>
        ///// Set paper size, run report and export as PDF.
        ///// </summary>
        ///// <param name="rpt"></param>
        ///// <param name="region_cd"></param>
        ///// <returns></returns>
        //public static ActionResult Run(SectionReport rpt, string region_cd) {
        //    SetPaperSize(rpt, region_cd);
        //    rpt.Run();
        //    return PDFResult(rpt);
        //}


        ///// <summary>
        ///// レポートをPDFファイルとしてブラウザに返す。
        ///// 
        ///// PDFファイルの内容がメモリに展開されそのままResponseストリームに出力される。
        ///// </summary>
        ///// <param name="rpt"></param>
        ///// <returns></returns>
        //public static ActionResult PDFResult(SectionReport rpt) {
        //    var p = new PdfExport();
        //    var m_stream = new System.IO.MemoryStream();
        //    p.Export(rpt.Document, m_stream);
        //    m_stream.Position = 0;
        //    return new FileStreamResult(m_stream, "application/pdf");
        //}

        ///// <summary>
        ///// レポートに地域毎の設定に応じた用紙サイズをセットする。
        ///// </summary>
        ///// <param name="rpt"></param>
        ///// <param name="region_cd"></param>
        //public static void SetPaperSize(SectionReport rpt, string region_cd) {
        //    //Letterをデフォルトとする。
        //    rpt.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Letter;

        //    //対象地域のRegionモデルオブジェクトを取得。(region.configファイルから。)
        //    var region = new RegionConfigRepository().Find(region_cd);

        //    //対象地域の用紙サイズ設定が「A4」であればA4に設定。
        //    if (region != null && "A4".Equals(region.paper_size)) {
        //        rpt.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
        //    }
        //}
    }
}