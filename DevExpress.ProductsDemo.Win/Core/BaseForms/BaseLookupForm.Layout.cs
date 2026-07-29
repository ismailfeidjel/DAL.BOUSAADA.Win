//using DevExpress.XtraEditors;
//using DevExpress.XtraLayout;
//using DevExpress.XtraGrid;
//using DevExpress.XtraGrid.Views.Grid;

//namespace DevExpress.ProductsDemo.Win.Core.BaseForms
//{
//    public abstract partial class BaseLookupForm<T> : XtraForm
//        where T : class, new()
//    {
//        protected LayoutControl layout;

//        protected SearchControl searchControl;

//        protected GridControl gridControl;

//        protected GridView gridView;

//        protected SimpleButton btnNew;
//        protected SimpleButton btnSave;
//        protected SimpleButton btnDelete;
//        protected SimpleButton btnRefresh;
//        protected SimpleButton btnClose;

//        private void BuildLayout()
//        {
//            layout = new LayoutControl();

//            layout.Dock = System.Windows.Forms.DockStyle.Fill;

//            Controls.Add(layout);

//            btnNew = new SimpleButton();
//            btnSave = new SimpleButton();
//            btnDelete = new SimpleButton();
//            btnRefresh = new SimpleButton();
//            btnClose = new SimpleButton();

//            btnNew.Text = "New";
//            btnSave.Text = "Save";
//            btnDelete.Text = "Delete";
//            btnRefresh.Text = "Refresh";
//            btnClose.Text = "Close";

//            btnNew.Click += (s, e) => Add();
//            btnSave.Click += (s, e) => Save();
//            btnDelete.Click += (s, e) => Delete();
//            btnRefresh.Click += (s, e) => RefreshData();
//            btnClose.Click += (s, e) => Close();

//            searchControl = new SearchControl();

//            gridControl = new GridControl();

//            gridView = new GridView(gridControl);

//            gridControl.MainView = gridView;

//            searchControl.Client = gridControl;

//            layout.Controls.Add(btnNew);
//            layout.Controls.Add(btnSave);
//            layout.Controls.Add(btnDelete);
//            layout.Controls.Add(btnRefresh);
//            layout.Controls.Add(btnClose);
//            layout.Controls.Add(searchControl);
//            layout.Controls.Add(gridControl);

//            var root = layout.Root;

//            root.EnableIndentsWithoutBorders =
//                DevExpress.Utils.DefaultBoolean.True;

//            root.AddItem("", btnNew);
//            root.AddItem("", btnSave);
//            root.AddItem("", btnDelete);
//            root.AddItem("", btnRefresh);
//            root.AddItem("", btnClose);
//            root.AddItem("", searchControl);
//            root.AddItem("", gridControl);
//        }
//    }
//}