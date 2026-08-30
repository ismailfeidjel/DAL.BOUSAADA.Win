using DevExpress.Utils.Svg;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using System;
using System.IO;
using System.Windows.Forms;

namespace DevExpress.ProductsDemo.Win.Core.BaseForms
{
    public class frmIconPicker : XtraForm
    {
        public string SelectedIconName { get; private set; }

        private readonly GalleryControl galleryControl = new GalleryControl();

        public frmIconPicker(string iconsFolder, string currentIconName = null)
        {
            Text = "اختيار أيقونة";
            Width = 480;
            Height = 420;
            RightToLeftLayout = true;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            galleryControl.Dock = DockStyle.Fill;
            galleryControl.Gallery.ColumnCount = 6;
            galleryControl.Gallery.ImageSize = new System.Drawing.Size(32, 32);
            galleryControl.Gallery.ItemCheckMode = ItemCheckMode.SingleCheck;

            var group = new GalleryItemGroup();
            galleryControl.Gallery.Groups.Add(group);

            if (Directory.Exists(iconsFolder))
            {
                foreach (var file in Directory.GetFiles(iconsFolder, "*.svg"))
                {
                    string fileName = Path.GetFileName(file);
                    var item = new DevExpress.XtraBars.Ribbon.GalleryItem();
                    item.Caption = Path.GetFileNameWithoutExtension(file);
                    item.Tag = fileName;

                    try
                    {
                        using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                            item.ImageOptions.SvgImage = SvgImage.FromStream(fs);
                    }
                    catch
                    {
                        continue; // skip unreadable/corrupt svg files
                    }

                    item.Checked = string.Equals(fileName, currentIconName, StringComparison.OrdinalIgnoreCase);
                    group.Items.Add(item);
                }
            }

            galleryControl.Gallery.ItemClick += (s, e) =>
            {
                SelectedIconName = e.Item.Tag as string;
                DialogResult = DialogResult.OK;
                Close();
            };

            var btnClear = new SimpleButton { Text = "بدون أيقونة", Dock = DockStyle.Bottom, Height = 32 };
            btnClear.Click += (s, e) =>
            {
                SelectedIconName = null;
                DialogResult = DialogResult.OK;
                Close();
            };

            Controls.Add(galleryControl);
            Controls.Add(btnClear);
        }
    }
}