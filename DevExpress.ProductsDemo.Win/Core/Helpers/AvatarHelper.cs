using DevExpress.ProductsDemo.Win.Domain;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DevExpress.ProductsDemo.Win.Core.Helpers
{
    /// <summary>
    /// One fixed avatar per role — every user with the same role sees the
    /// same image. Drawn on the fly (colored circle + letter), cached so it's
    /// only rendered once per role/size. No image assets required.
    /// </summary>
    public static class AvatarHelper
    {
        private static readonly Dictionary<string, Bitmap> _cache = new Dictionary<string, Bitmap>();

        private static readonly Dictionary<string, Color> RoleColors = new Dictionary<string, Color>
        {
            { UserRoles.Admin,     Color.FromArgb(192, 57, 43)  },
            { UserRoles.Manager,   Color.FromArgb(41, 128, 185) },
            { UserRoles.DataEntry, Color.FromArgb(39, 174, 96)  },
            { UserRoles.Viewer,    Color.FromArgb(127, 140, 141)}
        };

        private static readonly Dictionary<string, string> RoleGlyphs = new Dictionary<string, string>
        {
            { UserRoles.Admin,     "A" },
            { UserRoles.Manager,   "M" },
            { UserRoles.DataEntry, "D" },
            { UserRoles.Viewer,    "V" }
        };

        public static Bitmap GetAvatar(string role, int size = 120)
        {
            string key = $"{role}_{size}";
            if (_cache.TryGetValue(key, out var cached))
                return cached;

            Color color = RoleColors.TryGetValue(role, out var c) ? c : Color.Gray;
            string glyph = RoleGlyphs.TryGetValue(role, out var g) ? g : "?";

            var bmp = new Bitmap(size, size);
            using (var g2 = Graphics.FromImage(bmp))
            {
                g2.SmoothingMode = SmoothingMode.AntiAlias;
                g2.Clear(Color.Transparent);

                using (var brush = new SolidBrush(color))
                    g2.FillEllipse(brush, 0, 0, size - 1, size - 1);

                using (var font = new Font("Segoe UI", size * 0.4f, FontStyle.Bold))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var textSize = g2.MeasureString(glyph, font);
                    var point = new PointF((size - textSize.Width) / 2, (size - textSize.Height) / 2);
                    g2.DrawString(glyph, font, textBrush, point);
                }
            }

            _cache[key] = bmp;
            return bmp;
        }
    }
}