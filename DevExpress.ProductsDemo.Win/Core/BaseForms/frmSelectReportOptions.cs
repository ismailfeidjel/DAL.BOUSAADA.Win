using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.ProductsDemo.Win.Services; // Ensure this is here to access LifecycleGrouping
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public class frmSelectReportOptions : XtraForm
    {
        private readonly CheckedListBoxControl _checkList = new CheckedListBoxControl();
        private readonly RadioGroup _radioGroup = new RadioGroup();

        public List<int> SelectedDairaIds { get; private set; } = new List<int>();
        public LifecycleGrouping SelectedGrouping { get; private set; }

        public frmSelectReportOptions()
        {
            Text = "خيارات التقرير";
            Width = 550;
            Height = 500;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            // 1. Grouping Options (Radio Group for Modern UI)
            _radioGroup.Properties.Items.Add(new RadioGroupItem(LifecycleGrouping.ByStage, "حسب المرحلة"));
            _radioGroup.Properties.Items.Add(new RadioGroupItem(LifecycleGrouping.ByDaira, "حسب الدائرة"));
            _radioGroup.Properties.Items.Add(new RadioGroupItem(LifecycleGrouping.ByCommune, "حسب البلدية"));
            _radioGroup.SelectedIndex = 0; // Default selection
            _radioGroup.Dock = DockStyle.Fill;
            _radioGroup.Properties.BorderStyle = BorderStyles.NoBorder;
            _radioGroup.Properties.Columns = 3; // Align items horizontally

            var groupPanel = new GroupControl { Text = "طريقة عرض التقرير", Dock = DockStyle.Top, Height = 80 };
            groupPanel.Padding = new Padding(5);
            groupPanel.Controls.Add(_radioGroup);

            // 2. Dairas Selection
            var dairas = new LookupRepository().GetAll("dairas");
            foreach (var d in dairas)
                _checkList.Items.Add(d, false);

            _checkList.Dock = DockStyle.Fill;
            _checkList.BorderStyle = BorderStyles.NoBorder;
            _checkList.Appearance.Font = new Font(_checkList.Appearance.Font.FontFamily, 12f, FontStyle.Bold);
            _checkList.ItemHeight = 32; // Gives the checkboxes more breathing room

            var dairaPanel = new GroupControl { Text = "اختيار الدوائر", Dock = DockStyle.Fill };
            dairaPanel.Padding = new Padding(5);
            dairaPanel.Controls.Add(_checkList);

            // 3. Modern Buttons Panel
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10, 10, 10, 0)
            };

            var btnSelectAll = new SimpleButton { Text = "تحديد الكل", Width = 100 };
            var btnClear = new SimpleButton { Text = "إلغاء التحديد", Width = 100 };
            var btnOk = new SimpleButton { Text = "موافق", Width = 100 };
            var btnCancel = new SimpleButton { Text = "إلغاء", Width = 100, DialogResult = DialogResult.Cancel };

            // Highlight the primary action button
            btnOk.Appearance.BackColor = Color.FromArgb(0, 120, 215);
            btnOk.Appearance.Options.UseBackColor = true;

            // Use built-in DevExpress methods for cleaner code
            btnSelectAll.Click += (s, e) => _checkList.CheckAll();
            btnClear.Click += (s, e) => _checkList.UnCheckAll();

            btnOk.Click += (s, e) =>
            {
                // Capture selected Dairas
                SelectedDairaIds = _checkList.CheckedItems
                    .Cast<CheckedListBoxItem>()
                    .Select(x => ((LookupItem)x.Value).Id)
                    .ToList();

                // Capture selected Grouping
                SelectedGrouping = (LifecycleGrouping)_radioGroup.EditValue;

                DialogResult = DialogResult.OK;
                Close();
            };

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            btnPanel.Controls.Add(btnOk);
            btnPanel.Controls.Add(btnCancel);
            btnPanel.Controls.Add(btnClear);
            btnPanel.Controls.Add(btnSelectAll);

            //// 4. Assemble the layout with proper spacing
            //var mainContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            //mainContainer.Controls.Add(dairaPanel);
            //mainContainer.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top }); // Spacer
            //mainContainer.Controls.Add(groupPanel);

            //Controls.Add(mainContainer);
            //Controls.Add(btnPanel);

            // 4. Assemble the layout with proper spacing
            var mainContainer = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            mainContainer.Controls.Add(groupPanel);
            mainContainer.Controls.Add(new Panel { Height = 10, Dock = DockStyle.Top }); // Spacer
            mainContainer.Controls.Add(dairaPanel);
            dairaPanel.BringToFront(); // Forces it to behave correctly as the "Fill" control

            Controls.Add(mainContainer);
            Controls.Add(btnPanel);
        }
    }
}