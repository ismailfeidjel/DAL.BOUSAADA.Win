using DevExpress.DataProcessing.InMemoryDataProcessor;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ProductsDemo.Win;


namespace DevExpress.ProductsDemo.Win.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private string _conn =
            @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\DataBase\DBB.accdb;";

        public List<AProject> GetAllProjects()
        {
            List<AProject> list = new List<AProject>();

            string sql = @"
        SELECT
            AdsecT.ProjectID,
            daira.daira,
            commune.commune,
            AdsecT.IntutulePri,
            AdsecT.ProgrammeYe,
            AdsecT.Field,
            AdsecT.Sector,
            AdsecT.RegistrationMont,
            AdsecT.FinancialConsumption,
            AdsecP.FinancialMontontPre,
            AdsecP.FinancialRemaining,
            AdsecP.Contructor,
            AdsecP.Duration,
            AdsecP.Ods,
            AdsecP.PhysicalProgress,
            AdsecT.FinancialProgress,
            AdsecT.Status,
            AdsecP.Notes
        FROM
            (
                (daira
                INNER JOIN commune ON daira.cod_daira = commune.cod_daira)
                INNER JOIN AdsecT ON commune.id_com = AdsecT.CommuneID
            )
            INNER JOIN AdsecP ON AdsecT.ProjectID = AdsecP.ParentId;";

            using (OleDbConnection con = new OleDbConnection(_conn))
            {
                con.Open();

                using (OleDbCommand cmd = new OleDbCommand(sql, con))
                using (OleDbDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        AProject p = new AProject
                        {
                            ProjectID = Convert.ToInt32(r["ProjectID"]),

                            Daira = r["daira"].ToString(),
                            Commune = r["commune"].ToString(),

                            IntutulePri = r["IntutulePri"].ToString(),
                            ProgrammeYe = r["ProgrammeYe"].ToString(),
                            Field = r["Field"].ToString(),
                            Sector = r["Sector"].ToString(),

                            RegistrationMont = Convert.ToDecimal(r["RegistrationMont"]),
                            FinancialConsumption = Convert.ToDecimal(r["FinancialConsumption"]),

                            FinancialMontontPre = Convert.ToDecimal(r["FinancialMontontPre"]),
                            FinancialRemaining = Convert.ToDecimal(r["FinancialRemaining"]),

                            Contructor = r["Contructor"].ToString(),
                            Duration = Convert.ToInt32(r["Duration"]),
                            Ods = r["Ods"].ToString(),

                            PhysicalProgress = Convert.ToDouble(r["PhysicalProgress"]),
                            FinancialProgress = Convert.ToDecimal(r["FinancialProgress"]),

                            Status = r["Status"].ToString(),
                            Notes = r["Notes"].ToString()
                        };

                        list.Add(p);
                    }
                }
            }

            return list;
        }
    }
}