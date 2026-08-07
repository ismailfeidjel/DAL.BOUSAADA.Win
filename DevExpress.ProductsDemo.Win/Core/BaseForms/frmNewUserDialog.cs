using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmNewUserDialog : XtraForm
    {
        public string Username => txtUsername.Text.Trim();
        public string FullName => txtFullName.Text.Trim();
        public string Role => cmbRole.SelectedItem as string;
        public string Password => txtPassword.Text;

        public frmNewUserDialog()
        {
            InitializeComponent();

            cmbRole.Properties.Items.AddRange(UserRoles.All);
            cmbRole.SelectedIndex = Array.IndexOf(UserRoles.All, UserRoles.Viewer);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(FullName))
            {
                lblError.Text = "الرجاء تعبئة جميع الحقول.";
                return;
            }

            if (string.IsNullOrEmpty(Password) || Password.Length < 4)
            {
                lblError.Text = "كلمة المرور يجب أن تحتوي 4 أحرف على الأقل.";
                return;
            }

            if (Password != txtConfirm.Text)
            {
                lblError.Text = "كلمتا المرور غير متطابقتين.";
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}