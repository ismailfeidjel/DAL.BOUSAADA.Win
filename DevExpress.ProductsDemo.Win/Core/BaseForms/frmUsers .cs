using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Org.BouncyCastle.Asn1.Cmp;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmUsers : XtraForm
    {
        private readonly UserRepository _repo = new UserRepository();
        private BindingList<UserItem> _users;
        private bool _isDirty;

        public frmUsers()
        {
            InitializeComponent();
            Load += (s, e) => LoadUsers();
        }

        private void LoadUsers()
        {
            _users = new BindingList<UserItem>(_repo.GetAll());
            gridControl.DataSource = _users;

            GridColumn colId = gridView.Columns["Id"];
            if (colId != null) { colId.OptionsColumn.AllowEdit = false; colId.Visible = false; }

            GridColumn colPw = gridView.Columns["PlainPassword"];
            if (colPw != null) colPw.Visible = false; // never shown in the grid, ever

            GridColumn colUser = gridView.Columns["Username"];
            if (colUser != null) colUser.Caption = "اسم المستخدم";

            GridColumn colFull = gridView.Columns["FullName"];
            if (colFull != null) colFull.Caption = "الاسم الكامل";

            GridColumn colRole = gridView.Columns["Role"];
            if (colRole != null)
            {
                colRole.Caption = "الصلاحية";

                var roleCombo = new RepositoryItemComboBox();
                roleCombo.Items.AddRange(UserRoles.All);
                roleCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
                gridControl.RepositoryItems.Add(roleCombo);
                colRole.ColumnEdit = roleCombo;
            }

            GridColumn colActive = gridView.Columns["IsActive"];
            if (colActive != null) colActive.Caption = "نشط";

            foreach (GridColumn col in gridView.Columns)
            {
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            }

            _isDirty = false;
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            lblStatus.Text = $"عدد المستخدمين: {_users?.Count ?? 0}" + (_isDirty ? "   *تم التعديل*" : "");
        }

        private void gridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            _isDirty = true;
            UpdateStatus();
        }

        private void gridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            // no-op for now — placeholder if per-row detail panel is wanted later
        }

        // ── New user: password required up front, saved immediately ──
        private void btnNew_Click(object sender, EventArgs e)
        {
            using (var dlg = new frmNewUserDialog())
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                if (_repo.UsernameExists(dlg.Username))
                {
                    DialogHelper.Warning("اسم المستخدم موجود مسبقاً.", "تنبيه");
                    return;
                }

                var user = new UserItem
                {
                    Username = dlg.Username,
                    FullName = dlg.FullName,
                    Role = dlg.Role,
                    IsActive = true,
                    PlainPassword = dlg.Password
                };

                bool ok = DialogHelper.TryExecute(() => _repo.Insert(user));
                if (!ok) return;

                DialogHelper.Saved();
                LoadUsers();
            }
        }

        // ── Save: profile fields only (username/fullname/role/active) ──
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_isDirty) return;

            gridView.CloseEditor();
            gridView.UpdateCurrentRow();

            bool ok = DialogHelper.TryExecute(() =>
            {
                foreach (var user in _users)
                {
                    bool isUnique = _repo.UsernameExists(user.Username, user.Id) == false;

                    string error = ValidationHelper.FirstError(
                        (ValidationHelper.Required(user.Username), "اسم المستخدم مطلوب."),
                        (ValidationHelper.Required(user.FullName), "الاسم الكامل مطلوب."),
                        (isUnique, "اسم المستخدم مستخدم من طرف مستخدم آخر."),
                        (UserRoles.All.Contains(user.Role), "الرجاء اختيار صلاحية صحيحة.")
                    );

                    if (!string.IsNullOrEmpty(error))
                    {
                        DialogHelper.Validation(error);
                        throw new SilentCancelException();
                    }

                    _repo.Update(user);
                }
            });

            if (!ok) return;

            _isDirty = false;
            DialogHelper.Saved();
            LoadUsers();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var user = gridView.GetFocusedRow() as UserItem;
            if (user == null) return;

            if (CurrentSession.User != null && user.Id == CurrentSession.User.Id)
            {
                DialogHelper.Warning("لا يمكنك حذف حسابك الحالي.", "تنبيه");
                return;
            }

            if (!DialogHelper.ConfirmDelete(user.Username)) return;

            _repo.Delete(user.Id);
            _users.Remove(user);
        }

        private void btnResetPassword_Click(object sender, EventArgs e)
        {
            var user = gridView.GetFocusedRow() as UserItem;
            if (user == null)
            {
                DialogHelper.Warning("الرجاء اختيار مستخدم أولاً.", "تنبيه");
                return;
            }

            using (var dlg = new frmResetPasswordDialog(user.Username))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                bool ok = DialogHelper.TryExecute(() => _repo.ResetPassword(user.Id, dlg.NewPassword));
                if (!ok) return;

                DialogHelper.Info("تم تحديث كلمة المرور بنجاح.", "تم");
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (_isDirty && !DialogHelper.Confirm("سيتم فقد التعديلات غير المحفوظة. متابعة؟"))
                return;

            LoadUsers();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmUsers_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isDirty) return;

            var result = DialogHelper.ConfirmYesNoCancel("توجد تغييرات غير محفوظة. هل تريد الحفظ؟");

            if (result == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (result == DialogResult.Yes)
                btnSave_Click(sender, e);
        }
    }
}