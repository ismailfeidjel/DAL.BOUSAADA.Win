using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Services
{
    public class CommuneSummaryRow
    {
        public string Daira { get; set; }
        public string Commune { get; set; }
        public decimal LotBudget { get; set; }
        public decimal RegisteredAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal ConsumedAmount { get; set; }   


        public int AnnouncedCount { get; set; }
        public int ReceivedCount { get; set; }

        public int RemainingCount { get; set; }

        // Admin procedure stage counts
        public int DaftarCount { get; set; }      // 1
        public int AnnouncementCount { get; set; } // 2
        public int GrantCount { get; set; }        // 3
        public int StudyCount { get; set; }        // 4
        public int CfmCount { get; set; }           // 5
        public int TsCount { get; set; }            // 6
        public int Ts1Count { get; set; }           // 7
        public int SubsidyCount { get; set; }       // 8
        public int OpeningCount { get; set; }       // 9
        public int DeferredCount { get; set; }      // 10
        public int RejectedCount { get; set; }      // 11

        // Status counts
        public int TotalRegisteredGroupCount { get; set; } // sum of the 6 below
        public int RegisteredCount { get; set; }   // 2
        public int OngoingCount { get; set; }      // 3
        public int StoppedCount { get; set; }      // 4
        public int FinishedCount { get; set; }     // 5
        public int ReceivedFundsCount { get; set; } // 6
        public int ClosedCount { get; set; }        // 7


    }

    public static class CommuneSummaryReportBuilder
    {
        private static readonly string[] PartTemplateKeys =
       {
            "قالب_تقرير_البلديات_جزء1",
            "قالب_تقرير_البلديات_جزء2",
            "قالب_تقرير_البلديات_جزء3",
        };

        public static XtraReport Build(List<LotGridModel> data, string programName)
        {
            var rows = ComputeCommuneRows(data);

            var partReports = new List<XtraReport>();
            foreach (string key in PartTemplateKeys)
            {
                string path = Path.Combine(Application.StartupPath, "Reports", "Templates", key + ".repx");
                if (!File.Exists(path))
                    throw new InvalidOperationException($"القالب غير موجود: {path}\nيرجى إنشائه أولاً من تبويب التقارير.");

                XtraReport partReport = XtraReport.FromFile(path, true);
                GridReportBuilder.EnsureSafeMargins(partReport);
                partReport.Landscape = true;
                partReport.DataSource = rows;
                ApplyProgramTitle(partReport, programName);
                partReport.CreateDocument();

                partReports.Add(partReport);
            }

            // Merge every part's generated pages into the first report's document
            XtraReport combined = partReports[0];
            for (int i = 1; i < partReports.Count; i++)
            {
                XtraReport part = partReports[i];
                combined.ModifyDocument(modifier =>
                {
                    for (int p = 0; p < part.PrintingSystem.Pages.Count; p++)
                        modifier.InsertPage(combined.PrintingSystem.Pages.Count, part.PrintingSystem.Pages[p]);
                });
            }

            return combined;
        }
        private static void ApplyProgramTitle(XtraReport report, string programName)
        {
            var control = report.FindControl("cellProgramName", true); // the title label in your ReportHeader
            if (control is XRLabel lbl)
                lbl.Text = programName;
        }
        internal static List<CommuneSummaryRow> ComputeCommuneRows(List<LotGridModel> data)
        {
            var result = new List<CommuneSummaryRow>();

            var byCommune = data.GroupBy(r => new { r.DairaId, r.Daira, r.Commune });

            foreach (var g in byCommune.OrderBy(x => x.Key.DairaId).ThenBy(x => x.Key.Commune))
            {
                var rows = g.ToList();                                    // all LOT rows — used for money sums
                var projects = rows.GroupBy(r => r.ProjectId)
                                    .Select(pg => pg.First())
                                    .ToList();                              // one row per PROJECT — used for all counts

                int Count(Func<LotGridModel, bool> predicate) => projects.Count(predicate);

                int announced = projects.Count;
                int received = Count(r => r.AdministrativeProcedureId == 4 || r.ProjectStatusId == 2 || r.ProjectStatusId == 6
                                        || r.ProjectStatusId == 4 || r.ProjectStatusId == 3 || r.ProjectStatusId == 7 || r.ProjectStatusId == 5);
                int remaining = announced - received;

                var row = new CommuneSummaryRow
                {
                    Daira = g.Key.Daira,
                    Commune = g.Key.Commune,

                    // Money still sums across ALL lots (a project can have multiple budget lines)
                    LotBudget = rows.Sum(r => r.LotBudget),
                    RegisteredAmount = rows.Sum(r => r.RegisteredAmount),
                    ConsumedAmount = rows.Sum(r => r.ConsumedAmount),
                    RemainingAmount = rows.Sum(r => r.LotBudget) - rows.Sum(r => r.RegisteredAmount),

                    AnnouncedCount = announced,
                    ReceivedCount = received,
                    RemainingCount = remaining,

                    DaftarCount = Count(r => r.AdministrativeProcedureId == 1),
                    AnnouncementCount = Count(r => r.AdministrativeProcedureId == 2),
                    GrantCount = Count(r => r.AdministrativeProcedureId == 3),
                    StudyCount = Count(r => r.AdministrativeProcedureId == 4),
                    CfmCount = Count(r => r.AdministrativeProcedureId == 5),
                    TsCount = Count(r => r.AdministrativeProcedureId == 6),
                    Ts1Count = Count(r => r.AdministrativeProcedureId == 7),
                    SubsidyCount = Count(r => r.AdministrativeProcedureId == 8),
                    OpeningCount = Count(r => r.AdministrativeProcedureId == 9),
                    DeferredCount = Count(r => r.AdministrativeProcedureId == 10),
                    RejectedCount = Count(r => r.AdministrativeProcedureId == 11),

                    RegisteredCount = Count(r => r.ProjectStatusId == 2),
                    OngoingCount = Count(r => r.ProjectStatusId == 3),
                    StoppedCount = Count(r => r.ProjectStatusId == 4),
                    FinishedCount = Count(r => r.ProjectStatusId == 5),
                    ReceivedFundsCount = Count(r => r.ProjectStatusId == 6),
                    ClosedCount = Count(r => r.ProjectStatusId == 7),
                };

                row.TotalRegisteredGroupCount = row.RegisteredCount + row.OngoingCount + row.StoppedCount
                                               + row.FinishedCount + row.ReceivedFundsCount + row.ClosedCount;

                result.Add(row);
            }

            return result;
        }
    }

    public static class CommuneSummary2ReportBuilder
    {
        private static readonly string[] PartTemplateKeys =
       {
            "قالب_تقرير_البلديات",
        };

        public static XtraReport Build(List<LotGridModel> data, string programName)
        {
            var rows = CommuneSummaryReportBuilder.ComputeCommuneRows(data);

            var partReports = new List<XtraReport>();
            foreach (string key in PartTemplateKeys)
            {
                string path = Path.Combine(Application.StartupPath, "Reports", "Templates", key + ".repx");
                if (!File.Exists(path))
                    throw new InvalidOperationException($"القالب غير موجود: {path}\nيرجى إنشائه أولاً من تبويب التقارير.");

                XtraReport partReport = XtraReport.FromFile(path, true);
                GridReportBuilder.EnsureSafeMargins(partReport);
                partReport.Landscape = false;
                partReport.DataSource = rows;
                ApplyProgramTitle(partReport, programName);
                partReport.CreateDocument();

                partReports.Add(partReport);
            }

            // Merge every part's generated pages into the first report's document
            XtraReport combined = partReports[0];
            for (int i = 1; i < partReports.Count; i++)
            {
                XtraReport part = partReports[i];
                combined.ModifyDocument(modifier =>
                {
                    for (int p = 0; p < part.PrintingSystem.Pages.Count; p++)
                        modifier.InsertPage(combined.PrintingSystem.Pages.Count, part.PrintingSystem.Pages[p]);
                });
            }

            return combined;
        }
        private static void ApplyProgramTitle(XtraReport report, string programName)
        {
            var control = report.FindControl("cellProgramName", true); // the title label in your ReportHeader
            if (control is XRLabel lbl)
                lbl.Text = programName;
        }
    }

    public static class FinancialConsumptionReportBuilder
    {
        private static readonly string[] PartTemplateKeys =
        {
        "قالب_تقرير_الاستهلاك_المالي",
    };

        public static XtraReport Build(List<LotGridModel> data, string programName)
        {
            var rows = CommuneSummaryReportBuilder.ComputeCommuneRows(data);

            var partReports = new List<XtraReport>();
            foreach (string key in PartTemplateKeys)
            {
                string path = Path.Combine(Application.StartupPath, "Reports", "Templates", key + ".repx");
                if (!File.Exists(path))
                    throw new InvalidOperationException($"القالب غير موجود: {path}\nيرجى إنشائه أولاً من تبويب التقارير.");

                XtraReport partReport = XtraReport.FromFile(path, true);
                GridReportBuilder.EnsureSafeMargins(partReport);
                partReport.Landscape = false;
                partReport.DataSource = rows;
                ApplyProgramTitle(partReport, programName);
                partReport.CreateDocument();

                partReports.Add(partReport);
            }

            XtraReport combined = partReports[0];
            for (int i = 1; i < partReports.Count; i++)
            {
                XtraReport part = partReports[i];
                combined.ModifyDocument(modifier =>
                {
                    for (int p = 0; p < part.PrintingSystem.Pages.Count; p++)
                        modifier.InsertPage(combined.PrintingSystem.Pages.Count, part.PrintingSystem.Pages[p]);
                });
            }

            return combined;
        }

        private static void ApplyProgramTitle(XtraReport report, string programName)
        {
            var control = report.FindControl("cellProgramName", true);
            if (control is XRLabel lbl)
                lbl.Text = programName;
        }
    }
}