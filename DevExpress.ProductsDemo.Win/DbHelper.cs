using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.ProductsDemo.Win
{
    using System;
    using System.Data;
    using System.Data.OleDb;

    public class DbHelper
    {
        private string connectionString =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\db.accdb;";

        public DataTable GetData(string query)
        {
            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                using (OleDbDataAdapter da = new OleDbDataAdapter(query, con))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public int Execute(string query)
        {
            using (OleDbConnection con = new OleDbConnection(connectionString))
            {
                con.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, con))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
