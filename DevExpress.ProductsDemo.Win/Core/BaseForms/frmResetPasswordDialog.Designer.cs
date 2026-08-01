using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    partial class frmResetPasswordDialog
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

        private LabelControl lblPassword;
        private TextEdit txtPassword;
        private LabelControl lblConfirm;
        private TextEdit txtConfirm;
        private LabelControl lblError;
        private SimpleButton btnOk;
        private SimpleButton btnCancel;

        private void InitializeComponent()
        {
            this.lblPassword = new LabelControl();
            this.txtPassword = new TextEdit();
            this.lblConfirm = new LabelControl();
            this.txtConfirm = new TextEdit();
            this.lblError = new LabelControl();
            this.btnOk = new SimpleButton();
            this.btnCancel = new SimpleButton();

            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirm.Properties)).BeginInit();
            this.SuspendLayout();
            //
            // lblPassword
            //
            this.lblPassword.Location = new Point(230, 20);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Text = "كلمة المرور الجديدة";
            //
            // txtPassword
            //
            this.txtPassword.Location = new Point(20, 40);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new Size(320, 20);
            this.txtPassword.Properties.PasswordChar = '*';
            //
            // lblConfirm
            //
            this.lblConfirm.Location = new Point(230, 75);
            this.lblConfirm.Name = "lblConfirm";
            this.lblConfirm.Text = "تأكيد كلمة المرور";
            //
            // txtConfirm
            //
            this.txtConfirm.Location = new Point(20, 95);
            this.txtConfirm.Name = "txtConfirm";
            this.txtConfirm.Size = new Size(320, 20);
            this.txtConfirm.Properties.PasswordChar = '*';
            //
            // lblError
            //
            this.lblError.Location = new Point(20, 125);
            this.lblError.Name = "lblError";
            this.lblError.Text = "";
            this.lblError.ForeColor = Color.Red;
            this.lblError.Size = new Size(320, 20);
            //
            // btnOk
            //
            this.btnOk.Location = new Point(180, 150);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(90, 32);
            this.btnOk.Text = "حفظ";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new Point(80, 150);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(90, 32);
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // frmResetPasswordDialog
            //
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(380, 220);
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
            this.Name = "frmResetPasswordDialog";
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "إعادة تعيين كلمة المرور";

            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtConfirm.Properties)).EndInit();
            this.ResumeLayout(false);
        }
    }
}