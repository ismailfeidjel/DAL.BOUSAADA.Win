using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmSelectPrograms : XtraForm
    {
        public List<int> SelectedProgramIds { get; private set; } = new List<int>();

        private CheckedListBoxControl checkedListBox;

        public frmSelectPrograms()
        {
            Text = "اختيار البرامج";
            Width = 400;
            Height = 450;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            BuildUi();
            LoadPrograms();
        }

        private void BuildUi()
        {
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8)
            };

            var btnOk = new SimpleButton { Text = "موافق", Width = 90, DialogResult = DialogResult.OK };
            var btnCancel = new SimpleButton { Text = "إلغاء", Width = 90, DialogResult = DialogResult.Cancel };
            var btnSelectAll = new SimpleButton { Text = "تحديد الكل", Width = 90 };
            var btnClearAll = new SimpleButton { Text = "إلغاء التحديد", Width = 90 };

            btnOk.Click += BtnOk_Click;
            btnSelectAll.Click += (s, e) => SetAllChecked(true);
            btnClearAll.Click += (s, e) => SetAllChecked(false);

            toolbar.Controls.Add(btnCancel);
            toolbar.Controls.Add(btnOk);
            toolbar.Controls.Add(btnClearAll);
            toolbar.Controls.Add(btnSelectAll);

            checkedListBox = new CheckedListBoxControl { Dock = DockStyle.Fill };

            Controls.Add(checkedListBox);
            Controls.Add(toolbar);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        private void LoadPrograms()
        {
            var programs = new LookupRepository().GetAll("programs");
            checkedListBox.DataSource = programs;
            checkedListBox.DisplayMember = "Name";
            checkedListBox.ValueMember = "Id";
        }

        private void SetAllChecked(bool value)
        {
            for (int i = 0; i < checkedListBox.Items.Count; i++)
                checkedListBox.SetItemChecked(i, value);
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            SelectedProgramIds = checkedListBox.CheckedItems
                .Cast<LookupItem>()
                .Select(item => item.Id)
                .ToList();

            if (SelectedProgramIds.Count == 0)
            {
                XtraMessageBox.Show("الرجاء اختيار برنامج واحد على الأقل.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None; // keep the dialog open
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}