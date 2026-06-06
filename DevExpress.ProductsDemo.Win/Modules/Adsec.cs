using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Service;
using DevExpress.ProductsDemo.Win.Services;
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
        public override string ModuleName => "Projects";
        private readonly LotService _service;
        public AdsecModule()
        {
            InitializeComponent();
             _service =new LotService();
        }

        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data)
        {
            base.InitModule(manager, data);

            InitGrid();
            LoadData();
        }

        // =========================
        // GRID SETUP
        // =========================
        private void InitGrid()
        {
            gridView1.OptionsDetail.EnableMasterViewMode = true;
            gridView1.OptionsDetail.ShowDetailTabs = false;
            gridView1.OptionsDetail.AllowExpandEmptyDetails = true;

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
            var data = _service.GetGridData();


            gridControl1.DataSource = null;
            gridControl1.DataSource = data;
            gridView1.PopulateColumns();
            gridView1.RefreshData();
            gridControl1.RefreshDataSource();
            if (gridControl1 == null)
                throw new Exception("gridControl1 is NULL");

            if (gridView1 == null)
                throw new Exception("gridView1 is NULL");

            if (_service == null)
                throw new Exception("_service is NULL");

            if (data == null)
                throw new Exception("Data is NULL");
        }

        // =========================
        // MASTER GRID FORMATTING
        // =========================
     
        private void gridView1_RowCellStyle(object sender, XtraGrid.Views.Grid.RowCellStyleEventArgs e)
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
      


        private void gridView1_MasterRowEmpty(
    object sender,
    DevExpress.XtraGrid.Views.Grid.MasterRowEmptyEventArgs e)
        {
            e.IsEmpty = false;
        }

        private void gridView1_MasterRowGetRelationCount(
     object sender,
     DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventArgs e)
        {
            e.RelationCount = 1;
        }

        private void gridView1_MasterRowGetRelationName(
     object sender,
     DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventArgs e)
        {
            e.RelationName = "Details";
        }

        private void gridView1_MasterRowGetChildList(
    object sender,
    DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventArgs e)
        {
            var row =
                gridView1.GetRow(e.RowHandle) as LotGridModel;

            if (row == null)
                return;

            var lot =
                _service.GetById(row.Id);

            e.ChildList = new List<LotDetailModel>
    {
        new LotDetailModel
        {
            Contractor = lot.Contractor,
            ExecutionDuration = lot.ExecutionDuration,
            StartDate = lot.StartDate,
            //AdministrativeProcedure = lot.AdministrativeProcedureName,
            //SpecialStatus1 = lot.SpecialStatus1,
            //SpecialStatus2 = lot.SpecialStatus1,
            //SpecialStatus3 = lot.SpecialStatus1,
            Notes = lot.Notes
        }
    };
        }
    }
}