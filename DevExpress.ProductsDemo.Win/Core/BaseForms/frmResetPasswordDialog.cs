using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class  frmResetPasswordDialog : XtraForm
    {
        public string NewPassword => txtPassword.Text;

        private TextEdit txtPassword, txtConfirm;
        private LabelControl lblError;
        private SimpleButton btnOk, btnCancel;

        public frmResetPasswordDialog(string username)
        {
            Text = $"إعادة تعيين كلمة المرور — {username}";
            Width = 380;
            Height = 220;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lblP = new LabelControl { Text = "كلمة المرور الجديدة", Location = new Point(230, 20) };
            txtPassword = new TextEdit { Location = new Point(20, 40), Width = 320 };
            txtPassword.Properties.PasswordChar = '*';

            var lblC = new LabelControl { Text = "تأكيد كلمة المرور", Location = new Point(230, 75) };
            txtConfirm = new TextEdit { Location = new Point(20, 95), Width = 320 };
            txtConfirm.Properties.PasswordChar = '*';

            lblError = new LabelControl { Text = "", Location = new Point(20, 125), ForeColor = Color.Red, Width = 320 };

            btnOk = new SimpleButton { Text = "حفظ", Location = new Point(180, 150), Width = 90 };
            btnCancel = new SimpleButton { Text = "إلغاء", Location = new Point(80, 150), Width = 90 };

            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            Controls.AddRange(new Control[] { lblP, txtPassword, lblC, txtConfirm, lblError, btnOk, btnCancel });

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void BtnOk_Click(object sender, EventArgs e)
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
    }
}