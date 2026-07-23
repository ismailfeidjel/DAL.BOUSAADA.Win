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

        private static Dictionary<string, string> ComputeStats(List<LotGridModel> data)
        {
            var byProject = data.GroupBy(r => r.ProjectId).Select(g => g.First()).ToList();
            int Count(Func<LotGridModel, bool> predicate) => byProject.Count(predicate);

            int totalProjects = byProject.Count;
            int specialStatus2Count = Count(r => r.SpecialStatus2Id == 1);
            int remainingCount = totalProjects - specialStatus2Count;

            string percentText = totalProjects == 0
                ? ""
                : Math.Round(100.0 * remainingCount / totalProjects).ToString() + "%";
            string percentText1 = totalProjects == 0
              ? ""
              : Math.Round(100.0 * specialStatus2Count / totalProjects).ToString() + "%";
            string percentText3 = totalProjects == 0
              ? ""
              : Math.Round(100.0 * Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7) / totalProjects).ToString()+"%";


            return new Dictionary<string, string>
            {
                ["tableCell11"] = Count(r => r.AdministrativeProcedureId == 1).ToString(),
                ["tableCell14"] = Count(r => r.AdministrativeProcedureId == 2).ToString(),
                ["tableCell17"] = Count(r => r.AdministrativeProcedureId == 9).ToString(),
                ["tableCell20"] = Count(r => r.AdministrativeProcedureId == 3).ToString(),
                ["tableCell12"] = Count(r =>
                    r.AdministrativeProcedureId == 1 || r.AdministrativeProcedureId == 2 ||
                    r.AdministrativeProcedureId == 3 || r.AdministrativeProcedureId == 9).ToString(),

                ["tableCell36"] = Count(r => r.ProjectStatusId == 2).ToString(),
                ["cell_Ongoing"] = Count(r => r.ProjectStatusId == 3).ToString(),
                ["tableCell42"] = Count(r => r.ProjectStatusId == 4).ToString(),
                ["tableCell45"] = Count(r => r.ProjectStatusId == 5).ToString(),
                ["tableCell28"] = Count(r => r.ProjectStatusId == 6).ToString(),
                ["tableCell31"] = Count(r => r.ProjectStatusId == 7).ToString(),
                ["tableCell37"] = Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7).ToString(),

                ["tableCell62"] = totalProjects.ToString(),
                ["tableCell70"] = specialStatus2Count.ToString(),
                ["tableCell78"] = remainingCount.ToString(),
                ["tableCell4"] = percentText,   
                ["tableCell2"] = percentText1,  
                ["tableCell6"] = percentText3,

                //
             //   ["tableCell74"] = "",//عدد الملفات الواردة من البلديات للعمليات الاضافية
                ["tableCell96"] =( Count(r => r.AdministrativeProcedureId == 4)+ Count(r => r.AdministrativeProcedureId == 10).ToString()).ToString(),//عدد العمليات قيد التسجيل(على مستوى الادارة المحلية)
                ["tableCell100"] = Count(r => r.AdministrativeProcedureId == 5).ToString(),//عدد العمليات على مستوى الرقابة الميزانياتية للولاية
                ["tableCell106"] = Count(r => r.AdministrativeProcedureId == 10).ToString(),//عدد العمليات والحصص بدون تغطية مالية والمؤجلة
                ["tableCell112"] = Count(r => r.AdministrativeProcedureId == 6).ToString(),//عدد العمليات بصدد ارسالها لامين الخزينة الولائية للتسديد
                ["tableCell121"] = Count(r => r.AdministrativeProcedureId == 7).ToString(),//عدد العمليات على مستوى  الخزينة الولائية للتسديد
                 ["tableCell22"] = Count(r => r.AdministrativeProcedureId == 8).ToString(),//عدد العمليات التي تم صب مبالغها لدى امناء خزائن البلديات


                ["tableCell25"] = "",//البلديات التي اكملت ايداع ملفات التسجيل :
                ["tableCell7"] = "",//عدد :
                ["tableCell27"] = "",//البلديات التي لم تكمل ايداع ملفات التسجيل :
                ["tableCell32"] = "",//عدد :


                ["tableCell90"] = "",//الرصيد :
                ["tableCell94"] = "",//الباقي :


                ["tableCell84"] = "",//مبلغ العمليات المسجلة (المؤشرة من طرف المراقب الميزانياتي) :



                ["tableCell82"] = Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7).ToString(),
            };
        }

        // First-run only: a minimal report with a few placeholder labels,
        // just so there's something to open in the designer and build the real layout against.

        //private static Dictionary<string, string> ComputeStats(List<LotGridModel> data)
        //{
        //    // One row per project (first lot found) — every stat below counts PROJECTS, not lots
        //    var byProject = data.GroupBy(r => r.ProjectId).Select(g => g.First()).ToList();

        //    int Count(Func<LotGridModel, bool> predicate) => byProject.Count(predicate);

        //    return new Dictionary<string, string>
        //    {
        //        // Admin-procedure stages — replace IDs with your real values
        //        ["tableCell11"] = Count(r => r.AdministrativeProcedureId == 1).ToString(),
        //        ["tableCell14"] = Count(r => r.AdministrativeProcedureId == 2).ToString(),
        //        ["tableCell17"] = Count(r => r.AdministrativeProcedureId == 9).ToString(),
        //        ["tableCell20"] = Count(r => r.AdministrativeProcedureId == 3).ToString(),
        //        ["tableCell12"] = Count(r =>
        //            r.AdministrativeProcedureId == 1 || r.AdministrativeProcedureId == 2 ||
        //            r.AdministrativeProcedureId == 3 || r.AdministrativeProcedureId == 9).ToString(),

        //        // Project status breakdown — replace IDs with your real values
        //        ["tableCell36"] = Count(r => r.ProjectStatusId == 2).ToString(),
        //        ["cell_Ongoing"] = Count(r => r.ProjectStatusId == 3).ToString(),
        //        ["tableCell42"] = Count(r => r.ProjectStatusId == 4).ToString(),
        //        ["tableCell45"] = Count(r => r.ProjectStatusId == 5).ToString(),
        //        ["tableCell28"] = Count(r => r.ProjectStatusId == 6).ToString(),
        //        ["tableCell31"] = Count(r => r.ProjectStatusId == 7).ToString(),
        //        ["tableCell37"] = Count(r =>
        //            r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 || r.ProjectStatusId == 5 || r.ProjectStatusId == 6 ||
        //            r.ProjectStatusId == 7 ).ToString(),
        //        // 
        //        ["tableCell62"] = byProject.Count.ToString(),//totoal project 
        //        ["tableCell70"] = Count(r => r.SpecialStatus2Id == 1).ToString(),//العمليات الواردة

        //        ["tableCell78"] = (byProject.Count - Count(r => r.SpecialStatus2Id == 1)).ToString(),


        //        //[ReportItems.tableCell62].[Text] =persent  [ReportItems.tableCell62].[Text] / [ReportItems.tableCell78].[Text]

        //        ["tableCell82"] = Count(r =>
        //            r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 || r.ProjectStatusId == 5 || r.ProjectStatusId == 6 ||
        //            r.ProjectStatusId == 7).ToString(),
        //    };
        //}
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