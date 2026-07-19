using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmManagePrograms : XtraForm
    {
        private readonly ProgramsRepository _repo = new ProgramsRepository();
        private List<ProgramLookupItem> _data;

        private GridControl gridControl;
        private GridView gridView;

        public frmManagePrograms()
        {
            Text = "إدارة البرامج";
            Width = 650;
            Height = 480;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;

            BuildUi();
            LoadData();
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

            var btnAdd = new SimpleButton { Text = "+ إضافة", Width = 90 };
            var btnDelete = new SimpleButton { Text = "حذف", Width = 90 };
            var btnSave = new SimpleButton { Text = "حفظ الكل", Width = 90 };
            var btnClose = new SimpleButton { Text = "إغلاق", Width = 90 };

            btnAdd.Click += BtnAdd_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnClose.Click += (s, e) => Close();

            toolbar.Controls.Add(btnClose);
            toolbar.Controls.Add(btnSave);
            toolbar.Controls.Add(btnDelete);
            toolbar.Controls.Add(btnAdd);

            gridControl = new GridControl { Dock = DockStyle.Fill };
            gridView = new GridView(gridControl);
            gridControl.MainView = gridView;
            gridControl.ViewCollection.Add(gridView);

            gridView.OptionsBehavior.Editable = true;
            gridView.OptionsView.ShowGroupPanel = false;

            Controls.Add(gridControl);
            Controls.Add(toolbar);
        }

        private void LoadData()
        {
            _data = _repo.GetAll();
            gridControl.DataSource = _data;

            // Configure columns once data source is bound
            gridView.PopulateColumns();

            gridView.Columns["Id"].OptionsColumn.AllowEdit = false;
            gridView.Columns["Id"].Visible = false; // hide raw Id, keep it in the model only

            var typeColumn = gridView.Columns["Type"];
            var typeCombo = new RepositoryItemComboBox();
            typeCombo.Items.AddRange(new[] { "ADSEC", "CGSCL", "PSD" });
            typeCombo.TextEditStyle = TextEditStyles.DisableTextEditor;
            gridControl.RepositoryItems.Add(typeCombo);
            typeColumn.ColumnEdit = typeCombo;

            gridView.Columns["Year"].DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            gridView.Columns["Year"].DisplayFormat.FormatString = "0";

            gridView.Columns["IsClosed"].Caption = "مغلق";
            gridView.Columns["Type"].Caption = "النوع";
            gridView.Columns["Year"].Caption = "السنة";
            gridView.Columns["Name"].Caption = "الاسم";

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView.Columns)
                col.Width = 130;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var newProgram = new ProgramLookupItem
            {
                Id = 0, // 0 = not yet saved
                Type = "ADSEC",
                Year = DateTime.Now.Year,
                Name = $"ADSEC{DateTime.Now.Year}",
                IsClosed = false
            };
            _data.Add(newProgram);
            gridControl.RefreshDataSource();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (gridView.FocusedRowHandle < 0) return;
            var row = gridView.GetRow(gridView.FocusedRowHandle) as ProgramLookupItem;
            if (row == null) return;

            if (XtraMessageBox.Show($"حذف البرنامج \"{row.Name}\"؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            if (row.Id > 0)
                _repo.Delete(row.Id);

            _data.Remove(row);
            gridControl.RefreshDataSource();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            gridView.CloseEditor();
            gridView.UpdateCurrentRow();

            try
            {
                foreach (var program in _data)
                {
                    if (string.IsNullOrWhiteSpace(program.Type) || string.IsNullOrWhiteSpace(program.Name))
                    {
                        XtraMessageBox.Show("النوع والاسم مطلوبان لكل برنامج.", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (program.Id == 0)
                        _repo.Insert(program);
                    else
                        _repo.Update(program);
                }

                XtraMessageBox.Show("تم الحفظ بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // reload to pick up new Ids for inserted rows
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"فشل الحفظ:\n\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}