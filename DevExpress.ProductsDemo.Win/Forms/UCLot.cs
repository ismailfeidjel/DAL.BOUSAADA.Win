using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class UCLot : XtraUserControl
    {
        public int LotNumber { get; set; }
        public int? LotId { get; private set; } // null = new/unsaved lot
        public bool IsMainLot { get; private set; }


        public UCLot()
        {
            InitializeComponent();
        }
        public void BindLookups(LookupRepository lookup)
        {
            BindLookup(cmbAdminProcedure, lookup.GetAll("administrative_procedures"));
            BindLookup(cmbSpecialStatus1, lookup.GetAll("special_status1"));
            BindLookup(cmbSpecialStatus2, lookup.GetAll("special_status2"));
            BindLookup(cmbSpecialStatus3, lookup.GetAll("special_status3"));
            BindLookup(cmbProjectStatus, lookup.GetAll("project_statuses"));

            BindLookup(cmbProgram, lookup.GetAll("programs"));
            BindLookup(cmbDaira, lookup.GetAll("dairas"));
            BindLookup(cmbCommune, lookup.GetAll("communes"));
            BindLookup(cmbDomain, lookup.GetAll("domains"));
            BindLookup(cmbSector, lookup.GetAll("sectors"));
        }
        private static void BindLookup(LookUpEdit cmb, List<LookupItem> src)
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
        public void ShowProjectFields(bool show)
        {
            IsMainLot = show;
            //pnlProjectFields.Visible = show; // ← group panel wrapping the 6 project-level controls in the Designer
        }
        public void LoadLot(LotGridModel lot)
        {
            LotId = lot.Id;
            LotNumber = lot.LotNumber;

            // Project-level (only relevant/visible when IsMainLot, but loaded regardless
            // so toggling visibility later doesn't require a reload)
            string[] parts = (lot.OperationName ?? "").Split('\u001F');
            txtOperationName.Text = parts.Length > 0 ? parts[0].Trim() : "";
            cmbProgram.EditValue = lot.ProgramId;
            cmbDaira.EditValue = lot.DairaId;
            cmbCommune.EditValue = lot.CommuneId;
            cmbDomain.EditValue = lot.DomainId;
            cmbSector.EditValue = lot.SectorId;

            // Lot-level
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
        // Used when adding a brand-new lot with no data yet
        public void LoadNew(int lotNumber)
        {
            LotId = null;
            LotNumber = lotNumber;
            // controls stay blank/default
        }
        public Domain.Project GetProjectForSave(int projectId)
        {
            return new Domain.Project
            {
                Id = projectId,
                OperationName = txtOperationName.Text.Trim(),
                ProgramId = NullableId(cmbProgram) ?? 0,
                DairaId = NullableId(cmbDaira) ?? 0,
                CommuneId = NullableId(cmbCommune) ?? 0,
                DomainId = NullableId(cmbDomain) ?? 0,
                SectorId = NullableId(cmbSector) ?? 0,
            };
        }

        public Domain.Lot GetLotForSave()
        {
            return new Domain.Lot
            {
                Id = LotId ?? 0,
                LotNumber = LotNumber,
                LotName = txtLotName.Text.Trim(),
                LotBudget = string.IsNullOrWhiteSpace(txtLotBudget.Text) ? 0 : Convert.ToDecimal(txtLotBudget.Text),
                RegisteredAmount = string.IsNullOrWhiteSpace(txtRegisteredAmount.Text) ? 0 : Convert.ToDecimal(txtRegisteredAmount.Text),
                ConsumedAmount = string.IsNullOrWhiteSpace(txtConsumedAmount.Text) ? 0 : Convert.ToDecimal(txtConsumedAmount.Text),
                Contractor = NullIfBlank(txtContractor.Text),
                ExecutionDuration = spnExecutionDuration.Value > 0 ? (int?)Convert.ToInt32(spnExecutionDuration.Value) : null,
                StartDate = dtStartDate.EditValue == null || dtStartDate.EditValue == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dtStartDate.EditValue),
                PhysicalProgress = spnPhysicalProgress.Value,
                AdministrativeProcedureId = NullableId(cmbAdminProcedure),
                SpecialStatus1Id = NullableId(cmbSpecialStatus1),
                SpecialStatus2Id = NullableId(cmbSpecialStatus2),
                SpecialStatus3Id = NullableId(cmbSpecialStatus3),
                ProjectStatusId = NullableId(cmbProjectStatus),
                Notes = NullIfBlank(txtLotNotes.Text)
            };
        }
        public bool Validate()
        {
            if (!IsMainLot) return true;

            if (!Require(txtOperationName, "اسم العملية مطلوب")) return false;
            if (!RequireLookup(cmbProgram, "البرنامج مطلوب")) return false;
            if (!RequireLookup(cmbDaira, "الدائرة مطلوبة")) return false;
            if (!RequireLookup(cmbCommune, "البلدية مطلوبة")) return false;
            if (!RequireLookup(cmbDomain, "القطاع مطلوب")) return false;
            if (!RequireLookup(cmbSector, "المجال مطلوب")) return false;

            return true;
        }

        private bool Require(TextEdit txt, string msg)
        {
            if (!string.IsNullOrWhiteSpace(txt.Text)) return true;
            XtraMessageBox.Show(msg, "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txt.Focus();
            return false;
        }

        private bool RequireLookup(LookUpEdit cmb, string msg)
        {
            if (cmb.EditValue != null && cmb.EditValue != DBNull.Value) return true;
            XtraMessageBox.Show(msg, "تحقق", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            cmb.Focus();
            return false;
        }

        private static string NullIfBlank(string s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
        private static int? NullableId(LookUpEdit cmb) =>
       cmb.EditValue == null || cmb.EditValue == DBNull.Value ? (int?)null : Convert.ToInt32(cmb.EditValue);

    }
}
