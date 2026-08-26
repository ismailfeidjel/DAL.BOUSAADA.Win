using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraEditors;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public class frmSelectDairas : XtraForm
    {
        private readonly CheckedListBoxControl _checkList = new CheckedListBoxControl();

        public List<int> SelectedDairaIds { get; private set; } = new List<int>();

        public frmSelectDairas()
        {
            Text = "اختر الدوائر";
            Width = 350;
            Height = 450;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var dairas = new LookupRepository().GetAll("dairas");
            foreach (var d in dairas)
                _checkList.Items.Add(d, false);

            _checkList.Dock = DockStyle.Fill;

            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(8)
            };

            var btnSelectAll = new SimpleButton { Text = "تحديد الكل", Width = 90 };
            var btnClear = new SimpleButton { Text = "إلغاء التحديد", Width = 90 };
            var btnOk = new SimpleButton { Text = "موافق", Width = 90, DialogResult = DialogResult.OK };
            var btnCancel = new SimpleButton { Text = "إلغاء", Width = 90, DialogResult = DialogResult.Cancel };

            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < _checkList.Items.Count; i++)
                    _checkList.SetItemChecked(i, true);
            };

            btnClear.Click += (s, e) =>
            {
                for (int i = 0; i < _checkList.Items.Count; i++)
                    _checkList.SetItemChecked(i, false);
            };

            btnOk.Click += (s, e) =>
            {
                SelectedDairaIds = _checkList.CheckedItems
                    .Cast<DevExpress.XtraEditors.Controls.CheckedListBoxItem>()
                    .Select(x => ((LookupItem)x.Value).Id)
                    .ToList();
                DialogResult = DialogResult.OK;
                Close();
            };

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnClear);
            btnPanel.Controls.Add(btnSelectAll);

            Controls.Add(_checkList);
            Controls.Add(btnPanel);
        }
    }
}