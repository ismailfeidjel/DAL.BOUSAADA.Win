using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Services
{
    public class ProjectStageRow : LotGridModel
    {
        public int StageOrder { get; set; }
        public string StageLabel { get; set; }
    }
    public enum LifecycleGrouping
    {
        ByStage,
        ByDaira,
        ByCommune
    }
    public static class ProjectLifecycleReportBuilder
    {
        public const string TemplateKey = "قالب_تقرير_المشاريع_حسب_المرحلة";
        public const string TitleTemplateKey = "قالب_صفحة_فاصلة_البرنامج";

        private static readonly (int order, string label, bool isAdminProc, int id)[] Stages =
        {
            (1, "دفتر الشروط",   true,  1),
            (2, "الاعلان عن طلب العروض",  true,  2),
            (3, "الفتح والتقييم",    true,  9),
            (4, "المنح المؤقت",    true,  3),
            (5, "المراقبة الميزانياتية الولائية",true,  5),
            (6, "غير منطلقة",   false, 2),
            (7, "جارية",   false, 3),
            (8, "متوقفة",  false, 4),
            (9, "منتهية",  false, 5),
            (10, "مستلمة",  false, 6),
        };

        /// <summary>
        /// Builds the lifecycle report for a SINGLE program type (e.g. all ADSEC programs).
        /// Do not pass programs mixing multiple types — throws if it detects that.
        /// </summary>
        public static XtraReport Build(GridView gridView, List<ProgramLookupItem> programs, Func<int, List<LotGridModel>> getDataForProgram, LifecycleGrouping grouping = LifecycleGrouping.ByStage)
        {
            if (programs.Select(p => p.Type).Distinct().Count() > 1)
                throw new InvalidOperationException("هذا التقرير يجب أن يشمل نوع برنامج واحد فقط.");

            string listTemplatePath = Path.Combine(Application.StartupPath, "Reports", "Templates", TemplateKey + ".repx");
            if (!File.Exists(listTemplatePath))
                throw new InvalidOperationException($"القالب غير موجود: {listTemplatePath}\nيرجى إنشائه أولاً من تبويب التقارير.");

            string titleTemplatePath = Path.Combine(Application.StartupPath, "Reports", "Templates", TitleTemplateKey + ".repx");
            if (!File.Exists(titleTemplatePath))
                throw new InvalidOperationException($"قالب الصفحة الفاصلة غير موجود: {titleTemplatePath}\nيرجى إنشائه أولاً من تبويب التقارير.");

            var orderedPrograms = programs.OrderBy(p => p.Year).ToList();
            XtraReport combined = null;

            foreach (var program in orderedPrograms)
            {
                var data = getDataForProgram(program.Id);

                List<ProjectStageRow> stageRows;
                switch (grouping)
                {
                    case LifecycleGrouping.ByDaira:
                        stageRows = ComputeDairaRows(data);
                        break;
                    case LifecycleGrouping.ByCommune:
                        stageRows = ComputeCommuneRows(data);
                        break;
                    case LifecycleGrouping.ByStage:
                    default:
                        stageRows = ComputeStageRows(data);
                        break;
                }


                if (stageRows.Count == 0) continue;

                XtraReport titlePage = BuildTitlePage(titleTemplatePath, program.Name);
                titlePage.CreateDocument();

                XtraReport listPage = XtraReport.FromFile(listTemplatePath, true);
                GridReportBuilder.EnsureSafeMargins(listPage);
                listPage.DataSource = stageRows;

                // Respect the live grid's current column visibility/widths, same as the main Projects report
                // GridReportBuilder.ApplyGridColumnVisibility(listPage, gridView, out _, out _);
                var lifecycleOptions = new GridReportOptions
                {
                    GroupIdField = "ProjectId",
                    FieldAliases = new Dictionary<string, string>
                    {
                        { "Program", "ProgramId" },
                        { "ProjectStatus", "ProjectStatusId" },
                        { "Domain", "DomainId" },
                        { "Sector", "SectorId" }
                    },
                    FixedColumnWidths = new Dictionary<string, float>
                    {
                        { "__RowNumber__", 30f },
                        { "OperationNumber", 60f },
                        { "Daira", 50f },
                        { "Commune", 50f },
                        { "Program", 40f },
                        { "ExpectedEndDate", 50f },
                        { "LotBudget", 95f },
                        { "RegisteredAmount", 95f },
                        { "ConsumedAmount", 95f },
                        { "Remaining", 95f },
                        { "Contractor", 60f },
                        { "StartDate", 70f },
                        { "ExecutionDuration", 40f },
                        { "PhysicalProgress", 45f },
                        { "FinancialProgress", 40f },
                        { "Domain", 45f },
                        { "Sector", 45f },
                        { "ProjectStatus", 50f }
                    }
                };
                GridReportBuilder.ApplyGridColumnVisibility(listPage, gridView, lifecycleOptions, out _, out _);
                GridReportBuilder.ApplyGroupNumbering(listPage, stageRows, lifecycleOptions, "StageOrder");
                listPage.CreateDocument();

                if (combined == null)
                {
                    combined = titlePage;
                    AppendPages(combined, listPage);
                }
                else
                {
                    AppendPages(combined, titlePage);
                    AppendPages(combined, listPage);
                }
                combined.PrintingSystem.ContinuousPageNumbering = true; // optional, fixes page X of Y across parts

            }

            return combined ?? BuildTitlePage(titleTemplatePath, "لا توجد برامج");
        }
        private static void AppendPages(XtraReport target, XtraReport source)
        {
            target.ModifyDocument(modifier =>
            {
                for (int p = 0; p < source.PrintingSystem.Pages.Count; p++)
                    modifier.InsertPage(target.PrintingSystem.Pages.Count, source.PrintingSystem.Pages[p]);
            });
        }

        private static XtraReport BuildTitlePage(string titleTemplatePath, string programName)
        {
            XtraReport report = XtraReport.FromFile(titleTemplatePath, true);
            GridReportBuilder.EnsureSafeMargins(report);

            var control = report.FindControl("cellProgramName", true);
            if (control is XRLabel lbl) lbl.Text = programName;
            else if (control is XRTableCell cell) cell.Text = programName;

            return report;
        }

        private static List<ProjectStageRow> ComputeStageRows(List<LotGridModel> data)
        {
            var result = new List<ProjectStageRow>();
            var sourceProps = typeof(LotGridModel).GetProperties();

            // 1. Group the flat data by ProjectId to keep sibling lots together
            var groupedByProject = data.GroupBy(r => r.ProjectId);

            foreach (var projectGroup in groupedByProject)
            {
                // 2. Evaluate the stage using the first lot (the primary project row)
                var primaryLot = projectGroup.First();

                var stage = Stages.FirstOrDefault(s =>
                    s.isAdminProc
                        ? primaryLot.AdministrativeProcedureId == s.id
                        : primaryLot.ProjectStatusId == s.id);

                // If the project doesn't map to a stage, skip the entire project
                if (stage.label == null) continue;

                // 3. Loop through ALL lots in this project and force them into the same stage
                foreach (var row in projectGroup)
                {
                    var stageRow = new ProjectStageRow();

                    // Copy EVERY property from LotGridModel onto ProjectStageRow
                    foreach (var prop in sourceProps)
                    {
                        var targetProp = typeof(ProjectStageRow).GetProperty(prop.Name);
                        if (targetProp != null && targetProp.CanWrite)
                        {
                            targetProp.SetValue(stageRow, prop.GetValue(row));
                        }
                    }

                    stageRow.StageOrder = stage.order;
                    stageRow.StageLabel = stage.label;

                    result.Add(stageRow);
                }
            }

            // 4. Sort by Stage -> Project -> internal Lot order so they display perfectly grouped
            return result
                .OrderBy(r => r.StageOrder)
                .ThenBy(r => r.ProjectId)
                .ThenBy(r => r.LotNumber)
                .ToList();
        }

        private static List<ProjectStageRow> ComputeDairaRows(List<LotGridModel> data)
        {
            var result = new List<ProjectStageRow>();
            var sourceProps = typeof(LotGridModel).GetProperties();

            // Give each daira a stable order: by name (Arabic culture) so the same daira
            // always gets the same position across programs
            var dairaOrder = data
                .Select(r => new { r.DairaId, Name = r.Daira ?? "" })
                .Distinct()
                //.OrderBy(d => d.Name, StringComparer.Create(new System.Globalization.CultureInfo("ar-DZ"), true))
                .Select((d, index) => new { d.DairaId, d.Name, Order = index + 1 })
                .ToList();

            foreach (var row in data)
            {
                var daira = dairaOrder.First(d => d.DairaId == row.DairaId && d.Name == (row.Daira ?? ""));

                var groupRow = new ProjectStageRow();
                foreach (var prop in sourceProps)
                {
                    var targetProp = typeof(ProjectStageRow).GetProperty(prop.Name);
                    if (targetProp != null && targetProp.CanWrite)
                        targetProp.SetValue(groupRow, prop.GetValue(row));
                }

                // Reuse the template's group fields: order/label now describe the daira
                groupRow.StageOrder = daira.Order;
                groupRow.StageLabel = string.IsNullOrWhiteSpace(daira.Name) ? "بدون دائرة" : daira.Name;

                result.Add(groupRow);
            }

            return result
                .OrderBy(r => r.StageOrder)
                //.ThenBy(r => r.Commune)      // inside a daira, keep communes together
                .ThenBy(r => r.ProjectId)
                .ThenBy(r => r.LotNumber)
                .ToList();
        }

        private static List<ProjectStageRow> ComputeCommuneRows(List<LotGridModel> data)
        {
            var result = new List<ProjectStageRow>();
            var sourceProps = typeof(LotGridModel).GetProperties();

            var communeOrder = data
                .Select(r => new { r.CommuneId, Name = r.Commune ?? "" })
                .Distinct()
                .Select((c, index) => new { c.CommuneId, c.Name, Order = index + 1 })
                .ToList();

            foreach (var row in data)
            {
                var commune = communeOrder.First(c => c.CommuneId == row.CommuneId && c.Name == (row.Commune ?? ""));

                var groupRow = new ProjectStageRow();
                foreach (var prop in sourceProps)
                {
                    var targetProp = typeof(ProjectStageRow).GetProperty(prop.Name);
                    if (targetProp != null && targetProp.CanWrite)
                        targetProp.SetValue(groupRow, prop.GetValue(row));
                }

                groupRow.StageOrder = commune.Order;
                groupRow.StageLabel = string.IsNullOrWhiteSpace(commune.Name) ? "بدون بلدية" : commune.Name;

                result.Add(groupRow);
            }

            return result
                .OrderBy(r => r.StageOrder)
                .ThenBy(r => r.ProjectId)
                .ThenBy(r => r.LotNumber)
                .ToList();
        }
    }
}