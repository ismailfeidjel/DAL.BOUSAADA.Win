using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Services
{
    public static class StatusSummaryReportBuilder
    {
        public const string TemplateKey = "قالب_تقرير_الوضعية";

        public static XtraReport Build(List<LotGridModel> data)
        {
            string templatePath = Path.Combine(Application.StartupPath, "Reports", "Templates", TemplateKey + ".repx");

            if (!File.Exists(templatePath))
                throw new InvalidOperationException($"القالب غير موجود: {templatePath}\nيرجى إنشائه أولاً من تبويب التقارير.");

            XtraReport report = XtraReport.FromFile(templatePath, true);
            GridReportBuilder.EnsureSafeMargins(report);

            report.DataSource = data;   // ← new: template can now also bind fields/summaries directly

            var stats = ComputeStats(data);
            FillNamedControls(report, stats);

            return report;
        }

        // First-run only: a minimal report with a few placeholder labels,
        // just so there's something to open in the designer and build the real layout against.

        private static Dictionary<string, string> ComputeStats(List<LotGridModel> data)
        {
            var lookupRepo = new LookupRepository();
            var adminProcedures = lookupRepo.GetAll("administrative_procedures").ToDictionary(x => x.Id, x => x.Name);
            var status1 = lookupRepo.GetAll("special_status1").ToDictionary(x => x.Id, x => x.Name);
            var status2 = lookupRepo.GetAll("special_status2").ToDictionary(x => x.Id, x => x.Name);
            var status3 = lookupRepo.GetAll("special_status3").ToDictionary(x => x.Id, x => x.Name);

            string NameOf(Dictionary<int, string> dict, int? id) =>
                id.HasValue && dict.TryGetValue(id.Value, out var n) ? n : "";

            // ⚠ ASSUMPTION — verify these substrings actually match your real lookup Name values.
            bool IsRegistration(LotGridModel r) => NameOf(adminProcedures, r.AdministrativeProcedureId).Contains("تسجيل");
            bool IsDafatirShrout(LotGridModel r) => NameOf(adminProcedures, r.AdministrativeProcedureId).Contains("دفاتر الشروط");
            bool IsAnnouncements(LotGridModel r) => NameOf(adminProcedures, r.AdministrativeProcedureId).Contains("إعلان");
            bool IsOpeningEval(LotGridModel r) => NameOf(adminProcedures, r.AdministrativeProcedureId).Contains("فتح")
                                                || NameOf(adminProcedures, r.AdministrativeProcedureId).Contains("تقييم");
            bool IsProvisionalGrant(LotGridModel r) => NameOf(adminProcedures, r.AdministrativeProcedureId).Contains("منح");

            bool IsRoadMaintenance(LotGridModel r) =>
                NameOf(status1, r.SpecialStatus1Id).Contains("طرق") ||
                NameOf(status2, r.SpecialStatus2Id).Contains("طرق") ||
                NameOf(status3, r.SpecialStatus3Id).Contains("طرق");

            // Count by PROJECT, not by lot row — one project can have multiple lots
            var byProject = data.GroupBy(r => r.ProjectId).Select(g => g.First()).ToList();

            int total = byProject.Count;
            decimal totalBudget = data.Sum(r => r.LotBudget);

            var registration = byProject.Where(IsRegistration).ToList();
            var adminProc = byProject.Where(r => !IsRegistration(r)).ToList();

            var dafatir = adminProc.Where(IsDafatirShrout).ToList();
            var announcements = adminProc.Where(IsAnnouncements).ToList();
            var openingEval = adminProc.Where(IsOpeningEval).ToList();
            var provisionalGrant = adminProc.Where(IsProvisionalGrant).ToList();

            var roadMaintenance = byProject.Where(IsRoadMaintenance).ToList();
            var nonRoad = byProject.Where(r => !IsRoadMaintenance(r)).ToList();

            decimal SumBudget(List<LotGridModel> list) => list.Sum(r => r.LotBudget);
            string Pct(int part, int whole) => whole == 0 ? "0" : Math.Round(100.0 * part / whole).ToString();

            return new Dictionary<string, string>
            {
                ["lblTotalOperations"] = total.ToString(),
                ["lblTotalBudget"] = totalBudget.ToString("N2"),

                ["lblRegistrationCount"] = registration.Count.ToString(),
                ["lblRegistrationPercent"] = Pct(registration.Count, total),
                ["lblRegistrationAmount"] = SumBudget(registration).ToString("N2"),

                ["lblAdminProcCount"] = adminProc.Count.ToString(),
                ["lblAdminProcPercent"] = Pct(adminProc.Count, total),
                ["lblAdminProcAmount"] = SumBudget(adminProc).ToString("N2"),

                ["lblDafatirShroutCount"] = dafatir.Count.ToString(),
                ["lblAnnouncementsCount"] = announcements.Count.ToString(),
                ["lblOpeningEvalCount"] = openingEval.Count.ToString(),
                ["lblProvisionalGrantCount"] = provisionalGrant.Count.ToString(),

                ["lblRoadMaintenanceCount"] = roadMaintenance.Count.ToString(),
                ["lblRoadMaintenanceAmount"] = SumBudget(roadMaintenance).ToString("N2"),
                ["lblNonRoadCount"] = nonRoad.Count.ToString(),
                ["lblNonRoadAmount"] = SumBudget(nonRoad).ToString("N2"),
            };
        }

        // Fills ANY control on the report whose Name matches a computed stat key —
        // meaning you can freely redesign the layout later (add/remove/rename fields)
        // without touching this code, as long as the Name matches.
        private static void FillNamedControls(XtraReport report, Dictionary<string, string> stats)
        {
            foreach (var control in report.AllControls<XRControl>())
            {
                if (!stats.TryGetValue(control.Name, out string value)) continue;

                if (control is XRLabel lbl) lbl.Text = value;
                else if (control is XRTableCell cell) cell.Text = value;
            }
        }
    }
}