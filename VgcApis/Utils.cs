using System.Windows.Forms;
using VgcApis.Resources.Langs;

namespace VgcApis
{
    public static class Utils
    {
        public static bool Confirm(string content)
        {
            var confirm = MessageBox.Show(
                content,
                I18N.Confirm,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            return confirm == DialogResult.Yes;
        }

        public static void AutoSetFormIcon(Form form)
        {
#if DEBUG
            form.Icon = Properties.Resources.icon_light;
#else
            form.Icon = Properties.Resources.icon_dark;
#endif
        }
    }
}
