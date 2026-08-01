// frmLogin.Designer.cs
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    partial class frmLogin
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private LabelControl lblTitle;
        private LabelControl lblUsername;
        private TextEdit txtUsername;
        private LabelControl lblPassword;
        private TextEdit txtPassword;
        private SimpleButton btnLogin;
        private LabelControl lblError;

        private void InitializeComponent()
        {
            this.lblTitle = new LabelControl();
            this.lblUsername = new LabelControl();
            this.txtUsername = new TextEdit();
            this.lblPassword = new LabelControl();
            this.txtPassword = new TextEdit();
            this.btnLogin = new SimpleButton();
            this.lblError = new LabelControl();

            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).BeginInit();
            this.SuspendLayout();

            this.lblTitle.Text = "تسجيل الدخول";
            this.lblTitle.Location = new Point(120, 20);
            this.lblTitle.Appearance.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblTitle.Name = "lblTitle";

            this.lblUsername.Text = "اسم المستخدم";
            this.lblUsername.Location = new Point(230, 80);
            this.lblUsername.Name = "lblUsername";

            this.txtUsername.Location = new Point(50, 100);
            this.txtUsername.Size = new Size(260, 24);
            this.txtUsername.Name = "txtUsername";

            this.lblPassword.Text = "كلمة المرور";
            this.lblPassword.Location = new Point(230, 140);
            this.lblPassword.Name = "lblPassword";

            this.txtPassword.Location = new Point(50, 160);
            this.txtPassword.Size = new Size(260, 24);
            this.txtPassword.Properties.PasswordChar = '*';
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.KeyDown += new KeyEventHandler(this.txtPassword_KeyDown);

            this.lblError.Text = "";
            this.lblError.Location = new Point(50, 195);
            this.lblError.ForeColor = Color.Red;
            this.lblError.Name = "lblError";

            this.btnLogin.Text = "دخول";
            this.btnLogin.Location = new Point(50, 225);
            this.btnLogin.Size = new Size(260, 36);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(360, 290);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmLogin";
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "تسجيل الدخول";
            this.AcceptButton = this.btnLogin;

            ((System.ComponentModel.ISupportInitialize)(this.txtUsername.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPassword.Properties)).EndInit();
            this.ResumeLayout(false);
        }
    }
}