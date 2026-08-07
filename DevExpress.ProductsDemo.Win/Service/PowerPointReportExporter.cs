using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using System.Runtime.InteropServices;
using MsoTriState = Microsoft.Office.Core.MsoTriState;
using PPT = Microsoft.Office.Interop.PowerPoint;

namespace DevExpress.ProductsDemo.Win.Services
{
    public static class PowerPointReportExporter
    {
        public static void ExportReportToPptx(XtraReport report, string outputPptxPath)
        {
            if (report.PrintingSystem.Document.PageCount == 0)
                report.CreateDocument();

            int pageCount = report.Pages.Count;
            if (pageCount == 0) return;

            string tempFolder = Path.Combine(Path.GetTempPath(), "DevExpressReport_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            PPT.Application pptApp = null;
            PPT.Presentation presentation = null;

            try
            {
                pptApp = new PPT.Application();
                presentation = pptApp.Presentations.Add(MsoTriState.msoFalse);

                float slideWidthPoints = (report.PageWidth / 100f) * 72f;
                float slideHeightPoints = (report.PageHeight / 100f) * 72f;

                presentation.PageSetup.SlideWidth = slideWidthPoints;
                presentation.PageSetup.SlideHeight = slideHeightPoints;

                // التمرير على كل صفحة ومعالجتها بشكل معزول
                for (int i = 0; i < pageCount; i++)
                {
                    // إنشاء مجلد فرعي مستقل لكل صفحة لتفادي مشاكل تسمية DevExpress
                    string pageFolder = Path.Combine(tempFolder, $"page_{i + 1}");
                    Directory.CreateDirectory(pageFolder);

                    string targetImagePath = Path.Combine(pageFolder, "slide.png");

                    ImageExportOptions options = new ImageExportOptions
                    {
                        Format = System.Drawing.Imaging.ImageFormat.Png,
                        // استخدام وضع SingleFilePageByPage يجبر DevExpress على احترام الـ PageRange
                        ExportMode = ImageExportMode.SingleFilePageByPage,
                        PageRange = (i + 1).ToString(), // تصدير هذه الصفحة فقط
                        Resolution = 150
                    };

                    report.ExportToImage(targetImagePath, options);

                    // جلب الصورة الناتجة من المجلد بغض النظر عن الاسم العشوائي الذي أعطاه لها DevExpress
                    string[] exportedFiles = Directory.GetFiles(pageFolder, "*.png");

                    if (exportedFiles.Length > 0)
                    {
                        string actualImagePath = exportedFiles[0];

                        var slide = presentation.Slides.Add(i + 1, PPT.PpSlideLayout.ppLayoutBlank);
                        slide.Shapes.AddPicture(
                            actualImagePath,
                            MsoTriState.msoFalse,
                            MsoTriState.msoTrue,
                            0, 0, slideWidthPoints, slideHeightPoints
                        );
                    }
                }

                presentation.SaveAs(outputPptxPath, PPT.PpSaveAsFileType.ppSaveAsDefault);
                presentation.Close();
            }
            finally
            {
                if (presentation != null) Marshal.ReleaseComObject(presentation);
                if (pptApp != null)
                {
                    pptApp.Quit();
                    Marshal.ReleaseComObject(pptApp);
                }

                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }
    }
}