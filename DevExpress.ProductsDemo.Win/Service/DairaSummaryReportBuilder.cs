using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Services
{
    public class DairaSummaryRow
    {
        public string Daira { get; set; }
        public decimal AnnouncedAmount { get; set; }
        public decimal ConsumedAmount { get; set; }   
        public int AnnouncedCount { get; set; }
        public int ClosedCount { get; set; }


    }

    
    public static class  DairaSummaryReportBuilder
    {
        private static readonly string[] PartTemplateKeys =
       {
            "قالب_تقرير_الدائرة",
        };

        public static XtraReport Build(List<LotGridModel> data, string programName)
        {
            var rows = ComputeDairaRows(data);

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
        internal static List<DairaSummaryRow> ComputeDairaRows(List<LotGridModel> data)
        {
            var result = new List<DairaSummaryRow>();

            var byDaira = data.GroupBy(r => new { r.Daira, r.Program });

            foreach (var g in byDaira.OrderBy(x => x.Key.Daira).ThenBy(x => x.Key.Program))
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

                var row = new DairaSummaryRow
                {
                    Daira = g.Key.Daira,

                    // Money still sums across ALL lots (a project can have multiple budget lines)
                    AnnouncedAmount = rows.Sum(r => r.LotBudget),
                    ConsumedAmount = rows.Sum(r => r.ConsumedAmount),

                    AnnouncedCount = announced,

                    ClosedCount = Count(r => r.ProjectStatusId == 7),
                };


                result.Add(row);
            }

            return result;
        }
    }

}