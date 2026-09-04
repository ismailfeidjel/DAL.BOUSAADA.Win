using DevExpress.XtraSplashScreen;
using System;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class SplashScreen1 : SplashScreen
    {
        public SplashScreen1()
        {
            InitializeComponent();
            this.labelCopyright.Text = "ismail feidjel © 2026-" + DateTime.Now.Year.ToString();
        }

        #region Overrides

        public override void ProcessCommand(Enum cmd, object arg)
        {
            base.ProcessCommand(cmd, arg);
        }

        #endregion

       
        private void labelCopyright_Click(object sender, EventArgs e)
        {

        }
    }
}