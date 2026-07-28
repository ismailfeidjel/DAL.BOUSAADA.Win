using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using System.Drawing;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class GridHelper
    {
        public static void Configure(GridView view)
        {
            if (view == null)
                return;

            // ----------------------------
            // Behavior
            // ----------------------------
            view.OptionsBehavior.Editable = true;
            view.OptionsBehavior.AllowAddRows = DefaultBoolean.True;
            view.OptionsBehavior.AllowDeleteRows = DefaultBoolean.True;
            view.OptionsBehavior.EditorShowMode = EditorShowMode.Click;

            // ----------------------------
            // View
            // ----------------------------
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ShowIndicator = true;
            view.OptionsView.ShowColumnHeaders = true;
            view.OptionsView.ShowFooter = true;
            view.OptionsView.ShowAutoFilterRow = true;

            view.OptionsView.ColumnAutoWidth = false;

            view.OptionsView.EnableAppearanceEvenRow = true;
            view.OptionsView.EnableAppearanceOddRow = true;

            view.OptionsView.NewItemRowPosition =
                NewItemRowPosition.Top;

            // ----------------------------
            // Selection
            // ----------------------------
            view.OptionsSelection.EnableAppearanceFocusedCell = false;
            view.OptionsSelection.MultiSelect = true;
            view.OptionsSelection.MultiSelectMode =
                GridMultiSelectMode.RowSelect;

            // ----------------------------
            // Navigation
            // ----------------------------
            view.OptionsNavigation.EnterMoveNextColumn = true;

            // ----------------------------
            // Menu
            // ----------------------------
            view.OptionsMenu.EnableColumnMenu = true;

            // ----------------------------
            // Find
            // ----------------------------
            view.OptionsFind.AlwaysVisible = false;

            // ----------------------------
            // Indicator
            // ----------------------------
            view.IndicatorWidth = 45;

            // ----------------------------
            // Appearance
            // ----------------------------
            view.Appearance.HeaderPanel.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            view.Appearance.Row.Font =
                new Font("Segoe UI", 10);

            view.Appearance.FooterPanel.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            view.Appearance.HeaderPanel.TextOptions.HAlignment =
                HorzAlignment.Center;

            view.Appearance.FocusedRow.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            // ----------------------------
            // Events
            // ----------------------------
            view.CustomDrawRowIndicator -= View_CustomDrawRowIndicator;
            view.CustomDrawRowIndicator += View_CustomDrawRowIndicator;
        }

        private static void View_CustomDrawRowIndicator(
            object sender,
            RowIndicatorCustomDrawEventArgs e)
        {
            if (!e.Info.IsRowIndicator)
                return;

            if (e.RowHandle >= 0)
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        public static void BestFit(GridView view)
        {
            if (view == null)
                return;

            view.BestFitColumns();
        }

        public static void ReadOnly(GridView view)
        {
            view.OptionsBehavior.Editable = false;
        }

        public static void Editable(GridView view)
        {
            view.OptionsBehavior.Editable = true;
        }

        public static void HideColumn(GridView view, string fieldName)
        {
            GridColumn column = view.Columns[fieldName];

            if (column != null)
                column.Visible = false;
        }

        public static void SetCaption(
            GridView view,
            string fieldName,
            string caption)
        {
            GridColumn column = view.Columns[fieldName];

            if (column != null)
                column.Caption = caption;
        }
    }
}