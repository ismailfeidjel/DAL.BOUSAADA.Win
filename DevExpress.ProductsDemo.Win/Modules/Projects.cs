using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraNavBar;
using System.Collections;
using DevExpress.Utils;
using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.MailClient.Win;
using DevExpress.MailDemo.Win;
using DevExpress.XtraGrid.Columns;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Services;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace DevExpress.ProductsDemo.Win.Modules {
    public partial class ProjectModule : BaseModule {
       
        public override string ModuleName { get { return Properties.Resources.TasksName; } }
        private readonly LotService _service;
        private List<LotGridModel> _data;
        public ProjectModule() {
            InitializeComponent();
            _service = new LotService();


        }
        private TextEdit txtLotName, txtLotNumber, txtContractor, txtFinancial, txtRemaining;
        private SpinEdit spnDuration;
        private DateEdit dtStartDate;
        private ComboBoxEdit cmbAdminProc, cmbStatus1, cmbStatus2, cmbStatus3;
        private MemoEdit txtNotes;
        private SimpleButton btnSave, btnCancel;
        private LotGridModel _currentLot;
        private LotRepository _lotRepo = new LotRepository();

        private void BuildDetailPanel()
        {
            var panel = new DevExpress.XtraEditors.PanelControl();
            panel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            panel.RightToLeft = RightToLeft.Yes;
            panel.Dock = DockStyle.None;
            panel.AutoSize = true;

            int y = 8;
            int labelWidth = 110;
            int editorWidth = panel.Width - labelWidth - 20;

            void AddRow(string labelText, Control editor)
            {
                var lbl = new DevExpress.XtraEditors.LabelControl();
                lbl.Text = labelText;
                lbl.Location = new Point(panel.Width - labelWidth - 5, y);
                lbl.Width = labelWidth;
                lbl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                panel.Controls.Add(lbl);

                editor.Location = new Point(5, y - 2);
                editor.Width = editorWidth;
                panel.Controls.Add(editor);

                y += 30;
            }

            txtLotName = new TextEdit();
            txtLotNumber = new TextEdit();
            txtContractor = new TextEdit();
            spnDuration = new SpinEdit();
            dtStartDate = new DateEdit();
            txtFinancial = new TextEdit();
            txtRemaining = new TextEdit();
            cmbAdminProc = new ComboBoxEdit();
            cmbStatus1 = new ComboBoxEdit();
            cmbStatus2 = new ComboBoxEdit();
            cmbStatus3 = new ComboBoxEdit();
            txtNotes = new MemoEdit();
            txtNotes.Height = 60;

            AddRow("اسم الحصة", txtLotName);
            AddRow("رقم الحصة", txtLotNumber);
            AddRow("المقاول", txtContractor);
            AddRow("مدة الإنجاز", spnDuration);
            AddRow("تاريخ الانطلاق", dtStartDate);
            AddRow("التقدم المالي", txtFinancial);
            AddRow("الباقي", txtRemaining);
            AddRow("الإجراء الإداري", cmbAdminProc);
            AddRow("الوضعية الخاصة 1", cmbStatus1);
            AddRow("الوضعية الخاصة 2", cmbStatus2);
            AddRow("الوضعية الخاصة 3", cmbStatus3);
            AddRow("الملاحظة", txtNotes);



            // Save / Cancel buttons
            btnCancel = new SimpleButton() { Text = "إلغاء", Width = 80, Location = new Point(5, y + 5) };
            btnSave = new SimpleButton() { Text = "حفظ", Width = 80, Location = new Point(90, y + 5) };
            btnSave.Appearance.BackColor = Color.FromArgb(0, 122, 204);
            btnSave.Appearance.ForeColor = Color.White;
            btnSave.LookAndFeel.UseDefaultLookAndFeel = false;
            btnSave.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;

            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;

            panel.Controls.Add(btnSave);
            panel.Controls.Add(btnCancel);

            // Embed panel into NavBarGroup via NavBarControlItem
            nbgProjet.NavBar.Controls.Add(panel);


            //panel.BringToFront();





            SetDetailReadOnly(true);
        }

        private void SetDetailReadOnly(bool readOnly)
        {
            txtLotName.Properties.ReadOnly = readOnly;
            txtContractor.Properties.ReadOnly = readOnly;
            spnDuration.Properties.ReadOnly = readOnly;
            dtStartDate.Properties.ReadOnly = readOnly;
            cmbAdminProc.Properties.ReadOnly = readOnly;
            cmbStatus1.Properties.ReadOnly = readOnly;
            cmbStatus2.Properties.ReadOnly = readOnly;
            cmbStatus3.Properties.ReadOnly = readOnly;
            txtNotes.Properties.ReadOnly = readOnly;

            txtLotNumber.Properties.ReadOnly = true; // always
            txtFinancial.Properties.ReadOnly = true; // computed
            txtRemaining.Properties.ReadOnly = true; // computed

            btnSave.Enabled = !readOnly;
            btnCancel.Enabled = !readOnly;
        }

        private void SetupGrid()
        {
            gridControl1.MainView = gridView1;

            gridView1.OptionsView.ShowGroupPanel = false;
            gridView1.OptionsBehavior.Editable = false;
            gridView1.OptionsView.ShowIndicator = true;
            gridView1.OptionsView.ShowDetailButtons = true;
            gridView1.OptionsDetail.ShowDetailTabs = false;
            gridView1.OptionsDetail.EnableMasterViewMode = true;
            gridView1.OptionsDetail.AllowExpandEmptyDetails = true;

            AddCol("OperationNumber", "رقم العملية", 110);
            AddCol("OperationName", "اسم العملية", 180);
            AddCol("Daira", "الدائرة", 100);
            AddCol("Commune", "البلدية", 100);
            AddCol("LotBudget", "مبلغ الحصة", 110, "{0:N2}");
            AddCol("RegisteredAmount", "المبلغ المسجل", 110, "{0:N2}");
            AddCol("ConsumedAmount", "المبلغ المستهلك", 110, "{0:N2}");
            AddCol("PhysicalProgress", "التقدم الفيزيائي", 100, "{0:N2} %");
            AddCol("ProjectStatus", "وضعية العملية", 110);
            AddCol("Notes", "الملاحظة", 150);
        }
        internal override void InitModule(DevExpress.Utils.Menu.IDXMenuManager manager, object data) {
            base.InitModule(manager, data);
            SetupGrid();
            LoadData();
            BuildDetailPanel();   // ← add this

            //NavBarGroup group = nbgProjet;
            
          //  group.NavBar.MouseMove += new MouseEventHandler(NavBar_MouseMove);
        }
        private void AddCol(string field, string caption, int width, string format = null)
        {
            GridColumn col = gridView1.Columns.AddVisible(field, caption);
            col.Width = width;
            col.OptionsColumn.AllowEdit = false;
            if (format != null)
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
            }
        }
        private void LoadData()
        {
            var repo = new LotRepository();
            _data = repo.GetGridData();
            gridControl1.DataSource = _data;
        }
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            UpdateCurrentTask();
            ShowLotDetails();
        }

        private void ShowLotDetails()
        {
            if (gridView1.FocusedRowHandle < 0) return;

            _currentLot = gridView1.GetRow(gridView1.FocusedRowHandle) as LotGridModel;
            if (_currentLot == null) return;

            txtLotName.Text = _currentLot.LotName ?? "";
            txtLotNumber.Text = _currentLot.LotNumber.ToString();
            txtContractor.Text = _currentLot.Contractor ?? "";
            spnDuration.Value = _currentLot.ExecutionDuration ?? 0;
            dtStartDate.DateTime = _currentLot.StartDate ?? DateTime.Today;
            txtFinancial.Text = _currentLot.FinancialProgress.ToString("N2") + " %";
            txtRemaining.Text = _currentLot.Remaining.ToString("N2");
            cmbAdminProc.Text = _currentLot.AdministrativeProcedure ?? "";
            cmbStatus1.Text = _currentLot.SpecialStatus1 ?? "";
            cmbStatus2.Text = _currentLot.SpecialStatus2 ?? "";
            cmbStatus3.Text = _currentLot.SpecialStatus3 ?? "";
            txtNotes.Text = _currentLot.Notes ?? "";

            nbgProjet.Expanded = true;
            SetDetailReadOnly(true);
        }
        private void gridView1_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 2 && e.RowHandle >= 0)
                SetDetailReadOnly(false);
        }
        void NavBar_MouseMove(object sender, MouseEventArgs e) {
         
        }
        void CreateNavBarItems(NavBarGroup group) {
          
        }
        protected override void ShowReminder() {
            
        }
      
        protected override DevExpress.XtraGrid.GridControl Grid { get { return gridControl1; } }
        internal override void ShowModule(bool firstShow) {
            base.ShowModule(firstShow);
            if(firstShow) {
                GalleryItem item = OwnerForm.TaskGallery.Groups[0].Items[0];
                item.Checked = true;
                ButtonClick(string.Format("{0}", item.Tag));
            }
        }
        protected override void LookAndFeelStyleChanged() {
            base.LookAndFeelStyleChanged();
            ShowReminder();
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_currentLot == null) return;

            var lot = _lotRepo.GetById(_currentLot.Id);
            lot.LotName = txtLotName.Text;
            lot.Contractor = txtContractor.Text;
            lot.ExecutionDuration = (int)spnDuration.Value;
            lot.StartDate = dtStartDate.DateTime;
            lot.Notes = txtNotes.Text;

           // _lotRepo.Update(lot);
            LoadData();
            SetDetailReadOnly(true);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ShowLotDetails();
            SetDetailReadOnly(true);
        }
        private void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e) {
            if(e.Column.ColumnType == typeof(DateTime?)) {
                DateTime? value = e.Value as DateTime?;
                if(value == null || !value.HasValue)
                    e.DisplayText = Properties.Resources.None;
            }
        }

        private void gridView1_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e) {
            if(e.Column == colComplete) {
                Task task = gridView1.GetRow(e.RowHandle) as Task;
                if(task != null) {
                    task.Complete = !task.Complete;
                    gridView1.CloseEditor();
                    gridView1.UpdateCurrentRow();
                }
            }
        }

     

        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e) {
            if(e.RowHandle < 0) return;
            Task task = gridView1.GetRow(e.RowHandle) as Task;
            if(task == null) return;
            if(task.Status == TaskStatus.Completed) {
                e.Appearance.Font = FontResources.StrikeoutFont;
                e.Appearance.ForeColor = ColorHelper.DisabledTextColor;
            }
            if(task.Status == TaskStatus.Deferred) 
                e.Appearance.ForeColor = ColorHelper.DisabledTextColor;
            if(task.Status == TaskStatus.WaitingOnSomeoneElse)
                e.Appearance.ForeColor = ColorHelper.WarningColor;
            if(task.Priority == 2 && task.Status != TaskStatus.Completed) 
                e.Appearance.Font = FontResources.BoldFont;
            if(task.Overdue)
                e.Appearance.ForeColor = ColorHelper.CriticalColor; 
        }
        protected internal override void ButtonClick(string tag) {
           
        }
        
       
        internal override void FocusObject(object obj) {
            ColumnView view = gridControl1.MainView as ColumnView;
            if(view != null)
                GridHelper.GridViewFocusObject(view, obj);
        }
        DialogResult EditTask(Task task) {
            if(task == null) return DialogResult.Ignore;
            DialogResult ret = DialogResult.Cancel;
            Cursor.Current = Cursors.WaitCursor;
            using(frmEditTask frm = new frmEditTask(task, OwnerForm.Ribbon)) {
                ret = frm.ShowDialog(OwnerForm);
            }
            UpdateCurrentTask();
            Cursor.Current = Cursors.Default;
            return ret;
        }
       
        bool AllowEdit {
            get {
                if(CurrentTask == null) return false;
                if(gridView1.SelectedRowsCount == 1) return gridView1.FocusedRowHandle == gridView1.GetSelectedRows()[0];
                return gridView1.SelectedRowsCount == 0;
            }
        }
        void LoadDefaultLayout() {
            gridView1.ClearGrouping();
            gridView1.ClearSorting();
            gridView1.ActiveFilterString = string.Empty;
            for(int i = 0; i < gridView1.Columns.Count; i++)
                if(gridView1.Columns[i] != colOverdue && gridView1.Columns[i].OptionsColumn.ShowInCustomizationForm)
                    gridView1.Columns[i].VisibleIndex = i;
        }
        Task CurrentTask {
            get {
                if(gridView1.FocusedRowHandle < 0) return null;
                return gridView1.GetRow(gridView1.FocusedRowHandle) as Task;
            }
        }
       
        private void gridView1_SelectionChanged(object sender, Data.SelectionChangedEventArgs e) {
            UpdateCurrentTask();
        }
        private void gridView1_ColumnFilterChanged(object sender, EventArgs e) {
            UpdateCurrentTask();
        }

        void UpdateCurrentTask() {
            if(OwnerForm == null) return;
            if(CurrentTask != null)
                OwnerForm.EnabledFlagButtons(true, AllowEdit, CurrentTask);
            else OwnerForm.EnabledFlagButtons(false, AllowEdit, null);
        }

       

        private void gridView1_KeyDown(object sender, KeyEventArgs e) {
            if(e.KeyData == Keys.Enter && gridView1.FocusedRowHandle >=0)
                EditTask(CurrentTask);
        }
        internal override void ShowControlFirstTime() {
            GridHelper.SetFindControlImages(gridControl1);
        }

        private void gridView1_RowCellStyle(object sender, XtraGrid.Views.Grid.RowCellStyleEventArgs e) {
            if(e.RowHandle == gridView1.FocusedRowHandle && gridView1.FocusedColumn != e.Column) {
                e.Appearance.BackColor = gridView1.PaintAppearance.FocusedRow.BackColor;
                e.Appearance.ForeColor = gridView1.PaintAppearance.FocusedRow.ForeColor;
            }
        }
    }
}
