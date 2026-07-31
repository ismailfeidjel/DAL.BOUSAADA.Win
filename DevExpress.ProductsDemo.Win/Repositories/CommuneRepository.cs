using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class CommuneRepository
    {
        private readonly DbHelper _db = new DbHelper();

        public List<CommuneItem> GetAll()
        {
            var list = new List<CommuneItem>();
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "SELECT id, name, daira_id FROM communes ORDER BY daira_id, name";
                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new CommuneItem(
                            rd.GetInt32(rd.GetOrdinal("id")),
                            rd.GetString(rd.GetOrdinal("name")),
                            rd.GetInt32(rd.GetOrdinal("daira_id"))
                        ));
                    }
                }
            }
            return list;
        }

        public void Insert(CommuneItem item)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "INSERT INTO communes (name, daira_id) VALUES (@name, @daira_id)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@daira_id", item.DairaId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(CommuneItem item)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE communes SET name=@name, daira_id=@daira_id WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@daira_id", item.DairaId);
                    cmd.Parameters.AddWithValue("@id", item.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM communes WHERE id=@id";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex) when (ex.Number == 1451)
                    {
                        throw new InvalidOperationException(
                            "لا يمكن الحذف لوجود بيانات مرتبطة بهذا العنصر.", ex);
                    }
                }
            }
        }
    }
}