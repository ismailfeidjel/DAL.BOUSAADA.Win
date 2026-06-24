using DevExpress.MailClient.Win;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Forms {
    public partial class frmaddproject : RibbonForm {
      
        public frmaddproject() {
            InitializeComponent();
        }
        public frmaddproject( IDXMenuManager menuManager) {
            InitializeComponent();
          
            InitMenuManager(menuManager);
            
        }


        void InitMenuManager(IDXMenuManager menuManager)
        {
            foreach (Control ctrl in lcMain.Controls)
            {
                BaseEdit edit = ctrl as BaseEdit;
                if (edit != null)
                {
                    edit.MenuManager = menuManager;
                }
            }
        }
        private void btnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (!ValidateChildren()) return;

            try
            {
                var project = new Project
                {
                    OperationNumber = txtOperationNumber.Text.Trim(),
                    OperationName = txtOperationName.Text.Trim(),
                    ProgramId = Convert.ToInt32(cmbProgram.EditValue),
                    DairaId = Convert.ToInt32(cmbDaira.EditValue),
                    CommuneId = Convert.ToInt32(cmbCommune.EditValue),
                    DomainId = Convert.ToInt32(cmbDomain.EditValue),
                    SectorId = Convert.ToInt32(cmbSector.EditValue),
                    Notes = string.IsNullOrWhiteSpace(txtNotes.Text)
                                        ? null
                                        : txtNotes.Text.Trim()
                };

                var repo = new ProjectRepository();
                int newId = repo.Insert(project);
                project.Id = newId;

                NewProject = project;       // expose result to caller
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show($"Save failed:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_ItemClick(object sender, ItemClickEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        // Caller reads this after ShowDialog returns OK
        public Project NewProject { get; private set; }




        private void ucContactInfo1_Load(object sender, EventArgs e)
        {

        }

        
    }
}
