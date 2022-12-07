using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IronPdf;

namespace PawnShop.Forms.Extensions
{
    public static class PdfExtensions
    {
        public static void SaveOrMerge(string pdfPath, PdfDocument pdf)
        {
            if (File.Exists(pdfPath))
            {
                var merged = PdfDocument.Merge(PdfDocument.FromFile(pdfPath), pdf);
                merged.SaveAs(pdfPath);
            }
            else
            {
                pdf.SaveAs(pdfPath);
            }
        }
    }
}
