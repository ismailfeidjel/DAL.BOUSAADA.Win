using DevExpress.MailClient.Win;
using DevExpress.MailDemo.Win;
using DevExpress.ProductsDemo.Win.Controls;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Services;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraNavBar;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.ReportGeneration;
using DevExpress.XtraReports.UI;
using DevExpress.Data.Filtering; 


using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text.RegularExpressions;

namespace DevExpress.ProductsDemo.Win.Modules
{
    public partial class ProjectModule : BaseModule
    {
        public override string ModuleName { get { return Properties.Resources.TasksName; } }

        private List<LotGridModel> _data;
        private LotGridModel _currentLot;
        private readonly LotRepository _lotRepo = new LotRepository();
        private readonly ProjectRepository _projectRepo = new ProjectRepository();
        private bool _loadingLot = false;
        private const string ProjectsTemplateKey = "قالب_تقرير_المشاريع";
        private static readonly HashSet<string> SumFields = new HashSet<string> { "LotBudget", "ConsumedAmount", "RegisteredAmount" };
        private const string CountField = "Daira";
        public override bool HasProgramSelector => true;

        public override List<LookupItem> GetPrograms() =>
     new LookupRepository().GetPrograms("ADSEC").Cast<LookupItem>().ToList();



        private int? _selectedProgramId;
        public override int? SelectedProgramId
        {
            get => _selectedProgramId;
            set => _selectedProgramId = value;
        }
        public override void OnProgramChanged(int? programId)
        {
            _selectedProgramId = programId;
            LoadData();
        }


        private XtraReport BuildReportFromTemplateOrDefault()
        {
            var visibleData = GetVisibleGridData();
            string programName = GetPrograms()
        .FirstOrDefault(p => p.Id == _selectedProgramId)?.Name ?? "";

            return GridReportBuilder.Build(gridView1, visibleData, new GridReportOptions
            {
                TemplateKey = ProjectsTemplateKey,
                SumFields = SumFields,
                CountField = CountField,
                GroupIdField = "ProjectId",
                GenerateFooterRow = false ,
                 ProgramDisplayText = programName   // ← new

            });


        }



        private void CollectOperandValues(CriteriaOperator criteria, DevExpress.XtraGrid.Columns.GridColumn col, List<string> values)
        {
            switch (criteria)
            {
                case BinaryOperator bo:
                    CollectOperandValues(bo.LeftOperand, col, values);
                    CollectOperandValues(bo.RightOperand, col, values);
                    break;

                case GroupOperator go:
                    foreach (var op in go.Operands)
                        CollectOperandValues(op, col, values);
                    break;

                case UnaryOperator uo:
                    CollectOperandValues(uo.Operand, col, values);
                    break;

                case InOperator io:
                    foreach (var op in io.Operands)
                        CollectOperandValues(op, col, values);
                    break;

                case BetweenOperator bwo:
                    CollectOperandValues(bwo.BeginExpression, col, values);
                    CollectOperandValues(bwo.EndExpression, col, values);
                    break;

                case FunctionOperator fo: // e.g. Contains(), StartsWith() for text filters
                    foreach (var op in fo.Operands)
                        CollectOperandValues(op, col, values);
                    break;

                case OperandValue ov:
                    if (ov.Value != null)
                        values.Add(GetFilterValueDisplayText(col, ov.Value));
                    break;

                    // OperandProperty (the field name itself) and anything else — nothing to collect
            }
        }


        // Helper method to convert raw ID value to friendly text using column's repository editor
        // Helper method to convert raw ID value to friendly text using column's repository editor
        private string GetFilterValueDisplayText(DevExpress.XtraGrid.Columns.GridColumn col, object val)
{
    if (col == null || val == null) return val?.ToString() ?? "";

    // 1. Check if the column uses a LookUpEdit (e.g., DomainId, SectorId, Status)
    if (col.RealColumnEdit is DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit lookup)
    {
        // Try looking up with the raw object first
        object lookupText = lookup.GetDisplayValueByKeyValue(val);

        // Fallback: If 'val' is a string containing an ID (e.g., "3"), convert it to an integer 
        // to ensure DevExpress matches it with integer-based ValueMember IDs.
        if ((lookupText == null || lookupText == DBNull.Value) && int.TryParse(val.ToString(), out int intVal))
        {
            lookupText = lookup.GetDisplayValueByKeyValue(intVal);
        }

        if (lookupText != null && lookupText != DBNull.Value)
            return lookupText.ToString();
    }
    // 2. Check if the column uses an ImageComboBox
    else if (col.RealColumnEdit is DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox imgCombo)
    {
        var item = imgCombo.Items.Cast<DevExpress.XtraEditors.Controls.ImageComboBoxItem>()
                                 .FirstOrDefault(i => i.Value != null && i.Value.ToString() == val.ToString());
        if (item != null)
            return item.Description;
    }

    // Clean up surrounding quotes from normal strings
    return val.ToString().Replace("'", "").Replace("\"", "").Trim();
}



