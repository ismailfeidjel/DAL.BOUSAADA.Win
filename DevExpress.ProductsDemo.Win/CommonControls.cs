using DevExpress.XtraEditors;
using System.Windows.Forms;

namespace DevExpress.MailClient.Win
{
    public class PictureEditSimpleZoom : PictureEdit
    {
        protected override void OnMouseWheelCore(MouseEventArgs e)
        {
            if (Control.ModifierKeys != Keys.None) return;
            base.OnMouseWheelCore(e);
        }
    }
}
