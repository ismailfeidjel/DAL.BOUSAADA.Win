using DevExpress.MailClient.Win;
using DevExpress.MailDemo.Win;
using DevExpress.ProductsDemo.Win.Controls;
using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Services;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Modules
{
    public partial class ProjectModule : BaseModule
    {
        public override string ModuleName { get { return Properties.Resources.TasksName; } }

        private readonly LotService _service;
        private List<LotGridModel> _data;
        private LotGridModel _currentLot;
        private LotRepository _lotRepo = new LotRepository();

        public ProjectModule()
        {
            InitializeComponent();
            _service = new LotService();
        }

        // ── Grid Setup ───────────────────────────────────────────────
        private void SetupGrid()
        {
            RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();
            gridControl1.RepositoryItems.Add(memo);
            gridControl1.MainView = gridView1;
            gridView1.OptionsView.ShowGroupPanel = true;
            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsView.ShowIndicator = true;
            gridView1.OptionsSelection.MultiSelect = false;
            gridView1.OptionsSelection.MultiSelectMode =
                DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.RowSelect;
            //------
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            //------
            gridView1.Appearance.HeaderPanel.Font =
    new Font("Tahoma", 8, FontStyle.Bold);

            gridView1.Appearance.HeaderPanel.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Center;

            gridView1.Appearance.HeaderPanel.TextOptions.VAlignment =
                DevExpress.Utils.VertAlignment.Center;
            //-----------------------
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;

            gridView1.Appearance.FocusedRow.BackColor = Color.LightSteelBlue;
            gridView1.Appearance.HideSelectionRow.BackColor = Color.LightSteelBlue;
            //----------------------
            gridView1.BestFitColumns();

            //-----------------------
            gridView1.PaintStyleName = "Skin";
            gridView1.OptionsView.EnableAppearanceEvenRow = true;
            gridView1.OptionsView.EnableAppearanceOddRow = true;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.OptionsView.RowAutoHeight = true;

            //-------------------------

            gridView1.Appearance.EvenRow.BackColor = Color.White;
            gridView1.Appearance.OddRow.BackColor = Color.FromArgb(245, 245, 245);
            AddCol("OperationNumber", "رقم ", 110);
            AddCol("Daira", "الدائرة", 100);
            AddCol("Commune", "البلدية", 100);
            AddCol("OperationName", "اسم العملية", 180);
            AddCol("LotBudget", "مبلغ الحصة", 110, "{0:N2}");
            AddCol("RegisteredAmount", "المبلغ المسجل", 110, "{0:N2}");
            AddCol("ConsumedAmount", "المبلغ المستهلك", 110, "{0:N2}");
            AddCol("PhysicalProgress", "التقدم الفيزيائي", 100, "{0:N2} %");
            AddCol("ProjectStatus", "وضعية العملية", 110);
            AddCol("Notes", "الملاحظة", 150);
            //----------------------------------------------------------------------------
            gridView1.OptionsView.ShowFooter = true;

            gridView1.Columns["LotBudget"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "LotBudget", "{0:N2}");

            gridView1.Columns["ConsumedAmount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "ConsumedAmount", "{0:N2}");

            gridView1.Columns["RegisteredAmount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "RegisteredAmount", "{0:N2}");

                
            gridView1.Columns["Daira"].Summary.Add(
                DevExpress.Data.SummaryItemType.Count,
                "OperationNumber",
                "عددها: {0}");

            gridView1.Appearance.FooterPanel.Font =
    new Font("Tahoma", 8, FontStyle.Bold);

            gridView1.Appearance.FooterPanel.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Center;

            //----------
            gridView1.OptionsView.ShowAutoFilterRow = true;
            gridView1.Columns["OperationNumber"].Width = 50;
            gridView1.Columns["OperationNumber"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["Daira"].Width = 80;
            gridView1.Columns["Daira"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["Commune"].Width = 80;
            gridView1.Columns["Commune"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["PhysicalProgress"].Width = 70;
            gridView1.Columns["PhysicalProgress"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["ProjectStatus"].Width = 80;
            gridView1.Columns["ProjectStatus"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["LotBudget"].Width = 100;
            gridView1.Columns["LotBudget"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["RegisteredAmount"].Width = 100;
            gridView1.Columns["RegisteredAmount"].OptionsColumn.FixedWidth = true;

            gridView1.Columns["ConsumedAmount"].Width = 100;
            gridView1.Columns["ConsumedAmount"].OptionsColumn.FixedWidth = true;
            gridView1.Columns["OperationName"].BestFit();


            //--------
         
            //--------
            gridView1.Columns["OperationNumber"].AppearanceCell.TextOptions.HAlignment =
    DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["Daira"].AppearanceCell.TextOptions.HAlignment =
    DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["Commune"].AppearanceCell.TextOptions.HAlignment =
    DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["PhysicalProgress"].AppearanceCell.TextOptions.HAlignment =
    DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["ProjectStatus"].AppearanceCell.TextOptions.HAlignment =
    DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["LotBudget"].AppearanceCell.TextOptions.HAlignment =
   DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["RegisteredAmount"].AppearanceCell.TextOptions.HAlignment =
   DevExpress.Utils.HorzAlignment.Center;
            gridView1.Columns["ConsumedAmount"].AppearanceCell.TextOptions.HAlignment =
   DevExpress.Utils.HorzAlignment.Center;

            gridView1.Columns["OperationName"].ColumnEdit = memo;
            gridView1.OptionsView.RowAutoHeight = true;

        }

        private void AddCol(string field, string caption, int width, string format = null)
        {
            GridColumn col = gridView1.Columns.AddVisible(field, caption);
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            if (format != null)
            {
                col.DisplayFormat.FormatType = FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
            }
        }

        // ── Init ─────────────────────────────────────────────────────
        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);
            BuildDetailPanel();
            SetupGrid();
            LoadData();
            
        }

        // ── Data ─────────────────────────────────────────────────────
        private void LoadData()
        {
            _data = _lotRepo.GetGridData();
            gridControl1.DataSource = _data;
        }

        // ── Detail Panel ─────────────────────────────────────────────
        private void BuildDetailPanel()
        {
            _detailPanel.Dock = DockStyle.Fill;


        }
        public void LoadLot(LotGridModel lot)
        {


            // Edit tab
            //textBox1.Text = lot.LotName ?? "";
            textBox2.Text = lot.Contractor ?? "";
            textBox3.Text = lot.ExecutionDuration.HasValue ? lot.ExecutionDuration.Value.ToString() : "0";
            textBox4.Text = lot.StartDate.HasValue ? lot.StartDate.Value.ToString("yyyy-MM-dd") : DateTime.Today.ToString("yyyy-MM-dd");
            textBox5.Text = lot.AdministrativeProcedure ?? "";
            textBox6.Text = lot.SpecialStatus1 ?? "";
            textBox7.Text = lot.SpecialStatus2 ?? "";
            textBox8.Text = lot.SpecialStatus3 ?? "";
            textBox9.Text = lot.Notes ?? "";

        }

        private void ShowLotDetails()
        {
            if (gridView1.FocusedRowHandle < 0) return;
            _currentLot = gridView1.GetRow(gridView1.FocusedRowHandle) as LotGridModel;
            if (_currentLot == null) return;
            LoadLot(_currentLot);
        }

      

        // ── Grid Events ──────────────────────────────────────────────
        private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            ShowLotDetails();
        }

        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.ColumnType == typeof(DateTime?))
            {
                DateTime? value = e.Value as DateTime?;
                if (value == null || !value.HasValue)
                    e.DisplayText = Properties.Resources.None;
            }
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle == gridView1.FocusedRowHandle && gridView1.FocusedColumn != e.Column)
            {
                e.Appearance.BackColor = gridView1.PaintAppearance.FocusedRow.BackColor;
                e.Appearance.ForeColor = gridView1.PaintAppearance.FocusedRow.ForeColor;
            }
        }

        // ── BaseModule overrides ─────────────────────────────────────
        protected override DevExpress.XtraGrid.GridControl Grid { get { return gridControl1; } }

        protected override void ShowReminder() { }

        protected internal override void ButtonClick(string tag) { }

        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
            if (firstShow)
            {
                GalleryItem item = OwnerForm.TaskGallery.Groups[0].Items[0];
                item.Checked = true;
                ButtonClick(string.Format("{0}", item.Tag));
            }
        }

        protected override void LookAndFeelStyleChanged()
        {
            base.LookAndFeelStyleChanged();
            ShowReminder();
        }

        internal override void ShowControlFirstTime()
        {
            GridHelper.SetFindControlImages(gridControl1);
        }

        internal override void FocusObject(object obj)
        {
            var view = gridControl1.MainView as DevExpress.XtraGrid.Views.Base.ColumnView;
            if (view != null)
                GridHelper.GridViewFocusObject(view, obj);
        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;

            // Get current project id
            int currentProjectId = Convert.ToInt32(
                view.GetRowCellValue(e.RowHandle, "ProjectId"));

            // Check next row
            bool sameProject = false;

            if (e.RowHandle < view.RowCount - 1)
            {
                int nextProjectId = Convert.ToInt32(
                    view.GetRowCellValue(e.RowHandle + 1, "ProjectId"));

                sameProject = (currentProjectId == nextProjectId);
            }

            // Draw the cell normally
            e.DefaultDraw();

            // Draw bottom line
            Color lineColor = sameProject ? Color.White : Color.Black;
            int thickness = sameProject ? 0 :1;

            using (Pen pen = new Pen(lineColor, thickness))
            {
                e.Cache.DrawLine(
                    pen,
                    new Point(e.Bounds.Left, e.Bounds.Bottom - 1),
                    new Point(e.Bounds.Right, e.Bounds.Bottom - 1)

                );
            }

            e.Handled = true;
        }
    }
}