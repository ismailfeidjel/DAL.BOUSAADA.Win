using DevExpress.ProductsDemo.Win.Repositories;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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
            bool IsReceived(LotGridModel r) =>
        r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
        r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7 ||
        r.AdministrativeProcedureId == 4;
            int specialStatus2Count = Count(r => r.SpecialStatus2Id == 1);

            decimal totalBudget = data.Sum(r => r.LotBudget);
            decimal totalreg = data.Sum(r => r.RegisteredAmount);


            decimal var70 = Count(IsReceived);
            decimal var37 = Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7);

            string percentText = totalProjects == 0
                ? ""
                : Math.Round(100.0 * Convert.ToDouble((totalProjects - var70)) / totalProjects).ToString() + "%";
            string percentText1 = totalProjects == 0
              ? ""
              : Math.Round(100.0 * specialStatus2Count / totalProjects).ToString() + "%";
            string percentText3 = totalProjects == 0
              ? ""
              : Math.Round(100.0 * Count(r =>
                    r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7) / totalProjects).ToString()+"%";

            var communesAllStatus2 = byProject
        .GroupBy(r => r.Commune)
        .Where(g => g.All(IsReceived))          // ← every project in this commune is "received"
        .Select(g => g.Key)
        .ToList();

            var communesAnyStatus2Value2 = byProject
                .GroupBy(r => r.Commune)
                .Where(g => g.Any(r => !IsReceived(r)))  // ← at least one project in this commune is NOT received
                .Select(g => g.Key)
                .ToList();



            decimal specialStatus2Budget = data.Where(r => r.SpecialStatus2Id == 1).Sum(r => r.LotBudget);

            decimal registeredOperationsAmount2 = data
    .Where(r => r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7)
    .Sum(r => r.RegisteredAmount);
            decimal registeredOperationsAmount1 =
    data.Where(r => r.ProjectStatusId == 2 || r.ProjectStatusId == 3 || r.ProjectStatusId == 4 ||
                    r.ProjectStatusId == 5 || r.ProjectStatusId == 6 || r.ProjectStatusId == 7)
        .Sum(r => r.RegisteredAmount)
    + data.Where(r => r.AdministrativeProcedureId == 4)
          .Sum(r => r.LotBudget);

            



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
                ["tableCell37"] = var37.ToString(),//

                ["tableCell62"] = totalProjects.ToString(),//العدد الكلي
                ["tableCell70"] = var70.ToString(),// الواردة من البلديات

                ["tableCell78"] = (totalProjects-var70).ToString(),//على مستوى البلديات
                ["tableCell4"] = percentText,   
                //نسبة اولى
                ["tableCell2"] = (totalProjects == 0 ? "" : Math.Round(100.0 * Convert.ToDouble(var70 / totalProjects)).ToString()) + " %",
                //نسبة المؤشرة
                ["tableCell6"] = (totalProjects == 0 ? "" : Math.Round(100.0 * Convert.ToDouble(var37 / totalProjects)).ToString()) + " %",

                //
                //   ["tableCell74"] = "",//عدد الملفات الواردة من البلديات للعمليات الاضافية
                ["tableCell33"] =( Count(r => r.AdministrativeProcedureId == 4) + Count(r => r.AdministrativeProcedureId == 10)).ToString(),//عدد العمليات قيد التسجيل(على مستوى الادارة المحلية)
                ["tableCell100"] = Count(r => r.AdministrativeProcedureId == 5).ToString(),//عدد العمليات على مستوى الرقابة الميزانياتية للولاية
                ["tableCell106"] = Count(r => r.AdministrativeProcedureId == 10).ToString(),//عدد العمليات والحصص بدون تغطية مالية والمؤجلة
                ["tableCell112"] = Count(r => r.AdministrativeProcedureId == 6).ToString(),//عدد العمليات بصدد ارسالها لامين الخزينة الولائية للتسديد
                ["tableCell121"] = Count(r => r.AdministrativeProcedureId == 7).ToString(),//عدد العمليات على مستوى  الخزينة الولائية للتسديد
                 ["tableCell22"] = Count(r => r.AdministrativeProcedureId == 8).ToString(),//عدد العمليات التي تم صب مبالغها لدى امناء خزائن البلديات


                ["tableCell25"] = communesAllStatus2.Count == 0 ? "لا يوجد" : string.Join("، ", communesAllStatus2),//البلديات التي اكملت ايداع ملفات التسجيل :
                ["tableCell7"] = communesAllStatus2.Count.ToString(),//عدد :

                ["tableCell27"] =  communesAnyStatus2Value2.Count == 0 ? "لا يوجد" : string.Join("، ", communesAnyStatus2Value2),//البلديات التي لم تكمل ايداع ملفات التسجيل :
                ["tableCell32"] = communesAnyStatus2Value2.Count.ToString(),//عدد :

                ["tableCell68"] = totalBudget.ToString("N2", CultureInfo.InvariantCulture) + "دج",//الغلاف المالي :
                ["tableCell72"] = registeredOperationsAmount1.ToString("N2", CultureInfo.InvariantCulture) + "دج",//مبلغ تسجيل مبدئي :
                ["tableCell80"] = (totalBudget - registeredOperationsAmount1- (totalBudget - totalreg)).ToString("N2", CultureInfo.InvariantCulture) + "دج",//مبلغ غير مسجل :
                ["tableCell84"] = registeredOperationsAmount2.ToString("N2", CultureInfo.InvariantCulture) + "دج",//مبلغ تسجيل نهائي :
                ["tableCell90"] = totalreg.ToString("N2", CultureInfo.InvariantCulture) + "دج",//الرصيد :
                ["tableCell94"] =(totalBudget-totalreg).ToString("N2", CultureInfo.InvariantCulture) + "دج",//الباقي :


              //  //عدد العمليات المسجلة (المؤشرة من طرف المراقب الميزانياتي) :
                ["tableCell82"] = var37.ToString(),
            };
        }
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