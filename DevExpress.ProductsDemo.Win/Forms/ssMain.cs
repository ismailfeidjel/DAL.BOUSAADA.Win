using System;
using System.Windows.Forms;
using DevExpress.DXperience.Demos;
using DevExpress.MailClient.Win;
using DevExpress.XtraSplashScreen;

namespace DevExpress.ProductsDemo.Win.Forms {
    public partial class ssMain : DemoSplashScreen {
        int dotCount = 0;
        public ssMain() {
            DevExpress.Utils.LocalizationHelper.SetCurrentCulture(DataHelper.ApplicationArguments);
            InitializeComponent();
            labelControl1.Text = string.Format("{0}{1}", labelControl1.Text, GetYearString());
            this.DemoText = "Bousaada";
            this.ProductText = "????? ??????? ????? ?????? ??????? ????????";
            Timer tmr = new Timer();
            tmr.Interval = 300;
           // tmr.Tick += new EventHandler(tmr_Tick);
            tmr.Start();
        }

        
        string GetDots(int count) {
            string ret = string.Empty;
            for(int i = 0; i < count; i++) ret += ".";
            return ret;
        }
        int GetYearString() {
            int ret = TutorialConstants.Now.Year;
            return (ret < 2012 ? 2012 : ret);
        }

        private void marqueeProgressBarControl1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void ssMain_Load(object sender, EventArgs e)
        {

        }

        private void pictureEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
