namespace DevExpress.ProductsDemo.Win.Core.BaseForms
{
    partial class frmLookupBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.searchControl1 = new DevExpress.XtraEditors.SearchControl();
            this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
            this.searchControl = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
            this.sidePanel1 = new DevExpress.XtraEditors.SidePanel();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.btnNew = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.btnRefresh = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new DevExpress.XtraEditors.SimpleButton();
            this.gridControl = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lblStatus = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
            this.sidePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.AllowCustomization = false;
            this.layoutControl1.Controls.Add(this.lblStatus);
            this.layoutControl1.Controls.Add(this.gridControl);
            this.layoutControl1.Controls.Add(this.sidePanel1);
            this.layoutControl1.Controls.Add(this.searchControl1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.Root;
            this.layoutControl1.Size = new System.Drawing.Size(727, 475);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // searchControl1
            // 
            this.searchControl1.Location = new System.Drawing.Point(89, 12);
            this.searchControl1.Name = "searchControl1";
            this.searchControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Repository.ClearButton(),
            new DevExpress.XtraEditors.Repository.SearchButton()});
            this.searchControl1.Size = new System.Drawing.Size(626, 20);
            this.searchControl1.StyleController = this.layoutControl1;
            this.searchControl1.TabIndex = 4;
            // 
            // Root
            // 
            this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.Root.GroupBordersVisible = false;
            this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.searchControl,
            this.simpleSeparator1,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem1,
            this.layoutControlItem3});
            this.Root.Name = "Root";
            this.Root.Size = new System.Drawing.Size(727, 475);
            this.Root.TextVisible = false;
            // 
            // searchControl
            // 
            this.searchControl.Control = this.searchControl1;
            this.searchControl.Location = new System.Drawing.Point(0, 0);
            this.searchControl.Name = "searchControl";
            this.searchControl.Size = new System.Drawing.Size(707, 24);
            this.searchControl.TextSize = new System.Drawing.Size(65, 13);
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 25);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(707, 17);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // simpleSeparator1
            // 
            this.simpleSeparator1.AllowHotTrack = false;
            this.simpleSeparator1.Location = new System.Drawing.Point(0, 24);
            this.simpleSeparator1.Name = "simpleSeparator1";
            this.simpleSeparator1.Size = new System.Drawing.Size(707, 1);
            // 
            // sidePanel1
            // 
            this.sidePanel1.Controls.Add(this.btnClose);
            this.sidePanel1.Controls.Add(this.btnRefresh);
            this.sidePanel1.Controls.Add(this.btnDelete);
            this.sidePanel1.Controls.Add(this.btnSave);
            this.sidePanel1.Controls.Add(this.btnNew);
            this.sidePanel1.Location = new System.Drawing.Point(12, 54);
            this.sidePanel1.Name = "sidePanel1";
            this.sidePanel1.Size = new System.Drawing.Size(703, 65);
            this.sidePanel1.TabIndex = 5;
            this.sidePanel1.Text = "sidePanel1";
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.sidePanel1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 42);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(707, 69);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(3, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 0;
            this.btnNew.Text = "simpleButton1";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(158, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "simpleButton2";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(313, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "simpleButton3";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(468, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "simpleButton4";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(623, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "simpleButton5";
            // 
            // gridControl
            // 
            this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridControl.Location = new System.Drawing.Point(12, 123);
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.Size = new System.Drawing.Size(703, 323);
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
            this.gridView.OptionsView.ShowFooter = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.gridControl;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 111);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(707, 327);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
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
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.lblStatus;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 438);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(707, 17);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // frmLookupBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(727, 475);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmLookupBase";
            this.Text = "frmLookupBase";
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.searchControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
            this.sidePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private XtraLayout.LayoutControl layoutControl1;
        private XtraLayout.LayoutControlGroup Root;
        private XtraEditors.SearchControl searchControl1;
        private XtraLayout.LayoutControlItem searchControl;
        private XtraLayout.EmptySpaceItem emptySpaceItem1;
        private XtraLayout.SimpleSeparator simpleSeparator1;
        private XtraEditors.LabelControl lblStatus;
        private XtraGrid.GridControl gridControl;
        private XtraGrid.Views.Grid.GridView gridView;
        private XtraEditors.SidePanel sidePanel1;
        private XtraEditors.SimpleButton btnClose;
        private XtraEditors.SimpleButton btnRefresh;
        private XtraEditors.SimpleButton btnDelete;
        private XtraEditors.SimpleButton btnSave;
        private XtraEditors.SimpleButton btnNew;
        private XtraLayout.LayoutControlItem layoutControlItem1;
        private XtraLayout.LayoutControlItem layoutControlItem2;
        private XtraLayout.LayoutControlItem layoutControlItem3;
    }
}