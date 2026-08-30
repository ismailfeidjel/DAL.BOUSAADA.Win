using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Core.BaseForms
{
    public partial class frmManageSavedFilters : XtraForm
    {
        private System.ComponentModel.IContainer components = null;
        private readonly SavedFiltersRepository _repo = new SavedFiltersRepository();
        private List<SavedFilterItem> _data;
        private readonly HashSet<int> _dirtyIds = new HashSet<int>();
        public frmManageSavedFilters()
        {
            InitializeComponent();
            SetupForm();

        }

        private void SetupForm()
        {
            Text = "إدارة الفلاتر المحفوظة";

            gridView.Columns.Clear();
            var colName = gridView.Columns.AddVisible("Name", "اسم الفلتر");
            colName.Width = 400;
            colName.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            colName.OptionsColumn.AllowEdit = true;

            gridView.OptionsView.ShowIndicator = true;
            gridView.CellValueChanged += GridView_CellValueChanged;

            btnRefresh.Click += (s, e) => LoadData();
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnClose.Click += (s, e) => Close();

            LoadData();
        }
        private void LoadData()
        {
            _data = _repo.GetAll();
            _dirtyIds.Clear();
            gridControl.DataSource = _data;
            SetStatus("جاهز", Color.Gray);
        }
        private void GridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (gridView.GetRow(e.RowHandle) is SavedFilterItem item)
            {
                _dirtyIds.Add(item.Id);
                SetStatus("توجد تعديلات غير محفوظة", Color.DarkOrange);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            int[] selectedHandles = gridView.GetSelectedRows();
            if (selectedHandles.Length == 0)
            {
                XtraMessageBox.Show("الرجاء تحديد فلتر واحد على الأقل.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itemsToDelete = selectedHandles
                .Select(h => gridView.GetRow(h) as SavedFilterItem)
                .Where(i => i != null)
                .ToList();

            if (itemsToDelete.Count == 0) return;

            string names = string.Join(", ", itemsToDelete.Select(i => i.Name));
            if (XtraMessageBox.Show($"حذف الفلاتر التالية؟\n\n{names}", "تأكيد الحذف",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            foreach (var item in itemsToDelete)
            {
                _repo.Delete(item.Id);
                _dirtyIds.Remove(item.Id);
            }

            LoadData();
            SetStatus("تم الحذف", Color.Green);
        }
        private void BtnSave_Click(object sender, EventArgs e)
        {
            gridView.CloseEditor();
            gridView.UpdateCurrentRow();

            if (_dirtyIds.Count == 0)
            {
                SetStatus("لا توجد تغييرات لحفظها", Color.Gray);
                return;
            }

            foreach (var item in _data.Where(i => _dirtyIds.Contains(i.Id)))
            {
                if (string.IsNullOrWhiteSpace(item.Name))
                {
                    XtraMessageBox.Show("اسم الفلتر لا يمكن أن يكون فارغاً.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _repo.Update(item.Id, item.Name.Trim());
            }

            _dirtyIds.Clear();
            SetStatus("تم الحفظ بنجاح", Color.Green);
        }

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }


        protected void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManageSavedFilters));
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.sidePanel1 = new DevExpress.XtraEditors.SidePanel();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.bbiNewTask = new DevExpress.XtraBars.BarButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.sidePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.lblStatus);
            this.layoutControl1.Controls.Add(this.gridControl);
            this.layoutControl1.Controls.Add(this.sidePanel1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(764, 475);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(12, 450);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(31, 13);
            this.lblStatus.StyleController = this.layoutControl1;
            this.lblStatus.TabIndex = 7;
            this.lblStatus.Text = "Ready";
            // 
            // gridControl
            // 
            this.gridControl.Location = new System.Drawing.Point(12, 104);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.gridControl.Size = new System.Drawing.Size(740, 342);
            this.gridControl.TabIndex = 6;
            this.gridControl.UseEmbeddedNavigator = true;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.OptionsNavigation.EnterMoveNextColumn = true;
            this.gridView.OptionsSelection.MultiSelect = true;
            this.gridView.OptionsView.EnableAppearanceEvenRow = true;
            this.gridView.OptionsView.EnableAppearanceOddRow = true;
            this.gridView.OptionsView.ShowAutoFilterRow = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            // 
            // sidePanel1
            // 
            this.sidePanel1.Controls.Add(this.btnSave);
            this.sidePanel1.Controls.Add(this.btnClose);
            this.sidePanel1.Controls.Add(this.btnRefresh);
            this.sidePanel1.Controls.Add(this.btnDelete);
            this.sidePanel1.Location = new System.Drawing.Point(12, 31);
            this.sidePanel1.Name = "sidePanel1";
            this.sidePanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.sidePanel1.Size = new System.Drawing.Size(740, 69);
            this.sidePanel1.TabIndex = 5;
            this.sidePanel1.Text = "sidePanel1";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance.BorderColor = System.Drawing.Color.Wheat;
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseBorderColor = true;
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSave.ImageOptions.SvgImage")));
            this.btnSave.Location = new System.Drawing.Point(637, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSave.Size = new System.Drawing.Size(93, 48);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "حفظ";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnClose.ImageOptions.SvgImage")));
            this.btnClose.Location = new System.Drawing.Point(15, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnClose.Size = new System.Drawing.Size(75, 48);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "غلق";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnRefresh.ImageOptions.SvgImage")));
            this.btnRefresh.Location = new System.Drawing.Point(167, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnRefresh.Size = new System.Drawing.Size(75, 48);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "تحديث";
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnDelete.ImageOptions.SvgImage")));
            this.btnDelete.Location = new System.Drawing.Point(319, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnDelete.Size = new System.Drawing.Size(75, 48);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "حذف";
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.simpleSeparator1,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(764, 475);
            this.Root.TextVisible = false;
            // 
            // simpleSeparator1
            // 
            this.simpleSeparator1.AllowHotTrack = false;
            this.simpleSeparator1.Location = new System.Drawing.Point(0, 0);
            this.simpleSeparator1.Name = "simpleSeparator1";
            this.simpleSeparator1.Size = new System.Drawing.Size(744, 1);
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.sidePanel1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 19);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(744, 73);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControl;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 92);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(744, 346);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 1);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(744, 18);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblStatus;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 438);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(744, 17);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // bbiNewTask
            // 
            this.bbiNewTask.Name = "bbiNewTask";
            // 
            // frmManageSavedFilters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(764, 475);
            this.Controls.Add(this.layoutControl1);
            this.DoubleBuffered = true;
            this.IconOptions.Image = global::DevExpress.ProductsDemo.Win.Properties.Resources.BO_Task;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(766, 1007);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(766, 507);
            this.Name = "frmManageSavedFilters";
            this.RightToLeftLayout = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmLookupBase";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.sidePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        protected XtraLayout.LayoutControl layoutControl1;
        protected XtraLayout.LayoutControlGroup Root;
        protected XtraLayout.EmptySpaceItem emptySpaceItem1;
        protected XtraLayout.SimpleSeparator simpleSeparator1;
        protected XtraEditors.LabelControl lblStatus;
        protected XtraGrid.GridControl gridControl;
        protected XtraGrid.Views.Grid.GridView gridView;
        protected XtraEditors.SidePanel sidePanel1;
        protected XtraEditors.SimpleButton btnClose;
        protected XtraEditors.SimpleButton btnRefresh;
        protected XtraEditors.SimpleButton btnDelete;
        protected XtraLayout.LayoutControlItem layoutControlItem1;
        protected XtraLayout.LayoutControlItem layoutControlItem2;
        private XtraBars.BarButtonItem bbiNewTask;
        protected SimpleButton btnSave;
        protected XtraLayout.LayoutControlItem layoutControlItem3;
    }
}
