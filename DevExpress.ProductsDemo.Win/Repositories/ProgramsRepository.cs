using DevExpress.ProductsDemo.Win.Domain;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class ProgramsRepository
    {
        public List<ProgramLookupItem> GetAll()
        {
            var result = new List<ProgramLookupItem>();

            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Type, Year, Name, IsClosed FROM Programs ORDER BY Type, Year DESC";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new ProgramLookupItem
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Type = reader.GetString(reader.GetOrdinal("Type")),
                                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                IsClosed = reader.GetBoolean(reader.GetOrdinal("IsClosed"))
                            });
                        }
                    }
                }
            }

            return result;
        }

        public void Insert(ProgramLookupItem program)
        {
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Programs (Type, Year, Name, IsClosed) VALUES (@type, @year, @name, @closed)";
                    AddParam(cmd, "@type", program.Type);
                    AddParam(cmd, "@year", program.Year);
                    AddParam(cmd, "@name", program.Name);
                    AddParam(cmd, "@closed", program.IsClosed);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<string> GetDistinctTypes()
        {
            var result = new List<string>();
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DISTINCT Type FROM Programs ORDER BY Type";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            result.Add(reader.GetString(0));
                    }
                }
            }
            return result;
        }
        public void Update(ProgramLookupItem program)
        {
            using (var conn = new DbHelper().GetConnection())
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Programs SET Type=@type, Year=@year, Name=@name, IsClosed=@closed WHERE Id=@id";
                    AddParam(cmd, "@type", program.Type);
                    AddParam(cmd, "@year", program.Year);
                    AddParam(cmd, "@name", program.Name);
                    AddParam(cmd, "@closed", program.IsClosed);
                    AddParam(cmd, "@id", program.Id);
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
                    cmd.CommandText = "DELETE FROM Programs WHERE Id=@id";
                    AddParam(cmd, "@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AddParam(System.Data.IDbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }
    }
}