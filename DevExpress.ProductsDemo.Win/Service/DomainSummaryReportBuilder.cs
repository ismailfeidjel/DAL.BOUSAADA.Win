using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Services
{
    public class DomainSummaryRow
    {
        public int DomainId { get; set; }
        public string Domain { get; set; }
        public int AnnouncedCount { get; set; }
        public decimal LotBudget { get; set; }
        public decimal RegisteredAmount { get; set; }
        public int RegisteredCount { get; set; }     // ProjectStatusId >= 2
        public int UnregisteredCount { get; set; }   // AnnouncedCount - RegisteredCount
        public decimal RegistrationPercent { get; set; }
        public string Notes { get; set; } = "";
    }

    public static class DomainSummaryReportBuilder
    {
        public const string TemplateKey = "قالب_تقرير_حسب_مجال_التدخل";

        public static XtraReport Build(List<LotGridModel> data, string programName)
        {
            string templatePath = Path.Combine(Application.StartupPath, "Reports", "Templates", TemplateKey + ".repx");
            if (!File.Exists(templatePath))
                throw new InvalidOperationException($"القالب غير موجود: {templatePath}\nيرجى إنشائه أولاً من تبويب التقارير.");

            XtraReport report = XtraReport.FromFile(templatePath, true);
            GridReportBuilder.EnsureSafeMargins(report);

            report.DataSource = ComputeDomainRows(data);

            var programCell = report.FindControl("cellProgramName", true);
            if (programCell is XRLabel lbl) lbl.Text = programName;
            else if (programCell is XRTableCell ptc) ptc.Text = programName;

            return report;
        }

        private static List<DomainSummaryRow> ComputeDomainRows(List<LotGridModel> data)
        {
            var result = new List<DomainSummaryRow>();

            var byDomain = data
                .GroupBy(r => new { r.DomainId, r.Domain })
                .OrderBy(g => g.Key.DomainId);

            foreach (var g in byDomain)
            {
                var lots = g.ToList();
                var projects = lots.GroupBy(r => r.ProjectId)
                                   .Select(pg => pg.First())
                                   .ToList();

                int announced = projects.Count;
                int registered = projects.Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7);

                result.Add(new DomainSummaryRow
                {
                    DomainId = g.Key.DomainId ?? 0,
                    Domain = g.Key.Domain ?? "—",
                    AnnouncedCount = announced,
                    LotBudget = lots.Sum(r => r.LotBudget),
                    RegisteredAmount = lots.Sum(r => r.RegisteredAmount),
                    RegisteredCount = registered,
                    UnregisteredCount = announced - registered,
                    RegistrationPercent = announced > 0
                        ? Math.Round(100m * registered / announced, 0)
                        : 0,
                });
            }

            return result;
        }
    }
}