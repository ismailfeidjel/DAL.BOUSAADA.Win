using MySql.Data.MySqlClient;
using System;

namespace DevExpress.ProductsDemo.Win
{
    public class DbHelper
    {
        private readonly string _connectionString =
            "Server=localhost;Port=3306;Database=dal;Uid=root;Pwd=;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

       
    }
}