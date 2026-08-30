using DevExpress.ProductsDemo.Win.Domain;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class SavedFiltersRepository
    {
        public List<SavedFilterItem> GetAll()
        {
            var result = new List<SavedFilterItem>();
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, FilterCriteria FROM SavedFilters ORDER BY Name";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new SavedFilterItem
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                FilterCriteria = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            return result;
        }

        public void Insert(string name, string filterCriteria)
        {
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO SavedFilters (Name, FilterCriteria) VALUES (@name, @criteria)";
                    var p1 = cmd.CreateParameter(); p1.ParameterName = "@name"; p1.Value = name;
                    var p2 = cmd.CreateParameter(); p2.ParameterName = "@criteria"; p2.Value = filterCriteria;
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM SavedFilters WHERE Id = @id";
                    var p = cmd.CreateParameter(); p.ParameterName = "@id"; p.Value = id;
                    cmd.Parameters.Add(p);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}