using DevExpress.DataAccess.Native.Json;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win
{
    public class DbHelper
    {
        private static string _connectionString;

        public DbHelper()
        {
            if (_connectionString == null)
                _connectionString = LoadConnectionString();
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private static string LoadConnectionString()
        {
            string configPath = Path.Combine(Application.StartupPath, "appsettings.json");

            if (!File.Exists(configPath))
            {
                // Fallback for local dev machines with no config file present
                return "Server=localhost;Port=3306;Database=dal;Uid=root;Pwd=;";
            }

            try
            {
                var json = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText(configPath));
                var cs = json["ConnectionString"];

                string server = cs["Server"]?.ToString();
                string port = cs["Port"]?.ToString() ?? "3306";
                string database = cs["Database"]?.ToString();
                string uid = cs["Uid"]?.ToString();
                string pwd = cs["Pwd"]?.ToString() ?? "";

                return $"Server={server};Port={port};Database={database};Uid={uid};Pwd={pwd};";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"فشل قراءة إعدادات الاتصال بقاعدة البيانات:\n\n{ex.Message}\n\nسيتم استخدام الإعدادات الافتراضية.",
                    "خطأ في الإعدادات", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return "Server=localhost;Port=3306;Database=dal;Uid=root;Pwd=;";
            }
        }
    }
}