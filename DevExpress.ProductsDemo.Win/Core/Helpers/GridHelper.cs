using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class GridHelper
    {
        /// <summary>
        /// Applies standard configuration for lookup/CRUD grids.
        /// Row add/delete is handled by the host form's own buttons (btnNew/btnDelete),
        /// so the grid's native add/delete row behavior is disabled by default to avoid
        /// two competing ways of adding/removing rows. Pass allowNativeAddDelete: true
        /// to opt into grid-native inline add/delete instead.
        /// </summary>
        public static void Configure(GridView view, bool allowNativeAddDelete = false)
        {
            if (view == null)
                return;

            // ----------------------------
            // Behavior
            // ----------------------------
            view.OptionsBehavior.Editable = true;

            // Same as ProjectModule: first click focuses/selects the cell only;
            // a second click while already focused opens the editor.
            view.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
            view.OptionsNavigation.EnterMoveNextColumn = true;

            // ----------------------------
            // View
            // ----------------------------
            view.OptionsView.ShowGroupPanel = false;
            view.OptionsView.ShowIndicator = false;
            view.OptionsView.ShowColumnHeaders = true;
            view.OptionsView.ShowFooter = false;
            view.OptionsView.ShowAutoFilterRow = false;
            view.OptionsView.ColumnAutoWidth = true;
            view.OptionsView.EnableAppearanceEvenRow = true;
            view.OptionsView.EnableAppearanceOddRow = true;

            // Fixed taller row height — disable auto-height first, since
            // RowAutoHeight = true would otherwise override a fixed RowHeight.
            view.OptionsView.RowAutoHeight = false;
            view.RowHeight = 32;

            if (allowNativeAddDelete)
                view.OptionsView.NewItemRowPosition = NewItemRowPosition.Top;

            // ----------------------------
            // Selection
            // ----------------------------
            view.OptionsSelection.EnableAppearanceFocusedCell = true;

            // ----------------------------
            // Menu / Find
            // ----------------------------
            view.OptionsMenu.EnableColumnMenu = false;
            view.OptionsFind.AlwaysVisible = false;

            // ----------------------------
            // RTL
            // ----------------------------
            view.GridControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // ----------------------------
            // Navigation
            // ----------------------------
            view.GridControl.UseEmbeddedNavigator = false;

            // ----------------------------
            // Indicator
            // ----------------------------
            view.IndicatorWidth = 45;

            // ----------------------------
            // Appearance
            // ----------------------------
            view.Appearance.HeaderPanel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            view.Appearance.Row.Font = new Font("Segoe UI", 10);
            //view.Appearance.FooterPanel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            view.Appearance.HeaderPanel.TextOptions.HAlignment = HorzAlignment.Center;
            view.Appearance.FocusedRow.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // ----------------------------
            // Center text — header + cells
            // ----------------------------
            foreach (GridColumn col in view.Columns)
            {
                col.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                col.AppearanceCell.TextOptions.VAlignment = VertAlignment.Center;
                col.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
            }

            // ----------------------------
            // Row numbering
            // ----------------------------
            view.CustomDrawRowIndicator -= View_CustomDrawRowIndicator;
            view.CustomDrawRowIndicator += View_CustomDrawRowIndicator;
        }
        private static void View_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (!e.Info.IsRowIndicator)
                return;

            if (e.RowHandle >= 0)
                e.Info.DisplayText = (e.RowHandle + 1).ToString();
        }

        public static void BestFit(GridView view)
        {
            view?.BestFitColumns();
        }

        public static void ReadOnly(GridView view)
        {
            if (view != null)
                view.OptionsBehavior.Editable = false;
        }

        public static void Editable(GridView view)
        {
            if (view != null)
                view.OptionsBehavior.Editable = true;
        }

        public static void HideColumn(GridView view, string fieldName)
        {
            GridColumn column = view?.Columns[fieldName];
            if (column != null)
                column.Visible = false;
        }

        public static void SetCaption(GridView view, string fieldName, string caption)
        {
            GridColumn column = view?.Columns[fieldName];
            if (column != null)
                column.Caption = caption;
        }
    }
}