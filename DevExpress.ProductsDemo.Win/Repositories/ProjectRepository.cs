using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class ProjectRepository
    {
        private readonly DbHelper _db = new DbHelper();

        public List<Project> GetAll()
        {
            List<Project> list = new List<Project>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
                SELECT *
                FROM projects
                ORDER BY operation_number";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new Project
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            OperationNumber = rd["operation_number"].ToString(),
                            OperationName = rd["operation_name"].ToString(),

                            ProgramId = Convert.ToInt32(rd["program_id"]),
                            DairaId = Convert.ToInt32(rd["daira_id"]),
                            CommuneId = Convert.ToInt32(rd["commune_id"]),
                            DomainId = Convert.ToInt32(rd["domain_id"]),
                            SectorId = Convert.ToInt32(rd["sector_id"]),


                            HasLots = Convert.ToBoolean(rd["has_lots"]),

                            

                            CreatedAt = Convert.ToDateTime(rd["created_at"]),
                            UpdatedAt = Convert.ToDateTime(rd["updated_at"]),

                            UpdatedBy = rd["updated_by"] == DBNull.Value
         ? (int?)null
         : Convert.ToInt32(rd["updated_by"])
                        });
                    }
                }
            }

            return list;
        }

        public int Insert(Project p)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
        INSERT INTO projects
        (
            operation_number,
            operation_name,
            program_id,
            daira_id,
            commune_id,
            domain_id,
            sector_id,
            has_lots
        )
        VALUES
        (
            @operation_number,
            @operation_name,
            @program_id,
            @daira_id,
            @commune_id,
            @domain_id,
            @sector_id,
            @has_lots
        );

        SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@operation_number", p.OperationNumber);
                    cmd.Parameters.AddWithValue("@operation_name", p.OperationName);

                    cmd.Parameters.AddWithValue("@program_id", p.ProgramId);
                    cmd.Parameters.AddWithValue("@daira_id", p.DairaId);
                    cmd.Parameters.AddWithValue("@commune_id", p.CommuneId);

                    cmd.Parameters.AddWithValue("@domain_id", p.DomainId);
                    cmd.Parameters.AddWithValue("@sector_id", p.SectorId);


                    cmd.Parameters.AddWithValue("@has_lots", p.HasLots);


                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
    }
}