        //-------------------------------------------------------------------------------------------------------------------
        public override IPrintable PrintableComponent
        {
            get { return gridControl1; }
        }
        public override bool AllowRtfTitle { get { return true; } }

        public override void ShowColumnChooser() => gridView1.ShowCustomization();

        private string LayoutPath =>
    System.IO.Path.Combine(
        Application.StartupPath, "grid_layout_projects.xml");

        public ProjectModule()
        {
            InitializeComponent();
        }
        RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();



        // ── Grid Setup ───────────────────────────────────────────────
        private void SetupGrid()
        {
            gridControl1.RepositoryItems.Add(memo);
            memo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            memo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            memo.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            memo.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
            gridView1.ShowingEditor += gridView1_ShowingEditor;




            gridControl1.MainView = gridView1;
            gridView1.OptionsView.ShowGroupPanel = true;
            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsView.ShowIndicator = true;
            gridView1.OptionsSelection.MultiSelect = false;
            gridView1.ColumnWidthChanged += (s, e) => SaveLayout();
            gridView1.ColumnPositionChanged += (s, e) => SaveLayout();

            
            gridView1.Appearance.HeaderPanel.Font =
    new Font("Tahoma", 7, FontStyle.Bold);

            gridView1.Appearance.HeaderPanel.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Center;

            gridView1.Appearance.HeaderPanel.TextOptions.VAlignment =
                DevExpress.Utils.VertAlignment.Center;
            //-----------------------
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;

            gridView1.Appearance.FocusedRow.BackColor = Color.LightSteelBlue;
            gridView1.Appearance.HideSelectionRow.BackColor = Color.LightSteelBlue;
            //----------------------

            //-----------------------
            gridView1.PaintStyleName = "Skin";
            gridView1.OptionsView.EnableAppearanceEvenRow = false;
            gridView1.OptionsView.EnableAppearanceOddRow = false;
            gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            gridView1.OptionsView.RowAutoHeight = true;

            //-------------------------

            var dateEdit = new RepositoryItemDateEdit();
            dateEdit.DisplayFormat.FormatType = FormatType.DateTime;
            dateEdit.DisplayFormat.FormatString = "dd/MM/yyyy";
            dateEdit.EditFormat.FormatType = FormatType.DateTime;
            dateEdit.EditFormat.FormatString = "dd/MM/yyyy";
            gridControl1.RepositoryItems.Add(dateEdit);

            //

            // gridView1.Appearance.EvenRow.BackColor = Color.White;
            //gridView1.Appearance.OddRow.BackColor = Color.FromArgb(245, 245, 245);
            AddCol("OperationNumber", "رقم ", 110);
            AddCol("Daira", "الدائرة", 100);
           // gridView1.Columns["Daira"].OptionsColumn.AllowMerge = DefaultBoolean.True;
            AddCol("LotNumber", "N", 100);

            AddCol("Commune", "البلدية", 100);
            AddCol("ProgramId", "البرنامج", 110);
            var programLookup = new RepositoryItemLookUpEdit();
            programLookup.DataSource = new LookupRepository().GetAll("programs");
            programLookup.DisplayMember = "Name";
            programLookup.ValueMember = "Id";
            programLookup.ShowHeader = false;
            programLookup.NullText = "— اختر —";
            programLookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(programLookup);
            gridView1.Columns["ProgramId"].ColumnEdit = programLookup;
            gridView1.Columns["ProgramId"].OptionsColumn.AllowEdit = true;

            AddCol("FinancialProgress", "التقدم المالي", 100, "{0:N0} %");
            AddCol("OperationName", "اسم العملية", 180);
            AddCol("DomainId", "القطاع", 110);
            var domainLookup = new RepositoryItemLookUpEdit();
            domainLookup.DataSource = new LookupRepository().GetAll("domains");
            domainLookup.DisplayMember = "Name";
            domainLookup.ValueMember = "Id";
            domainLookup.ShowHeader = false;
            domainLookup.NullText = "— اختر —";
            domainLookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(domainLookup);
            gridView1.Columns["DomainId"].ColumnEdit = domainLookup;
            gridView1.Columns["DomainId"].OptionsColumn.AllowEdit = true;



            AddCol("SectorId", "المجال", 110);
            var sectorLookup = new RepositoryItemLookUpEdit();
            sectorLookup.DataSource = new LookupRepository().GetAll("sectors");
            sectorLookup.DisplayMember = "Name";
            sectorLookup.ValueMember = "Id";
            sectorLookup.ShowHeader = false;
            sectorLookup.NullText = "— اختر —";
            sectorLookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(sectorLookup);
            gridView1.Columns["SectorId"].ColumnEdit = sectorLookup;
            gridView1.Columns["SectorId"].OptionsColumn.AllowEdit = true;


            AddCol("LotBudget", "الغلاف المالي", 110, "{0:N2}");
            AddCol("RegisteredAmount", "المبلغ المسجل", 110, "{0:N2}");
            AddCol("ConsumedAmount", "المبلغ المستهلك", 110, "{0:N2}");
            AddCol("Remaining", "الباقي", 110, "{0:N2}");
            AddCol("Contractor", "المقاول", 110);
            AddCol("StartDate", "تاريخ امر الانطلاق", 110, "{0:dd/MM/yyyy}", FormatType.DateTime);
            gridView1.Columns["StartDate"].ColumnEdit = dateEdit;
            AddCol("ExecutionDuration", "اجال التنفيذ", 110, "{0:N0}يوم");
            AddCol("PhysicalProgress", "التقدم الفيزيائي", 100, "{0:N0} %");

            AddCol("ProjectStatusId", "وضعية العملية", 110);
            var statusLookup = new RepositoryItemLookUpEdit();
            statusLookup.DataSource = new LookupRepository().GetAll("project_statuses");
            statusLookup.DisplayMember = "Name";
            statusLookup.ValueMember = "Id";
            statusLookup.ShowHeader = false;
            statusLookup.NullText = "— اختر —";
            statusLookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(statusLookup);
            gridView1.Columns["ProjectStatusId"].ColumnEdit = statusLookup;
            gridView1.Columns["ProjectStatusId"].OptionsColumn.AllowEdit = true;


            AddCol("AdministrativeProcedureId", "الإجراء الإداري", 130);
            var adminProcedureLookup = new RepositoryItemLookUpEdit();
            adminProcedureLookup.DataSource = new LookupRepository().GetAll("administrative_procedures");
            adminProcedureLookup.DisplayMember = "Name";
            adminProcedureLookup.ValueMember = "Id";
            adminProcedureLookup.ShowHeader = false;
            adminProcedureLookup.NullText = "— اختر —";
            adminProcedureLookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(adminProcedureLookup);
            gridView1.Columns["AdministrativeProcedureId"].ColumnEdit = adminProcedureLookup;
            gridView1.Columns["AdministrativeProcedureId"].OptionsColumn.AllowEdit = true;

            AddCol("SpecialStatus1Id", "الوضعية1", 130);
            var specialStatus1Lookup = new RepositoryItemLookUpEdit();
            specialStatus1Lookup.DataSource = new LookupRepository().GetAll("special_status1");
            specialStatus1Lookup.DisplayMember = "Name";
            specialStatus1Lookup.ValueMember = "Id";
            specialStatus1Lookup.ShowHeader = false;
            specialStatus1Lookup.NullText = "——";
            specialStatus1Lookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(specialStatus1Lookup);
            gridView1.Columns["SpecialStatus1Id"].ColumnEdit = specialStatus1Lookup;
            gridView1.Columns["SpecialStatus1Id"].OptionsColumn.AllowEdit = true;

            AddCol("SpecialStatus2Id", "الوضعية2", 130);
            var specialStatus2Lookup = new RepositoryItemLookUpEdit();
            specialStatus2Lookup.DataSource = new LookupRepository().GetAll("special_status2");
            specialStatus2Lookup.DisplayMember = "Name";
            specialStatus2Lookup.ValueMember = "Id";
            specialStatus2Lookup.ShowHeader = false;
            specialStatus2Lookup.NullText = "——";
            specialStatus2Lookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(specialStatus2Lookup);
            gridView1.Columns["SpecialStatus2Id"].ColumnEdit = specialStatus2Lookup;
            gridView1.Columns["SpecialStatus2Id"].OptionsColumn.AllowEdit = true;

            AddCol("SpecialStatus3Id", "الوضعية3", 130);
            var specialStatus3Lookup = new RepositoryItemLookUpEdit();
            specialStatus3Lookup.DataSource = new LookupRepository().GetAll("special_status3");
            specialStatus3Lookup.DisplayMember = "Name";
            specialStatus3Lookup.ValueMember = "Id";
            specialStatus3Lookup.ShowHeader = false;
            specialStatus3Lookup.NullText = "——";
            specialStatus3Lookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(specialStatus3Lookup);
            gridView1.Columns["SpecialStatus3Id"].ColumnEdit = specialStatus3Lookup;
            gridView1.Columns["SpecialStatus3Id"].OptionsColumn.AllowEdit = true;

            AddCol("Notes", "الملاحظة", 150);




            gridView1.Columns["LotBudget"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["RegisteredAmount"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["ConsumedAmount"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["PhysicalProgress"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["ProjectStatusId"].OptionsColumn.AllowEdit = true;

            gridView1.Columns["Contractor"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["StartDate"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["ExecutionDuration"].OptionsColumn.AllowEdit = true;

            // Keep these readonly in grid — handled by left panel
            gridView1.Columns["OperationNumber"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["OperationName"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["Daira"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["Commune"].OptionsColumn.AllowEdit = false;
            gridView1.Columns["Notes"].OptionsColumn.AllowEdit = true;


            //----------------------------------------------------------------------------
            gridView1.OptionsView.ShowFooter = true;

            gridView1.Columns["LotBudget"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "LotBudget", "{0:N2}");

            gridView1.Columns["ConsumedAmount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "ConsumedAmount", "{0:N2}");

            gridView1.Columns["RegisteredAmount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "RegisteredAmount", "{0:N2}");

                
            gridView1.Columns["Daira"].Summary.Add(
                DevExpress.Data.SummaryItemType.Count,
                "OperationNumber",
                "عددها: {0}");

            gridView1.Appearance.FooterPanel.Font =
    new Font("Tahoma", 8, FontStyle.Bold);

            gridView1.Appearance.FooterPanel.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Center;

           
         
            //--------
         

          //  gridView1.Columns["OperationName"].ColumnEdit = memo;
            gridView1.OptionsView.RowAutoHeight = true;

            // gridView1.Columns["Notes"].ColumnEdit = memo;
            gridView1.RowUpdated += gridView1_RowUpdated;
            foreach (GridColumn col in gridView1.Columns)
            {
                col.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Center;
                col.AppearanceCell.TextOptions.VAlignment = VertAlignment.Center;
            }


        }
        private bool _layoutReady = false;
        private void SaveLayout()
        {
            if (!_layoutReady) return; // ← don't save during setup
            gridView1.SaveLayoutToXml(LayoutPath);
        }

        private void LoadLayout()
        {
            if (System.IO.File.Exists(LayoutPath))
                gridView1.RestoreLayoutFromXml(LayoutPath);
        }
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Visible) SaveLayout();
        }
        private void AddCol(string field, string caption, int width, string format = null, FormatType formatType = FormatType.Numeric)
        {
            GridColumn col = gridView1.Columns.AddVisible(field, caption);
            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            if (format == null)
                col.ColumnEdit = memo;
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            if (format != null)
            {
                col.ColumnEdit = memo;

                col.DisplayFormat.FormatType = formatType;
                col.DisplayFormat.FormatString = format;
            }
        }
        private void gridView1_RowUpdated(object sender, RowObjectEventArgs e)
        {
            var lot = e.Row as LotGridModel;
            if (lot == null) return;

            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var updatedLot = new Domain.Lot
                        {
                            Id = lot.Id,
                            ProjectId = lot.ProjectId,
                            LotNumber = lot.LotNumber,
                            LotName = lot.LotName,
                            LotBudget = lot.LotBudget,
                            RegisteredAmount = lot.RegisteredAmount,
                            ConsumedAmount = lot.ConsumedAmount,
                            Contractor = lot.Contractor,
                            ExecutionDuration = lot.ExecutionDuration,
                            StartDate = lot.StartDate,
                            PhysicalProgress = lot.PhysicalProgress,
                            AdministrativeProcedureId = lot.AdministrativeProcedureId,
                            SpecialStatus1Id = lot.SpecialStatus1Id,
                            SpecialStatus2Id = lot.SpecialStatus2Id,
                            SpecialStatus3Id = lot.SpecialStatus3Id,
                            ProjectStatusId = lot.ProjectStatusId,
                            Notes = lot.Notes
                        };

                        var updatedProject = new Domain.Project
                        {
                            Id = lot.ProjectId,
                            OperationNumber = lot.OperationNumber,
                            OperationName = lot.OperationName.Split('\u001F')[0].Trim(),
                            ProgramId = lot.ProgramId ?? 0,
                            DairaId = lot.DairaId ?? 0,
                            CommuneId = lot.CommuneId ?? 0,
                            DomainId = lot.DomainId ?? 0,
                            SectorId = lot.SectorId ?? 0,
                            HasLots = lot.LotNumber > 1,
                            UpdatedBy = 1
                        };

                        _projectRepo.Update(updatedProject, conn, transaction);
                        _lotRepo.Update(updatedLot, conn, transaction);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        XtraMessageBox.Show(
                            $"فشل الحفظ، تم التراجع عن جميع التغييرات.\n\n{ex.Message}",
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }
        // ── Init ─────────────────────────────────────────────────────
        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data)
        {
            try
            {
                base.InitModule(manager, data);
                BuildDetailPanel();
                SetupGrid();
                // In InitModule, replace the old OrderByDescending(p => p.Id) line with:
                var programs = GetPrograms();
                _selectedProgramId = programs.FirstOrDefault()?.Id; // already sorted by Year DESC from the repository
                LoadData();
                LoadLayout();
                _layoutReady = true;
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(
                    $"خطأ في تحميل الوحدة:\n\n{ex.Message}\n\n{ex.InnerException?.Message}\n\n{ex.StackTrace}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ── Data ─────────────────────────────────────────────────────

        
       
        private void LoadData()
        {
            var all = _lotRepo.GetGridData();
            _data = _selectedProgramId.HasValue
                ? all.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                : all;
            gridControl1.DataSource = _data;
        }

        // ── Detail Panel ─────────────────────────────────────────────
        private void BuildDetailPanel()
        {
            _detailPanel.Dock = DockStyle.Fill;

            var lookup = new LookupRepository();
            BindSidePanel(lookUp5, lookup.GetAll("administrative_procedures"));
            BindSidePanel(lookUp6, lookup.GetAll("special_status1"));
            BindSidePanel(lookUp7, lookup.GetAll("special_status2"));
            BindSidePanel(lookUp8, lookup.GetAll("special_status3"));

            lookUp5.EditValueChanged += SidePanelLookup_Changed;
            lookUp6.EditValueChanged += SidePanelLookup_Changed;
            lookUp7.EditValueChanged += SidePanelLookup_Changed;
            lookUp8.EditValueChanged += SidePanelLookup_Changed;
        }

        private static void BindSidePanel(LookUpEdit cmb, List<LookupItem> src)
        {
            var srcWithNull = new List<LookupItem>();
            srcWithNull.Add(new LookupItem { Id = -1, Name = "— بدون —" });
            srcWithNull.AddRange(src);

            cmb.Properties.DataSource = srcWithNull;
            cmb.Properties.DisplayMember = "Name";
            cmb.Properties.ValueMember = "Id";
            cmb.Properties.ShowHeader = false;
            cmb.Properties.NullText = "—";
          //  cmb.Properties.ReadOnly = true;
            cmb.Properties.Columns.Clear();
            cmb.Properties.Columns.Add(new LookUpColumnInfo("Name", 200));
            
        }

        private static int? GetLookupValue(LookUpEdit cmb)
        {
            if (cmb.EditValue == null || cmb.EditValue == DBNull.Value) return null;
            int val = Convert.ToInt32(cmb.EditValue);
            return val == -1 ? (int?)null : val;
        }


        public void LoadLot(LotGridModel lot)
        {
            _loadingLot = true; // ← suppress events


            // Status
            lookUp5.EditValue = lot.AdministrativeProcedureId;
            lookUp6.EditValue = lot.SpecialStatus1Id;
            lookUp7.EditValue = lot.SpecialStatus2Id;
            lookUp8.EditValue = lot.SpecialStatus3Id;

            // Update info
            txtupdateby.Text = lot.UpdatedBy ?? "—";
                txtupdateddate.Text = lot.UpdatedAt.HasValue
                    ? lot.UpdatedAt.Value.ToString("dd/MM/yyyy HH:mm")
                    : "—";

             
              

                txtexceptedend.Text = lot.ExpectedEndDate.HasValue
                    ? lot.ExpectedEndDate.Value.ToString("dd/MM/yyyy")
                    : "—";
            txtexceptedend.ReadOnly = true;

                if (lot.DaysRemaining.HasValue)
                {
                    int days = lot.DaysRemaining.Value;
                    if (days < 0)
                    {
                        txtremaningdays.Text = $"متأخر بـ {Math.Abs(days)} يوم";
                        txtremaningdays.ForeColor = Color.Red;
                    }
                    else if (days <= 30)
                    {
                        txtremaningdays.Text = $"متبقي {days} يوم";
                        txtremaningdays.ForeColor = Color.Orange;
                    }
                    else
                    {
                        txtremaningdays.Text = $"متبقي {days} يوم";
                        txtremaningdays.ForeColor = Color.Green;
                    }
                }
                else
                {
                    txtremaningdays.Text = "—";
                    txtremaningdays.ForeColor = Color.Gray;
                }

            _loadingLot = false; // ← re-enable events



        }

        private void ShowLotDetails()
        {
            if (gridView1.FocusedRowHandle < 0) return;
            _currentLot = gridView1.GetRow(gridView1.FocusedRowHandle) as LotGridModel;
            if (_currentLot == null) return;
            LoadLot(_currentLot);
        }

      

        // ── Grid Events ──────────────────────────────────────────────
        private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            ShowLotDetails();
            var lot = gridView1.GetRow(gridView1.FocusedRowHandle) as LotGridModel;
            
        }
        private void gridView1_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (gridView1.FocusedColumn?.FieldName != "OperationName") return;

            e.Cancel = true;

            var lot = gridView1.GetFocusedRow() as LotGridModel;
            if (lot == null) return;

            string[] parts = (lot.OperationName ?? "").Split('\u001F');
            string projectName = parts.Length > 0 ? parts[0].Trim() : "";
            string lotName = parts.Length > 1 ? parts[1].Trim() : "";

            using (var dlg = new OperationNameEditDialog(projectName, lotName))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                lot.OperationName = $"{dlg.ProjectName}\u001F{dlg.LotName}";
                lot.LotName = dlg.LotName;

                foreach (var row in _data)
                {
                    if (row.ProjectId == lot.ProjectId && row.Id != lot.Id)
                    {
                        string[] p = (row.OperationName ?? "").Split('\u001F');
                        string oldLotName = p.Length > 1 ? p[1].Trim() : "";
                        row.OperationName = $"{dlg.ProjectName}\u001F{oldLotName}";
                    }
                }

                gridView1.RefreshData();

                gridView1_RowUpdated(sender, new RowObjectEventArgs(gridView1.FocusedRowHandle, lot));
            }
        }

        private void SidePanelLookup_Changed(object sender, EventArgs e)

        {
            if (_loadingLot) return; // ← ignore programmatic changes

            if (_currentLot == null) return;

            _currentLot.AdministrativeProcedureId = GetLookupValue(lookUp5);
            _currentLot.SpecialStatus1Id = GetLookupValue(lookUp6);
            _currentLot.SpecialStatus2Id = GetLookupValue(lookUp7);
            _currentLot.SpecialStatus3Id = GetLookupValue(lookUp8);

            // Save to database
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        var updatedLot = new Domain.Lot
                        {
                            Id = _currentLot.Id,
                            ProjectId = _currentLot.ProjectId,
                            LotNumber = _currentLot.LotNumber,
                            LotName = _currentLot.LotName,
                            LotBudget = _currentLot.LotBudget,
                            RegisteredAmount = _currentLot.RegisteredAmount,
                            ConsumedAmount = _currentLot.ConsumedAmount,
                            Contractor = _currentLot.Contractor,
                            ExecutionDuration = _currentLot.ExecutionDuration,
                            StartDate = _currentLot.StartDate,
                            PhysicalProgress = _currentLot.PhysicalProgress,
                            AdministrativeProcedureId = _currentLot.AdministrativeProcedureId,
                            SpecialStatus1Id = _currentLot.SpecialStatus1Id,
                            SpecialStatus2Id = _currentLot.SpecialStatus2Id,
                            SpecialStatus3Id = _currentLot.SpecialStatus3Id,
                            ProjectStatusId = _currentLot.ProjectStatusId,
                            Notes = _currentLot.Notes
                        };

                        _lotRepo.Update(updatedLot, conn, transaction);
                        transaction.Commit();
                        gridView1.RefreshRow(gridView1.FocusedRowHandle);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        XtraMessageBox.Show(
                            $"فشل الحفظ، تم التراجع.\n\n{ex.Message}",
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }


        }
        private void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "OperationName")
            {
                e.DisplayText = (e.Value?.ToString() ?? "")
                    .Replace('\u001F', '\n');
                  //  .Replace('|', '\n'); // safety fallback
                return;
            }

            if (e.Column.ColumnType == typeof(DateTime?))
            {
                DateTime? value = e.Value as DateTime?;
                if (value == null || !value.HasValue)
                    e.DisplayText = Properties.Resources.None;
            }
           
        }

        private void gridView1_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            if (e.RowHandle == gridView1.FocusedRowHandle && gridView1.FocusedColumn != e.Column)
            {
                e.Appearance.BackColor = gridView1.PaintAppearance.FocusedRow.BackColor;
                e.Appearance.ForeColor = gridView1.PaintAppearance.FocusedRow.ForeColor;
            }
        }

        public void ShowPreview()
        {
            if (gridView1.FocusedRowHandle < 0) return;
            var lot = gridView1.GetRow(gridView1.FocusedRowHandle) as LotGridModel;
            if (lot == null) return;

            using (var form = new frmeditproject(lot, FormMode.Preview))
            {
                form.ShowDialog();
                LoadData();
            }
        }

       
     
        // ── BaseModule overrides ─────────────────────────────────────
        protected override DevExpress.XtraGrid.GridControl Grid { get { return gridControl1; } }

        protected override void ShowReminder() { }

        protected internal override void ButtonClick(string tag) { }

        internal override void ShowModule(bool firstShow)
        {
            base.ShowModule(firstShow);
            if (firstShow)
            {
                GalleryItem item = OwnerForm.TaskGallery.Groups[0].Items[0];
                item.Checked = true;
                ButtonClick(string.Format("{0}", item.Tag));
            }
        }

        protected override void LookAndFeelStyleChanged()
        {
            base.LookAndFeelStyleChanged();
            ShowReminder();
        }

        internal override void ShowControlFirstTime()
        {
            GridHelper.SetFindControlImages(gridControl1);
        }

        internal override void FocusObject(object obj)
        {
            var view = gridControl1.MainView as DevExpress.XtraGrid.Views.Base.ColumnView;
            if (view != null)
                GridHelper.GridViewFocusObject(view, obj);
        }

        public void ExportGridToDesignerReport()
        {
            if (gridView1.RowCount == 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("لا توجد بيانات لتصديرها.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!(this.FindForm() is frmMain mainForm)) return;

            XtraReport report = BuildReportFromTemplateOrDefault();
            report.DisplayName = $"تقرير_{DateTime.Now:yyyyMMdd_HHmmss}";

            mainForm.SwitchToReportsAndLoad(report);

            if (mainForm.GetReportsModule() is ReportsModule reportsMod)
                reportsMod.SaveReportToPanel(report, report.DisplayName);
        }
        public void PrintStatusSummaryReport()
        {
            try
            {
                var allData = _lotRepo.GetGridData();
                var data = _selectedProgramId.HasValue
                    ? allData.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                    : allData;

                var report = StatusSummaryReportBuilder.Build(data);
                report.CreateDocument();
                report.ShowPreviewDialog();
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public override XtraReport GetPrintReport()
        {
            if (gridView1.RowCount == 0) return null;
            return BuildReportFromTemplateOrDefault();
        }
        private List<LotGridModel> GetVisibleGridData()
        {
            var visibleRows = new List<LotGridModel>();

            // view.DataRowCount holds the exact count of filtered data rows
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                // 'i' is already the correct row handle for the filtered list
                if (gridView1.GetRow(i) is LotGridModel row)
                {
                    visibleRows.Add(row);
                }
            }

            return visibleRows;
        }

        private void gridView1_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            // FIX: Ensure we are only customizing standard data rows. 
            // If e.RowHandle is a Footer (-2147483642) or Group row, exit immediately.
            if (e.RowHandle < 0) return;

            GridView view = sender as GridView;

            // Get current project id
            int currentProjectId = Convert.ToInt32(
                view.GetRowCellValue(e.RowHandle, "ProjectId"));

            // Check next row
            bool sameProject = false;

            if (e.RowHandle < view.RowCount - 1)
            {
                int nextProjectId = Convert.ToInt32(
                    view.GetRowCellValue(e.RowHandle + 1, "ProjectId"));

                sameProject = (currentProjectId == nextProjectId);
            }

            // Draw the cell normally
            e.DefaultDraw();

            // Draw bottom line
            Color lineColor = sameProject ? Color.White : Color.Black;
            int thickness = sameProject ? 0 : 1;

            using (Pen pen = new Pen(lineColor, thickness))
            {
                e.Cache.DrawLine(
                    pen,
                    new Point(e.Bounds.Left, e.Bounds.Bottom - 1),
                    new Point(e.Bounds.Right, e.Bounds.Bottom - 1)
                );
            }

            // Tells the grid we successfully handled this specific data cell's custom painting
            e.Handled = true;
        }

      

    }

    public class OperationNameEditDialog : XtraForm
    {
        public string ProjectName { get; private set; }
        public string LotName { get; private set; }

        private MemoEdit txtProject = new MemoEdit();
        private MemoEdit txtLot = new MemoEdit();

        public OperationNameEditDialog(string projectName, string lotName)
        {
            // ── Form settings ─────────────────────────────────────────
            Text = "تعديل اسم العملية";
            Width = 450;
            Height = 380;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Padding = new Padding(15);

            // ── Main layout ───────────────────────────────────────────
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                Padding = new Padding(10),
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));  // label project
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));   // txt project
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));  // label lot
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));   // txt lot
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));  // buttons

