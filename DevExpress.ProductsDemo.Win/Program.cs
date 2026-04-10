using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Data.Filtering;
using DevExpress.DXperience.Demos;
using DevExpress.MailClient.Win;
using DevExpress.MailDemo.Win;
using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace DevExpress.ProductsDemo.Win {
    static class Program {
      
        [STAThread]
        static void Main(string[] arguments) {
            if(!System.Windows.Forms.SystemInformation.TerminalServerSession && Screen.AllScreens.Length > 1)
                DevExpress.XtraEditors.WindowsFormsSettings.SetPerMonitorDpiAware();
            else
                DevExpress.XtraEditors.WindowsFormsSettings.SetDPIAware();
            AppHelper.WarmUp();
            AppDomain.CurrentDomain.AssemblyResolve += OnCurrentDomainAssemblyResolve;
            WindowsFormsSettings.ApplyDemoSettings();
            DataHelper.ApplicationArguments = arguments;
            System.Globalization.CultureInfo enUs = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = enUs;
            System.Threading.Thread.CurrentThread.CurrentUICulture = enUs;
            DevExpress.Utils.LocalizationHelper.SetCurrentCulture(DataHelper.ApplicationArguments);
            DevExpress.UserSkins.BonusSkins.Register();
            DevExpress.Utils.AppearanceObject.DefaultFont = new Font("Segoe UI", 8);
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle("Office 2019 Colorful");
            SkinManager.EnableFormSkins();
            EnumProcessingHelper.RegisterEnum<TaskStatus>();
            EnumProcessingHelper.RegisterEnum(typeof(TaskStatus), "DevExpress.ProductsDemo.Win.TaskStatus");
            MainFormHelper.InitTakeScreen(Environment.GetCommandLineArgs());
            if(!MainFormHelper.TakeScreens)
              //  SplashScreenManager.ShowSkinSplashScreen(Tutorials.ucOverviewPage.GetSVGLogoImage(), "ولاية بوسعادة متابعة البرامج التنموية");
                SplashScreenManager.ShowForm(typeof(SplashScreen1));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }
        //
        static Assembly OnCurrentDomainAssemblyResolve(object sender, ResolveEventArgs args) {
            string partialName = DevExpress.Utils.AssemblyHelper.GetPartialName(args.Name).ToLower();
            if(partialName == "entityframework" || partialName == "system.data.sqlite") {
                string path = Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), "..\\..\\bin", partialName + ".dll");
                return DevExpress.Data.Internal.SafeTypeResolver.GetOrLoadAssemblyFrom(path);
            }
            return null;

        }
    }
}
