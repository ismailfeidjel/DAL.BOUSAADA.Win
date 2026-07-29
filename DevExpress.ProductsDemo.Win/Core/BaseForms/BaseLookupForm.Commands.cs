//using DevExpress.XtraEditors;
//using DevExpress.XtraGrid.Columns;

//namespace DevExpress.ProductsDemo.Win.Core.BaseForms
//{
//    public abstract partial class BaseLookupForm<T> : XtraForm
//        where T : class, new()
//    {
//        protected void HideColumn(string field)
//        {
//            if (gridView.Columns[field] != null)
//                gridView.Columns[field].Visible = false;
//        }

//        protected void Caption(string field, string caption)
//        {
//            if (gridView.Columns[field] != null)
//                gridView.Columns[field].Caption = caption;
//        }

//        protected void Width(string field, int width)
//        {
//            if (gridView.Columns[field] != null)
//                gridView.Columns[field].Width = width;
//        }

//        protected void ReadOnly(string field)
//        {
//            GridColumn col = gridView.Columns[field];

//            if (col != null)
//                col.OptionsColumn.AllowEdit = false;
//        }

//        protected void RefreshGrid()
//        {
//            gridControl.RefreshDataSource();
//        }

//        protected T Current
//        {
//            get
//            {
//                return gridView.GetFocusedRow() as T;
//            }
//        }
//    }
//}