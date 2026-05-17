using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
using System.IO;
using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;

namespace DevExpress.ProductsDemo.Win.Modules {
    public partial class SpreadsheetModule : BaseModule {

        public SpreadsheetModule() {
            InitializeComponent();
        }
                
        protected override bool AutoMergeRibbon { get { return true; } }
        public override bool AllowRtfTitle { get { return false; } }
        internal override void ShowModule(bool firstShow) {
            base.ShowModule(firstShow);
           // MainRibbon.SelectedPage = MainRibbon.MergedPages.GetPageByName(homeRibbonPage1.Name);
        }

        private void spreadsheetCommandBarButtonItem2_ItemClick(object sender, XtraBars.ItemClickEventArgs e)
        {

        }

        private void spreadsheetFormulaBarControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
