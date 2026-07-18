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
using System.IO;

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
            SetupTemplatesPanel();
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

        private DockPanel _templatesPanel;
        private ListBoxControl _templatesList;

        // Add near your existing SetupTemplatesPanel() in ReportsModule

        private void SetupTemplatesPanel()
        {
            Directory.CreateDirectory(TemplatesFolder);

            DockManager dockManager = this.dockManager1;
            if (dockManager == null) return;

            _templatesPanel = dockManager.AddPanel(DockingStyle.Right);
            _templatesPanel.Text = "القوالب";
            _templatesPanel.Width = 220;
            _templatesPanel.Options.ShowCloseButton = false;

            var container = new Panel { Dock = DockStyle.Fill };

            var btnNew = new SimpleButton { Text = "+ قالب جديد", Dock = DockStyle.Top, Height = 30 };
            btnNew.Click += BtnNewTemplate_Click;

            _templatesList = new ListBoxControl { Dock = DockStyle.Fill };
            _templatesList.DoubleClick += TemplatesList_DoubleClick;

            container.Controls.Add(_templatesList);
            container.Controls.Add(btnNew);
            _templatesPanel.ControlContainer.Controls.Add(container);

            RefreshTemplatesList();
        }

        private void BtnNewTemplate_Click(object sender, EventArgs e)
        {
            using (var dlg = new SimpleInputDialog("قالب جديد", "اسم القالب الجديد:"))
            {
                if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.InputText)) return;

                string name = dlg.InputText;
                string path = Path.Combine(TemplatesFolder, name + ".repx");
                if (File.Exists(path))
                {
                    XtraMessageBox.Show("يوجد قالب بهذا الاسم مسبقاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var blank = new XtraReport
                {
                    PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4,
                    RightToLeft = DevExpress.XtraReports.UI.RightToLeft.Yes,
                    RightToLeftLayout = DevExpress.XtraReports.UI.RightToLeftLayout.Yes
                };
                blank.Bands.Add(new DetailBand { HeightF = 700f });
                blank.SaveLayoutToXml(path);

                RefreshTemplatesList();

                reportDesigner1.ContainerControl = this;
                reportDesigner1.OpenReport(path);
                if (MainRibbon != null && ribbonPagePreview != null)
                    MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByName(ribbonPagePreview.Name);
            }
        }
        private void RefreshTemplatesList()
        {
            if (_templatesList == null) return;

            _templatesList.Items.Clear();
            if (!Directory.Exists(TemplatesFolder)) return;

            foreach (string file in Directory.GetFiles(TemplatesFolder, "*.repx").OrderBy(f => f))
                _templatesList.Items.Add(Path.GetFileNameWithoutExtension(file));
        }

        private void TemplatesList_DoubleClick(object sender, EventArgs e)
        {
            string name = _templatesList.SelectedItem as string;
            if (string.IsNullOrEmpty(name)) return;

            string path = Path.Combine(TemplatesFolder, name + ".repx");
            if (!File.Exists(path))
            {
                XtraMessageBox.Show($"لم يتم العثور على القالب:\n{path}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            reportDesigner1.ContainerControl = this;

            // Open BY PATH (not as a report object) — this is what makes the
            // designer's native Save button write directly back to this file.
            reportDesigner1.OpenReport(path);

            if (MainRibbon != null && ribbonPagePreview != null)
                MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByName(ribbonPagePreview.Name);
        }
        public void SaveReportAsTemplate(XtraReport report, string templateKey)
        {
            Directory.CreateDirectory(TemplatesFolder);
            string path = Path.Combine(TemplatesFolder, templateKey + ".repx");
            report.SaveLayoutToXml(path); // layout only — by design
        }
       

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
    internal class SimpleInputDialog : XtraForm
    {
        public string InputText { get; private set; }

        public SimpleInputDialog(string title, string prompt, string defaultValue = "")
        {
            Text = title;
            Width = 400;
            Height = 160;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            RightToLeftLayout = true;

            var lbl = new LabelControl { Text = prompt, Left = 15, Top = 15, Width = 350 };

            var txt = new TextEdit { Left = 15, Top = 40, Width = 350, Text = defaultValue };

            var btnOk = new SimpleButton { Text = "موافق", Left = 195, Top = 75, Width = 80, DialogResult = DialogResult.OK };
            var btnCancel = new SimpleButton { Text = "إلغاء", Left = 285, Top = 75, Width = 80, DialogResult = DialogResult.Cancel };

            btnOk.Click += (s, e) => { InputText = txt.Text.Trim(); };

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            Controls.Add(lbl);
            Controls.Add(txt);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);

            Shown += (s, e) => txt.Focus();
        }
    }
}