using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmResetPasswordDialog : XtraForm
    {
        public string NewPassword => txtPassword.Text;

        public frmResetPasswordDialog(string username)
        {
            InitializeComponent();

            // Username is dynamic per-call (not known at design time), so it's
            // set here rather than baked into InitializeComponent()
            Text = $"إعادة تعيين كلمة المرور — {username}";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 4)
            {
                lblError.Text = "كلمة المرور يجب أن تحتوي 4 أحرف على الأقل.";
                return;
            }

            if (NewPassword != txtConfirm.Text)
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