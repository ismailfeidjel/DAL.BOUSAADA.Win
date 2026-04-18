using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace DevExpress.ProductsDemo.Win
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper()
        {
            _connectionString =
                "Server=localhost;Port=3306;Database=dbb;Uid=root;Pwd=;";
        }

        // 🔹 Create and return connection
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        // 🔹 Execute SELECT and return DataTable
        public DataTable GetData(string query, params MySqlParameter[] parameters)
        {
            using (var con = GetConnection())
            {
                using (var cmd = new MySqlCommand(query, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (var da = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // 🔹 Execute INSERT / UPDATE / DELETE
        public int Execute(string query, params MySqlParameter[] parameters)
        {
            using (var con = GetConnection())
            {
                con.Open();

                using (var cmd = new MySqlCommand(query, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        // 🔹 Execute scalar (e.g., COUNT, SUM)
        public object ExecuteScalar(string query, params MySqlParameter[] parameters)
        {
            using (var con = GetConnection())
            {
                con.Open();

                using (var cmd = new MySqlCommand(query, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
        }

        // 🔹 Test connection
        public bool TestConnection()
        {
            try
            {
                using (var con = GetConnection())
                {
                    con.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
