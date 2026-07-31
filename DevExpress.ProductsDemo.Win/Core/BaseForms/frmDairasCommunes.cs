using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmDairasCommunes : XtraForm
    {
        private readonly LookupRepository _lookupRepo = new LookupRepository();
        private readonly CommuneRepository _communeRepo = new CommuneRepository();

        private BindingList<LookupItem> _dairas;
        private BindingList<CommuneItem> _communes;

        private bool _dairasDirty;
        private bool _communesDirty;
        private int? _selectedDairaId;

        public frmDairasCommunes()
        {
            InitializeComponent();
            Load += (s, e) => LoadDairas();
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.Configure(viewDairas);
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.Configure(viewCommunes);

        }

        // ── Dairas ───────────────────────────────────────────────────
        private void LoadDairas()
        {
            _dairas = new BindingList<LookupItem>(_lookupRepo.GetAll("dairas"));
            gridDairas.DataSource = _dairas;

            GridColumn colId = viewDairas.Columns["Id"];
            if (colId != null) { colId.OptionsColumn.AllowEdit = false; colId.Visible = false; }
            Core.Helpers.GridHelper.SetCaption(viewDairas, "Name", "الاسم");

            _dairasDirty = false;
        }

        private void viewDairas_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            var daira = viewDairas.GetFocusedRow() as LookupItem;
            if (daira == null || daira.Id == 0)
            {
                // daira.Id == 0 means it's a new, unsaved row — no real FK target yet
                _selectedDairaId = null;
                gridCommunes.DataSource = null;
                lblCommunesTitle.Text = daira == null
                    ? "البلديات — اختر دائرة"
                    : "البلديات — احفظ الدائرة أولاً";
                SetCommuneButtonsEnabled(false);
                return;
            }

            _selectedDairaId = daira.Id;
            lblCommunesTitle.Text = $"البلديات — {daira.Name}";
            SetCommuneButtonsEnabled(true);
            LoadCommunes(daira.Id);
        }
        private void viewDairas_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            _dairasDirty = true;
        }

        private void btnNewDaira_Click(object sender, EventArgs e)
        {
            var item = new LookupItem(0, "");
            _dairas.Add(item);
            viewDairas.MoveLast();
            _dairasDirty = true;
        }

        private void btnSaveDaira_Click(object sender, EventArgs e)
        {
            if (!_dairasDirty) return;

            viewDairas.CloseEditor();
            viewDairas.UpdateCurrentRow();

            bool ok = DialogHelper.TryExecute(() =>
            {
                foreach (var item in _dairas)
                {
                    bool isUnique = !_dairas.Any(x =>
                        x.Id != item.Id &&
                        string.Equals(x.Name?.Trim(), item.Name?.Trim(), StringComparison.OrdinalIgnoreCase));

                    string error = ValidationHelper.FirstError(
                        (ValidationHelper.Required(item.Name), "الاسم مطلوب."),
                        (isUnique, "يوجد دائرة أخرى بنفس الاسم.")
                    );

                    if (!string.IsNullOrEmpty(error))
                    {
                        DialogHelper.Validation(error);
                        throw new SilentCancelException();
                    }

                    if (item.Id == 0)
                        _lookupRepo.Insert("dairas", item.Name);
                    else
                        _lookupRepo.Update("dairas", item.Id, item.Name);
                }
            });

            if (!ok) return;

            DialogHelper.Saved();
            LoadDairas();
        }

        private void btnDeleteDaira_Click(object sender, EventArgs e)
        {
            var daira = viewDairas.GetFocusedRow() as LookupItem;
            if (daira == null) return;

            bool hasCommunes = _communeRepo.GetAll().Any(c => c.DairaId == daira.Id);
            if (hasCommunes)
            {
                DialogHelper.Warning(
                    $"لا يمكن حذف \"{daira.Name}\" لوجود بلديات مرتبطة بها. احذف البلديات أولاً.",
                    "تعذر الحذف");
                return;
            }

            if (!DialogHelper.ConfirmDelete(daira.Name)) return;

            _lookupRepo.Delete("dairas", daira.Id);
            _dairas.Remove(daira);
        }

        // ── Communes (scoped to selected daira) ─────────────────────
        private void LoadCommunes(int dairaId)
        {
            var all = _communeRepo.GetAll();
            _communes = new BindingList<CommuneItem>(all.Where(c => c.DairaId == dairaId).ToList());
            gridCommunes.DataSource = _communes;

            GridColumn colId = viewCommunes.Columns["Id"];
            if (colId != null) { colId.OptionsColumn.AllowEdit = false; colId.Visible = false; }
            GridColumn colDairaId = viewCommunes.Columns["DairaId"];
            if (colDairaId != null) { colDairaId.OptionsColumn.AllowEdit = false; colDairaId.Visible = false; }
            Core.Helpers.GridHelper.SetCaption(viewCommunes, "Name", "الاسم");

            _communesDirty = false;
        }

        private void SetCommuneButtonsEnabled(bool enabled)
        {
            btnNewCommune.Enabled = enabled;
            btnSaveCommune.Enabled = enabled;
            btnDeleteCommune.Enabled = enabled;
        }

        private void btnNewCommune_Click(object sender, EventArgs e)
        {
            if (_selectedDairaId == null || _selectedDairaId == 0)
            {
                DialogHelper.Warning("الرجاء حفظ الدائرة أولاً قبل إضافة بلدية.", "تنبيه");
                return;
            }

            var item = new CommuneItem(0, "", _selectedDairaId.Value);
            _communes.Add(item);
            viewCommunes.MoveLast();
            _communesDirty = true;
        }

        private void viewCommunes_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            _communesDirty = true;
        }

        private void btnSaveCommune_Click(object sender, EventArgs e)
        {
            if (!_communesDirty || _selectedDairaId == null) return;

            viewCommunes.CloseEditor();
            viewCommunes.UpdateCurrentRow();

            bool ok = DialogHelper.TryExecute(() =>
            {
                foreach (var item in _communes)
                {
                    bool isUnique = !_communes.Any(x =>
                        x.Id != item.Id &&
                        string.Equals(x.Name?.Trim(), item.Name?.Trim(), StringComparison.OrdinalIgnoreCase));

                    string error = ValidationHelper.FirstError(
                        (ValidationHelper.Required(item.Name), "الاسم مطلوب."),
                        (isUnique, "يوجد بلدية أخرى بنفس الاسم في هذه الدائرة.")
                    );

                    if (!string.IsNullOrEmpty(error))
                    {
                        DialogHelper.Validation(error);
                        throw new SilentCancelException();
                    }

                    if (item.Id == 0)
                        _communeRepo.Insert(item);
                    else
                        _communeRepo.Update(item);
                }
            });

            if (!ok) return;

            DialogHelper.Saved();
            LoadCommunes(_selectedDairaId.Value);
        }

        private void btnDeleteCommune_Click(object sender, EventArgs e)
        {
            var commune = viewCommunes.GetFocusedRow() as CommuneItem;
            if (commune == null) return;

            bool hasProjects = new ProjectRepository().GetAll().Any(p => p.CommuneId == commune.Id);
            if (hasProjects)
            {
                DialogHelper.Warning(
                    $"لا يمكن حذف \"{commune.Name}\" لوجود عمليات مرتبطة بها.",
                    "تعذر الحذف");
                return;
            }

            if (!DialogHelper.ConfirmDelete(commune.Name)) return;

            _communeRepo.Delete(commune.Id);
            _communes.Remove(commune);
        }

        // ── Closing ──────────────────────────────────────────────────
        private void frmDairasCommunes_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_dairasDirty && !_communesDirty) return;

            var result = DialogHelper.ConfirmYesNoCancel("توجد تغييرات غير محفوظة. هل تريد الحفظ قبل الإغلاق؟");

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (result == DialogResult.Yes)
            {
                if (_dairasDirty) btnSaveDaira_Click(sender, e);
                if (_communesDirty) btnSaveCommune_Click(sender, e);
            }
        }
    }
}