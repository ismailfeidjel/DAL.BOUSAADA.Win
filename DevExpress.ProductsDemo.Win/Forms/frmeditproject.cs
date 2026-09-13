using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public enum FormMode { Preview, Edit }

    public partial class frmeditproject : RibbonForm
    {
        private List<UCLot> _lotEditors = new List<UCLot>();
        public Domain.Project NewProject { get; private set; }
        private readonly LookupRepository _lookup = new LookupRepository();
        private FormMode _mode;
        private LotGridModel _lot;

        bool HasLots = false;

        private readonly ProjectRepository _projectRepo = new ProjectRepository();
        private readonly LotRepository _lotRepo = new LotRepository();

        public frmeditproject()
        {
            InitializeComponent();
        }

        public frmeditproject(LotGridModel lot, FormMode mode) : this()
        {
            _mode = mode;
            LoadProjectData(lot);
            ApplyMode();
        }

        private void LoadProjectData(LotGridModel sourceLot)
        {
            _lot = sourceLot;

            var lots = _lotRepo.GetByProjectId(sourceLot.ProjectId).OrderBy(l => l.LotNumber).ToList();

            foreach (var tab in tabContainer.TabPages.Where(t => t != tabMain).ToList())
                tabContainer.TabPages.Remove(tab);

            _lotEditors.Clear();

            var lot1 = lots.FirstOrDefault(l => l.LotNumber == 1);
            if (lot1 != null)
            {
                ucLotMain.BindLookups(_lookup);   // ← embedded UCLot instance living on tabMain
                ucLotMain.LoadLot(lot1);
                ucLotMain.ShowProjectFields(true);
                _lotEditors.Add(ucLotMain);
            }

            foreach (var lot in lots.Where(l => l.LotNumber != 1))
                AddLotTab(lot);

            HasLots = lots.Count > 1;
        }

        private void AddLotTab(LotGridModel lot)
        {
            var editor = new UCLot { Dock = DockStyle.Fill };
            editor.BindLookups(_lookup);
            editor.LoadLot(lot);
            editor.ShowProjectFields(false);

            var page = new DevExpress.XtraTab.XtraTabPage { Text = $"الحصة {lot.LotNumber}" };
            page.Controls.Add(editor);

            tabContainer.TabPages.Add(page);
            _lotEditors.Add(editor);
        }

        private void ApplyMode()
        {
            bool editable = _mode == FormMode.Edit;

            foreach (Control c in GetAllControls(this))
            {
                if (c is TextEdit || c is LookUpEdit || c is DateEdit || c is SpinEdit || c is MemoEdit || c is CheckEdit)
                    c.Enabled = editable;
            }

            btnAddLot.Enabled = editable;
            btnRemoveLot.Enabled = editable && _lotEditors.Count > 1;
            btnsave.Enabled = editable;
        }

        private void bbiEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            _mode = FormMode.Edit;
            ApplyMode();
        }

        private IEnumerable<Control> GetAllControls(Control root)
        {
            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (var cc in GetAllControls(c)) yield return cc;
            }
        }

        private bool ValidateAll()
        {
            foreach (var editor in _lotEditors)
            {
                if (!editor.Validate())
                    return false;
            }
            return true;
        }

        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_mode != FormMode.Edit) return;
            if (!ValidateAll()) return;

            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Project fields come from the main lot's editor
                        var project = ucLotMain.GetProjectForSave(_lot.ProjectId);
                        project.HasLots = HasLots;
                        _projectRepo.Update(project, conn, transaction);
                        NewProject = project;

                        // 2. Then every lot (main + extras)
                        foreach (var editor in _lotEditors)
                        {
                            var lotToSave = editor.GetLotForSave();
                            lotToSave.ProjectId = _lot.ProjectId;

                            if (lotToSave.Id == 0)
                                _lotRepo.Insert(lotToSave);
                            else
                                _lotRepo.Update(lotToSave, conn, transaction);
                        }

                        transaction.Commit();
                        DialogResult = DialogResult.OK;
                        XtraMessageBox.Show("تم تعديل بيانات المشروع", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        XtraMessageBox.Show(
                            $"فشل التعديل، تم التراجع عن جميع التغييرات.\n\n{ex.Message}\n{ex.InnerException?.Message}",
                            "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {
            int nextLotNumber = _lotEditors.Count + 1;

            var editor = new UCLot { Dock = DockStyle.Fill };
            editor.BindLookups(_lookup);
            editor.LoadNew(nextLotNumber);
            editor.ShowProjectFields(false);

            var page = new DevExpress.XtraTab.XtraTabPage { Text = $"الحصة {nextLotNumber}" };
            page.Controls.Add(editor);

            tabContainer.TabPages.Add(page);
            _lotEditors.Add(editor);
            tabContainer.SelectedTabPage = page;

            HasLots = _lotEditors.Count > 1;
            btnRemoveLot.Enabled = true;
        }

        private void btnRemoveLot_ItemClick(object sender, ItemClickEventArgs e)
        {
            var currentPage = tabContainer.SelectedTabPage;
            if (currentPage == tabMain) return; // Lot 1 can't be removed this way

            var editor = _lotEditors.FirstOrDefault(ed => ed.Parent == currentPage);
            if (editor == null) return;

            if (!DialogHelper.ConfirmDelete($"الحصة رقم {editor.LotNumber}"))
                return;

            if (editor.LotId.HasValue)
            {
                try { _lotRepo.Delete(editor.LotId.Value); }
                catch (Exception ex) { DialogHelper.DatabaseError(ex); return; }
            }

            _lotEditors.Remove(editor);
            tabContainer.TabPages.Remove(currentPage);

            HasLots = _lotEditors.Count > 1;
            btnAddLot.Enabled = true;
            btnRemoveLot.Enabled = _lotEditors.Count > 1;
        }
    }
}
