using DevExpress.XtraEditors;
using System;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    public static class DialogHelper
    {
        public static void Info(string message)
        {
            XtraMessageBox.Show(
                message,
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void Info(string message, string caption)
        {
            XtraMessageBox.Show(
                message,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        public static void Warning(string message)
        {
            XtraMessageBox.Show(
                message,
                "Warning",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public static void Warning(string message, string caption)
        {
            XtraMessageBox.Show(
                message,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        public static void Error(string message)
        {
            XtraMessageBox.Show(
                message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void Error(string message, string caption)
        {
            XtraMessageBox.Show(
                message,
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static void Exception(Exception ex)
        {
            Error(ex.Message, "Unexpected Error");
        }

        public static void Exception(Exception ex, string caption)
        {
            Error(ex.Message, caption);
        }

        public static bool Confirm(string message)
        {
            return XtraMessageBox.Show(
                message,
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2)
                == DialogResult.Yes;
        }

        public static bool Confirm(string message, string caption)
        {
            return XtraMessageBox.Show(
                message,
                caption,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2)
                == DialogResult.Yes;
        }

        public static DialogResult ConfirmYesNoCancel(string message)
        {
            return XtraMessageBox.Show(
                message,
                "Confirmation",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }

        public static DialogResult ConfirmYesNoCancel(string message, string caption)
        {
            return XtraMessageBox.Show(
                message,
                caption,
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);
        }

        public static bool ConfirmDelete(string itemName)
        {
            return Confirm(
                $"Are you sure you want to delete \"{itemName}\"?",
                "Delete");
        }

        public static bool ConfirmExit(bool hasChanges)
        {
            if (!hasChanges)
                return true;

            return Confirm(
                "There are unsaved changes.\n\nDo you want to close anyway?",
                "Close");
        }

        public static bool ConfirmSaveChanges()
        {
            return Confirm(
                "Do you want to save your changes?",
                "Save");
        }

        public static void NotImplemented()
        {
            Info(
                "This feature has not been implemented yet.",
                "Coming Soon");
        }

        public static void Saved()
        {
            Info(
                "Data saved successfully.",
                "Success");
        }

        public static void Deleted()
        {
            Info(
                "Item deleted successfully.",
                "Success");
        }

        public static void Updated()
        {
            Info(
                "Data updated successfully.",
                "Success");
        }

        public static void Added()
        {
            Info(
                "Item added successfully.",
                "Success");
        }

        public static void Validation(string message)
        {
            Warning(
                message,
                "Validation");
        }

        public static void DatabaseError(Exception ex)
        {
            Error(
                $"Database Error\n\n{ex.Message}",
                "Database");
        }
    }
}