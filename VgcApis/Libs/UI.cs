using System.Threading.Tasks;
using System.Windows.Forms;
using VgcApis.Resources.Langs;

namespace VgcApis.Libs
{
    public static class UI
    {
        public static string ShowSelectFileDialog(string extension)
        {
            OpenFileDialog readFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = extension,
                RestoreDirectory = true,
                CheckFileExists = true,
                CheckPathExists = true,
                ShowHelp = true,
            };

            var fileName = string.Empty;

            if (readFileDialog.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            return readFileDialog.FileName;
        }

        public static void VisitUrl(string msg, string url)
        {
            var text = string.Format("{0}\n{1}", msg, url);
            if (Confirm(text))
            {
                Task.Factory.StartNew(() => System.Diagnostics.Process.Start(url));
            }
        }

        public static void MsgBox(string title, string content)
        {
            MessageBox.Show(content ?? string.Empty, title ?? string.Empty);
        }

        public static void MsgBoxAsync(string title, string content)
        {
            Task.Factory.StartNew(() => MsgBox(title, content));
        }

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

        public static System.Drawing.Icon GetAppIcon()
        {
#if DEBUG
            return Properties.Resources.icon_light;
#else
            return Properties.Resources.icon_dark;
#endif
        }
    }
}
