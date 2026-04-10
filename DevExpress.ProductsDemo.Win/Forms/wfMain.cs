using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.MailClient.Win;
using DevExpress.XtraWaitForm;

namespace DevExpress.ProductsDemo.Win.Forms {
    public partial class wfMain : DemoWaitForm {
        public wfMain() {
            DevExpress.Utils.LocalizationHelper.SetCurrentCulture(DataHelper.ApplicationArguments);
            InitializeComponent();
            ProgressPanel.Caption = DevExpress.ProductsDemo.Win.Properties.Resources.ProgressPanelCaption;
            ProgressPanel.Description = DevExpress.ProductsDemo.Win.Properties.Resources.ProgressPanelDescription;
        }
    }
}
