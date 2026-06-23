using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Controls
{
   public partial class LotDetailPanel : UserControl
{

    public void LoadLot(LotGridModel lot)
    {

        // View tab
        //lblViewLotName.Text     = lot.LotName ?? "-";
       // lblViewLotNumber.Text   = lot.LotNumber.ToString();
       // lblViewContractor.Text  = lot.Contractor ?? "-";
      //  lblViewDuration.Text    = lot.ExecutionDuration.HasValue
                                  //? lot.ExecutionDuration.Value + " يوم" : "-";
      //  lblViewStartDate.Text   = lot.StartDate.HasValue
                                  //? lot.StartDate.Value.ToString("yyyy-MM-dd") : "-";
      //  lblViewFinancial.Text   = lot.FinancialProgress.ToString("N2") + " %";
      //  lblViewRemaining.Text   = lot.Remaining.ToString("N2");
     //   progressFinancial.EditValue = (int)Math.Min(lot.FinancialProgress, 100);
      //  lblViewAdminProc.Text   = lot.AdministrativeProcedure ?? "-";
     //   lblViewStatus1.Text     = lot.SpecialStatus1 ?? "-";
        //lblViewStatus2.Text     = lot.SpecialStatus2 ?? "-";
        //lblViewStatus3.Text     = lot.SpecialStatus3 ?? "-";
        //lblViewNotes.Text       = lot.Notes ?? "-";

        //// Edit tab
        //txtLotName.Text      = lot.LotName ?? "";
        //txtContractor.Text   = lot.Contractor ?? "";
        //spnDuration.Value    = lot.ExecutionDuration ?? 0;
        //dtStartDate.DateTime = lot.StartDate ?? DateTime.Today;
        //cmbAdminProc.Text    = lot.AdministrativeProcedure ?? "";
        //txtStatus1.Text      = lot.SpecialStatus1 ?? "";
        //txtStatus2.Text      = lot.SpecialStatus2 ?? "";
        //txtStatus3.Text      = lot.SpecialStatus3 ?? "";
        //txtNotes.Text        = lot.Notes ?? "";

    }

    // Expose save event to parent
    public event EventHandler<LotSaveEventArgs> OnSave;

    private void btnSave_Click(object sender, EventArgs e)
    {
        OnSave?.Invoke(this, new LotSaveEventArgs
        {
            //LotName           = txtLotName.Text,
            //Contractor        = txtContractor.Text,
            //ExecutionDuration = (int)spnDuration.Value,
            //StartDate         = dtStartDate.DateTime,
            //AdminProcedure    = cmbAdminProc.Text,
            //SpecialStatus1    = txtStatus1.Text,
            //SpecialStatus2    = txtStatus2.Text,
            //SpecialStatus3    = txtStatus3.Text,
            //Notes             = txtNotes.Text
        });
    }

}

// Simple event args class
public class LotSaveEventArgs : EventArgs
{
    public string LotName           { get; set; }
    public string Contractor        { get; set; }
    public int    ExecutionDuration { get; set; }
    public DateTime StartDate       { get; set; }
    public string AdminProcedure    { get; set; }
    public string SpecialStatus1    { get; set; }
    public string SpecialStatus2    { get; set; }
    public string SpecialStatus3    { get; set; }
    public string Notes             { get; set; }
}
}
