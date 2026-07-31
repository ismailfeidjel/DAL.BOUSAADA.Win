using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    partial class frmDairasCommunes
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private SplitContainerControl splitContainerControl1;

        private LabelControl lblDairasTitle;
        private FlowLayoutPanel dairaButtonsPanel;
        private SimpleButton btnNewDaira;
        private SimpleButton btnSaveDaira;
        private SimpleButton btnDeleteDaira;
        private GridControl gridDairas;
        private GridView viewDairas;

        private LabelControl lblCommunesTitle;
        private FlowLayoutPanel communeButtonsPanel;
        private SimpleButton btnNewCommune;
        private SimpleButton btnSaveCommune;
        private GridControl gridCommunes;
        private GridView viewCommunes;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDairasCommunes));
            this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
            this.gridDairas = new DevExpress.XtraGrid.GridControl();
            this.viewDairas = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dairaButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNewDaira = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveDaira = new DevExpress.XtraEditors.SimpleButton();
            this.btnDeleteDaira = new DevExpress.XtraEditors.SimpleButton();
            this.lblDairasTitle = new DevExpress.XtraEditors.LabelControl();
            this.gridCommunes = new DevExpress.XtraGrid.GridControl();
            this.viewCommunes = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.communeButtonsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnNewCommune = new DevExpress.XtraEditors.SimpleButton();
            this.btnSaveCommune = new DevExpress.XtraEditors.SimpleButton();
            this.btnDeleteCommune = new DevExpress.XtraEditors.SimpleButton();
            this.lblCommunesTitle = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).BeginInit();
            this.splitContainerControl1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).BeginInit();
            this.splitContainerControl1.Panel2.SuspendLayout();
            this.splitContainerControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridDairas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDairas)).BeginInit();
            this.dairaButtonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridCommunes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewCommunes)).BeginInit();
            this.communeButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerControl1
            // 
            this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerControl1.Horizontal = false;
            this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
            this.splitContainerControl1.Name = "splitContainerControl1";
            // 
            // splitContainerControl1.Panel1
            // 
            this.splitContainerControl1.Panel1.Controls.Add(this.gridDairas);
            this.splitContainerControl1.Panel1.Controls.Add(this.dairaButtonsPanel);
            this.splitContainerControl1.Panel1.Controls.Add(this.lblDairasTitle);
            // 
            // splitContainerControl1.Panel2
            // 
            this.splitContainerControl1.Panel2.Controls.Add(this.gridCommunes);
            this.splitContainerControl1.Panel2.Controls.Add(this.communeButtonsPanel);
            this.splitContainerControl1.Panel2.Controls.Add(this.lblCommunesTitle);
            this.splitContainerControl1.Size = new System.Drawing.Size(900, 550);
            this.splitContainerControl1.SplitterPosition = 260;
            this.splitContainerControl1.TabIndex = 0;
            // 
            // gridDairas
            // 
            this.gridDairas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDairas.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridDairas.Location = new System.Drawing.Point(0, 68);
            this.gridDairas.MainView = this.viewDairas;
            this.gridDairas.Name = "gridDairas";
            this.gridDairas.Size = new System.Drawing.Size(900, 192);
            this.gridDairas.TabIndex = 0;
            this.gridDairas.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewDairas});
            // 
            // viewDairas
            // 
            this.viewDairas.GridControl = this.gridDairas;
            this.viewDairas.Name = "viewDairas";
            this.viewDairas.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
            this.viewDairas.OptionsDragDrop.AllowDataReordering = false;
            this.viewDairas.OptionsFind.AllowFindPanel = false;
            this.viewDairas.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.viewDairas_FocusedRowChanged);
            this.viewDairas.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.viewDairas_CellValueChanged);
            // 
            // dairaButtonsPanel
            // 
            this.dairaButtonsPanel.Controls.Add(this.btnNewDaira);
            this.dairaButtonsPanel.Controls.Add(this.btnSaveDaira);
            this.dairaButtonsPanel.Controls.Add(this.btnDeleteDaira);
            this.dairaButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.dairaButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.dairaButtonsPanel.Location = new System.Drawing.Point(0, 19);
            this.dairaButtonsPanel.Name = "dairaButtonsPanel";
            this.dairaButtonsPanel.Size = new System.Drawing.Size(900, 49);
            this.dairaButtonsPanel.TabIndex = 1;
            // 
            // btnNewDaira
            // 
            this.btnNewDaira.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewDaira.Appearance.Options.UseFont = true;
            this.btnNewDaira.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnNewDaira.ImageOptions.SvgImage")));
            this.btnNewDaira.Location = new System.Drawing.Point(3, 3);
            this.btnNewDaira.Name = "btnNewDaira";
            this.btnNewDaira.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnNewDaira.Size = new System.Drawing.Size(70, 40);
            this.btnNewDaira.TabIndex = 0;
            this.btnNewDaira.Text = "جديد";
            this.btnNewDaira.Click += new System.EventHandler(this.btnNewDaira_Click);
            // 
            // btnSaveDaira
            // 
            this.btnSaveDaira.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveDaira.Appearance.Options.UseFont = true;
            this.btnSaveDaira.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSaveDaira.ImageOptions.SvgImage")));
            this.btnSaveDaira.Location = new System.Drawing.Point(79, 3);
            this.btnSaveDaira.Name = "btnSaveDaira";
            this.btnSaveDaira.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSaveDaira.Size = new System.Drawing.Size(70, 40);
            this.btnSaveDaira.TabIndex = 1;
            this.btnSaveDaira.Text = "حفظ";
            this.btnSaveDaira.Click += new System.EventHandler(this.btnSaveDaira_Click);
            // 
            // btnDeleteDaira
            // 
            this.btnDeleteDaira.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteDaira.Appearance.Options.UseFont = true;
            this.btnDeleteDaira.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnDeleteDaira.ImageOptions.SvgImage")));
            this.btnDeleteDaira.Location = new System.Drawing.Point(155, 3);
            this.btnDeleteDaira.Name = "btnDeleteDaira";
            this.btnDeleteDaira.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnDeleteDaira.Size = new System.Drawing.Size(70, 40);
            this.btnDeleteDaira.TabIndex = 2;
            this.btnDeleteDaira.Text = "حذف";
            this.btnDeleteDaira.Click += new System.EventHandler(this.btnDeleteDaira_Click);
            // 
            // lblDairasTitle
            // 
            this.lblDairasTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblDairasTitle.Appearance.Options.UseFont = true;
            this.lblDairasTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblDairasTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDairasTitle.Location = new System.Drawing.Point(0, 0);
            this.lblDairasTitle.Name = "lblDairasTitle";
            this.lblDairasTitle.Size = new System.Drawing.Size(900, 19);
            this.lblDairasTitle.TabIndex = 2;
            this.lblDairasTitle.Text = "الدوائر";
            // 
            // gridCommunes
            // 
            this.gridCommunes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCommunes.Location = new System.Drawing.Point(0, 68);
            this.gridCommunes.MainView = this.viewCommunes;
            this.gridCommunes.Name = "gridCommunes";
            this.gridCommunes.Size = new System.Drawing.Size(900, 212);
            this.gridCommunes.TabIndex = 0;
            this.gridCommunes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.viewCommunes});
            // 
            // viewCommunes
            // 
            this.viewCommunes.GridControl = this.gridCommunes;
            this.viewCommunes.Name = "viewCommunes";
            this.viewCommunes.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
            this.viewCommunes.OptionsDragDrop.AllowDataReordering = false;
            this.viewCommunes.OptionsFind.AllowFindPanel = false;
            this.viewCommunes.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.viewCommunes_CellValueChanged);
            // 
            // communeButtonsPanel
            // 
            this.communeButtonsPanel.Controls.Add(this.btnNewCommune);
            this.communeButtonsPanel.Controls.Add(this.btnSaveCommune);
            this.communeButtonsPanel.Controls.Add(this.btnDeleteCommune);
            this.communeButtonsPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.communeButtonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.communeButtonsPanel.Location = new System.Drawing.Point(0, 19);
            this.communeButtonsPanel.Name = "communeButtonsPanel";
            this.communeButtonsPanel.Size = new System.Drawing.Size(900, 49);
            this.communeButtonsPanel.TabIndex = 1;
            this.communeButtonsPanel.WrapContents = false;
            // 
            // btnNewCommune
            // 
            this.btnNewCommune.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNewCommune.Appearance.Options.UseFont = true;
            this.btnNewCommune.Enabled = false;
            this.btnNewCommune.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnNewCommune.ImageOptions.SvgImage")));
            this.btnNewCommune.Location = new System.Drawing.Point(3, 3);
            this.btnNewCommune.Name = "btnNewCommune";
            this.btnNewCommune.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnNewCommune.Size = new System.Drawing.Size(70, 40);
            this.btnNewCommune.TabIndex = 0;
            this.btnNewCommune.Text = "جديد";
            this.btnNewCommune.Click += new System.EventHandler(this.btnNewCommune_Click);
            // 
            // btnSaveCommune
            // 
            this.btnSaveCommune.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveCommune.Appearance.BorderColor = System.Drawing.Color.Wheat;
            this.btnSaveCommune.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveCommune.Appearance.Options.UseBorderColor = true;
            this.btnSaveCommune.Appearance.Options.UseFont = true;
            this.btnSaveCommune.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveCommune.Enabled = false;
            this.btnSaveCommune.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnSaveCommune.ImageOptions.SvgImage")));
            this.btnSaveCommune.Location = new System.Drawing.Point(79, 3);
            this.btnSaveCommune.Name = "btnSaveCommune";
            this.btnSaveCommune.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnSaveCommune.Size = new System.Drawing.Size(70, 40);
            this.btnSaveCommune.TabIndex = 1;
            this.btnSaveCommune.Text = "حفظ";
            this.btnSaveCommune.Click += new System.EventHandler(this.btnSaveCommune_Click);
            // 
            // btnDeleteCommune
            // 
            this.btnDeleteCommune.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteCommune.Appearance.Options.UseFont = true;
            this.btnDeleteCommune.ImageOptions.SvgImage = ((DevExpress.Utils.Svg.SvgImage)(resources.GetObject("btnDeleteCommune.ImageOptions.SvgImage")));
            this.btnDeleteCommune.Location = new System.Drawing.Point(155, 3);
            this.btnDeleteCommune.Name = "btnDeleteCommune";
            this.btnDeleteCommune.PaintStyle = DevExpress.XtraEditors.Controls.PaintStyles.Light;
            this.btnDeleteCommune.Size = new System.Drawing.Size(70, 40);
            this.btnDeleteCommune.TabIndex = 3;
            this.btnDeleteCommune.Text = "حذف";
            this.btnDeleteCommune.Click += new System.EventHandler(this.btnDeleteCommune_Click);
            // 
            // lblCommunesTitle
            // 
            this.lblCommunesTitle.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblCommunesTitle.Appearance.Options.UseFont = true;
            this.lblCommunesTitle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCommunesTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCommunesTitle.Location = new System.Drawing.Point(0, 0);
            this.lblCommunesTitle.Name = "lblCommunesTitle";
            this.lblCommunesTitle.Size = new System.Drawing.Size(900, 19);
            this.lblCommunesTitle.TabIndex = 2;
            this.lblCommunesTitle.Text = "البلديات — اختر دائرة";
            // 
            // frmDairasCommunes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 550);
            this.Controls.Add(this.splitContainerControl1);
            this.MinimumSize = new System.Drawing.Size(902, 582);
            this.Name = "frmDairasCommunes";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "إدارة الدوائر والبلديات";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDairasCommunes_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel1)).EndInit();
            this.splitContainerControl1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1.Panel2)).EndInit();
            this.splitContainerControl1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
            this.splitContainerControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridDairas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewDairas)).EndInit();
            this.dairaButtonsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridCommunes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewCommunes)).EndInit();
            this.communeButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private SimpleButton btnDeleteCommune;
    }
}