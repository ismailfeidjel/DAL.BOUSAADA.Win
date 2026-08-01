using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    partial class frmUsers
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private SidePanel sidePanel1;
        private SimpleButton btnNew, btnSave, btnDelete, btnResetPassword, btnRefresh, btnClose;
        private GridControl gridControl;
        private GridView gridView;
        private LabelControl lblStatus;

        private void InitializeComponent()
        {
            this.sidePanel1 = new SidePanel();
            this.btnNew = new SimpleButton();
            this.btnSave = new SimpleButton();
            this.btnDelete = new SimpleButton();
            this.btnResetPassword = new SimpleButton();
            this.btnRefresh = new SimpleButton();
            this.btnClose = new SimpleButton();
            this.gridControl = new GridControl();
            this.gridView = new GridView();
            this.lblStatus = new LabelControl();

            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.sidePanel1.SuspendLayout();
            this.SuspendLayout();

            //
            // sidePanel1
            //
            this.sidePanel1.Dock = DockStyle.Top;
            this.sidePanel1.Height = 55;
            this.sidePanel1.RightToLeft = RightToLeft.Yes;
            this.sidePanel1.Controls.Add(this.btnClose);
            this.sidePanel1.Controls.Add(this.btnRefresh);
            this.sidePanel1.Controls.Add(this.btnResetPassword);
            this.sidePanel1.Controls.Add(this.btnDelete);
            this.sidePanel1.Controls.Add(this.btnSave);
            this.sidePanel1.Controls.Add(this.btnNew);
            this.sidePanel1.Name = "sidePanel1";

            this.btnNew.Text = "جديد";
            this.btnNew.Location = new Point(660, 5);
            this.btnNew.Size = new Size(75, 44);
            this.btnNew.Name = "btnNew";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.Text = "حفظ";
            this.btnSave.Location = new Point(575, 5);
            this.btnSave.Size = new Size(75, 44);
            this.btnSave.Name = "btnSave";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.Text = "حذف";
            this.btnDelete.Location = new Point(490, 5);
            this.btnDelete.Size = new Size(75, 44);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.btnResetPassword.Text = "إعادة تعيين كلمة المرور";
            this.btnResetPassword.Location = new Point(350, 5);
            this.btnResetPassword.Size = new Size(130, 44);
            this.btnResetPassword.Name = "btnResetPassword";
            this.btnResetPassword.Click += new System.EventHandler(this.btnResetPassword_Click);

            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.Location = new Point(265, 5);
            this.btnRefresh.Size = new Size(75, 44);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            this.btnClose.Text = "غلق";
            this.btnClose.Location = new Point(15, 5);
            this.btnClose.Size = new Size(75, 44);
            this.btnClose.Name = "btnClose";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            //
            // gridControl
            //
            this.gridControl.Dock = DockStyle.Fill;
            this.gridControl.MainView = this.gridView;
            this.gridControl.Name = "gridControl";
            this.gridControl.RightToLeft = RightToLeft.Yes;
            this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { this.gridView });

            //
            // gridView
            //
            this.gridView.GridControl = this.gridControl;
            this.gridView.Name = "gridView";
            this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView_FocusedRowChanged);
            this.gridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView_CellValueChanged);

            //
            // lblStatus
            //
            this.lblStatus.Dock = DockStyle.Bottom;
            this.lblStatus.Text = "Ready";
            this.lblStatus.Name = "lblStatus";

            //
            // frmUsers
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(760, 480);
            this.Controls.Add(this.gridControl);
            this.Controls.Add(this.sidePanel1);
            this.Controls.Add(this.lblStatus);
            this.MinimumSize = new Size(760, 480);
            this.Name = "frmUsers";
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "إدارة المستخدمين";
            this.FormClosing += new FormClosingEventHandler(this.frmUsers_FormClosing);

            ((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.sidePanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}