using DevExpress.ProductsDemo.Win.Core.BaseForms;
using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmPrograms : frmLookupBase<ProgramLookupItem>
    {
        private readonly ProgramsRepository _repo = new ProgramsRepository();

        protected override string EntityName => "البرنامج";

        public frmPrograms()
        {
            Text = "إدارة البرامج";
        }

        protected override List<ProgramLookupItem> GetData()
        {
            return _repo.GetAll();
        }

        protected override void ConfigureColumns()
        {
            // Use the new consolidated grid setup helper
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.Configure(gridView);

            GridColumn colId = gridView.Columns["Id"];
            if (colId != null)
            {
                colId.OptionsColumn.AllowEdit = false;
                colId.Visible = false;
            }
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.DisableSorting(gridView, "Type", "Year", "Name", "IsClosed");


            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.SetCaption(gridView, "Type", "النوع");
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.SetCaption(gridView, "Year", "السنة");
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.SetCaption(gridView, "Name", "الاسم");
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.SetCaption(gridView, "IsClosed", "مغلق");

            // Type column as a dropdown, matching the values used elsewhere (dynamic tabs, filters)
            var typeCombo = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
            typeCombo.Items.AddRange(new[] { "ADSEC", "CGSCL", "PSD" });
            typeCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            gridControl.RepositoryItems.Add(typeCombo);

            GridColumn colType = gridView.Columns["Type"];
            if (colType != null)
                colType.ColumnEdit = typeCombo;
        }

        protected override ProgramLookupItem CreateNew()
        {
            return new ProgramLookupItem
            {
                Id = 0,
                Type = "",
                Year = DateTime.Now.Year,
                Name = $"....{DateTime.Now.Year}",
                IsClosed = false
            };
        }

        protected override void Validate(ProgramLookupItem entity)
        {
            string error = ValidationHelper.FirstError(
                (ValidationHelper.Required(entity.Type), "النوع مطلوب."),
                (ValidationHelper.Required(entity.Name), "الاسم مطلوب."),
                (ValidationHelper.ValidYear(entity.Year), "السنة غير صحيحة."),
                (ValidationHelper.UniqueProgram(DataSource, entity), $"يوجد برنامج آخر من نوع {entity.Type} لسنة {entity.Year}.")
            );

            if (!string.IsNullOrEmpty(error))
            {
                DialogHelper.Validation(error);
                throw new SilentCancelException();

            }
        }

        protected override void Save(ProgramLookupItem entity)
        {
            if (entity.Id == 0)
                _repo.Insert(entity);
            else
                _repo.Update(entity);
        }

        protected override void Delete(ProgramLookupItem entity)
        {
            if (entity.Id > 0)
                _repo.Delete(entity.Id);
        }
    }
}