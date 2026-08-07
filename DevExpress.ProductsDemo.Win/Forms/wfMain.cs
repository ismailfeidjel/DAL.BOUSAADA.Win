using DevExpress.MailClient.Win;
using DevExpress.XtraWaitForm;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class wfMain : DemoWaitForm
    {
        public wfMain()
        {
            DevExpress.Utils.LocalizationHelper.SetCurrentCulture(DataHelper.ApplicationArguments);
            InitializeComponent();
            this.RightToLeft = RightToLeft.Yes;
            ProgressPanel.Caption = DevExpress.ProductsDemo.Win.Properties.Resources.ProgressPanelCaption;

            ProgressPanel.Description = DevExpress.ProductsDemo.Win.Properties.Resources.ProgressPanelDescription;
        }
    }
}
