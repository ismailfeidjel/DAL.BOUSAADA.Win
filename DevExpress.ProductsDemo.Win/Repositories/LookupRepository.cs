using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class LookupRepository
    {
        private readonly DbHelper _db = new DbHelper();

        private List<LookupItem> GetLookup(
            string tableName,
            string orderBy = "name")
        {
            List<LookupItem> list =
                new List<LookupItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql =
                    $"SELECT id,name FROM {tableName} ORDER BY {orderBy}";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new LookupItem
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            Name = rd["name"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<LookupItem> GetPrograms()
        {
            return GetLookup("programs");
        }

        public List<LookupItem> GetDairas()
        {
            return GetLookup("dairas");
        }

        public List<LookupItem> GetDomains()
        {
            return GetLookup("domains");
        }

        public List<LookupItem> GetSectors()
        {
            return GetLookup("sectors");
        }

        public List<LookupItem> GetProjectStatuses()
        {
            return GetLookup("project_statuses");
        }

        public List<LookupItem> GetAdministrativeProcedures()
        {
            return GetLookup("administrative_procedures");
        }

        public List<LookupItem> GetSpecialStatus1()
        {
            return GetLookup("special_status1");
        }

        public List<LookupItem> GetSpecialStatus2()
        {
            return GetLookup("special_status2");
        }

        public List<LookupItem> GetSpecialStatus3()
        {
            return GetLookup("special_status3");
        }
        public List<LookupItem> GetCommunesByDaira(int dairaId)
        {
            List<LookupItem> list =
                new List<LookupItem>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
            SELECT id,name
            FROM communes
            WHERE daira_id=@daira_id
            ORDER BY name";

                using (var cmd =
                    new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@daira_id",
                        dairaId);

                    using (var rd =
                        cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            list.Add(new LookupItem
                            {
                                Id = Convert.ToInt32(rd["id"]),
                                Name = rd["name"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }
    }
}