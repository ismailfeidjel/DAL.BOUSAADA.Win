using DevExpress.ProductsDemo.Win.Forms;
using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class DialogHelper
    {
        // ── Primitives ───────────────────────────────────────────────
        public static void Info(string message, string caption = "معلومة")
        {
            XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void Warning(string message, string caption = "تنبيه")
        {
            XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void Error(string message, string caption = "خطأ")
        {
            XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void Exception(Exception ex, string caption = "خطأ غير متوقع")
        {
            Error(ex.Message, caption);
        }

        public static void DatabaseError(Exception ex)
        {
            Error($"حدث خطأ في قاعدة البيانات:\n\n{ex.Message}", "خطأ في قاعدة البيانات");
        }

        // ── Confirmations ────────────────────────────────────────────
        public static bool Confirm(string message, string caption = "تأكيد")
        {
            return XtraMessageBox.Show(message, caption, MessageBoxButtons.YesNo,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        public static DialogResult ConfirmYesNoCancel(string message, string caption = "تأكيد")
        {
            return XtraMessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
        }

        public static bool ConfirmDelete(string itemName)
        {
            return Confirm($"هل أنت متأكد من حذف \"{itemName}\"؟", "حذف");
        }

        public static bool ConfirmExit(bool hasChanges)
        {
            if (!hasChanges) return true;
            return Confirm("توجد تغييرات غير محفوظة.\n\nهل تريد الإغلاق على أي حال؟", "إغلاق");
        }

        // ── Semantic shortcuts ───────────────────────────────────────
        public static void Saved() => Info("تم حفظ البيانات بنجاح.", "تم");
        public static void Deleted() => Info("تم حذف العنصر بنجاح.", "تم");
        public static void Updated() => Info("تم تحديث البيانات بنجاح.", "تم");
        public static void Added() => Info("تمت الإضافة بنجاح.", "تم");
        public static void Validation(string message) => Warning(message, "تحقق من البيانات");

        // ── The key design piece: ONE place that owns error dialogs ──
        /// <summary>
        /// Runs <paramref name="action"/>. On success returns true.
        /// On failure, shows exactly one error dialog and returns false —
        /// so callers never need their own try/catch just to show a message,
        /// and repositories/services never need to know about dialogs at all.
        /// </summary>
        public static bool TryExecute(Action action, string errorContext = null)
        {
            try
            {
                action();
                return true;
            }
            catch (SilentCancelException)
            {
                return false; 
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errorContext))
                    Error($"{errorContext}\n\n{ex.Message}", "خطأ");
                else
                    Exception(ex);

                return false;
            }
        }

        /// <summary>Same as <see cref="TryExecute"/> but for functions that return a value.
        /// Returns default(T) on failure — check the bool to know whether it actually succeeded.</summary>
        public static bool TryExecute<T>(Func<T> func, out T result, string errorContext = null)
        {
            try
            {
                result = func();
                return true;
            }
            catch (Exception ex)
            {
                result = default;
                if (!string.IsNullOrEmpty(errorContext))
                    Error($"{errorContext}\n\n{ex.Message}", "خطأ");
                else
                    Exception(ex);

                return false;
            }
        }
    }
    public class SilentCancelException : Exception
    {
    }
}