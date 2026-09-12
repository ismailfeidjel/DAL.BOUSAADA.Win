using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.UI;
using System;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public class LotEditorControl : XtraUserControl
    {
        public int LotId { get; set; }          // 0 = not yet saved (new lot)
        public int LotNumber { get; set; }

        public TextEdit txtLotName = new TextEdit();
        public TextEdit txtContractor = new TextEdit();
        public TextEdit txtLotBudget = new TextEdit();
        public TextEdit txtRegisteredAmount = new TextEdit();
        public TextEdit txtConsumedAmount = new TextEdit();
        public SpinEdit spnExecutionDuration = new SpinEdit();
        public DateEdit dtStartDate = new DateEdit();
        public SpinEdit spnPhysicalProgress = new SpinEdit();
        public LookUpEdit cmbAdminProcedure = new LookUpEdit();
        public LookUpEdit cmbSpecialStatus1 = new LookUpEdit();
        public LookUpEdit cmbSpecialStatus2 = new LookUpEdit();
        public LookUpEdit cmbSpecialStatus3 = new LookUpEdit();
        public LookUpEdit cmbProjectStatus = new LookUpEdit();
        public MemoEdit txtLotNotes = new MemoEdit();

        public LotEditorControl()
        {
            Dock = DockStyle.Fill;
            //RightToLeftLayout = true;
            BuildLayout();
        }

        private void BuildLayout()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                Padding = new Padding(10),
                AutoScroll = true
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            AddRow(layout, 0, "اسم الحصة", txtLotName);
            AddRow(layout, 1, "المقاول", txtContractor);
            AddRow(layout, 2, "الغلاف المالي", txtLotBudget);
            AddRow(layout, 3, "المبلغ المسجل", txtRegisteredAmount);
            AddRow(layout, 4, "المبلغ المستهلك", txtConsumedAmount);
            AddRow(layout, 5, "آجال التنفيذ (يوم)", spnExecutionDuration);
            AddRow(layout, 6, "تاريخ الانطلاق", dtStartDate);
            AddRow(layout, 7, "التقدم الفيزيائي (%)", spnPhysicalProgress);

            // Second block — lookups + notes, in a second panel below
            var layout2 = new TableLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 250,
                ColumnCount = 2,
                RowCount = 6,
                Padding = new Padding(10)
            };
            layout2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));

            AddRow(layout2, 0, "الإجراء الإداري", cmbAdminProcedure);
            AddRow(layout2, 1, "الوضعية 1", cmbSpecialStatus1);
            AddRow(layout2, 2, "الوضعية 2", cmbSpecialStatus2);
            AddRow(layout2, 3, "الوضعية 3", cmbSpecialStatus3);
            AddRow(layout2, 4, "وضعية العملية", cmbProjectStatus);
            AddRow(layout2, 5, "الملاحظة", txtLotNotes);

            Controls.Add(layout);
            Controls.Add(layout2);
        }

        private void AddRow(TableLayoutPanel table, int row, string labelText, Control editor)
        {
            var label = new LabelControl { Text = labelText, Dock = DockStyle.Fill, Appearance = { TextOptions = { HAlignment = DevExpress.Utils.HorzAlignment.Far } } };
            editor.Dock = DockStyle.Fill;
            table.Controls.Add(label, 0, row);
            table.Controls.Add(editor, 1, row);
        }

        public void BindLookups(LookupRepository lookup)
        {
            BindOne(cmbAdminProcedure, lookup.GetAll("administrative_procedures"));
            BindOne(cmbSpecialStatus1, lookup.GetAll("special_status1"));
            BindOne(cmbSpecialStatus2, lookup.GetAll("special_status2"));
            BindOne(cmbSpecialStatus3, lookup.GetAll("special_status3"));
            BindOne(cmbProjectStatus, lookup.GetAll("project_statuses"));
        }

        private static void BindOne(LookUpEdit cmb, System.Collections.Generic.List<LookupItem> src)
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

        public void LoadFrom(LotGridModel lot)
        {
            LotId = lot.Id;
            LotNumber = lot.LotNumber;
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

        public Lot ToLot(int projectId)
        {
            return new Lot
            {
                Id = LotId,
                ProjectId = projectId,
                LotNumber = LotNumber,
                LotName = txtLotName.Text.Trim(),
                LotBudget = string.IsNullOrWhiteSpace(txtLotBudget.Text) ? 0 : Convert.ToDecimal(txtLotBudget.Text),
                RegisteredAmount = string.IsNullOrWhiteSpace(txtRegisteredAmount.Text) ? 0 : Convert.ToDecimal(txtRegisteredAmount.Text),
                ConsumedAmount = string.IsNullOrWhiteSpace(txtConsumedAmount.Text) ? 0 : Convert.ToDecimal(txtConsumedAmount.Text),
                Contractor = string.IsNullOrWhiteSpace(txtContractor.Text) ? null : txtContractor.Text.Trim(),
                ExecutionDuration = spnExecutionDuration.Value > 0 ? (int?)Convert.ToInt32(spnExecutionDuration.Value) : null,
                StartDate = dtStartDate.EditValue == null || dtStartDate.EditValue == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dtStartDate.EditValue),
                PhysicalProgress = spnPhysicalProgress.Value,
                AdministrativeProcedureId = GetId(cmbAdminProcedure),
                SpecialStatus1Id = GetId(cmbSpecialStatus1),
                SpecialStatus2Id = GetId(cmbSpecialStatus2),
                SpecialStatus3Id = GetId(cmbSpecialStatus3),
                ProjectStatusId = GetId(cmbProjectStatus),
                Notes = string.IsNullOrWhiteSpace(txtLotNotes.Text) ? null : txtLotNotes.Text.Trim()
            };
        }

        private static int? GetId(LookUpEdit cmb) =>
            cmb.EditValue == null || cmb.EditValue == DBNull.Value ? (int?)null : Convert.ToInt32(cmb.EditValue);
    }
}