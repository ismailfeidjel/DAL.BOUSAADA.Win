using DevExpress.DataProcessing.InMemoryDataProcessor;
using DevExpress.MailClient.Win;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
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
    public partial class frmaddproject : RibbonForm
    {
        public Domain.Project NewProject { get; private set; }
        private readonly LookupRepository _lookup = new LookupRepository();

        public Lot NewLot { get; private set; }

        private readonly ProjectRepository _projectRepo = new ProjectRepository();
        private readonly LotRepository _lotRepo = new LotRepository();

        public frmaddproject()
        {
            InitializeComponent();
            PopulateLookups();
#if DEBUG
            LoadTestData();
#endif


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

        private void LoadTestData()
        {
            txtOperationNumber.Text = "OP-2026-001";
            txtOperationName.Text = "إنجاز مدرسة ابتدائية";

            if (cmbProgram.Properties.DataSource is List<LookupItem> programs && programs.Any())
                cmbProgram.EditValue = programs.First().Id;

            if (cmbDaira.Properties.DataSource is List<LookupItem> dairas && dairas.Any())
                cmbDaira.EditValue = dairas.First().Id;

            if (cmbCommune.Properties.DataSource is List<LookupItem> communes && communes.Any())
                cmbCommune.EditValue = communes.First().Id;

            if (cmbDomain.Properties.DataSource is List<LookupItem> domains && domains.Any())
                cmbDomain.EditValue = domains.First().Id;

            if (cmbSector.Properties.DataSource is List<LookupItem> sectors && sectors.Any())
                cmbSector.EditValue = sectors.First().Id;

            spnLotNumber.Value = 1;
            txtLotName.Text = "الحصة رقم 01";

            txtLotBudget.Text = "5000000";
            txtRegisteredAmount.Text = "5000000";
            txtConsumedAmount.Text = "1200000";

            txtContractor.Text = "مؤسسة البناء الحديثة";

            spnExecutionDuration.Value = 12;
            spnPhysicalProgress.Value = 25;

            dtStartDate.DateTime = DateTime.Today.AddMonths(-3);

            if (cmbAdminProcedure.Properties.DataSource is List<LookupItem> admin && admin.Any())
                cmbAdminProcedure.EditValue = admin.First().Id;

            if (cmbSpecialStatus1.Properties.DataSource is List<LookupItem> s1 && s1.Any())
                cmbSpecialStatus1.EditValue = s1.First().Id;

            if (cmbProjectStatus.Properties.DataSource is List<LookupItem> status && status.Any())
                cmbProjectStatus.EditValue = status.First().Id;

            txtLotNotes.Text = "حصة تجريبية";
        }


        private void bbiSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ValidateAll()) return;

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
                    HasLots = true,
                    UpdatedBy=1
                };

                project.Id = _projectRepo.Insert(project);

                var lot = new Lot
                {
                    ProjectId = project.Id,
                    LotNumber = Convert.ToInt32(spnLotNumber.Value),
                    LotName = txtLotName.Text.Trim(),
                    RegisteredAmount = string.IsNullOrWhiteSpace(txtRegisteredAmount.Text) ? 0 : Convert.ToDecimal(txtRegisteredAmount.Text),
                    LotBudget = string.IsNullOrWhiteSpace(txtLotBudget.Text)  ? 0  : Convert.ToDecimal(txtLotBudget.Text),
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
                    UpdatedBy=1
                };

            lot.Id = _lotRepo.Insert(lot);
            NewProject = project;
            NewLot = lot;

            DialogResult = DialogResult.OK;
            XtraMessageBox.Show(
    "Project saved successfully",
    "Success",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information);
                Close();
        }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"{ex.Message}\n\n{ex.InnerException?.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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
                    return;
                }

                if (!tabLot3.PageVisible && tabLot2.PageVisible)
                {
                tabLot3.PageVisible = true;
                tabContainer.SelectedTabPage = tabLot3;
                btnRemoveLot.Enabled = true;
                btnAddLot.Enabled = false;
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


            }

            btnAddLot.Enabled = true;
        }
    }
}
