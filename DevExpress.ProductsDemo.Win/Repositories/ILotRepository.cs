using DevExpress.ProductsDemo.Win.Domain;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public interface ILotRepository
    {
         List<LotGridModel> GetGridData();
        List<Lot> GetByProjectId(int projectId);

        Lot GetById(int id);

        int Insert(Lot lot);

        bool Update(Lot lot);

        bool Delete(int id);
    }
}