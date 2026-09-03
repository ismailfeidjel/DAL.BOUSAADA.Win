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
    public class startedSummaryRow
    {
        public string ProgramType { get; set; }
        public string ProgramYear { get; set; }
        public string ProgramName { get; set; }
        public string Daira { get; set; }
        public decimal AnnouncedAmount { get; set; }
        public decimal RegistredAmount { get; set; }
        public decimal ConsumedAmount { get; set; }   
        public int AnnouncedCount { get; set; }
        public int TsCount { get; set; }
        public int ApCount { get; set; }
        public int NotstartedCount { get; set; }
        public int StartedCount { get; set; }
        public int CfCount { get; set; }


    }

    
    public static class  StartedSummaryReportBuilder
    {
        private static readonly string[] PartTemplateKeys =
       {
            "قالب_تقرير_نسبة_البدء",
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

                    int tsCount = g.Where(r => r.AdministrativeProcedureId == 6)
                                       .Select(r => r.ProjectId).Distinct().Count();
                    int cfCount = g.Where(r => r.AdministrativeProcedureId == 5)
                                       .Select(r => r.ProjectId).Distinct().Count();

                    int apCount = g.Where(r => r.AdministrativeProcedureId == 1 || r.AdministrativeProcedureId == 2 || r.AdministrativeProcedureId == 3 || r.AdministrativeProcedureId == 4)
                                       .Select(r => r.ProjectId).Distinct().Count();
                    int notstartedCount = g.Where(r => r.ProjectStatusId == 2 || r.ProjectStatusId == 1)
                                       .Select(r => r.ProjectId).Distinct().Count();

                    int startedCount = g.Where(r => r.ProjectStatusId == 3 || r.ProjectStatusId == 4 || r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId ==7 || r.ProjectStatusId == 8)
                                       .Select(r => r.ProjectId).Distinct().Count();

                    return new startedSummaryRow
                    {
                        // Extract directly from the Group Key
                        ProgramType = g.Key.ProgramType ?? "",
                        ProgramYear = g.Key.ProgramYear ?? "",
                        ProgramName = g.Key.ProgramName ?? "",
                        Daira = g.Key.Daira ?? "",

                        AnnouncedAmount = g.Sum(r => r.LotBudget),
                        RegistredAmount = g.Sum(r => r.RegisteredAmount),
                        AnnouncedCount = distinctProjectIds.Count,
                        TsCount=tsCount,
                        CfCount = cfCount,
                        ApCount =apCount,
                        StartedCount=startedCount,
                        NotstartedCount=notstartedCount
                    };
                })
                .OrderBy(r => r.ProgramType)
                .ThenByDescending(r => r.ProgramYear)
                .ToList();

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