using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    partial class frmNewUserDialog
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

        private LabelControl lblUsername;
        private TextEdit txtUsername;
        private LabelControl lblFullName;
        private TextEdit txtFullName;
        private LabelControl lblRole;
        private ComboBoxEdit cmbRole;
        private LabelControl lblPassword;
        private TextEdit txtPassword;
        private LabelControl lblConfirm;
        private TextEdit txtConfirm;
        private LabelControl lblError;
        private SimpleButton btnOk;
        private SimpleButton btnCancel;

        private void InitializeComponent()
        {
            this.lblUsername = new LabelControl();
            this.txtUsername = new TextEdit();
            this.lblFullName = new LabelControl();
            this.txtFullName = new TextEdit();
            this.lblRole = new LabelControl();
            this.cmbRole = new ComboBoxEdit();
            this.lblPassword = new LabelControl();
            this.txtPassword = new TextEdit();
            this.lblConfirm = new LabelControl();
            this.txtConfirm = new TextEdit();
            this.lblError = new LabelControl();
            this.btnOk = new SimpleButton();
            this.btnCancel = new SimpleButton();

            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFullName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRole.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirm.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblUsername
            //
            this.lblUsername.Location = new Point(230, 20);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Text = "اسم المستخدم";
            //
            // txtUsername
            //
            this.txtUsername.Location = new Point(20, 40);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new Size(320, 20);
            //
            // lblFullName
            //
            this.lblFullName.Location = new Point(230, 75);
            this.lblFullName.Name = "lblFullName";
            this.lblFullName.Text = "الاسم الكامل";
            //
            // txtFullName
            //
            this.txtFullName.Location = new Point(20, 95);
            this.txtFullName.Name = "txtFullName";
            this.txtFullName.Size = new Size(320, 20);
            //
            // lblRole
            //
            this.lblRole.Location = new Point(230, 130);
            this.lblRole.Name = "lblRole";
            this.lblRole.Text = "الصلاحية";
            //
            // cmbRole
            //
            this.cmbRole.Location = new Point(20, 150);
            this.cmbRole.Name = "cmbRole";
            this.cmbRole.Size = new Size(320, 20);
            this.cmbRole.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            //
            // lblPassword
            //
            this.lblPassword.Location = new Point(230, 185);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Text = "كلمة المرور";
            //
            // txtPassword
            //
            this.txtPassword.Location = new Point(20, 205);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new Size(320, 20);
            this.txtPassword.Properties.PasswordChar = '*';
            //
            // lblConfirm
            //
            this.lblConfirm.Location = new Point(230, 240);
            this.lblConfirm.Name = "lblConfirm";
            this.lblConfirm.Text = "تأكيد كلمة المرور";
            //
            // txtConfirm
            //
            this.txtConfirm.Location = new Point(20, 260);
            this.txtConfirm.Name = "txtConfirm";
            this.txtConfirm.Size = new Size(320, 20);
            this.txtConfirm.Properties.PasswordChar = '*';
            //
            // lblError
            //
            this.lblError.Location = new Point(20, 290);
            this.lblError.Name = "lblError";
            this.lblError.Text = "";
            this.lblError.ForeColor = Color.Red;
            this.lblError.Size = new Size(320, 20);
            //
            // btnOk
            //
            this.btnOk.Location = new Point(180, 315);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(90, 32);
            this.btnOk.Text = "حفظ";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new Point(80, 315);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(90, 32);
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmNewUserDialog
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(380, 380);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblFullName);
            this.Controls.Add(this.txtFullName);
            this.Controls.Add(this.lblRole);
            this.Controls.Add(this.cmbRole);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblConfirm);
            this.Controls.Add(this.txtConfirm);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNewUserDialog";
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "مستخدم جديد";

            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFullName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cmbRole.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirm.Properties)).EndInit();
            this.ResumeLayout(false);
        }
    }
}