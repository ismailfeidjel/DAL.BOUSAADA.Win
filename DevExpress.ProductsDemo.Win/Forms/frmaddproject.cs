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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms {
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
                    Notes = NullIfBlank(txtProjectNotes.Text)
                };

                project.Id = _projectRepo.Insert(project);

                var lot = new Lot
                {
                    ProjectId = project.Id,
                    LotNumber = Convert.ToInt32(spnLotNumber.Value),
                    LotName = txtLotName.Text.Trim(),
                    LotBudget = Convert.ToDecimal(txtLotBudget),
                    RegisteredAmount = Convert.ToDecimal(txtRegisteredAmount),
                    ConsumedAmount = Convert.ToDecimal(txtConsumedAmount),
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
                    Notes = NullIfBlank(txtLotNotes.Text)
                };

                lot.Id = _lotRepo.Insert(lot);
                NewProject = project;
                NewLot = lot;

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Save failed:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            void Req(string v, LookUpEdit c = null) { /* see below */ }

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
    }
}
