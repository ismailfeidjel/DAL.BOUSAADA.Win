using DevExpress.ProductsDemo.Win.Core.BaseForms;
using DevExpress.ProductsDemo.Win.Core.Helpers;
using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraGrid.Columns;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DevExpress.ProductsDemo.Win.Forms
{
    public partial class frmDomains : frmLookupBase<LookupItem>
    {
        private const string TableName = "domains";
        private readonly LookupRepository _repo = new LookupRepository();

        protected override string EntityName => "القطاع";

        public frmDomains()
        {
            Text = "إدارة القطاعات";
        }

        protected override List<LookupItem> GetData()
        {
            return _repo.GetAll(TableName);
        }

        protected override void ConfigureColumns()
        {
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.Configure(gridView);

            GridColumn colId = gridView.Columns["Id"];
            if (colId != null)
            {
                colId.OptionsColumn.AllowEdit = false;
                colId.Visible = false;
            }
            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.DisableSorting(gridView, "Name");


            DevExpress.ProductsDemo.Win.Core.Helpers.GridHelper.SetCaption(gridView, "Name", "الاسم");
        }

        protected override LookupItem CreateNew()
        {
            return new LookupItem(0, "");
        }

        protected override void Validate(LookupItem entity)
        {
            bool isUnique = !DataSource.Any(x =>
                x.Id != entity.Id &&
                string.Equals(x.Name?.Trim(), entity.Name?.Trim(), StringComparison.OrdinalIgnoreCase));

            string error = ValidationHelper.FirstError(
                (ValidationHelper.Required(entity.Name), "الاسم مطلوب."),
                (isUnique, "يوجد قطاع آخر بنفس الاسم.")
            );

            if (!string.IsNullOrEmpty(error))
            {
                DialogHelper.Validation(error);
                throw new SilentCancelException();

            }
        }

        protected override void Save(LookupItem entity)
        {
            if (entity.Id == 0)
                _repo.Insert(TableName, entity.Name);
            else
                _repo.Update(TableName, entity.Id, entity.Name);
        }

        protected override void Delete(LookupItem entity)
        {
            if (entity.Id > 0)
                _repo.Delete(TableName, entity.Id);
        }
    }

}