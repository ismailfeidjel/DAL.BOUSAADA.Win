using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Services;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraExport.Helpers;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Modules
{
    public partial class ProjectModule : BaseModule
    {
        public override string ModuleName => $"{_programType} - {Properties.Resources.TasksName}";


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
    new LookupRepository().GetPrograms(_programType).Cast<LookupItem>().ToList();

        private string _programType = "ADSEC"; // sensible default if no data passed (keeps old static tab working)
        private RepositoryItemLookUpEdit domainLookup;
        private RepositoryItemLookUpEdit sectorLookup;
        private RepositoryItemLookUpEdit programLookup;
        private RepositoryItemLookUpEdit statusLookup;
        private RepositoryItemLookUpEdit adminProcedureLookup;
        private RepositoryItemLookUpEdit specialStatus1Lookup;
        private RepositoryItemLookUpEdit specialStatus2Lookup;
        private RepositoryItemLookUpEdit specialStatus3Lookup;



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
        private Func<LotGridModel, bool> _currentLotFilter = null;
        // Stores the IDs of projects that have at least one matching lot


        private HashSet<int> _matchedProjectIds = new HashSet<int>(); private CriteriaOperator _lastCriteria = null;


        // ... inside your ProjectModule class ...

        private bool _isCustomFiltering = false;

        private void gridView1_ColumnFilterChanged(object sender, EventArgs e)
        {
            if (_isCustomFiltering) return;

            // Pass 1: The grid has naturally filtered the rows based on the user's input.
            // We loop through the currently visible data rows and collect their Project IDs.
            _matchedProjectIds.Clear();

            // In DevExpress, row handles 0 through (DataRowCount - 1) represent the filtered, visible data rows.
            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (gridView1.GetRow(i) is LotGridModel lot)
                {
                    _matchedProjectIds.Add(lot.ProjectId);
                }
            }

            // Pass 2: Re-evaluate row visibility to bring back the sibling lots
            _isCustomFiltering = true;

            // Save the user's cursor position so typing in the auto-filter row isn't interrupted
            var focusedColumn = gridView1.FocusedColumn;
            var focusedRow = gridView1.FocusedRowHandle;

            gridView1.RefreshData(); // This triggers CustomRowFilter below for the second pass

            // Restore focus to the filter row if the user was typing
            if (focusedRow == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle && focusedColumn != null)
            {
                gridView1.FocusedRowHandle = focusedRow;
                gridView1.FocusedColumn = focusedColumn;
                gridView1.ShowEditor();

                // Move the cursor to the end of the text they were typing
                if (gridView1.ActiveEditor is TextEdit editor && editor.Text != null)
                {
                    editor.SelectionStart = editor.Text.Length;
                }
            }

            _isCustomFiltering = false;
        }

        private void gridView1_CustomRowFilter(object sender, DevExpress.XtraGrid.Views.Base.RowFilterEventArgs e)
        {
            // Check if either AutoFilter/Criteria OR Find Panel search is active
            bool hasActiveFilter = !ReferenceEquals(gridView1.ActiveFilterCriteria, null)
                                || !string.IsNullOrEmpty(gridView1.FindFilterText);

            if (!hasActiveFilter)
                return;

            // Only intervene during custom second pass
            if (_isCustomFiltering)
            {
                var dataSource = gridView1.DataSource as System.Collections.IList;
                if (dataSource != null && e.ListSourceRow >= 0 && e.ListSourceRow < dataSource.Count)
                {
                    if (dataSource[e.ListSourceRow] is LotGridModel lot)
                    {
                        // Force this lot to be visible if its ProjectId was collected in Pass 1
                        e.Visible = _matchedProjectIds.Contains(lot.ProjectId);
                        e.Handled = true;
                    }
                }
            }
        }

        private void ApplyProjectLevelFilter(Func<LotGridModel, bool> lotPredicate)
        {
            _currentLotFilter = lotPredicate;

            if (lotPredicate == null)
            {
                // No filter — show everything
                gridControl1.DataSource = _data;
                gridView1.ActiveFilterString = "";
                return;
            }

            // Find which projects have at least one lot matching the predicate
            var matchingProjectIds = _data
                .Where(lotPredicate)
                .Select(r => r.ProjectId)
                .ToHashSet();

            // Show ALL lots for those projects — not just the matching lots
            var filtered = _data
                .Where(r => matchingProjectIds.Contains(r.ProjectId))
                .ToList();

            gridControl1.DataSource = filtered;
            gridView1.ActiveFilterString = ""; // clear grid-level filter since we filter at data level
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
                HideBorderField = "HideTopBorder",
                GenerateFooterRow = false,
                CustomFilterText = _currentFilterLabel,
                ProgramDisplayText = programName,

                FieldAliases = new Dictionary<string, string>
    {
        { "Program", "ProgramId" },
        { "ProjectStatus", "ProjectStatusId" },
        { "Domain", "DomainId" },
        { "Sector", "SectorId" }
    },

                FixedColumnWidths = new Dictionary<string, float>
    {
        { "__RowNumber__", 30f },
        { "OperationNumber", 60f },
        { "Daira", 50f },
        { "Commune", 50f },
        { "Program", 40f },
        { "ExpectedEndDate", 50f },
        { "LotBudget", 80f },
        { "RegisteredAmount", 80f },
        { "ConsumedAmount", 80f },
        { "Remaining", 80f },
        { "Contractor", 60f },
        { "StartDate", 65f },
        { "ExecutionDuration", 40f },
        { "PhysicalProgress", 40f },
        { "FinancialProgress", 40f },
        { "ProjectStatus", 40f },
        { "Domain", 45f },
        { "Sector", 45f }
    }
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

        //    private string LayoutPath =>
        //System.IO.Path.Combine(
        //    Application.StartupPath, "grid_layout_projects.xml");

        private string LayoutPath =>
    System.IO.Path.Combine(
        Application.StartupPath, $"grid_layout_projects_{_programType}.xml");

        public ProjectModule()
        {
            InitializeComponent();
            NotifyStartupCompleted();

        }
        RepositoryItemMemoEdit memo = new RepositoryItemMemoEdit();



        // ── Grid Setup ───────────────────────────────────────────────
        private void SetupGrid()
        {
            var imageList = new ImageList();
            imageList.ImageSize = new Size(16, 16);

            // Empty/blank
            var bmp0 = new Bitmap(16, 16);
            imageList.Images.Add(bmp0);


            // Red flag
            var bmp1 = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp1))
                g.FillRectangle(Brushes.Red, 0, 0, 16, 16);
            imageList.Images.Add(bmp1);

            // Yellow flag
            var bmp2 = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp2))
                g.FillRectangle(Brushes.Gold, 0, 0, 16, 16);
            imageList.Images.Add(bmp2);

            // Green flag
            var bmp3 = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp3))
                g.FillRectangle(Brushes.Green, 0, 0, 16, 16);
            imageList.Images.Add(bmp3);

            // Blue flag
            var bmp4 = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bmp4))
                g.FillRectangle(Brushes.Blue, 0, 0, 16, 16);
            imageList.Images.Add(bmp4);



            gridControl1.RepositoryItems.Add(memo);
            memo.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            memo.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            memo.AppearanceFocused.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            memo.AppearanceFocused.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDownFocused;
            gridView1.ShowingEditor += gridView1_ShowingEditor;
            gridView1.CalcRowHeight += gridView1_CalcRowHeight;

            gridView1.ColumnFilterChanged += gridView1_ColumnFilterChanged;
            gridView1.CustomRowFilter += gridView1_CustomRowFilter;




            gridControl1.MainView = gridView1;
            gridView1.OptionsView.ShowGroupPanel = true;
            gridView1.OptionsBehavior.Editable = true;
            gridView1.OptionsView.ShowIndicator = true;
            gridView1.OptionsSelection.MultiSelect = false;
            gridView1.ColumnWidthChanged += (s, e) => SaveLayout();
            gridView1.ColumnPositionChanged += (s, e) => SaveLayout();


            gridView1.Appearance.HeaderPanel.Font =
    new Font("Segoe UI", 8, FontStyle.Bold);
            gridView1.Appearance.Row.Font = new Font("Segoe UI", 9);
            gridView1.Appearance.FocusedRow.Font = new Font("Segoe UI", 9);
            gridView1.Appearance.SelectedRow.Font = new Font("Segoe UI", 9);

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
            AddCol("FlagsId", "الرايات", 80);
            // Create ImageComboBox tied to the imagelist
            var flagsCombo = new RepositoryItemImageComboBox();
            flagsCombo.SmallImages = imageList;  //
            flagsCombo.Items.AddRange(new DevExpress.XtraEditors.Controls.ImageComboBoxItem[]
            {
        new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 0, 0),
        new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 1, 1),
        new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 2, 2),
        new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 3, 3),
        new DevExpress.XtraEditors.Controls.ImageComboBoxItem("", 4, 4),
            });
            flagsCombo.GlyphAlignment = HorzAlignment.Center;
            gridControl1.RepositoryItems.Add(flagsCombo);
            gridView1.Columns["FlagsId"].ColumnEdit = flagsCombo;
            gridView1.Columns["FlagsId"].OptionsColumn.AllowEdit = true;
            gridView1.Columns["FlagsId"].Width = 60;

            AddCol("Daira", "الدائرة", 100);
            AddCol("Commune", "البلدية", 100);
            AddCol("ProgramId", "البرنامج", 110);
            AddCol("ExpectedEndDate", " الاجال", 120, "{0:dd/MM/yyyy}", FormatType.DateTime);
            gridView1.Columns["ExpectedEndDate"].ColumnEdit = dateEdit;
            gridView1.Columns["ExpectedEndDate"].OptionsColumn.AllowEdit = false;

            programLookup = new RepositoryItemLookUpEdit();
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
            if (gridView1.Columns["OperationName"] != null)
            {
                gridView1.Columns["OperationName"].FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            }
            gridView1.OptionsFind.FindFilterColumns = "OperationName";
            // gridView1.OptionsFind.AlwaysHighlightFindResults = true;
            gridView1.OptionsFind.Behavior = FindPanelBehavior.Filter;
            AddCol("DomainId", "القطاع", 110);
            domainLookup = new RepositoryItemLookUpEdit();
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
            sectorLookup = new RepositoryItemLookUpEdit();
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
            statusLookup = new RepositoryItemLookUpEdit();
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
            adminProcedureLookup = new RepositoryItemLookUpEdit();
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
            specialStatus1Lookup = new RepositoryItemLookUpEdit();
            specialStatus1Lookup.DataSource = new LookupRepository().GetAll("special_status1");
            specialStatus1Lookup.DisplayMember = "Name";
            specialStatus1Lookup.ValueMember = "Id";
            specialStatus1Lookup.ShowHeader = false;
            specialStatus1Lookup.NullText = "——";
            specialStatus1Lookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(specialStatus1Lookup);
            gridView1.Columns["SpecialStatus1Id"].ColumnEdit = specialStatus1Lookup;
            gridView1.Columns["SpecialStatus1Id"].OptionsColumn.AllowEdit = true;
            gridView1.OptionsBehavior.AllowIncrementalSearch = false;

            AddCol("SpecialStatus2Id", "الوضعية2", 130);
            specialStatus2Lookup = new RepositoryItemLookUpEdit();
            specialStatus2Lookup.DataSource = new LookupRepository().GetAll("special_status2");
            specialStatus2Lookup.DisplayMember = "Name";
            specialStatus2Lookup.ValueMember = "Id";
            specialStatus2Lookup.ShowHeader = false;
            specialStatus2Lookup.NullText = "—";
            specialStatus2Lookup.Columns.Add(new LookUpColumnInfo("Name", 200));
            gridControl1.RepositoryItems.Add(specialStatus2Lookup);
            gridView1.Columns["SpecialStatus2Id"].ColumnEdit = specialStatus2Lookup;
            gridView1.Columns["SpecialStatus2Id"].OptionsColumn.AllowEdit = true;

            AddCol("SpecialStatus3Id", "الوضعية3", 130);
            specialStatus3Lookup = new RepositoryItemLookUpEdit();
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

            //
            Core.Helpers.GridHelper.DisableSorting(gridView1,
    "OperationNumber",
    "FlagsId",
    "LotNumber",
    "Commune",
    "ProgramId",
    "ExpectedEndDate",
    "FinancialProgress",
    "OperationName",
    "DomainId",
    "SectorId",
    "LotBudget",
    "RegisteredAmount",
    "ConsumedAmount",
    "Remaining",
    "Contractor",
    "StartDate",
    "ExecutionDuration",
    "PhysicalProgress",
    "ProjectStatusId",
    "AdministrativeProcedureId",
    "SpecialStatus1Id",
    "SpecialStatus2Id",
    "SpecialStatus3Id",
    "Notes"
);




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


            // Subscribe to the custom calculation event
            gridView1.CustomSummaryCalculate -= gridView1_CustomSummaryCalculate;
            gridView1.CustomSummaryCalculate += gridView1_CustomSummaryCalculate;

            // Configure custom summary item
            var distinctProjectSummary = new DevExpress.XtraGrid.GridColumnSummaryItem(
                DevExpress.Data.SummaryItemType.Custom,
                "OperationName",
                "المشاريع: {0}"
            )
            {
                Tag = "DistinctProjectCount"
            };

            gridView1.Columns["OperationName"].Summary.Clear();
            gridView1.Columns["OperationName"].Summary.Add(distinctProjectSummary);


            gridView1.Columns["LotBudget"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "LotBudget", "{0:N2}");

            gridView1.Columns["ConsumedAmount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "ConsumedAmount", "{0:N2}");

            gridView1.Columns["RegisteredAmount"].Summary.Add(
                DevExpress.Data.SummaryItemType.Sum, "RegisteredAmount", "{0:N2}");

            gridView1.Columns["Remaining"].Summary.Add(
               DevExpress.Data.SummaryItemType.Sum, "Remaining", "{0:N2}");


            gridView1.Appearance.FooterPanel.Font =
    new Font("Tahoma", 8, FontStyle.Bold);

            gridView1.Appearance.FooterPanel.TextOptions.HAlignment =
                DevExpress.Utils.HorzAlignment.Center;
            gridView1.Appearance.FooterPanel.TextOptions.WordWrap =
    DevExpress.Utils.WordWrap.Wrap;  // ← add this

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
        /// <summary>
        /// 
        /// 
        // HashSet to keep track of unique projects during the summary calculation
        private readonly HashSet<int> _distinctProjectsForSummary = new HashSet<int>();

        private void gridView1_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            var summaryItem = e.Item as DevExpress.XtraGrid.GridSummaryItem;
            if (summaryItem == null || summaryItem.Tag?.ToString() != "DistinctProjectCount")
                return;

            // 1. Reset set on calculation start
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Start)
            {
                _distinctProjectsForSummary.Clear();
            }

            // 2. Fetch value via e.GetValue (DevExpress reliable API)
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Calculate)
            {
                object projectIdVal = e.GetValue("ProjectId");
                if (projectIdVal != null && projectIdVal != DBNull.Value)
                {
                    _distinctProjectsForSummary.Add(Convert.ToInt32(projectIdVal));
                }
            }

            // 3. Assign total count
            if (e.SummaryProcess == DevExpress.Data.CustomSummaryProcess.Finalize)
            {
                e.TotalValue = _distinctProjectsForSummary.Count;
            }
        }




        private LotGridModel _draggedRow;
        private Point _dragStartPoint;
        private bool _startupCompleted;

        private void NotifyStartupCompleted()
        {
            if (_startupCompleted)
                return;

            _startupCompleted = true;

            BeginInvoke(new Action(() =>
            {
                gridControl1.RefreshDataSource();

                gridView1.BestFitColumns();

                gridView1.RefreshData();

                Application.DoEvents();

                var mainForm = FindForm() as frmMain;

                mainForm?.CloseStartupSplash();
            }));
        }

        private void SetupProjectDragReorder()
        {
            gridControl1.AllowDrop = true;

            gridView1.MouseDown += GridView1_MouseDown_ForDrag;
            gridView1.MouseMove += GridView1_MouseMove_ForDrag;

            gridControl1.DragOver += GridControl1_DragOver;
            gridControl1.DragDrop += GridControl1_DragDrop;
        }

        private void GridView1_MouseDown_ForDrag(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            var hitInfo = gridView1.CalcHitInfo(new Point(e.X, e.Y));
            if (!hitInfo.InRow) { _draggedRow = null; return; }

            _draggedRow = gridView1.GetRow(hitInfo.RowHandle) as LotGridModel;
            _dragStartPoint = e.Location;
        }

        private void GridView1_MouseMove_ForDrag(object sender, MouseEventArgs e)
        {
            if (_draggedRow == null || e.Button != MouseButtons.Left) return;

            // Only start the actual drag once the mouse has moved a few pixels —
            // avoids triggering a drag on a simple click.
            if (Math.Abs(e.X - _dragStartPoint.X) < 5 && Math.Abs(e.Y - _dragStartPoint.Y) < 5)
                return;

            var rowToDrag = _draggedRow;
            _draggedRow = null; // consume — prevents re-triggering mid-drag

            gridControl1.DoDragDrop(rowToDrag, DragDropEffects.Move);
        }

        private void GridControl1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(LotGridModel))
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }
        private void GridControl1_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(LotGridModel))) return;

            var sourceRow = e.Data.GetData(typeof(LotGridModel)) as LotGridModel;
            if (sourceRow == null || _data == null) return;

            Point clientPoint = gridControl1.PointToClient(new Point(e.X, e.Y));
            var hitInfo = gridView1.CalcHitInfo(clientPoint);
            if (!hitInfo.InRow) return;

            var targetRow = gridView1.GetRow(hitInfo.RowHandle) as LotGridModel;
            if (targetRow == null || sourceRow.ProjectId == targetRow.ProjectId) return;

            var draggedLots = _data
                .Where(r => r.ProjectId == sourceRow.ProjectId)
                .OrderBy(r => r.LotNumber)
                .ToList();

            if (!draggedLots.Any()) return;

            foreach (var lot in draggedLots)
                _data.Remove(lot);

            int targetIndex = _data.IndexOf(targetRow);
            if (targetIndex < 0) targetIndex = _data.Count;

            // Use the GridView's own row info (via ViewInfo) to find the row's
            // vertical bounds — avoids the missing GetRowBounds API.
            var viewInfo = gridView1.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
            var rowInfo = viewInfo?.GetGridRowInfo(hitInfo.RowHandle);

            bool dropAfter = true; // sensible default if we can't determine bounds
            if (rowInfo != null)
            {
                int rowTop = rowInfo.Bounds.Top;
                int rowHeight = rowInfo.Bounds.Height;
                dropAfter = clientPoint.Y > (rowTop + rowHeight / 2);
            }

            if (dropAfter)
            {
                while (targetIndex < _data.Count && _data[targetIndex].ProjectId == targetRow.ProjectId)
                    targetIndex++;
            }
            else
            {
                while (targetIndex > 0 && _data[targetIndex - 1].ProjectId == targetRow.ProjectId)
                    targetIndex--;
            }

            _data.InsertRange(targetIndex, draggedLots);

            OnProjectRowsReordered();

            gridControl1.RefreshDataSource();
        }


        private void OnProjectRowsReordered()
        {
            var projectOrder = new Dictionary<int, int>();
            int order = 0;
            int? lastProjectId = null;

            foreach (var row in _data)
            {
                if (row.ProjectId != lastProjectId)
                {
                    order++;
                    lastProjectId = row.ProjectId;
                }

                if (!projectOrder.ContainsKey(row.ProjectId))
                {
                    projectOrder[row.ProjectId] = order;
                }

                row.SortOrder = order;
            }

            foreach (var kvp in projectOrder)
            {
                _projectRepo.UpdateSortOrder(kvp.Key, kvp.Value);
            }
        }

        //
        public void RefreshLookups()
        {
            var lookupRepo = new LookupRepository();

            programLookup.DataSource = lookupRepo.GetAll("programs");
            domainLookup.DataSource = lookupRepo.GetAll("domains");
            sectorLookup.DataSource = lookupRepo.GetAll("sectors");
            statusLookup.DataSource = lookupRepo.GetAll("project_statuses");
            adminProcedureLookup.DataSource = lookupRepo.GetAll("administrative_procedures");
            specialStatus1Lookup.DataSource = lookupRepo.GetAll("special_status1");
            specialStatus2Lookup.DataSource = lookupRepo.GetAll("special_status2");
            specialStatus3Lookup.DataSource = lookupRepo.GetAll("special_status3");

            // Side-panel lookups (lookUp5–lookUp8) also need refreshing —
            // BindSidePanel wraps the source list with a "— —" null row, so reuse it.
            BindSidePanel(lookUp5, lookupRepo.GetAll("administrative_procedures"));
            BindSidePanel(lookUp6, lookupRepo.GetAll("special_status1"));
            BindSidePanel(lookUp7, lookupRepo.GetAll("special_status2"));
            BindSidePanel(lookUp8, lookupRepo.GetAll("special_status3"));

            // Names shown in the grid (e.g. domain_name via SQL join) also need
            // a fresh pull, since they're baked into LotGridModel per row.
            LoadData();
        }
        private const int MinRowHeight = 70;

        private void gridView1_CalcRowHeight(object sender, DevExpress.XtraGrid.Views.Grid.RowHeightEventArgs e)
        {
            if (e.RowHandle == DevExpress.XtraGrid.GridControl.AutoFilterRowHandle)
                return; // leave the auto filter row at its default height
            if (e.RowHandle == -2147483642) // footer row
                e.RowHeight = Math.Max(e.RowHeight, 40);
            if (e.RowHeight < MinRowHeight)
                e.RowHeight = MinRowHeight;
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
                            Notes = lot.Notes,
                            FlagsId = lot.FlagsId

                        };

                        var updatedProject = new Domain.Project
                        {
                            Id = lot.ProjectId,
                            //   OperationNumber = lot.OperationNumber,
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
            if (data is string typeStr && !string.IsNullOrWhiteSpace(typeStr))
                _programType = typeStr;
            try
            {
                base.InitModule(manager, data);
                BuildDetailPanel();
                SetupGrid();
                SetupProjectDragReorder();
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

            // Re-apply the current project-level filter if one is active,
            // otherwise bind the full dataset directly
            ApplyProjectLevelFilter(_currentLotFilter);
            NotifyStartupCompleted();

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
            srcWithNull.Add(new LookupItem { Id = -1, Name = "—  —" });
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
                return;
            }

            if (e.RowHandle < 0) return;
            //
            if (e.Column.FieldName == "StartDate")
            {
                object dateVal = gridView1.GetRowCellValue(e.RowHandle, "ExpectedEndDate");
                if (dateVal != null && dateVal != DBNull.Value && DateTime.TryParse(dateVal.ToString(), out DateTime endDate))
                {
                    int daysRemaining = (int)(endDate - DateTime.Now).TotalDays;

                    if (daysRemaining < 0)
                    {
                        // أحمر: انتهت المدة
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else if (daysRemaining <= 30)
                    {
                        // برتقالي: قريبة الانتهاء
                        e.Appearance.ForeColor = Color.FromArgb(255, 140, 0);  // orange

                    }
                    else
                    {
                        // أخضر: مدة كافية
                        e.Appearance.ForeColor = Color.Green;
                    }
                    //  return;
                }
            }


            //





            // Highlight ProjectStatusId and Notes in yellow when the project is closed
            //  if ((e.Column.FieldName == "ProjectStatusId" || e.Column.FieldName == "Notes"))
            //{
            object statusVal = gridView1.GetRowCellValue(e.RowHandle, "ProjectStatusId");
            if (statusVal != null && Convert.ToInt32(statusVal) == 7) // 7 = Closed, matches StatusFilterClosed
            {
                e.Appearance.BackColor = Color.FromArgb(255, 245, 150); // soft yellow
                return;
            }
            //}
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
        public void ShowAdd()
        {
            using (var form = new frmaddproject())
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                {

                }
                LoadData();
            }
        }


        // ── BaseModule overrides ─────────────────────────────────────
        protected override DevExpress.XtraGrid.GridControl Grid { get { return gridControl1; } }

        protected override void ShowReminder() { }

        public void PrintCurrentGridReport()
        {
            if (gridView1.RowCount == 0)
            {
                XtraMessageBox.Show("لا توجد بيانات لتصديرها.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string currentFilter = gridView1.ActiveFilterString;
            LoadData();
            if (!string.IsNullOrEmpty(currentFilter))
            {
                gridView1.ActiveFilterString = currentFilter;
            }

            var report = BuildReportFromTemplateOrDefault();
            report.CreateDocument();
            report.ShowPreviewDialog();
        }
        private string _currentFilterLabel = "";

        private static readonly Dictionary<string, string> FilterTagLabels = new Dictionary<string, string>
        {
            ["StatusFilterClosed"] = " مغلقة",
            ["StatusFilterOverdueActive"] = " آجال الانجاز منتهية",
            ["StatusFilterOngoing"] = " جارية",
            ["StatusFilterUnregistered"] = " غير مسجلة",
            ["StatusFilterRegistered"] = " غير منطلقة",
            ["ClearFilter"] = "",
        };

        //-------------------------------------------------------------------------------------------------------------------
        private void SaveCurrentFilterAsNew()
        {
            var criteria = gridView1.ActiveFilterCriteria;
            if (criteria == null)
            {
                XtraMessageBox.Show("لا يوجد فلتر نشط. أنشئ فلتراً في الجدول أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var dlg = new SimpleInputDialog("حفظ الفلتر", "اسم الفلتر:"))
            {
                if (dlg.ShowDialog() != DialogResult.OK || string.IsNullOrWhiteSpace(dlg.InputText)) return;

                new Repositories.SavedFiltersRepository().Insert(dlg.InputText, criteria.ToString());
            }

            (OwnerForm as frmMain)?.RefreshSavedFilterGallery();
        }
        private void ApplySavedFilter(int filterId)
        {
            var saved = new Repositories.SavedFiltersRepository().GetAll().FirstOrDefault(f => f.Id == filterId);
            if (saved == null) return;
            try
            {
                gridView1.ActiveFilterCriteria = CriteriaOperator.Parse(saved.FilterCriteria);
            }
            catch
            {
                gridView1.ActiveFilterString = saved.FilterCriteria; // fallback if parsing fails
            }

            _currentFilterLabel = saved.Name; // ← show the saved filter's name in cellFilterText

            // Force the existing sibling-restore logic to run, since setting the
            // criteria in code isn't guaranteed to raise ColumnFilterChanged reliably.
            gridView1_ColumnFilterChanged(gridView1, EventArgs.Empty);
        }





        //---------------------------------------------------------------------------------------------------



        protected internal override void ButtonClick(string tag)
        {
            //   XtraMessageBox.Show($"Tag received: {tag}");   


            if (tag != null && tag.StartsWith("SavedFilter:"))
            {
                if (int.TryParse(tag.Substring("SavedFilter:".Length), out int filterId))
                    ApplySavedFilter(filterId);
                return;
            }

            switch (tag)
            {
                case "SaveCurrentFilter":
                    SaveCurrentFilterAsNew();
                    return; // don't fall through to the filter-label logic below
                case "PrintGrid":
                    PrintCurrentGridReport();
                    break;
                case "PrintStatusSummary":
                    PrintStatusSummaryReport();
                    break;

                case "PrintCommuneSummary":
                    PrintCommuneSummaryReport();
                    break;
                case "PrintCommuneSummary2":
                    PrintCommuneSummaryReport2();
                    break;
                case "PrintDairaSummary":
                    PrintDairaSummaryReport();
                    break;
                case "PrintStartedSummary":
                    PrintStartedSummaryReport();
                    break;
                case "PrintDomainSummary":
                    PrintDomainSummaryReport();
                    break;
                case "PrintFinancialConsumptionReport":
                    PrintFinancialConsumptionReport();
                    break;
                case "PrintProjectLifecycleReport":
                    PrintProjectLifecycleReport();
                    break;
                case "btnExportToPowerPoint_Click":
                    btnExportToPowerPoint_Click();
                    break;

                // existing status-filter tags, if not already handled elsewhere:
                case "StatusFilterClosed":
                    ApplyProjectLevelFilter(r => r.ProjectStatusId == 7);
                    break;
                case "StatusFilterOverdueActive":
                    ApplyProjectLevelFilter(r =>
                        r.ProjectStatusId != 7 &&
                        r.ExpectedEndDate.HasValue &&
                        r.ExpectedEndDate.Value < DateTime.Now &&
                        r.StartDate.HasValue);
                    break;

                case "StatusFilterOngoing":
                    ApplyProjectLevelFilter(r => r.ProjectStatusId == 3);
                    break;
                case "StatusFilterUnregistered":
                    ApplyProjectLevelFilter(r => r.ProjectStatusId == 1);
                    break;
                case "StatusFilterRegistered":
                    ApplyProjectLevelFilter(r => r.ProjectStatusId == 2);
                    break;
                case "ClearFilter":
                    ApplyProjectLevelFilter(null);

                    break;
            }
            _currentFilterLabel = FilterTagLabels.TryGetValue(tag, out string label) ? label : "";


        }





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


        public void PrintStatusSummaryReport()
        {
            try
            {
                LoadData();
                var allData = _lotRepo.GetGridData();
                var data = _selectedProgramId.HasValue
                    ? allData.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                    : allData;
                string programName = GetPrograms()
                    .FirstOrDefault(p => p.Id == _selectedProgramId)?.Name ?? "";

                var report = StatusSummaryReportBuilder.Build(data, programName);
                report.CreateDocument();
                report.ShowPreviewDialog();
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void PrintCommuneSummaryReport()
        {
            try
            {
                LoadData();
                var allData = _lotRepo.GetGridData();
                var data = _selectedProgramId.HasValue
                    ? allData.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                    : allData;

                string programName = GetPrograms()
                    .FirstOrDefault(p => p.Id == _selectedProgramId)?.Name ?? "";

                var report = CommuneSummaryReportBuilder.Build(data, programName);
                report.ShowPreviewDialog();   // ← no CreateDocument() here — Build() already produced the merged document
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void PrintFinancialConsumptionReport()
        {
            try
            {
                LoadData();
                var allData = _lotRepo.GetGridData();
                var data = _selectedProgramId.HasValue
                    ? allData.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                    : allData;

                string programName = GetPrograms()
                    .FirstOrDefault(p => p.Id == _selectedProgramId)?.Name ?? "";

                var report = FinancialConsumptionReportBuilder.Build(data, programName);
                report.ShowPreviewDialog();
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void PrintDomainSummaryReport()
        {
            try
            {
                var allData = _lotRepo.GetGridData();
                var data = _selectedProgramId.HasValue
                    ? allData.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                    : allData;

                string programName = GetPrograms()
                    .FirstOrDefault(p => p.Id == _selectedProgramId)?.Name ?? "";

                var report = DomainSummaryReportBuilder.Build(data, programName);
                report.CreateDocument();
                report.ShowPreviewDialog();
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void PrintCommuneSummaryReport2()
        {
            try
            {
                LoadData();
                var allData = _lotRepo.GetGridData();
                var data = _selectedProgramId.HasValue
                    ? allData.Where(r => r.ProgramId == _selectedProgramId.Value).ToList()
                    : allData;

                string programName = GetPrograms()
                    .FirstOrDefault(p => p.Id == _selectedProgramId)?.Name ?? "";

                var report = CommuneSummary2ReportBuilder.Build(data, programName);
                report.ShowPreviewDialog();   // ← no CreateDocument() here — Build() already produced the merged document
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void PrintDairaSummaryReport()
        {
            List<int> selectedProgramIds;

            using (var dlg = new frmSelectPrograms())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return; // user cancelled

                selectedProgramIds = dlg.SelectedProgramIds;
            }

            try
            {
                var allData = _lotRepo.GetGridData()
                    .Where(r => r.ProgramId.HasValue && selectedProgramIds.Contains(r.ProgramId.Value))
                    .ToList();

                var report = DairaSummaryReportBuilder.Build(allData);
                report.CreateDocument();
                report.ShowPreviewDialog();
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        public void PrintStartedSummaryReport()
        {
            List<int> selectedProgramIds;

            using (var dlg = new frmSelectPrograms())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return; // user cancelled

                selectedProgramIds = dlg.SelectedProgramIds;
            }

            try
            {
                var allData = _lotRepo.GetGridData()
                    .Where(r => r.ProgramId.HasValue && selectedProgramIds.Contains(r.ProgramId.Value))
                    .ToList();

                var report = StartedSummaryReportBuilder.Build(allData);
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
            // 1. Save the user's Live Grid filter
            string currentFilter = gridView1.ActiveFilterString;

            LoadData(); // ← re-pull fresh joined data from DB before printing

            // 2. Restore the filter
            if (!string.IsNullOrEmpty(currentFilter))
            {
                gridView1.ActiveFilterString = currentFilter;
            }
            return BuildReportFromTemplateOrDefault();
        }
        private List<LotGridModel> GetVisibleGridData()
        {
            var visibleRows = new List<LotGridModel>();

            for (int i = 0; i < gridView1.DataRowCount; i++)
            {
                if (gridView1.GetRow(i) is LotGridModel row && row.FlagsId != 4) // exclude Blue-flagged rows from printing
                {
                    visibleRows.Add(row);
                }
            }

            return visibleRows;
        }
        public void PrintProjectLifecycleReport()
        {
            List<int> selectedDairaIds;

            using (var dlg = new frmSelectDairas())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return; // user cancelled — don't generate anything

                selectedDairaIds = dlg.SelectedDairaIds;
            }

            try
            {
                var programs = GetPrograms().Cast<ProgramLookupItem>().ToList();
                var report = ProjectLifecycleReportBuilder.Build(gridView1, programs, programId =>
                {
                    var all = _lotRepo.GetGridData();
                    return all.Where(r =>
                        r.ProgramId == programId &&
                        (selectedDairaIds.Count == 0 || (r.DairaId.HasValue && selectedDairaIds.Contains(r.DairaId.Value)))
                    ).ToList();
                });
                report.ShowPreviewDialog();
            }
            catch (InvalidOperationException ex)
            {
                XtraMessageBox.Show(ex.Message, "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnExportToPowerPoint_Click()
        {
            List<int> selectedDairaIds;

            using (var dlg = new frmSelectDairas())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return; // user cancelled — don't generate anything

                selectedDairaIds = dlg.SelectedDairaIds;
            }
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "PowerPoint Presentation|*.pptx";
                dialog.Title = "تصدير التقرير إلى عرض تقديمي";
                dialog.FileName = "444.pptx";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var programs = GetPrograms().Cast<ProgramLookupItem>().ToList();



                        var report = ProjectLifecycleReportBuilder.Build(gridView1, programs, programId =>
                        {
                            var all = _lotRepo.GetGridData();
                            return all.Where(r =>
                                r.ProgramId == programId &&
                                (selectedDairaIds.Count == 0 || (r.DairaId.HasValue && selectedDairaIds.Contains(r.DairaId.Value)))
                            ).ToList();
                        });

                        // 2. Export to PPTX
                        PowerPointReportExporter.ExportReportToPptx(report, dialog.FileName);

                        // 3. Optionally open the file automatically for the user
                        System.Diagnostics.Process.Start(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        XtraMessageBox.Show(
                            $"فشل التصدير إلى PowerPoint.\n\n{ex.Message}",
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                    }
                }
            }
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