using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Core.BaseForms
{
    public abstract partial class frmLookupBase<T> : frmLookupBaseDesignerSurrogate
    where T : class, new()
    {
        protected BindingList<T> DataSource;

        protected bool IsDirty;
        protected abstract List<T> GetData();

        protected abstract void ConfigureColumns();

        protected abstract T CreateNew();

        protected abstract void Save(T entity);

        protected abstract void Delete(T entity);

        protected abstract void Validate(T entity);

        protected virtual string EntityName => "سجل";
        protected frmLookupBase()
        {

            Load += BaseLookupForm_Load;
            gridView.OptionsView.ColumnAutoWidth = true;   // was false — stretches columns to fill the grid's full width
            FormClosing += BaseLookupForm_FormClosing;
            // In frmLookupBase constructor, alongside the other event wiring:
            gridView.CellValueChanged += (s, e) =>
            {
                IsDirty = true;
                UpdateStatus();
            };

            btnNew.Click += btnNew_Click;
            btnSave.Click += btnSave_Click;
            btnDelete.Click += btnDelete_Click;
            btnRefresh.Click += btnRefresh_Click;
            btnClose.Click += btnClose_Click;
            //searchControl.Client = gridControl;
        }

        private void BaseLookupForm_Load(object sender, EventArgs e)
        {
            ConfigureGrid();

            LoadData();
        }
        protected virtual void ConfigureGrid()
        {
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.Configure(gridView);
        }
        protected virtual void LoadData()
        {
            DataSource = new BindingList<T>(GetData());

            DataSource.ListChanged += (_, __) =>
            {
                IsDirty = true;
                UpdateStatus();
            };

            gridControl.DataSource = DataSource;

            ConfigureColumns();

            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.BestFit(gridView);

            IsDirty = false;

            UpdateStatus();
        }
        protected virtual void UpdateStatus()
        {
            lblStatus.Text =
                $"Records : {DataSource?.Count ?? 0}" +
                (IsDirty ? "   *Modified*" : "");
        }
        private void btnNew_Click(object sender, EventArgs e)
        {
            var item = CreateNew();

            DataSource.Add(item);

            gridView.MoveLast();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsDirty)
                return;
            gridView.CloseEditor();
            gridView.UpdateCurrentRow();

            bool ok = DialogHelper.TryExecute(() =>
            {
                foreach (var item in DataSource)
                {
                    Validate(item);// still throws OperationCanceledException after showing its own Validation() message
                        Save(item);
                }
            });

            if (!ok) {

                IsDirty = false;
                LoadData();
                return;
            }

            IsDirty = false;
            DialogHelper.Saved();
            LoadData();

        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (gridView.FocusedRowHandle < 0)
                return;

            var item = gridView.GetFocusedRow() as T;

            if (item == null)
                return;

            if (!DialogHelper.ConfirmDelete(EntityName))
                return;

            Delete(item);

            DataSource.Remove(item);

            UpdateStatus();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (IsDirty)
            {
                if (!DialogHelper.Confirm("Discard unsaved changes?"))
                    return;
            }

            LoadData();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void BaseLookupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDirty)
                return;

            var result = DialogHelper.ConfirmYesNoCancel(
                "Save changes before closing?");

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (result == DialogResult.Yes)
            {
                btnSave.PerformClick();
            }

            // result == DialogResult.No → falls through, form closes.
            // Nothing is persisted, so edits are effectively discarded —
            // but IsDirty/DataSource are left dirty in memory until the form
            // is disposed. Harmless since the form's going away, but let's
            // be explicit for clarity and in case anything else reads IsDirty
            // between here and Dispose.
        }
    }
}