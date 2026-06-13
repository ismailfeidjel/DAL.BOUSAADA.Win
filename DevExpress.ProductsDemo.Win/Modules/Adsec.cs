using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Service;
using DevExpress.ProductsDemo.Win.Services;
using DevExpress.ProductsDemo.Win.UI;
using DevExpress.Utils;
using DevExpress.XtraGrid;
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
    public partial class AdsecModule : BaseModule
    {
        public override string ModuleName => "Projects";
        private readonly LotService _service;

        private List<LotGridModel> _data;

        public AdsecModule()
        {
            InitializeComponent();
            _service = new LotService();
        }

        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);

            MessageBox.Show("InitModule called");

            SetupGrid();
            MessageBox.Show("SetupGrid done");

            LoadData();
            MessageBox.Show("LoadData done. Rows=" + (_data?.Count ?? -1));

            SetupMasterDetail();
            MessageBox.Show("SetupMasterDetail done");

            this.BeginInvoke(new Action(() =>
            {
                MessageBox.Show(
                    "Delayed check\n" +
                    "ShowIndicator: " + gridView2.OptionsView.ShowIndicator +
                    "\nShowDetailButtons: " + gridView2.OptionsView.ShowDetailButtons +
                    "\nLevels: " + gridControl2.LevelTree.Nodes.Count +
                    "\nRowCount: " + gridView2.RowCount +
                    "\nIndicatorWidth: " + gridView2.IndicatorWidth);
            }));
        }

        // =========================
        // GRID SETUP
        // =========================
        private void SetupGrid()
        {
            gridControl2.MainView = gridView2;
            gridControl2.Dock = DockStyle.Fill;

            gridView2.OptionsView.ShowGroupPanel = false;
            gridView2.OptionsBehavior.Editable = false;
            gridView2.OptionsView.ShowIndicator = true;
            gridView2.OptionsView.ShowDetailButtons = true;

            AddCol("OperationNumber", "رقم العملية", 110);
            AddCol("OperationName", "اسم العملية", 180);
            AddCol("Daira", "الدائرة", 100);
            AddCol("Commune", "البلدية", 100);
            AddCol("LotBudget", "مبلغ الحصة", 110, "{0:N2}");
            AddCol("RegisteredAmount", "المبلغ المسجل", 110, "{0:N2}");
            AddCol("ConsumedAmount", "المبلغ المستهلك", 110, "{0:N2}");
            AddCol("PhysicalProgress", "التقدم الفيزيائي", 100, "{0:N2} %");
            AddCol("ProjectStatus", "وضعية العملية", 110);
            AddCol("Notes", "الملاحظة", 150);
        }

        private void AddCol(string field, string caption, int width, string format = null)
        {
            GridColumn col = gridView2.Columns.AddVisible(field, caption);
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;

            if (format != null)
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
            }
        }

        // =========================
        // MASTER-DETAIL (native GridView, label/value rows)
        // =========================
        private void SetupMasterDetail()
        {
            gridView2.MasterRowGetChildList += GridView2_MasterRowGetChildList;
            gridView2.MasterRowGetRelationName += GridView2_MasterRowGetRelationName;
            gridView2.MasterRowGetRelationCount += GridView2_MasterRowGetRelationCount;

            var detailView = CreateDetailView();

            gridControl2.LevelTree.Nodes.Add(
                new GridLevelNode(gridControl2.LevelTree, "LotDetail", detailView));

            MessageBox.Show(
                "Levels: " + gridControl2.LevelTree.Nodes.Count +
                "\nDetailView columns: " + detailView.Columns.Count +
                "\nData rows: " + (_data?.Count ?? -1));
        }

        private GridView CreateDetailView()
        {
            var view = new GridView(gridControl2);
            view.Name = "DetailView";
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ShowIndicator = false;
            view.OptionsView.ShowColumnHeaders = true;
            view.OptionsBehavior.Editable = false;
            view.OptionsNavigation.AutoFocusNewRow = false;

            var colField = view.Columns.AddVisible("Field", "الحقل");
            colField.Width = 150;
            colField.OptionsColumn.AllowEdit = false;

            var colValue = view.Columns.AddVisible("Value", "القيمة");
            colValue.Width = 300;
            colValue.OptionsColumn.AllowEdit = false;

            gridControl2.ViewCollection.Add(view);

            return view;
        }

        private void GridView2_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void GridView2_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "LotDetail";
        }

        private void GridView2_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
        {
            var view = (GridView)sender;
            var lot = view.GetRow(e.RowHandle) as LotGridModel;

            MessageBox.Show("ChildList called. RowHandle=" + e.RowHandle + " lot null? " + (lot == null));

            if (lot == null)
            {
                e.ChildList = new List<LotDetailRow>();
                return;
            }

            e.ChildList = new List<LotDetailRow>
            {
                new LotDetailRow("اسم الحصة", lot.LotName ?? "-"),
                new LotDetailRow("رقم الحصة", lot.LotNumber.ToString()),
                new LotDetailRow("المقاول", string.IsNullOrEmpty(lot.Contractor) ? "-" : lot.Contractor),
                new LotDetailRow("مدة الإنجاز", lot.ExecutionDuration.HasValue ? lot.ExecutionDuration.Value + " يوم" : "-"),
                new LotDetailRow("تاريخ الانطلاق", lot.StartDate.HasValue ? lot.StartDate.Value.ToString("yyyy-MM-dd") : "-"),
                new LotDetailRow("التقدم المالي", lot.FinancialProgress.ToString("N2") + " %"),
                new LotDetailRow("الباقي", lot.Remaining.ToString("N2")),
                new LotDetailRow("الإجراء الإداري", string.IsNullOrEmpty(lot.AdministrativeProcedure) ? "-" : lot.AdministrativeProcedure),
                new LotDetailRow("الوضعية الخاصة 1", string.IsNullOrEmpty(lot.SpecialStatus1) ? "-" : lot.SpecialStatus1),
                new LotDetailRow("الوضعية الخاصة 2", string.IsNullOrEmpty(lot.SpecialStatus2) ? "-" : lot.SpecialStatus2),
                new LotDetailRow("الوضعية الخاصة 3", string.IsNullOrEmpty(lot.SpecialStatus3) ? "-" : lot.SpecialStatus3),
            };
        }

        // =========================
        // LOAD DATA
        // =========================
        private void LoadData()
        {
            var repo = new LotRepository();
            _data = repo.GetGridData();
            gridControl2.DataSource = _data;
        }

        // =========================
        // BASE MODULE OVERRIDES
        // =========================
        protected override GridControl Grid => gridControl2;

        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
        }

        internal override void ShowControlFirstTime()
        {
            GridHelper.SetFindControlImages(gridControl2);

            MessageBox.Show(
                "ShowControlFirstTime\n" +
                "ShowIndicator: " + gridView2.OptionsView.ShowIndicator +
                "\nShowDetailButtons: " + gridView2.OptionsView.ShowDetailButtons +
                "\nLevels: " + gridControl2.LevelTree.Nodes.Count +
                "\nRowCount: " + gridView2.RowCount);
        }
    }
}
