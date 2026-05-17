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

        public override string ModuleName => "AProjects";

        public AdsecModule()
        {
            InitializeComponent();
        }

        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);

            InitGrid();
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
        // LOAD DATA
        // =========================
        private void LoadData()
        {
            var data = _service.GetAllProjects();

            gridControl1.DataSource = null;
            gridControl1.DataSource = data;
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
        private void gridView1_RowCellStyle(object sender, XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            if (e.RowHandle == gridView1.FocusedRowHandle && gridView1.FocusedColumn != e.Column)
            {
                e.Appearance.BackColor = gridView1.PaintAppearance.FocusedRow.BackColor;
                e.Appearance.ForeColor = gridView1.PaintAppearance.FocusedRow.ForeColor;
            }
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