using System;
using System.IO;
using System.Runtime.InteropServices;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using PPT = Microsoft.Office.Interop.PowerPoint;
using Office = Microsoft.Office.Core;

namespace DevExpress.ProductsDemo.Win.Services
{
    public static class PowerPointReportExporter
    {
        public static void ExportReportToPptx(XtraReport report, string outputPptxPath)
        {
            // Ensure document is generated
            if (report.PrintingSystem.Document.PageCount == 0)
                report.CreateDocument();

            int pageCount = report.Pages.Count;
            if (pageCount == 0) return;

            // Create a temporary folder to hold the page images
            string tempFolder = Path.Combine(Path.GetTempPath(), "DevExpressReport_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            PPT.Application pptApp = null;
            PPT.Presentation presentation = null;

            try
            {
                pptApp = new PPT.Application();

                // Add a new presentation (hidden from the user while building)
                presentation = pptApp.Presentations.Add(Office.MsoTriState.msoFalse);

                // Match slide size to the DevExpress report size
                // DevExpress uses HundredthsOfAnInch (1/100 inch). PowerPoint uses Points (1/72 inch).
                float slideWidthPoints = (report.PageWidth / 100f) * 72f;
                float slideHeightPoints = (report.PageHeight / 100f) * 72f;

                presentation.PageSetup.SlideWidth = slideWidthPoints;
                presentation.PageSetup.SlideHeight = slideHeightPoints;

                // Loop through every page of the report
                for (int i = 0; i < pageCount; i++)
                {
                    string imagePath = Path.Combine(tempFolder, $"slide_{i + 1}.png");

                    // Export just this specific page as a high-quality PNG
                    ImageExportOptions options = new ImageExportOptions
                    {
                        Format = System.Drawing.Imaging.ImageFormat.Png,
                        ExportMode = ImageExportMode.SingleFile,
                        PageRange = (i + 1).ToString(),
                        Resolution = 150 // 150 is a great balance between crisp text and file size
                    };

                    report.ExportToImage(imagePath, options);

                    // Create a blank slide
                    var slide = presentation.Slides.Add(i + 1, PPT.PpSlideLayout.ppLayoutBlank);

                    // Embed the image filling the entire slide
                    slide.Shapes.AddPicture(
                        imagePath,
                        Office.MsoTriState.msoFalse, // Don't link
                        Office.MsoTriState.msoTrue,  // Embed with document
                        0, 0, slideWidthPoints, slideHeightPoints
                    );
                }

                // Save and close
                presentation.SaveAs(outputPptxPath, PPT.PpSaveAsFileType.ppSaveAsDefault);
                presentation.Close();
            }
            finally
            {
                // Clean up background COM processes to prevent memory leaks
                if (presentation != null) Marshal.ReleaseComObject(presentation);
                if (pptApp != null)
                {
                    pptApp.Quit();
                    Marshal.ReleaseComObject(pptApp);
                }

                // Delete the temporary image files
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }
    }
}