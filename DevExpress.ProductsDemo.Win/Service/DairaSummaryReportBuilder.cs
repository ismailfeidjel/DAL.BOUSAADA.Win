using DevExpress.Diagram.Core.Native.Generation;
using DevExpress.ProductsDemo.Win.Domain;
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
        public string ProgramType { get; set; }
        public string ProgramYear { get; set; }
        public string ProgramName { get; set; }
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
        // Removed the programs list parameter. Now it only relies on allData.
        public static XtraReport Build(List<LotGridModel> allData)
        {
            // Group directly by the program fields available in the grid model, along with the Daira
            var rows = allData
                .GroupBy(r => new {
                    r.ProgramType, // Ensure these properties exist in your LotGridModel
                    r.ProgramYear,
                    r.ProgramName,
                    r.Daira
                })
                .Select(g =>
                {
                    var distinctProjectIds = g.Select(r => r.ProjectId).Distinct().ToList();

                    int closedCount = g.Where(r => r.ProjectStatusId == 7)
                                       .Select(r => r.ProjectId).Distinct().Count();

                    return new DairaSummaryRow
                    {
                        // Extract directly from the Group Key
                        ProgramType = g.Key.ProgramType ?? "",
                        ProgramYear = g.Key.ProgramYear ?? "",
                        ProgramName = g.Key.ProgramName ?? "",
                        Daira = g.Key.Daira ?? "",

                        AnnouncedAmount = g.Sum(r => r.LotBudget),
                        AnnouncedCount = distinctProjectIds.Count,
                        ClosedCount = closedCount,
                        ConsumedAmount = g.Sum(r => r.ConsumedAmount)
                    };
                })
                .OrderBy(r => r.ProgramType)
                .ThenByDescending(r => r.ProgramYear)
                .ThenBy(r => r.Daira)
                .ToList();

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


    }

}