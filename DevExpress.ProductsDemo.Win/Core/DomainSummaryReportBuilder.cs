using DevExpress.ProductsDemo.Win.Repositories;
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
        public int UnregisteredCount { get; set; }

        // الخصائص الجديدة لحالات المشروع
        public int NotstartedCount { get; set; }
        public int OngoingCount { get; set; }
        public int StoppedCount { get; set; }
        public int FinishedCount { get; set; }
        public int ReceivedFundsCount { get; set; }
        public int ClosedCount { get; set; }
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

            // Start from ALL domains in the DB, not just the ones present in the data
            var allDomains = new LookupRepository().GetAll("domains");

            var byDomain = data
                .GroupBy(r => r.DomainId)
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var domain in allDomains.OrderBy(d => d.Id))
            {
                byDomain.TryGetValue(domain.Id, out var lots);
                lots = lots ?? new List<LotGridModel>(); // no projects for this domain — empty, not skipped

                // تجميع الحصص للحصول على قائمة المشاريع الأساسية بدون تكرار
                var projects = lots.GroupBy(r => r.ProjectId)
                                   .Select(pg => pg.First())
                                   .ToList();

                int announced = projects.Count;
                int registered = projects.Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7);

                // إضافة العدادات الجديدة بناءً على قائمة projects
                int notStarted = projects.Count(r => r.ProjectStatusId == 2);
                int ongoing = projects.Count(r => r.ProjectStatusId == 3);
                int stopped = projects.Count(r => r.ProjectStatusId == 4);
                int finished = projects.Count(r => r.ProjectStatusId == 5);
                int receivedFunds = projects.Count(r => r.ProjectStatusId == 6);
                int closed = projects.Count(r => r.ProjectStatusId == 7);

                result.Add(new DomainSummaryRow
                {
                    DomainId = domain.Id,
                    Domain = domain.Name,
                    AnnouncedCount = announced,
                    LotBudget = lots.Sum(r => r.LotBudget),
                    RegisteredAmount = lots.Sum(r => r.RegisteredAmount),
                    RegisteredCount = registered,
                    UnregisteredCount = announced - registered,

                    // إسناد القيم الجديدة للكائن
                    NotstartedCount = notStarted,
                    OngoingCount = ongoing,
                    StoppedCount = stopped,
                    FinishedCount = finished,
                    ReceivedFundsCount = receivedFunds,
                    ClosedCount = closed
                });
            }

            return result;
        }
    }
}