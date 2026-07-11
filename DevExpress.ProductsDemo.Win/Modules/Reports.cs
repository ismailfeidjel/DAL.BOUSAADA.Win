using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraEditors;
using DevExpress.ProductsDemo.Win.Repositories;

namespace DevExpress.ProductsDemo.Win.Modules
{
    public partial class ReportsModule : BaseModule
    {
        private string TemplatesFolder => Path.Combine(Application.StartupPath, "Reports", "Templates");
        private XtraReport _currentOpenReport;
        public XtraReport CurrentOpenReport => _currentOpenReport;

        static ReportsModule()
        {
            string dbPath = DevExpress.Utils.FilesHelper.FindingFileName(AppDomain.CurrentDomain.BaseDirectory, @"Data\nwind.mdb", false);

            AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetDirectoryName(dbPath));


            // Trust the assembly containing report data-source types (LotGridModel, etc.)
            DevExpress.Utils.DeserializationSettings.RegisterTrustedAssembly(typeof(LotGridModel).Assembly);

        }

        // ── Saved Reports Panel ────────────────────────────────────
        private DockPanel _savedReportsPanel;
        private ListBoxControl _savedReportsList;
        private string SavedReportsFolder =>
            Path.Combine(Application.StartupPath, "Reports", "Saved");

        public ReportsModule()
        {
            InitializeComponent();
            SetupSavedReportsPanel();
        }
        private void SetupTemplateSaveButton()
        {
            var btn = new DevExpress.XtraBars.BarButtonItem(ribbonControl1.Manager, "حفظ كقالب");
            btn.ItemClick += (s, e) =>
            {
                XtraReport currentReport = reportDesigner1.ActiveDesignPanel?.Report;
                if (currentReport == null) return;

                SaveReportAsTemplate(currentReport, "قالب_تقرير_المشاريع");
                XtraMessageBox.Show("تم حفظ التعديلات كقالب — سيتم استخدامها في التقارير القادمة.",
                    "تم الحفظ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            ribbonPagePreview.Groups[0].ItemLinks.Add(btn);
        }
        public void SaveReportAsTemplate(XtraReport report, string templateKey)
        {
            Directory.CreateDirectory(TemplatesFolder);
            string path = Path.Combine(TemplatesFolder, templateKey + ".repx");
            report.SaveLayoutToXml(path); // layout only — by design
        }
        public string GetTemplatePath(string templateKey) =>
    Path.Combine(TemplatesFolder, templateKey + ".repx");

        private void SetupSavedReportsPanel()
        {
            Directory.CreateDirectory(SavedReportsFolder);

            // Requires a DockManager on the form. If your designer doesn't have one yet,
            // drop a DockManager from the toolbox onto ReportsModule and rename it dockManager1,
            // OR create one here in code (see fallback below).
            DockManager dockManager = this.dockManager1;
            if (dockManager == null) return;

            _savedReportsPanel = dockManager.AddPanel(DockingStyle.Left);
            _savedReportsPanel.Text = "التقارير المحفوظة"; // "Saved Reports"
            _savedReportsPanel.Width = 220;
            _savedReportsPanel.Options.ShowCloseButton = false;

            _savedReportsList = new ListBoxControl { Dock = DockStyle.Fill };
            _savedReportsList.DoubleClick += SavedReportsList_DoubleClick;
            _savedReportsPanel.ControlContainer.Controls.Add(_savedReportsList);

            RefreshSavedReportsList();
            SetupTemplateSaveButton();
        }

        private void RefreshSavedReportsList()
        {
            if (_savedReportsList == null) return;

            _savedReportsList.Items.Clear();
            if (!Directory.Exists(SavedReportsFolder)) return;

            foreach (string file in Directory.GetFiles(SavedReportsFolder, "*.repx")
                                              .OrderBy(f => f))
            {
                _savedReportsList.Items.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        private void SavedReportsList_DoubleClick(object sender, EventArgs e)
        {
            OpenSavedReportByName(_savedReportsList.SelectedItem as string);
        }



        private void OpenSavedReportByName(string reportName)
        {
            if (string.IsNullOrEmpty(reportName)) return;

            string path = Path.Combine(SavedReportsFolder, reportName + ".repx");
            if (!File.Exists(path))
            {
                XtraMessageBox.Show($"لم يتم العثور على ملف التقرير:\n{path}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                XtraReport report = XtraReport.FromFile(path, true);

                // Layout-only file — rebind current data before showing it
                report.DataSource = new LotRepository().GetGridData();

                OpenExternalReport(report);
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"فشل فتح التقرير:\n\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Call this after generating a report (e.g. from ProjectModule.ExportGridToDesignerReport)
        /// to persist it to disk and add it to the saved-reports panel.
        /// </summary>
        public void SaveReportToPanel(XtraReport report, string reportName)
        {
            if (report == null || string.IsNullOrWhiteSpace(reportName)) return;
            Directory.CreateDirectory(SavedReportsFolder);
            string path = Path.Combine(SavedReportsFolder, reportName + ".repx");
            report.SaveLayoutToXml(path);
            RefreshSavedReportsList();
        }

        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
            if (firstShow)
            {
                reportDesigner1.ContainerControl = this;
                XtraReport report = new DevExpress.ProductsDemo.Win.MasterDetailReport.Report();
                report.ReportPrintOptions.DetailCountAtDesignTime = 0;
                foreach (XtraReportBase item in report.AllControls<XtraReportBase>())
                {
                    item.ReportPrintOptions.DetailCountAtDesignTime = 0;
                }
                reportDesigner1.OpenReport(report);
                _currentOpenReport = report;   // ← track it

                MainRibbon.AutoHideEmptyItems = true;
                MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByText("VIEW");
                MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByName(ribbonPagePreview.Name);
                var reportControl = reportDesigner1.ActiveDesignPanel.GetService(typeof(DevExpress.XtraReports.Design.ReportTabControl)) as DevExpress.XtraReports.Design.ReportTabControl;
                if (reportControl == null || reportControl.PreviewControl == null) return;
                DevExpress.XtraBars.Docking.DockPanel documentMapDockPanel = reportControl.PreviewControl.GetDockPanel(XtraPrinting.Preview.PreviewDockPanelKind.DocumentMap);
                if (documentMapDockPanel != null)
                {
                    documentMapDockPanel.SavedDock = DevExpress.XtraBars.Docking.DockingStyle.Right;
                }
                return;
            }
            MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByName(ribbonPagePreview.Name);
        }

        public void OpenExternalReport(XtraReport report)
        {
            if (report == null) return;
            reportDesigner1.ContainerControl = this;
            reportDesigner1.OpenReport(report);
            _currentOpenReport = report;   // ← track it

            if (MainRibbon != null && ribbonPagePreview != null)
            {
                MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByName(ribbonPagePreview.Name);
            }
        }
        protected override bool AutoMergeRibbon { get { return true; } }
        private void printPreviewBarItem1_ItemClick(object sender, XtraBars.ItemClickEventArgs e)
        {
        }
    }
}