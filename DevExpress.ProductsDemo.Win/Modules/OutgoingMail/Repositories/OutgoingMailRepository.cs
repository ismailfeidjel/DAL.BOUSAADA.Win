using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExpress.ProductsDemo.Win.Modules.OutgoingMail
{
    public class OutgoingMailRepository : IOutgoingMailRepository
    {

        private readonly DbHelper _db;

        public OutgoingMailRepository()
        {
            _db = new DbHelper();
        }

        public List<OutgoingMail> GetAll()
        {
            List<OutgoingMail> list = new List<OutgoingMail>();

            string sql = @"
SELECT *
FROM outgoing_mail
ORDER BY id DESC";

            using (MySqlConnection con = _db.GetConnection())
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new OutgoingMail
                        {
                            Id = Convert.ToInt32(reader["id"]),
                            MailNumber = reader["mail_number"].ToString(),
                            MailDate = Convert.ToDateTime(reader["mail_date"]),
                            Destination = reader["destination"].ToString(),
                            Subject = reader["subject"].ToString(),
                            FileName = reader["file_name"].ToString(),
                            FilePath = reader["file_path"].ToString(),
                            Notes = reader["notes"].ToString(),
                            CreatedAt = Convert.ToDateTime(reader["created_at"])
                        });
                    }
                }
            }

            return list;
        }

        public void Add(OutgoingMail mail)
        {
            string sql = @"
INSERT INTO outgoing_mail
(
    mail_number,
    mail_date,
    destination,
    subject,
    file_name,
    file_path,
    notes
)
VALUES
(
    @mail_number,
    @mail_date,
    @destination,
    @subject,
    @file_name,
    @file_path,
    @notes
)";

            using (MySqlConnection con = _db.GetConnection())
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@mail_number", mail.MailNumber);
                    cmd.Parameters.AddWithValue("@mail_date", mail.MailDate);
                    cmd.Parameters.AddWithValue("@destination", mail.Destination);
                    cmd.Parameters.AddWithValue("@subject", mail.Subject);
                    cmd.Parameters.AddWithValue("@file_name", mail.FileName);
                    cmd.Parameters.AddWithValue("@file_path", mail.FilePath);
                    cmd.Parameters.AddWithValue("@notes", mail.Notes);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }

}

