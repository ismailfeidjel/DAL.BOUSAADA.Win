using DevExpress.XtraEditors.CustomEditor;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.UI
{
    public partial class DetailLotControl : UserControl, IAnyControlEdit
    {
        private TableLayoutPanel _layout;

        private Label _lblLotNameVal;
        private Label _lblLotNumberVal;
        private Label _lblContractorVal;
        private Label _lblExecutionDurationVal;
        private Label _lblStartDateVal;
        private Label _lblFinancialProgressVal;
        private Label _lblRemainingVal;
        private Label _lblAdministrativeProcedureVal;
        private Label _lblSpecialStatus1Val;
        private Label _lblSpecialStatus2Val;
        private Label _lblSpecialStatus3Val;

        public DetailLotControl()
        {
            InitializeControl();
        }

        private void InitializeControl()
        {
            this.RightToLeft = RightToLeft.Yes;
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);
            this.BackColor = Color.White;

            _layout = new TableLayoutPanel();
            _layout.Dock = DockStyle.Fill;
            _layout.ColumnCount = 4;
            _layout.RowCount = 6;
            _layout.AutoSize = false;

            for (int c = 0; c < 4; c++)
                _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            for (int r = 0; r < 6; r++)
                _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F / 6F));

            this.Controls.Add(_layout);

            int row = 0;

            _lblLotNameVal = AddRow(row++, "اسم الحصة", 0);
            _lblLotNumberVal = AddRow(row - 1, "رقم الحصة", 2);

            _lblContractorVal = AddRow(row++, "المقاول", 0);
            _lblExecutionDurationVal = AddRow(row - 1, "مدة الإنجاز", 2);

            _lblStartDateVal = AddRow(row++, "تاريخ الانطلاق", 0);
            _lblFinancialProgressVal = AddRow(row - 1, "التقدم المالي", 2);

            _lblRemainingVal = AddRow(row++, "الباقي", 0);
            _lblAdministrativeProcedureVal = AddRow(row - 1, "الإجراء الإداري", 2);

            _lblSpecialStatus1Val = AddRow(row++, "الوضعية الخاصة 1", 0);
            _lblSpecialStatus2Val = AddRow(row - 1, "الوضعية الخاصة 2", 2);

            _lblSpecialStatus3Val = AddRow(row++, "الوضعية الخاصة 3", 0);
        }

        /// <summary>
        /// Adds a "caption | value" pair starting at the given column.
        /// captionCol = 0 -> caption in col0, value in col1
        /// captionCol = 2 -> caption in col2, value in col3
        /// </summary>
        private Label AddRow(int rowIndex, string caption, int captionCol)
        {
            var lblCaption = new Label
            {
                Text = caption,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            var lblValue = new Label
            {
                Text = "-",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.Black,
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill,
                AutoSize = false
            };

            _layout.Controls.Add(lblCaption, captionCol, rowIndex);
            _layout.Controls.Add(lblValue, captionCol + 1, rowIndex);

            return lblValue;
        }

        public bool SupportsDraw => false;
        public bool AllowBorder => true;
        public bool AllowBitmapCache => false;

        public void SetupAsEditControl() { }
        public void SetupAsDrawControl() { }

        public Size CalcSize(Graphics g) => this.Size;

        public void Draw(DevExpress.Utils.Drawing.GraphicsCache cache,
            AnyControlEditViewInfo viewInfo)
        { }

        public string GetDisplayText(object editValue) => string.Empty;

        public bool IsNeededKey(KeyEventArgs e) => false;

        public bool AllowClick(Point p) => true;

        public event EventHandler EditValueChanged;

        private object _editValue;
        public object EditValue
        {
            get => _editValue;
            set
            {
                _editValue = value;
                if (value is LotGridModel lot)
                    SetData(lot);
                EditValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void SetData(LotGridModel lot)
        {
            if (lot == null) return;

            _lblLotNameVal.Text = lot.LotName ?? "-";
            _lblLotNumberVal.Text = lot.LotNumber.ToString();

            _lblContractorVal.Text = string.IsNullOrEmpty(lot.Contractor) ? "-" : lot.Contractor;
            _lblExecutionDurationVal.Text = lot.ExecutionDuration.HasValue
                ? lot.ExecutionDuration.Value + " يوم"
                : "-";

            _lblStartDateVal.Text = lot.StartDate.HasValue
                ? lot.StartDate.Value.ToString("yyyy-MM-dd")
                : "-";

            _lblFinancialProgressVal.Text = lot.FinancialProgress.ToString("N2") + " %";

            _lblRemainingVal.Text = lot.Remaining.ToString("N2");

            _lblAdministrativeProcedureVal.Text = string.IsNullOrEmpty(lot.AdministrativeProcedure)
                ? "-" : lot.AdministrativeProcedure;

            _lblSpecialStatus1Val.Text = string.IsNullOrEmpty(lot.SpecialStatus1) ? "-" : lot.SpecialStatus1;
            _lblSpecialStatus2Val.Text = string.IsNullOrEmpty(lot.SpecialStatus2) ? "-" : lot.SpecialStatus2;
            _lblSpecialStatus3Val.Text = string.IsNullOrEmpty(lot.SpecialStatus3) ? "-" : lot.SpecialStatus3;
        }
    }
}
