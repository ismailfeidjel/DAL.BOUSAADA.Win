using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;

namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DbHelper _db;

        public ProjectRepository()
        {
            _db = new DbHelper();
        }


        public List<AProject> GetAllProjects()
        {
            var dict = new Dictionary<int, AProject>();

            const string sql = @"
SELECT
    p.id AS Id,
    d.name AS Daira,
    c.name AS Commune,
    p.intitule_pri AS IntutulePri,
    p.programme_year AS ProgrammeYe,
    p.field_name AS Field,
    p.sector AS Sector,
    p.registration_montanat AS RegistrationMont,
    p.financial_consumption AS FinancialConsumption,
    p.financial_progress AS FinancialProgress,
    p.project_status AS Status,

    t.task_title AS TaskTitle,
    t.financial_montont_pre AS FinancialMontontPre,
    t.financial_remaining AS FinancialRemaining,
    t.contructor AS Contructor,
    t.duration AS Duration,
    t.ods_date AS Ods,
    t.pysical_progress AS PhysicalProgress,
    t.notes AS Notes
FROM adsec_projects p
LEFT JOIN comunes c ON c.id = p.comune_id
LEFT JOIN daira d ON d.id = c.iddaira
LEFT JOIN adsec_project_tasks t ON t.parent_id = p.id
ORDER BY p.id, t.id;";

            using (var con = new OdbcConnection(_conn))
            {
                con.Open();

                using (var cmd = new OdbcCommand(sql, con))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        int id = SafeInt(r["ProjectID"]);

                        if (!dict.ContainsKey(id))
                        {
                            dict[id] = new AProject
                            {
                                Id = id,
                                Daira = SafeString(r["daira"]),
                                Commune = SafeString(r["commune"]),
                                IntutulePri = SafeString(r["IntutulePri"]),
                                ProgrammeYe = SafeString(r["ProgrammeYe"]),
                                Field = SafeString(r["Field"]),
                                Sector = SafeString(r["Sector"]),
                                RegistrationMont = SafeDecimal(r["RegistrationMont"]),
                                FinancialConsumption = SafeDecimal(r["FinancialConsumption"]),
                                FinancialProgress = SafeDecimal(r["FinancialProgress"]),
                                Status = SafeString(r["Status"])
                            };
                        }

                        if (!string.IsNullOrWhiteSpace(SafeString(r["TaskTitle"])))
                        {
                            var task = new ProjectTask
                            {
                                FinancialMontontPre = SafeDecimal(r["FinancialMontontPre"]),
                                FinancialRemaining = SafeDecimal(r["FinancialRemaining"]),
                                Contructor = SafeString(r["Contructor"]),
                                Duration = SafeInt(r["Duration"]),
                                Ods = SafeDateString(r["Ods"]),
                                PhysicalProgress = SafeDouble(r["PhysicalProgress"]),
                                Notes = SafeString(r["Notes"])
                            };

                            dict[id].Tasks.Add(task);
                        }
                    }
                }
            }

            return dict.Values.ToList();
        }

        private string SafeString(object value)
        {
            return value == DBNull.Value ? string.Empty : value.ToString();
        }

        private string SafeDateString(object value)
        {
            if (value == DBNull.Value)
                return string.Empty;

            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date))
                return date.ToString("yyyy-MM-dd");

            return value.ToString();
        }

        private int SafeInt(object value)
        {
            return value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private decimal SafeDecimal(object value)
        {
            return value == DBNull.Value ? 0m : Convert.ToDecimal(value);
        }

        private double SafeDouble(object value)
        {
            return value == DBNull.Value ? 0d : Convert.ToDouble(value);
        }
    }
}
