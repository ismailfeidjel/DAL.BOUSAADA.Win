using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    /// <summary>
    /// Reads any lookup table that has an `id` (int) and a `name` (varchar) column.
    /// Usage:  var repo = new LookupRepository();
    ///         List&lt;LookupItem&gt; items = repo.GetAll("programs");
    /// </summary>
    public class LookupRepository
    {
        private readonly DbHelper _db = new DbHelper();

        /// <summary>
        /// Returns all rows from <paramref name="tableName"/> ordered by name.
        /// </summary>
        public List<LookupItem> GetAll(string tableName)
        {
            var list = new List<LookupItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                // Table name is injected directly — never comes from user input,
                // so string interpolation here is safe.
                string sql = $"SELECT id, name FROM `{tableName}` ORDER BY name";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new LookupItem(
                            rd.GetInt32(0),
                            rd.GetString(1)
                        ));
                    }
                }
            }

            return list;
        }
    }
}