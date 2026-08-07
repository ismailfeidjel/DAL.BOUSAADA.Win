// frmLogin.cs
using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmLogin : XtraForm
    {
        private readonly UserRepository _repo = new UserRepository();

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            AttemptLogin();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                AttemptLogin();
        }

        private void AttemptLogin()
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "الرجاء إدخال اسم المستخدم وكلمة المرور.";
                return;
            }

            var user = _repo.ValidateLogin(username, password);
            if (user == null)
            {
                lblError.Text = "اسم المستخدم أو كلمة المرور غير صحيحة.";
                txtPassword.Text = "";
                return;
            }

            CurrentSession.SignIn(user);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}