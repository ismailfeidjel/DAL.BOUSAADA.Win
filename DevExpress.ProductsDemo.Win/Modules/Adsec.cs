using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.Utils;
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
        private ProjectService _service;

        public override string ModuleName => "Projects";

        public AdsecModule()
        {
            InitializeComponent();
        }

        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);

            InitGrid();
            InitMasterDetail();

            _service = new ProjectService(new ProjectRepository());

            LoadData();
        }

        // =========================
        // GRID SETUP
        // =========================
        private void InitGrid()
        {
            gridView1.Columns.Clear();

            gridView1.OptionsBehavior.AutoPopulateColumns = true;
            gridView1.OptionsBehavior.Editable = false;

            gridView1.OptionsView.ShowAutoFilterRow = true;
            gridView1.OptionsView.ShowGroupPanel = true;
        }

        // =========================
        // MASTER DETAIL SETUP
        // =========================
        private void InitMasterDetail()
        {

            gridView1.OptionsDetail.EnableMasterViewMode = true;
            gridView1.OptionsDetail.ShowDetailTabs = false;
            gridView1.OptionsDetail.AllowExpandEmptyDetails = true;
            gridView1.OptionsView.ShowIndicator = true;

            gridView1.MasterRowGetRelationCount += (s, e) =>
            {
                e.RelationCount = 1;
            };

            gridView1.MasterRowGetRelationName += (s, e) =>
            {
                e.RelationName = "Tasks";
            };

            gridView1.MasterRowGetChildList += (s, e) =>
            {
                var project = gridView1.GetRow(e.RowHandle) as AProject;
                MessageBox.Show("FIRED"); // DEBUG

                // VERY IMPORTANT FIX
                if (project != null && project.Tasks != null)
                    e.ChildList = project.Tasks;
                else
                    e.ChildList = new List<ProjectTask>();
            };

            gridView1.MasterRowExpanded += gridView1_MasterRowExpanded;
        }

        // =========================
        // LOAD DATA
        // =========================
        private void LoadData()
        {
            var data = _service.GetAllProjects();

            gridControl1.DataSource = null;
            gridControl1.DataSource = data;
         //   MessageBox.Show(data == null ? "NULL" : $"Count = {data.Count}");

            gridView1.PopulateColumns();

            gridView1.RefreshData();
            gridControl1.RefreshDataSource();

            FormatMasterColumns();
        }

        // =========================
        // MASTER GRID FORMATTING
        // =========================
        private void FormatMasterColumns()
        {

        }

        private void SetCurrencyFormat(string columnName)
        {
            var col = gridView1.Columns[columnName];
            if (col != null)
                col.DisplayFormat.FormatString = "n2";
        }

        // =========================
        // DETAIL GRID (TASKS)
        // =========================
        private void gridView1_MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            GridView detailView = gridView1.GetDetailView(e.RowHandle, e.RelationIndex) as GridView;

            if (detailView == null) return;

            detailView.PopulateColumns();

            detailView.RowCellStyle += DetailView_RowCellStyle;
        }

        private void SetDetailFormat(GridView view, string columnName)
        {
            var col = view.Columns[columnName];
            if (col != null)
                col.DisplayFormat.FormatString = "n2";
        }

        // =========================
        // OPTIONAL: STYLE TASK ROWS
        // =========================
        private void DetailView_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
           
        }

        // =========================
        // BASE MODULE OVERRIDES
        // =========================
        protected override DevExpress.XtraGrid.GridControl Grid => gridControl1;

        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
        }

        internal override void ShowControlFirstTime()
        {
            GridHelper.SetFindControlImages(gridControl1);
        }
    }
}