//using DevExpress.ProductsDemo.Win.Core.Helpers;
//using DevExpress.XtraEditors;
//using DevExpress.XtraGrid;
//using DevExpress.XtraGrid.Views.Grid;
//using System;
//using System.ComponentModel;
//using System.Windows.Forms;

//namespace DevExpress.ProductsDemo.Win.Core.BaseForms
//{
//    public abstract partial class BaseLookupForm<T> : XtraForm
//        where T : class, new()
//    {
//        protected BindingList<T> Data;

//        protected GridControl Grid;
//        protected GridView View;
//        protected SearchControl Search;

//        protected SimpleButton BtnNew;
//        protected SimpleButton BtnSave;
//        protected SimpleButton BtnDelete;
//        protected SimpleButton BtnRefresh;
//        protected SimpleButton BtnClose;

//        protected PanelControl Toolbar;

//        protected bool IsDirty;

//        protected BaseLookupForm()
//        {
//            InitializeForm();
//        }

//        private void InitializeForm()
//        {
//            StartPosition = FormStartPosition.CenterParent;

//            Width = 900;
//            Height = 600;

//            RightToLeft = RightToLeft.Yes;
//            RightToLeftLayout = true;

//            BuildToolbar();
//            BuildGrid();

//            Load += BaseLookupForm_Load;
//            FormClosing += BaseLookupForm_FormClosing;
//        }

//        private void BaseLookupForm_Load(object sender, EventArgs e)
//        {
//            Reload();
//        }

//        protected virtual void Reload()
//        {
//            Data = new BindingList<T>(LoadData());

//            Data.ListChanged += Data_ListChanged;

//            Grid.DataSource = Data;

//            ConfigureColumns();

//           // GridHelper.Configure(View);

//          //  GridHelper.BestFit(View);

//            IsDirty = false;
//        }

//        private void Data_ListChanged(object sender, ListChangedEventArgs e)
//        {
//            IsDirty = true;
//        }
//        private void BuildToolbar()
//        {
//            Toolbar = new PanelControl();

//            Toolbar.Dock = DockStyle.Top;
//            Toolbar.Height = 45;

//            BtnNew = new SimpleButton();
//            BtnSave = new SimpleButton();
//            BtnDelete = new SimpleButton();
//            BtnRefresh = new SimpleButton();
//            BtnClose = new SimpleButton();

//            BtnNew.Text = "جديد";
//            BtnSave.Text = "حفظ";
//            BtnDelete.Text = "حذف";
//            BtnRefresh.Text = "تحديث";
//            BtnClose.Text = "إغلاق";

//            BtnClose.Dock = DockStyle.Left;
//            BtnRefresh.Dock = DockStyle.Left;
//            BtnDelete.Dock = DockStyle.Left;
//            BtnSave.Dock = DockStyle.Left;
//            BtnNew.Dock = DockStyle.Left;

//            Toolbar.Controls.Add(BtnClose);
//            Toolbar.Controls.Add(BtnRefresh);
//            Toolbar.Controls.Add(BtnDelete);
//            Toolbar.Controls.Add(BtnSave);
//            Toolbar.Controls.Add(BtnNew);

//            Controls.Add(Toolbar);

//            BtnNew.Click += BtnNew_Click;
//            BtnSave.Click += BtnSave_Click;
//            BtnDelete.Click += BtnDelete_Click;
//            BtnRefresh.Click += BtnRefresh_Click;
//            BtnClose.Click += BtnClose_Click;
//        }
//        private void BuildGrid()
//        {
//            Search = new SearchControl();

//            Search.Dock = DockStyle.Top;

//            Grid = new GridControl();

//            Grid.Dock = DockStyle.Fill;

//            View = new GridView();

//            Grid.MainView = View;

//            Grid.ViewCollection.Add(View);

//            Search.Client = Grid;

//            Controls.Add(Grid);

//            Controls.Add(Search);
//        }
//        protected virtual void BtnNew_Click(object sender, EventArgs e)
//        {
//            Data.Add(CreateNewItem());
//        }

//        protected virtual void BtnSave_Click(object sender, EventArgs e)
//        {
//            SaveAll();
//        }

//        protected virtual void BtnDelete_Click(object sender, EventArgs e)
//        {
//            DeleteCurrent();
//        }

//        protected virtual void BtnRefresh_Click(object sender, EventArgs e)
//        {
//            Reload();
//        }

//        protected virtual void BtnClose_Click(object sender, EventArgs e)
//        {
//            Close();
//        }
//        protected virtual void SaveAll()
//        {
//            View.CloseEditor();
//            View.UpdateCurrentRow();

//            foreach (T item in Data)
//            {
//                ValidateItem(item);

//                SaveItem(item);
//            }

//            IsDirty = false;

//            DialogHelper.Saved();

//            Reload();
//        }
//        protected virtual void DeleteCurrent()
//        {
//            if (View.FocusedRowHandle < 0)
//                return;

//            T item = View.GetRow(View.FocusedRowHandle) as T;

//            if (item == null)
//                return;

//            if (!DialogHelper.Confirm("Delete selected item?"))
//                return;

//            DeleteItem(item);

//            Data.Remove(item);
//        }
//        private void BaseLookupForm_FormClosing(object sender,
//    FormClosingEventArgs e)
//        {
//            if (!IsDirty)
//                return;

//            DialogResult result =
//                DialogHelper.ConfirmYesNoCancel(
//                    "Save changes?");

//            if (result == DialogResult.Cancel)
//            {
//                e.Cancel = true;
//                return;
//            }

//            if (result == DialogResult.Yes)
//                SaveAll();
//        }
//        protected abstract BindingList<T> CreateBindingList();

//        protected abstract System.Collections.Generic.List<T> LoadData();

//        protected abstract T CreateNewItem();

//        protected abstract void SaveItem(T item);

//        protected abstract void DeleteItem(T item);

//        protected abstract void ConfigureColumns();

//        protected abstract void ValidateItem(T item);
//    }
//}