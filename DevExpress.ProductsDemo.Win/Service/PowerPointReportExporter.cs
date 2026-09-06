using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using MsoTriState = Microsoft.Office.Core.MsoTriState;
using PPT = Microsoft.Office.Interop.PowerPoint;
using System.Drawing;
using System.Linq;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraEditors;
using System.Windows.Forms;

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
            PPT.Presentations presentations = null;
            PPT.Presentation presentation = null;
            PPT.PageSetup pageSetup = null;

            try
            {
                pptApp = new PPT.Application();
                presentations = pptApp.Presentations;
                presentation = presentations.Add(MsoTriState.msoFalse);
                pageSetup = presentation.PageSetup;

                // 1. Keep Report Height, calculate Widescreen 16:9 Width
                float slideHeightPoints = (report.PageHeight / 100f) * 72f;
                float slideWidthPoints = slideHeightPoints * (16f / 9f);

                pageSetup.SlideWidth = slideWidthPoints;
                pageSetup.SlideHeight = slideHeightPoints;

                // 2. Loop through pages using your isolated folder technique
                for (int i = 0; i < pageCount; i++)
                {
                    // Create an isolated folder for this specific page
                    string pageFolder = Path.Combine(tempFolder, $"page_{i + 1}");
                    Directory.CreateDirectory(pageFolder);

                    string targetImagePath = Path.Combine(pageFolder, "slide.png");

                    ImageExportOptions options = new ImageExportOptions
                    {
                        Format = System.Drawing.Imaging.ImageFormat.Png,
                        ExportMode = ImageExportMode.SingleFilePageByPage,
                        PageRange = (i + 1).ToString(), // Export this specific page
                        Resolution = 150
                    };

                    report.ExportToImage(targetImagePath, options);

                    // Grab whatever file DevExpress actually generated inside this folder
                    string[] exportedFiles = Directory.GetFiles(pageFolder, "*.png");

                    if (exportedFiles.Length > 0)
                    {
                        string actualImagePath = exportedFiles[0];

                        PPT.Slides slides = null;
                        PPT.Slide slide = null;
                        PPT.Shapes shapes = null;
                        PPT.Shape picture = null;

                        try
                        {
                            slides = presentation.Slides;
                            slide = slides.Add(i + 1, PPT.PpSlideLayout.ppLayoutBlank);
                            shapes = slide.Shapes;

                            // 3. Stretch the image horizontally to fit the 16:9 screen
                            picture = shapes.AddPicture(
                                actualImagePath,
                                MsoTriState.msoFalse,
                                MsoTriState.msoTrue,
                                0, 0, slideWidthPoints, slideHeightPoints
                            );
                        }
                        finally
                        {
                            // Release slide-level COM objects immediately
                            if (picture != null) Marshal.ReleaseComObject(picture);
                            if (shapes != null) Marshal.ReleaseComObject(shapes);
                            if (slide != null) Marshal.ReleaseComObject(slide);
                            if (slides != null) Marshal.ReleaseComObject(slides);
                        }
                    }
                }

                presentation.SaveAs(outputPptxPath, PPT.PpSaveAsFileType.ppSaveAsDefault);
            }
            finally
            {
                // 4. Safely clean up Application-level COM objects
                if (pageSetup != null) Marshal.ReleaseComObject(pageSetup);
                if (presentation != null)
                {
                    presentation.Close();
                    Marshal.ReleaseComObject(presentation);
                }
                if (presentations != null) Marshal.ReleaseComObject(presentations);

                if (pptApp != null)
                {
                    pptApp.Quit();
                    Marshal.ReleaseComObject(pptApp);
                }

                // Force Garbage Collection to catch stray Interop wrappers
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Clean up the temporary directory
                if (Directory.Exists(tempFolder))
                {
                    Directory.Delete(tempFolder, true);
                }
            }
        }
        private static void EnsureDocument(XtraReport report)
        {
            if (report.PrintingSystem == null || report.PrintingSystem.Pages.Count == 0)
                report.CreateDocument();
        }

        public static void ShowPreviewWithPowerPointButton(XtraReport report)
        {
            if (report == null) throw new ArgumentNullException(nameof(report));
            EnsureDocument(report);

            report.PrintingSystem.SetCommandVisibility(
        new[] { PrintingSystemCommand.Save, PrintingSystemCommand.Open },
        CommandVisibility.None);
            using (var printTool = new ReportPrintTool(report))
            {
                PrintPreviewRibbonFormEx form = printTool.PreviewRibbonForm;
                form.Load += (s, e) =>
                {
                    RibbonControl ribbon = form.Ribbon;
                    RibbonBarManager manager = ribbon.Manager;
                    RibbonPageGroup exportGroup = null;
                    int exportIndex = -1;
                    // 1. Make every built-in command a small icon and locate the "Export To" button
                    foreach (RibbonPage page in ribbon.Pages)
                    {
                        foreach (RibbonPageGroup group in page.Groups)
                        {
                            for (int i = 0; i < group.ItemLinks.Count; i++)
                            {
                                BarItemLink link = group.ItemLinks[i];
                                //link.Item.RibbonStyle = RibbonItemStyles.Default;
                                var previewItem = link.Item as PrintPreviewBarItem;
                                if (previewItem != null &&
                                    previewItem.Command == PrintingSystemCommand.ExportFile)
                                {
                                    exportGroup = group;
                                    exportIndex = i;
                                }
                            }
                        }
                    }
                    // 2. Create the PowerPoint button
                    var btnPptx = new BarButtonItem(manager, "PowerPoint")
                    {
                        RibbonStyle = RibbonItemStyles.Default,
                        Hint = " Export to PowerPoint (.pptx)"
                    };
                    // Use ONE of these two lines depending on your resource type:
                    btnPptx.ImageOptions.SvgImage = Properties.Resources.Overdue;   // SVG resource
                    // btnPptx.ImageOptions.Image = Properties.Resources.pptx_16;   // PNG/BMP resource
                    btnPptx.ImageOptions.SvgImageSize = new Size(16, 16);
                    btnPptx.ItemClick += (sender, args) => ExportWithDialog(report);
                    // 3. Insert next to "Export To", or fall back to the first group
                    if (exportGroup != null)
                        exportGroup.ItemLinks.Insert(exportIndex + 1, btnPptx);
                    else if (ribbon.Pages.Count > 0 && ribbon.Pages[0].Groups.Count > 0)
                        ribbon.Pages[0].Groups[0].ItemLinks.Add(btnPptx);
                };
                printTool.ShowRibbonPreviewDialog();
            }
        }
        private static void ExportWithDialog(XtraReport report)
        {
            using (var dialog = new SaveFileDialog
            {
                Filter = "PowerPoint Presentation (*.pptx)|*.pptx",
                Title = "تصدير التقرير إلى عرض تقديمي",
                FileName = "تقرير.pptx",
                DefaultExt = "pptx",
                AddExtension = true
            })
            {
                if (dialog.ShowDialog() != DialogResult.OK) return;
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ExportReportToPptx(report, dialog.FileName);   // your existing method
                    Cursor.Current = Cursors.Default;
                    if (XtraMessageBox.Show("تم التصدير بنجاح. هل تريد فتح الملف؟", "تصدير PowerPoint",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(dialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    Cursor.Current = Cursors.Default;
                    XtraMessageBox.Show("فشل التصدير إلى PowerPoint.\n\n" + ex.Message, "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        }
}