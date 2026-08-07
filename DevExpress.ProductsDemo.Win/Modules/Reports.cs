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
           // string dbPath = DevExpress.Utils.FilesHelper.FindingFileName(AppDomain.CurrentDomain.BaseDirectory, @"Data\nwind.mdb", false);

            //AppDomain.CurrentDomain.SetData("DataDirectory", Path.GetDirectoryName(dbPath));


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
            SetupTemplatesPanel();
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
      

       

        





 
        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
            if (firstShow)
            {
                reportDesigner1.ContainerControl = this;

                string templatePath = Path.Combine(Application.StartupPath, "Reports", "Templates", "قالب_تقرير_المشاريع.repx");

                if (File.Exists(templatePath))
                {
                    XtraReport report = XtraReport.FromFile(templatePath, true);

                    foreach (XtraReportBase item in report.AllControls<XtraReportBase>())
                    {
                        item.ReportPrintOptions.DetailCountAtDesignTime = 0;
                    }

                    reportDesigner1.OpenReport(report);
                    _currentOpenReport = report;   // ← track it
                }
                else
                {
                    XtraMessageBox.Show(
                        $"القالب غير موجود:\n{templatePath}",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

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