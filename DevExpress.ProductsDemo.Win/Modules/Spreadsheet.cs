using DevExpress.ProductsDemo.Win.Modules.OutgoingMail;
using System;

namespace DevExpress.ProductsDemo.Win.Modules
{
    public partial class SpreadsheetModule : BaseModule
    {

        private readonly IOutgoingMailRepository _repository;


        public SpreadsheetModule()
        {
            InitializeComponent();

            _repository = new OutgoingMailRepository();

            LoadData();
        }

        protected override bool AutoMergeRibbon { get { return true; } }
        public override bool AllowRtfTitle { get { return true; } }
        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
        }
        private void LoadData()
        {
            gridControl1.DataSource = _repository.GetAll();
        }

        private void spreadsheetCommandBarButtonItem2_ItemClick(object sender, XtraBars.ItemClickEventArgs e)
        {

        }

        private void spreadsheetFormulaBarControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
