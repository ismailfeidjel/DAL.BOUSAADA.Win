using DevExpress.XtraCharts.Native;
using System;
using System.Data;
using System.Data.OleDb;

public class DbHelper
{
    private readonly string connectionString =
    @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Database\dbb.accdb;";
    private string conn =
   @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\Database\dbb.accdb;";

    // 🔌 Get connection
    private OleDbConnection GetConnection()
    {
        return new OleDbConnection(connectionString);
    }

    // 🧪 Test connection
    public bool TestConnection()
    {
        try
        {
            using (var con = new OleDbConnection(connectionString))
            {
                con.Open();
                return true;
            }
        }
        catch (Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(ex.Message);
            return false;
        }
    }

    public DataTable GetTasks()
    {
        using (OleDbConnection con = new OleDbConnection(conn))
        {
            using (OleDbDataAdapter da =
                new OleDbDataAdapter("SELECT * FROM AdsecT", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }

    // 📊 Get Data (SELECT)
    public DataTable GetData(string query, OleDbParameter[] parameters = null)
    {
        DataTable dt = new DataTable();

        using (var con = GetConnection())
        using (var cmd = new OleDbCommand(query, con))
        using (var da = new OleDbDataAdapter(cmd))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            da.Fill(dt);
        }

        return dt;
    }

    // ⚙️ Execute INSERT / UPDATE / DELETE
    public int Execute(string query, OleDbParameter[] parameters = null)
    {
        using (var con = GetConnection())
        {
            con.Open();

            using (var cmd = new OleDbCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteNonQuery();
            }
        }
    }

    // 🔢 Get single value (SUM, COUNT, etc.)
    public object ExecuteScalar(string query, OleDbParameter[] parameters = null)
    {
        using (var con = GetConnection())
        {
            con.Open();

            using (var cmd = new OleDbCommand(query, con))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);

                return cmd.ExecuteScalar();
            }
        }
    }
}