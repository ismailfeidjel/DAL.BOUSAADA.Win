using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public interface ILotRepository
    {
         List<LotGridModel> GetGridData();
        List<LotGridModel> GetByProjectId(int projectId);

        Lot GetById(int id);

        int Insert(Lot lot);

        bool Update(Lot lot, MySqlConnection conn, MySqlTransaction transaction);

        bool Delete(int id);
    }
}