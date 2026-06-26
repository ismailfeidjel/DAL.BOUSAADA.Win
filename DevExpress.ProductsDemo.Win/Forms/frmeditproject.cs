using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.MailClient.Win;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using Mysqlx.Crud;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public enum FormMode { Preview, Edit }

    public partial class frmeditproject : RibbonForm
    {
        public Domain.Project NewProject { get; private set; }
        private readonly LookupRepository _lookup = new LookupRepository();
        private FormMode _mode;
        private LotGridModel _lot; // store current lot

        public Lot NewLot { get; private set; }
        public Lot NewLot2 { get; private set; }
        public Lot NewLot3 { get; private set; }

        bool HasLots = false;

        private readonly ProjectRepository _projectRepo = new ProjectRepository();
        private readonly LotRepository _lotRepo = new LotRepository();

        public frmeditproject()
        {
            InitializeComponent();
            PopulateLookups();


        }
        public frmeditproject(LotGridModel lot, FormMode mode) : this()
        {
            _mode = mode;
            LoadProjectData(lot);
            ApplyMode();
        }

        private void LoadProjectData( LotGridModel sourceLot)
        {
            // ── Project fields from LotGridModel ─────────────────────
            txtOperationNumber.Text = sourceLot.OperationNumber ?? "";
            txtOperationName.Text = sourceLot.OperationName ?? "";
            cmbDaira.EditValue = sourceLot.DairaId;
            cmbCommune.EditValue = sourceLot.CommuneId;
            cmbDomain.EditValue = sourceLot.DomainId;
            cmbSector.EditValue = sourceLot.SectorId;

            // ── Lots from database ────────────────────────────────────
            var lots = _lotRepo.GetByProjectId(sourceLot.ProjectId);

            var lot1 = lots.FirstOrDefault(l => l.LotNumber == 1);
            if (lot1 != null) LoadLot1(lot1);

            var lot2 = lots.FirstOrDefault(l => l.LotNumber == 2);
            if (lot2 != null) { tabLot2.PageVisible = true; LoadLot2(lot2); }

            var lot3 = lots.FirstOrDefault(l => l.LotNumber == 3);
            if (lot3 != null) { tabLot3.PageVisible = true; LoadLot3(lot3); }
        }


        
        private void LoadLot1(LotGridModel lot)
        {
            txtLotName.Text = lot.LotName ?? "";
            txtContractor.Text = lot.Contractor ?? "";
            txtLotBudget.Text = lot.LotBudget.ToString();
            txtRegisteredAmount.Text = lot.RegisteredAmount.ToString();
            txtConsumedAmount.Text = lot.ConsumedAmount.ToString();
            spnExecutionDuration.Value = lot.ExecutionDuration ?? 0;
            dtStartDate.EditValue = lot.StartDate;
            spnPhysicalProgress.Value = lot.PhysicalProgress;
            cmbAdminProcedure.EditValue = lot.AdministrativeProcedureId;
            cmbSpecialStatus1.EditValue = lot.SpecialStatus1Id;
            cmbSpecialStatus2.EditValue = lot.SpecialStatus2Id;
            cmbSpecialStatus3.EditValue = lot.SpecialStatus3Id;
            cmbProjectStatus.EditValue = lot.ProjectStatusId;
            txtLotNotes.Text = lot.Notes ?? "";
        }

        private void LoadLot2(LotGridModel lot)
        {
            textEdit2.Text = lot.LotName ?? "";
            textEdit4.Text = lot.Contractor ?? "";
            textEdit6.Text = lot.LotBudget.ToString();
            textEdit5.Text = lot.RegisteredAmount.ToString();
            textEdit7.Text = lot.ConsumedAmount.ToString();
            spinEdit2.Value = lot.ExecutionDuration ?? 0;
            dateEdit1.EditValue = lot.StartDate;
            spinEdit3.Value = lot.PhysicalProgress;
            lookUpEdit7.EditValue = lot.AdministrativeProcedureId;
            lookUpEdit8.EditValue = lot.SpecialStatus1Id;
            lookUpEdit6.EditValue = lot.SpecialStatus2Id;
            lookUpEdit9.EditValue = lot.SpecialStatus3Id;
            lookUpEdit10.EditValue = lot.ProjectStatusId;
            memoEdit1.Text = lot.Notes ?? "";
        }

        private void LoadLot3(LotGridModel lot)
        {
            textEdit9.Text = lot.LotName ?? "";
            textEdit11.Text = lot.Contractor ?? "";
            textEdit13.Text = lot.LotBudget.ToString();
            textEdit12.Text = lot.RegisteredAmount.ToString();
            textEdit14.Text = lot.ConsumedAmount.ToString();
            spinEdit5.Value = lot.ExecutionDuration ?? 0;
            dateEdit2.EditValue = lot.StartDate;
            spinEdit6.Value = lot.PhysicalProgress;
            lookUpEdit17.EditValue = lot.AdministrativeProcedureId;
            lookUpEdit18.EditValue = lot.SpecialStatus1Id;
            lookUpEdit16.EditValue = lot.SpecialStatus2Id;
            lookUpEdit19.EditValue = lot.SpecialStatus3Id;
            lookUpEdit20.EditValue = lot.ProjectStatusId;
            memoEdit2.Text = lot.Notes ?? "";
        }

        private void ApplyMode()
        {
            bool editable = _mode == FormMode.Edit;

            foreach (Control c in GetAllControls(this))
            {
                if (c is TextEdit ||
                    c is LookUpEdit ||
                    c is DateEdit ||
                    c is SpinEdit ||
                    c is MemoEdit ||
                    c is CheckEdit)
                {
                    c.Enabled = editable;
                }
            }

           // bbiSave.Enabled = editable;
           // bbiEdit.Enabled = !editable;
            btnAddLot.Enabled = editable;
            btnRemoveLot.Enabled = editable;
        }

        private void bbiEdit_ItemClick(object sender, ItemClickEventArgs e)
        {
            _mode = FormMode.Edit;
            ApplyMode();
        }

        private void PopulateLookups()
        {
            try
            {
                // ── Project lookups ───────────────────────────────────────────────
                BindLookup(cmbProgram, _lookup.GetAll("programs"));
                BindLookup(cmbDaira, _lookup.GetAll("dairas"));
                BindLookup(cmbCommune, _lookup.GetAll("communes"));
                BindLookup(cmbDomain, _lookup.GetAll("domains"));
                BindLookup(cmbSector, _lookup.GetAll("sectors"));

                // ── Lot lookups ───────────────────────────────────────────────────
                BindLookup(cmbAdminProcedure, _lookup.GetAll("administrative_procedures"));
                BindLookup(cmbSpecialStatus1, _lookup.GetAll("special_status1"));
                BindLookup(cmbSpecialStatus2, _lookup.GetAll("special_status2"));
                BindLookup(cmbSpecialStatus3, _lookup.GetAll("special_status3"));
                BindLookup(cmbProjectStatus, _lookup.GetAll("project_statuses"));

                // ── Lot2 lookups ───────────────────────────────────────────────────
                BindLookup(lookUpEdit7, _lookup.GetAll("administrative_procedures"));
                BindLookup(lookUpEdit8, _lookup.GetAll("special_status1"));
                BindLookup(lookUpEdit6, _lookup.GetAll("special_status2"));
                BindLookup(lookUpEdit9, _lookup.GetAll("special_status3"));
                BindLookup(lookUpEdit10, _lookup.GetAll("project_statuses"));

                // ── Lot3 lookups ───────────────────────────────────────────────────
                BindLookup(lookUpEdit17, _lookup.GetAll("administrative_procedures"));
                BindLookup(lookUpEdit18, _lookup.GetAll("special_status1"));
                BindLookup(lookUpEdit16, _lookup.GetAll("special_status2"));
                BindLookup(lookUpEdit19, _lookup.GetAll("special_status3"));
                BindLookup(lookUpEdit20, _lookup.GetAll("project_statuses"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to load lookup data:\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private static void BindLookup(LookUpEdit cmb, System.Collections.Generic.List<LookupItem> src)
        {
            cmb.Properties.DataSource = src;
            cmb.Properties.DisplayMember = "Name";
            cmb.Properties.ValueMember = "Id";
            cmb.Properties.ShowHeader = false;
            cmb.Properties.NullText = "— اختر —";
            cmb.Properties.SearchMode = SearchMode.AutoFilter;
            cmb.Properties.Columns.Clear();
            cmb.Properties.Columns.Add(new LookUpColumnInfo("Name", 240));
        }


        private IEnumerable<Control> GetAllControls(Control root)
        {
            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (var cc in GetAllControls(c)) yield return cc;
            }
        }

       





        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ValidateAll()) return;

            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var project = new Domain.Project
                        {
                            OperationNumber = txtOperationNumber.Text.Trim(),
                            OperationName = txtOperationName.Text.Trim(),
                            ProgramId = Convert.ToInt32(cmbProgram.EditValue),
                            DairaId = Convert.ToInt32(cmbDaira.EditValue),
                            CommuneId = Convert.ToInt32(cmbCommune.EditValue),
                            DomainId = Convert.ToInt32(cmbDomain.EditValue),
                            SectorId = Convert.ToInt32(cmbSector.EditValue),
                            HasLots = HasLots,
                            UpdatedBy = 1
                        };

                        project.Id = _projectRepo.Insert(project);
                        NewProject = project;
                        var lot = new Lot
                        {
                            ProjectId = project.Id,
                            LotNumber = 1,
                            LotName = txtLotName.Text.Trim(),
                            RegisteredAmount = string.IsNullOrWhiteSpace(txtRegisteredAmount.Text) ? 0 : Convert.ToDecimal(txtRegisteredAmount.Text),
                            LotBudget = string.IsNullOrWhiteSpace(txtLotBudget.Text) ? 0 : Convert.ToDecimal(txtLotBudget.Text),
                            ConsumedAmount = string.IsNullOrWhiteSpace(txtConsumedAmount.Text) ? 0 : Convert.ToDecimal(txtConsumedAmount.Text),
                            Contractor = NullIfBlank(txtContractor.Text),
                            ExecutionDuration = spnExecutionDuration.Value > 0
                                    ? (int?)Convert.ToInt32(spnExecutionDuration.Value) : null,
                            StartDate = dtStartDate.EditValue == null ||
                                    dtStartDate.EditValue == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(dtStartDate.EditValue),
                            PhysicalProgress = spnPhysicalProgress.Value,
                            AdministrativeProcedureId = NullableId(cmbAdminProcedure),
                            SpecialStatus1Id = NullableId(cmbSpecialStatus1),
                            SpecialStatus2Id = NullableId(cmbSpecialStatus2),
                            SpecialStatus3Id = NullableId(cmbSpecialStatus3),
                            ProjectStatusId = NullableId(cmbProjectStatus),
                            Notes = NullIfBlank(txtLotNotes.Text),
                            UpdatedBy = 1
                        };
                        lot.Id = _lotRepo.Insert(lot);
                        NewLot = lot;

                        if (tabLot2.PageVisible)
                        {
                            var lot2 = new Lot
                            {
                                ProjectId = project.Id,
                                LotNumber = 2,
                                LotName = textEdit2.Text.Trim(),
                                RegisteredAmount = string.IsNullOrWhiteSpace(textEdit5.Text) ? 0 : Convert.ToDecimal(textEdit5.Text),
                                LotBudget = string.IsNullOrWhiteSpace(textEdit6.Text) ? 0 : Convert.ToDecimal(textEdit6.Text),
                                ConsumedAmount = string.IsNullOrWhiteSpace(textEdit7.Text) ? 0 : Convert.ToDecimal(textEdit7.Text),
                                Contractor = NullIfBlank(textEdit4.Text),
                                ExecutionDuration = spinEdit2.Value > 0
                                   ? (int?)Convert.ToInt32(spinEdit2.Value) : null,
                                StartDate = dateEdit1.EditValue == null ||
                                   dateEdit1.EditValue == DBNull.Value
                                   ? (DateTime?)null
                                   : Convert.ToDateTime(dateEdit1.EditValue),
                                PhysicalProgress = spinEdit3.Value,
                                AdministrativeProcedureId = NullableId(lookUpEdit7),
                                SpecialStatus1Id = NullableId(lookUpEdit8),
                                SpecialStatus2Id = NullableId(lookUpEdit6),
                                SpecialStatus3Id = NullableId(lookUpEdit9),
                                ProjectStatusId = NullableId(lookUpEdit10),
                                Notes = NullIfBlank(memoEdit1.Text),
                                UpdatedBy = 1
                            };
                            lot2.Id = _lotRepo.Insert(lot2);
                            NewLot2 = lot2;
                        }
                        if (tabLot3.PageVisible)
                        {
                            var lot3 = new Lot
                            {
                                ProjectId = project.Id,
                                LotNumber = 3,
                                LotName = textEdit9.Text.Trim(),
                                RegisteredAmount = string.IsNullOrWhiteSpace(textEdit12.Text) ? 0 : Convert.ToDecimal(textEdit12.Text),
                                LotBudget = string.IsNullOrWhiteSpace(textEdit13.Text) ? 0 : Convert.ToDecimal(textEdit13.Text),
                                ConsumedAmount = string.IsNullOrWhiteSpace(textEdit14.Text) ? 0 : Convert.ToDecimal(textEdit14.Text),
                                Contractor = NullIfBlank(textEdit11.Text),
                                ExecutionDuration = spinEdit5.Value > 0
                                   ? (int?)Convert.ToInt32(spinEdit5.Value) : null,
                                StartDate = dateEdit2.EditValue == null ||
                                   dateEdit2.EditValue == DBNull.Value
                                   ? (DateTime?)null
                                   : Convert.ToDateTime(dateEdit2.EditValue),
                                PhysicalProgress = spinEdit6.Value,
                                AdministrativeProcedureId = NullableId(lookUpEdit17),
                                SpecialStatus1Id = NullableId(lookUpEdit18),
                                SpecialStatus2Id = NullableId(lookUpEdit16),
                                SpecialStatus3Id = NullableId(lookUpEdit19),
                                ProjectStatusId = NullableId(lookUpEdit20),
                                Notes = NullIfBlank(memoEdit2.Text),
                                UpdatedBy = 1
                            };
                            lot3.Id = _lotRepo.Insert(lot3);
                            NewLot3 = lot3;

                        }

                        transaction.Commit();
                        DialogResult = DialogResult.OK;
                        XtraMessageBox.Show( "تم حفظ ال مشروع", "Success",    MessageBoxButtons.OK, MessageBoxIcon.Information);     Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback(); 
                        XtraMessageBox.Show(
                            $"فشل الحفظ، تم التراجع عن جميع التغييرات.\n\n{ex.Message}\n{ex.InnerException?.Message}",
                            "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }


           
        }
private void bbiCancel_ItemClick(object sender, ItemClickEventArgs e)
{
    DialogResult = DialogResult.Cancel;
    Close();
}
        // ── Validation ────────────────────────────────────────────────────

        private bool ValidateAll()
{
    bool ok = true;

    ok &= Require(txtOperationNumber, "Operation Number is required");
    ok &= Require(txtOperationName, "Operation Name is required");
    ok &= RequireLookup(cmbProgram, "Program is required");
    ok &= RequireLookup(cmbDaira, "Daira is required");
    ok &= RequireLookup(cmbCommune, "Commune is required");
    ok &= RequireLookup(cmbDomain, "Domain is required");
    ok &= RequireLookup(cmbSector, "Sector is required");
    ok &= Require(txtLotName, "Lot Name is required");
    return ok;
}
private bool Require(TextEdit txt, string msg)
{
    if (!string.IsNullOrWhiteSpace(txt.Text)) return true;
    XtraMessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    txt.Focus();
    return false;
}
private bool RequireLookup(LookUpEdit cmb, string msg)
{
    if (cmb.EditValue != null && cmb.EditValue != DBNull.Value) return true;
    XtraMessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    cmb.Focus();
    return false;
}
private bool Fail(string msg)
{
    XtraMessageBox.Show(msg, "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    return false;
}

// ── Helpers ───────────────────────────────────────────────────────
private static string NullIfBlank(string s) =>
    string.IsNullOrWhiteSpace(s) ? null : s.Trim();

private static int? NullableId(LookUpEdit cmb) =>
    cmb.EditValue == null || cmb.EditValue == DBNull.Value
        ? (int?)null : Convert.ToInt32(cmb.EditValue);



// Caller reads this after ShowDialog returns OK




private void ucContactInfo1_Load(object sender, EventArgs e)
{

}

        private void groupControl5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }

        private void barButtonItem4_ItemClick(object sender, ItemClickEventArgs e)
        {


            if (!tabLot2.PageVisible)
                {
                    tabLot2.PageVisible = true;
                    tabContainer.SelectedTabPage = tabLot2;
                    btnRemoveLot.Enabled=true;
                HasLots = true; // أو chkHasLots.Checked = true

                return;
                }

                if (!tabLot3.PageVisible && tabLot2.PageVisible)
                {
                tabLot3.PageVisible = true;
                tabContainer.SelectedTabPage = tabLot3;
                btnRemoveLot.Enabled = true;
                btnAddLot.Enabled = false;
                HasLots = true;
                    return;
                }
            

        }

        private void btnRemoveLot_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (tabContainer.SelectedTabPage == tabLot3)
            {
                tabLot3.PageVisible = false;
            }
            else if (tabContainer.SelectedTabPage == tabLot2)
            {

                tabLot2.PageVisible = false;
                tabLot3.PageVisible = false;
                btnRemoveLot.Enabled = false;
                HasLots=false;


            }

            btnAddLot.Enabled = true;
        }
    }
}
