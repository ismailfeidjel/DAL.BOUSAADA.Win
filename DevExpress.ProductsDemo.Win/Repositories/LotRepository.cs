using DevExpress.ProductsDemo.Win.Domain;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class LotRepository : ILotRepository
    {
        private readonly DbHelper _db = new DbHelper();

        public List<LotGridModel> GetGridData()
        {
            var list = new List<LotGridModel>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
        SELECT

            l.id,
            l.project_id,

            p.operation_number,
            p.operation_name,

            pr.name AS program,
            d.name AS daira,
            c.name AS commune,
            dm.name AS domain_name,
            s.name AS sector,

            l.lot_number,
            l.lot_name,

            l.lot_budget,
            l.registered_amount,
            l.consumed_amount,

            l.contractor,
            l.execution_duration,
            l.start_date,
            l.physical_progress,

            ap.name AS administrative_procedure,

            ss1.name AS special_status1,
            ss2.name AS special_status2,
            ss3.name AS special_status3,

            ps.name AS project_status,

            l.notes

        FROM lots l

        INNER JOIN projects p
            ON p.id = l.project_id

        LEFT JOIN programs pr
            ON pr.id = p.program_id

        LEFT JOIN dairas d
            ON d.id = p.daira_id

        LEFT JOIN communes c
            ON c.id = p.commune_id

        LEFT JOIN domains dm
            ON dm.id = p.domain_id

        LEFT JOIN sectors s
            ON s.id = p.sector_id

        LEFT JOIN administrative_procedures ap
            ON ap.id = l.administrative_procedure_id

        LEFT JOIN special_status1 ss1
            ON ss1.id = l.special_status1_id

        LEFT JOIN special_status2 ss2
            ON ss2.id = l.special_status2_id

        LEFT JOIN special_status3 ss3
            ON ss3.id = l.special_status3_id

        LEFT JOIN project_statuses ps
            ON ps.id = l.project_status_id

        ORDER BY
            p.operation_number,
            l.lot_number";

                using (var cmd = new MySqlCommand(sql, conn))
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new LotGridModel
                        {
                            Id = Convert.ToInt32(rd["id"]),
                            ProjectId = Convert.ToInt32(rd["project_id"]),

                            OperationNumber = rd["operation_number"].ToString(),
                            OperationName = rd["operation_name"].ToString(),

                            Program = rd["program"]?.ToString(),
                            Daira = rd["daira"]?.ToString(),
                            Commune = rd["commune"]?.ToString(),
                            Domain = rd["domain_name"]?.ToString(),
                            Sector = rd["sector"]?.ToString(),

                            LotNumber = Convert.ToInt32(rd["lot_number"]),
                            LotName = rd["lot_name"].ToString(),

                            LotBudget = Convert.ToDecimal(rd["lot_budget"]),
                            RegisteredAmount = Convert.ToDecimal(rd["registered_amount"]),
                            ConsumedAmount = Convert.ToDecimal(rd["consumed_amount"]),

                            Contractor = rd["contractor"] == DBNull.Value
                                ? null
                                : rd["contractor"].ToString(),

                            ExecutionDuration = rd["execution_duration"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(rd["execution_duration"]),

                            StartDate = rd["start_date"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(rd["start_date"]),

                            PhysicalProgress = Convert.ToDecimal(rd["physical_progress"]),

                            AdministrativeProcedure =
                                rd["administrative_procedure"]?.ToString(),

                            SpecialStatus1 =
                                rd["special_status1"]?.ToString(),

                            SpecialStatus2 =
                                rd["special_status2"]?.ToString(),

                            SpecialStatus3 =
                                rd["special_status3"]?.ToString(),

                            ProjectStatus =
                                rd["project_status"]?.ToString(),

                            Notes =
                                rd["notes"] == DBNull.Value
                                    ? null
                                    : rd["notes"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<Lot> GetByProjectId(int projectId)
        {
            var list = new List<Lot>();

            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
                SELECT *
                FROM lots
                WHERE project_id = @project_id
                ORDER BY lot_number";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@project_id", projectId);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            list.Add(MapLot(rd));
                        }
                    }
                }
            }

            return list;
        }

        public Lot GetById(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
                SELECT *
                FROM lots
                WHERE id=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                            return MapLot(rd);
                    }
                }
            }

            return null;
        }

        public int Insert(Lot lot)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
                INSERT INTO lots
                (
                    project_id,
                    lot_number,
                    lot_name,
                    lot_budget,
                    registered_amount,
                    consumed_amount,
                    contractor,
                    execution_duration,
                    start_date,
                    physical_progress,
                    administrative_procedure_id,
                    special_status1_id,
                    special_status2_id,
                    special_status3_id,
                    project_status_id,
                    notes
                )
                VALUES
                (
                    @project_id,
                    @lot_number,
                    @lot_name,
                    @lot_budget,
                    @registered_amount,
                    @consumed_amount,
                    @contractor,
                    @execution_duration,
                    @start_date,
                    @physical_progress,
                    @administrative_procedure_id,
                    @special_status1_id,
                    @special_status2_id,
                    @special_status3_id,
                    @project_status_id,
                    @notes
                );

                SELECT LAST_INSERT_ID();";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    FillParameters(cmd, lot);

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(Lot lot)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql = @"
                UPDATE lots SET

                    lot_number=@lot_number,
                    lot_name=@lot_name,
                    lot_budget=@lot_budget,
                    registered_amount=@registered_amount,
                    consumed_amount=@consumed_amount,
                    contractor=@contractor,
                    execution_duration=@execution_duration,
                    start_date=@start_date,
                    physical_progress=@physical_progress,
                    administrative_procedure_id=@administrative_procedure_id,
                    special_status1_id=@special_status1_id,
                    special_status2_id=@special_status2_id,
                    special_status3_id=@special_status3_id,
                    project_status_id=@project_status_id,
                    notes=@notes

                WHERE id=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    FillParameters(cmd, lot);

                    cmd.Parameters.AddWithValue("@id", lot.Id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (var conn = _db.GetConnection())
            {
                conn.Open();

                string sql =
                    "DELETE FROM lots WHERE id=@id";

                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        private static void FillParameters(
            MySqlCommand cmd,
            Lot lot)
        {
            cmd.Parameters.AddWithValue("@project_id", lot.ProjectId);
            cmd.Parameters.AddWithValue("@lot_number", lot.LotNumber);
            cmd.Parameters.AddWithValue("@lot_name", lot.LotName);

            cmd.Parameters.AddWithValue("@lot_budget", lot.LotBudget);
            cmd.Parameters.AddWithValue("@registered_amount", lot.RegisteredAmount);
            cmd.Parameters.AddWithValue("@consumed_amount", lot.ConsumedAmount);

            cmd.Parameters.AddWithValue("@contractor",
                (object)lot.Contractor ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@execution_duration",
                (object)lot.ExecutionDuration ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@start_date",
                (object)lot.StartDate ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@physical_progress",
                lot.PhysicalProgress);

            cmd.Parameters.AddWithValue("@administrative_procedure_id",
                (object)lot.AdministrativeProcedureId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@special_status1_id",
                (object)lot.SpecialStatus1Id ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@special_status2_id",
                (object)lot.SpecialStatus2Id ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@special_status3_id",
                (object)lot.SpecialStatus3Id ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@project_status_id",
                (object)lot.ProjectStatusId ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@notes",
                (object)lot.Notes ?? DBNull.Value);
        }

        private static Lot MapLot(MySqlDataReader rd)
        {
            return new Lot
            {
                Id = Convert.ToInt32(rd["id"]),
                ProjectId = Convert.ToInt32(rd["project_id"]),

                LotNumber = Convert.ToInt32(rd["lot_number"]),
                LotName = rd["lot_name"].ToString(),

                LotBudget = Convert.ToDecimal(rd["lot_budget"]),
                RegisteredAmount = Convert.ToDecimal(rd["registered_amount"]),
                ConsumedAmount = Convert.ToDecimal(rd["consumed_amount"]),

                Contractor = rd["contractor"] == DBNull.Value
                    ? null
                    : rd["contractor"].ToString(),

                ExecutionDuration = rd["execution_duration"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(rd["execution_duration"]),

                StartDate = rd["start_date"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(rd["start_date"]),

                PhysicalProgress = Convert.ToDecimal(rd["physical_progress"]),

                AdministrativeProcedureId = rd["administrative_procedure_id"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(rd["administrative_procedure_id"]),

                SpecialStatus1Id = rd["special_status1_id"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(rd["special_status1_id"]),

                SpecialStatus2Id = rd["special_status2_id"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(rd["special_status2_id"]),

                SpecialStatus3Id = rd["special_status3_id"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(rd["special_status3_id"]),

                ProjectStatusId = rd["project_status_id"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(rd["project_status_id"]),

                Notes = rd["notes"] == DBNull.Value
                    ? null
                    : rd["notes"].ToString()
            };
        }
    }
}