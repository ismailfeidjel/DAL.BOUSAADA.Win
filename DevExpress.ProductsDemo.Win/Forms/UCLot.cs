using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class UCLot : XtraUserControl
    {
        public UCLot()
        {
            InitializeComponent();
        }

        public int LotNumber { get; set; }

        public string LotName
        {
            get => txtLotName.Text;
            set => txtLotName.Text = value;
        }

        public decimal LotBudget
        {
            get => Convert.ToDecimal(txtLotBudget.EditValue ?? 0);
            set => txtLotBudget.EditValue = value;
        }

        public decimal RegisteredAmount
        {
            get => Convert.ToDecimal(txtRegisteredAmount.EditValue ?? 0);
            set => txtRegisteredAmount.EditValue = value;
        }

        public decimal ConsumedAmount
        {
            get => Convert.ToDecimal(txtConsumedAmount.EditValue ?? 0);
            set => txtConsumedAmount.EditValue = value;
        }

        public string Contractor
        {
            get => txtContractor.Text;
            set => txtContractor.Text = value;
        }

        public decimal PhysicalProgress
        {
            get => spnPhysicalProgress.Value;
            set => spnPhysicalProgress.Value = value;
        }
    }
}