            // ── Labels ───────────────────────────────────────────────
            var lblProject = new LabelControl
            {
                Text = "اسم العملية",
                Dock = DockStyle.Fill,
                Appearance = { Font = new Font("Tahoma", 9, FontStyle.Bold) }
            };

            var lblLot = new LabelControl
            {
                Text = "اسم الحصة",
                Dock = DockStyle.Fill,
                Appearance = { Font = new Font("Tahoma", 9, FontStyle.Bold) }
            };

            // ── Text editors ─────────────────────────────────────────
            txtProject.Text = projectName;
            txtProject.Dock = DockStyle.Fill;
            txtProject.Properties.Appearance.Font = new Font("Tahoma", 9);
            txtProject.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            txtProject.Properties.ScrollBars = ScrollBars.None;

            txtLot.Text = lotName;
            txtLot.Dock = DockStyle.Fill;
            txtLot.Properties.Appearance.Font = new Font("Tahoma", 9);
            txtLot.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
            txtLot.Properties.ScrollBars = ScrollBars.None;

            // ── Buttons panel ─────────────────────────────────────────
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 5, 0, 0)
            };

            var btnOk = new SimpleButton
            {
                Text = "حفظ",
                Width = 90,
                Height = 32,
                Appearance = { Font = new Font("Tahoma", 9, FontStyle.Bold) }
            };

            var btnCancel = new SimpleButton
            {
                Text = "إلغاء",
                Width = 90,
                Height = 32,
                Appearance = { Font = new Font("Tahoma", 9) }
            };

            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtProject.Text))
                {
                    XtraMessageBox.Show("اسم العملية مطلوب", "تحذير",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProject.Focus();
                    return;
                }
                ProjectName = txtProject.Text.Trim();
                LotName = txtLot.Text.Trim();
                DialogResult = DialogResult.OK;
                Close();
            };

            btnCancel.Click += (s, e) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            // ── Accept / Cancel keys ──────────────────────────────────
            AcceptButton = btnOk;
            CancelButton = btnCancel;

            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);

            // ── Add to layout ─────────────────────────────────────────
            layout.Controls.Add(lblProject, 0, 0);
            layout.Controls.Add(txtProject, 0, 1);
            layout.Controls.Add(lblLot, 0, 2);
            layout.Controls.Add(txtLot, 0, 3);
            layout.Controls.Add(btnPanel, 0, 4);

            Controls.Add(layout);

            // ── Focus project on open ─────────────────────────────────
            Shown += (s, e) => txtProject.Focus();
        }
    }
}