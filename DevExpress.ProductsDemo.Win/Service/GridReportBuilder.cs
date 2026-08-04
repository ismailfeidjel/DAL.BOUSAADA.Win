using DevExpress.Data.Filtering;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.ReportGeneration;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Services
{
    /// <summary>
    /// Per-report configuration — tells GridReportBuilder what to do
    /// without hardcoding any module-specific field names.
    /// </summary>
    /// 
  
    public class GridReportOptions
    {
        public string TemplateKey { get; set; }
        public Dictionary<string, float> FixedColumnWidths { get; set; } = new Dictionary<string, float>();
        public Dictionary<string, string> FieldAliases { get; set; } = new Dictionary<string, string>();

        public HashSet<string> SumFields { get; set; } = new HashSet<string>();
        public string CountField { get; set; }

        public bool HideTopBorder { get; set; }

        /// <summary>Field used to group rows for sequential numbering (e.g. "ProjectId").
        /// Leave null to number every row individually instead of by group.</summary>
        public string GroupIdField { get; set; }
        public string HideBorderField { get; set; }

        public string NumberingCellName { get; set; } = "cellProjectNumber";
        public string CounterCellName { get; set; } = "cellProjectCounter";
        public string CounterCellName2 { get; set; } = "tb27"; // some templates name it this way
        public string FilterCellName { get; set; } = "cellFilterText";
        public string FilterCellNameAlt { get; set; } = "tbFilter";
        public string ProgramCellName { get; set; } = "cellProgramName";
        public string CustomFilterText { get; set; }
        public string ProgramDisplayText { get; set; } // set by the caller, e.g. "ADSEC2025"

        public bool GenerateFooterRow { get; set; } = false;
    }

    public static class GridReportBuilder
    {
        // ── Entry point ──────────────────────────────────────────────
        public static XtraReport Build<T>(GridView gridView, List<T> visibleData, GridReportOptions options) 
        {
            string templatePath = Path.Combine(Application.StartupPath, "Reports", "Templates", options.TemplateKey + ".repx");

        XtraReport report;
            if (File.Exists(templatePath))
            {
                report = XtraReport.FromFile(templatePath, true);
            }
            else
            {
                report = ReportGenerator.GenerateReport(gridView);
                report.RightToLeft = DevExpress.XtraReports.UI.RightToLeft.Yes;
                report.RightToLeftLayout = DevExpress.XtraReports.UI.RightToLeftLayout.Yes;
            }

            EnsureSafeMargins(report);

            var groupProp = typeof(T).GetProperty(options.GroupIdField);
            var hideProp = typeof(T).GetProperty(options.HideBorderField);

            if (groupProp != null && hideProp != null)
            {
                object previousKey = null;

                foreach (var row in visibleData)
                {
                    object currentKey = groupProp.GetValue(row);

                    bool sameGroup =
                        previousKey != null &&
                        Equals(previousKey, currentKey);

                    hideProp.SetValue(row, sameGroup);

                    previousKey = currentKey;
                }
            }



            report.DataSource = visibleData;
            ApplyFilterDisplayText(report, gridView, options);
            ApplyProgramText(report, options);   // ← new


            if (!string.IsNullOrEmpty(options.GroupIdField))
                ApplyGroupNumbering(report, visibleData, options);

            ApplyGridColumnVisibility(report, gridView, options, out List<string> visibleKeys, out Dictionary<string, float> keyWidths); if (options.GenerateFooterRow)
              GenerateFooter(report, visibleKeys, keyWidths, options);

            return report;
        }
        private static void ApplyProgramText(XtraReport report, GridReportOptions options)
        {
            if (string.IsNullOrEmpty(options.ProgramDisplayText)) return;

            var cell = report.FindControl(options.ProgramCellName, true);
            if (cell is XRLabel lbl) lbl.Text = options.ProgramDisplayText;
            else if (cell is XRTableCell tcell) tcell.Text = options.ProgramDisplayText;
        }
       

        // ── Margins ──────────────────────────────────────────────────
        public static void EnsureSafeMargins(XtraReport report)
        {
            const int minMargin = 20; // ≈ 5mm — safe for most printers

            var m = report.Margins;
            report.Margins = new System.Drawing.Printing.Margins(
                 Math.Max((int)m.Left, minMargin),
                Math.Max((int)m.Right, minMargin),
                Math.Max((int)m.Top, minMargin),
                Math.Max((int)m.Bottom, minMargin)
            );
        }

        // ── Filter display text ──────────────────────────────────────
        private static void ApplyFilterDisplayText(XtraReport report, GridView gridView, GridReportOptions options)
        {
            var filterCell = report.FindControl(options.FilterCellName, true) as XRTableCell
                  ?? report.FindControl(options.FilterCellNameAlt, true) as XRTableCell;

            if (filterCell == null) return;

            if (!string.IsNullOrEmpty(options.CustomFilterText))
            {
                filterCell.Text = options.CustomFilterText;
                return;
            }

            var parts = new List<string>();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView.Columns)
            {
                CriteriaOperator criteria = col.FilterInfo?.FilterCriteria;
                if (criteria == null) continue;

                if (!string.IsNullOrEmpty(options.CustomFilterText))
                {
                    filterCell.Text = options.CustomFilterText;
                    return;
                }

                var values = new List<string>();
                CollectOperandValues(criteria, col, values);

                var distinctValues = values.Distinct().ToList();
                if (distinctValues.Count > 0)
                    parts.Add($"{col.Caption}: {string.Join(", ", distinctValues)}");
            }

            filterCell.Text = parts.Count == 0 ? "الكل" : string.Join("  |  ", parts);
        }

        private static void CollectOperandValues(CriteriaOperator criteria, DevExpress.XtraGrid.Columns.GridColumn col, List<string> values)
        {
            switch (criteria)
            {
                case BinaryOperator bo:
                    CollectOperandValues(bo.LeftOperand, col, values);
                    CollectOperandValues(bo.RightOperand, col, values);
                    break;

                case GroupOperator go:
                    foreach (var op in go.Operands)
                        CollectOperandValues(op, col, values);
                    break;

                case UnaryOperator uo:
                    CollectOperandValues(uo.Operand, col, values);
                    break;

                case InOperator io:
                    foreach (var op in io.Operands)
                        CollectOperandValues(op, col, values);
                    break;

                case BetweenOperator bwo:
                    CollectOperandValues(bwo.BeginExpression, col, values);
                    CollectOperandValues(bwo.EndExpression, col, values);
                    break;

                case FunctionOperator fo:
                    foreach (var op in fo.Operands)
                        CollectOperandValues(op, col, values);
                    break;

                case OperandValue ov:
                    if (ov.Value != null)
                        values.Add(GetFilterValueDisplayText(col, ov.Value));
                    break;
            }
        }

        private static string GetFilterValueDisplayText(DevExpress.XtraGrid.Columns.GridColumn col, object val)
        {
            if (col == null || val == null) return val?.ToString() ?? "";

            if (col.RealColumnEdit is RepositoryItemLookUpEdit lookup)
            {
                object lookupText = lookup.GetDisplayValueByKeyValue(val);

                if ((lookupText == null || lookupText == DBNull.Value) && int.TryParse(val.ToString(), out int intVal))
                {
                    lookupText = lookup.GetDisplayValueByKeyValue(intVal);
                }

                if (lookupText != null && lookupText != DBNull.Value)
                    return lookupText.ToString();
            }
            else if (col.RealColumnEdit is RepositoryItemImageComboBox imgCombo)
            {
                var item = imgCombo.Items.Cast<DevExpress.XtraEditors.Controls.ImageComboBoxItem>()
                                         .FirstOrDefault(i => i.Value != null && i.Value.ToString() == val.ToString());
                if (item != null)
                    return item.Description;
            }

            return val.ToString().Replace("'", "").Replace("\"", "").Trim();
        }

        // ── Group numbering (e.g. number by project, not by row) ─────
        private static void ApplyGroupNumbering<T>(XtraReport report, List<T> visibleData, GridReportOptions options)
        {
            var cell = report.FindControl(options.NumberingCellName, true) as XRTableCell;

            var prop = typeof(T).GetProperty(options.GroupIdField);
            if (prop == null) return;

            var groupNumbers = new Dictionary<object, int>();
            int nextNumber = 1;

            foreach (var row in visibleData)
            {
                object key = prop.GetValue(row);
                if (key == null) continue;

                if (!groupNumbers.ContainsKey(key))
                {
                    groupNumbers[key] = nextNumber;
                    nextNumber++;
                }
            }

            int totalGroupsCount = nextNumber - 1;

            var counterCell1 = report.FindControl(options.CounterCellName, true) as XRTableCell;
            var counterCell2 = report.FindControl(options.CounterCellName2, true) as XRTableCell;

            if (counterCell1 != null)
                counterCell1.BeforePrint += (s, e) => ((XRTableCell)s).Text = totalGroupsCount.ToString();

            if (counterCell2 != null)
                counterCell2.BeforePrint += (s, e) => ((XRTableCell)s).Text = totalGroupsCount.ToString();

            if (cell == null) return;

            cell.BeforePrint += (s, e) =>
            {
                var currentCell = (XRTableCell)s;
                object groupIdObj = currentCell.Report.GetCurrentColumnValue(options.GroupIdField);

                if (groupIdObj == null || groupIdObj == DBNull.Value)
                {
                    currentCell.Text = "";
                    return;
                }

                currentCell.Text = groupNumbers.TryGetValue(groupIdObj, out int num) ? num.ToString() : "";
            };
        }

        // ── Column visibility + proportional widths ──────────────────

        internal static void ApplyGridColumnVisibility(XtraReport report, GridView gridView, GridReportOptions options, out List<string> finalKeys, out Dictionary<string, float> finalWidths)
        {
            finalKeys = new List<string>();
            finalWidths = new Dictionary<string, float>();

            var captionToField = new Dictionary<string, string>();
            var visibleFields = new HashSet<string>();

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in gridView.Columns)
            {
                if (!string.IsNullOrEmpty(col.Caption))
                    captionToField[col.Caption.Trim()] = col.FieldName;

                if (col.Visible)
                    visibleFields.Add(col.FieldName);
            }

            var allRows = report.AllControls<XRTableRow>().ToList();
            if (allRows.Count == 0) return;

            XRTableRow referenceRow = allRows[0];
            var keysInOrder = referenceRow.Cells.Cast<XRTableCell>()
                .Select(c => ResolveColumnKey(c, captionToField, options.NumberingCellName))
                .ToList();

            // Hide any template column whose field isn't currently visible in the live grid
            // (covers both explicitly-hidden grid columns and fields the grid doesn't have at all).
            // The row-number cell is a special case: it isn't bound to any data field (its text
            // is set separately by ApplyGroupNumbering), so it must never be treated as hideable.
            var hiddenIndices = new HashSet<int>();
            for (int i = 0; i < keysInOrder.Count; i++)
            {
                if (referenceRow.Cells[i] is XRTableCell refCell && refCell.Name == options.NumberingCellName)
                    continue; // always visible — not a real data field

                string key = keysInOrder[i];
                string gridFieldName = key != null && options.FieldAliases.TryGetValue(key, out string alias)
                    ? alias
                    : key;

                if (key == null || !visibleFields.Contains(gridFieldName))
                    hiddenIndices.Add(i);
            }

            var visibleKeysInOrder = keysInOrder
                .Where((key, i) => !hiddenIndices.Contains(i))
                .ToList();

            float printableWidth = (report.PageWidth - report.Margins.Left - report.Margins.Right) * 0.99f;

            var fixedKeys = visibleKeysInOrder.Where(k => options.FixedColumnWidths.ContainsKey(k)).ToList();
            var flexibleKeys = visibleKeysInOrder.Where(k => !options.FixedColumnWidths.ContainsKey(k)).ToList();

            float totalFixedWidth = fixedKeys.Sum(k => options.FixedColumnWidths[k]);
            float remainingWidth = Math.Max(0, printableWidth - totalFixedWidth);
            float flexibleWidthEach = flexibleKeys.Count > 0 ? remainingWidth / flexibleKeys.Count : 0;

            var resolvedWidths = new Dictionary<string, float>();
            foreach (var k in fixedKeys)
                resolvedWidths[k] = options.FixedColumnWidths[k];
            foreach (var k in flexibleKeys)
                resolvedWidths[k] = flexibleWidthEach;

            var touchedTables = new HashSet<XRTable>();

            foreach (XRTableRow row in allRows)
            {
                var cells = row.Cells.Cast<XRTableCell>().ToList();
                if (cells.Count != keysInOrder.Count) continue;

                float rowTotal = 0f;

                for (int i = 0; i < cells.Count; i++)
                {
                    if (hiddenIndices.Contains(i))
                    {
                        row.Cells.Remove(cells[i]);
                    }
                    else
                    {
                        string key = keysInOrder[i];
                        float w = resolvedWidths.TryGetValue(key, out float rw) ? rw : 0f;
                        cells[i].WidthF = w;
                        cells[i].Weight = w;   // Weight drives actual layout, WidthF alone doesn't stick

                        rowTotal += w;

                        if (!finalWidths.ContainsKey(key))
                        {
                            finalWidths[key] = w;
                            finalKeys.Add(key);
                        }
                    }
                }

                row.WidthF = rowTotal;

                if (row.Parent is XRTable table)
                    touchedTables.Add(table);
            }

            foreach (var table in touchedTables)
                table.WidthF = printableWidth;
        }

        private static string ResolveColumnKey(XRTableCell cell, Dictionary<string, string> captionToField, string numberingCellName)
        {
            if (cell.Name == numberingCellName)
                return "__RowNumber__"; // synthetic key — not a real data field, always kept visible

            string field = GetCellFieldName(cell);
            if (field != null) return field;

            string text = (cell.Text ?? "").Trim();
            return captionToField.TryGetValue(text, out string mapped) ? mapped : null;
        }

        private static string GetCellFieldName(XRTableCell cell)
        {
            string expr = null;

            foreach (ExpressionBinding binding in cell.ExpressionBindings)
            {
                if (binding.PropertyName == "Text" && !string.IsNullOrEmpty(binding.Expression))
                {
                    expr = binding.Expression;
                    break;
                }
            }

            if (expr == null && cell.DataBindings.Count > 0 && cell.DataBindings["Text"] != null)
                return cell.DataBindings["Text"].DataMember;

            if (expr == null) return null;

            var match = Regex.Match(expr, @"\[([^\]]+)\]");
            return match.Success ? match.Groups[1].Value : null;
        }

        // ── Footer (sums / counts row) ─────────────────────────────
        private static void GenerateFooter(XtraReport report, List<string> visibleKeys, Dictionary<string, float> keyWidths, GridReportOptions options)
        {
            if (visibleKeys.Count == 0) return;

            var footerBand = new ReportFooterBand();
            var table = new XRTable();
            table.BeginInit();

            float totalWidth = keyWidths.Values.Sum();
            table.WidthF = totalWidth;
            table.LocationF = new PointF(0, 0);
            table.RightToLeft = report.RightToLeft;

            var row = new XRTableRow();
            row.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            row.ForeColor = Color.Black;
            row.BackColor = Color.FromArgb(230, 230, 230);
            table.Rows.Add(row);

            var cellsList = new List<XRTableCell>();

            foreach (string key in visibleKeys)
            {
                var cell = new XRTableCell();
                cell.WordWrap = false;
                cell.CanGrow = false;
                cell.CanShrink = true;
                cell.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
                cell.Borders = DevExpress.XtraPrinting.BorderSide.All;
                cell.BorderWidth = 1f;
                cell.BorderColor = Color.Black;
                cell.Padding = new DevExpress.XtraPrinting.PaddingInfo(4, 4, 2, 2, 100);

                if (options.SumFields.Contains(key))
                {
                    cell.Summary = new XRSummary { Running = SummaryRunning.Report, Func = SummaryFunc.Sum };
                    cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", $"Sum([{key}])"));
                    cell.TextFormatString = "{0:N0}";
                }
                else if (key == options.CountField)
                {
                    cell.Summary = new XRSummary { Running = SummaryRunning.Report, Func = SummaryFunc.Count };
                    cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "Count()"));
                }
                else
                {
                    cell.Text = string.Empty;
                }

                row.Cells.Add(cell);
                cellsList.Add(cell);
            }

            for (int i = 0; i < visibleKeys.Count; i++)
            {
                float cellWidth = keyWidths[visibleKeys[i]];
                cellsList[i].Weight = (double)cellWidth / totalWidth;
                cellsList[i].WidthF = cellWidth;
            }

            row.WidthF = totalWidth;
            table.EndInit();

            footerBand.Controls.Add(table);
            footerBand.HeightF = 35f;
            report.Bands.Add(footerBand);
        }
    }
}