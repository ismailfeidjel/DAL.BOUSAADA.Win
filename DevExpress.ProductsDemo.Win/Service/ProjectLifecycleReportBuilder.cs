using DevExpress.ProductsDemo.Win.Domain;
using DevExpress.ProductsDemo.Win.MasterDetailReport;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.Design.AxImporter;

namespace DevExpress.ProductsDemo.Win.Services
{
    public class ProjectStageRow : LotGridModel
    {
        public int StageOrder { get; set; }
        public string StageLabel { get; set; }
    }
    public static class ProjectLifecycleReportBuilder
    {
        public const string TemplateKey = "قالب_تقرير_المشاريع_حسب_المرحلة";
        public const string TitleTemplateKey = "قالب_صفحة_فاصلة_البرنامج";

        private static readonly (int order, string label, bool isAdminProc, int id)[] Stages =
        {
            (1, "دفتر",   true,  1),
            (2, "اعلان",  true,  2),
            (3, "فتح",    true,  9),
            (4, "منح",    true,  3),
            (5, "CFM",    true,  5),
            (6, "جارية",   false, 3),
            (7, "متوقفة",  false, 4),
            (8, "منتهية",  false, 5),
            (9, "مستلمة",  false, 6),
        };

        /// <summary>
        /// Builds the lifecycle report for a SINGLE program type (e.g. all ADSEC programs).
        /// Do not pass programs mixing multiple types — throws if it detects that.
        /// </summary>
        public static XtraReport Build(GridView gridView, List<ProgramLookupItem> programs, Func<int, List<LotGridModel>> getDataForProgram)
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
                var stageRows = ComputeStageRows(data);

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
        { "ProjectStatus", 40f }
    }

                    // add HighlightField/HighlightValue/HighlightColor or UniqueRowIdField here if you want those to apply too
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

            foreach (var row in data)
            {
                var stage = Stages.FirstOrDefault(s =>
                    s.isAdminProc
                        ? row.AdministrativeProcedureId == s.id
                        : row.ProjectStatusId == s.id);

                if (stage.label == null) continue;

                var stageRow = new ProjectStageRow();

                // Copy EVERY property from LotGridModel onto ProjectStageRow — no more manual field lists to keep in sync
                foreach (var prop in sourceProps)
                {
                    var targetProp = typeof(ProjectStageRow).GetProperty(prop.Name);
                    if (targetProp != null && targetProp.CanWrite)
                        targetProp.SetValue(stageRow, prop.GetValue(row));
                }

                stageRow.StageOrder = stage.order;
                stageRow.StageLabel = stage.label;

                result.Add(stageRow);
            }

            return result.OrderBy(r => r.StageOrder).ThenBy(r => r.ProjectId).ToList();
        }
    }
}