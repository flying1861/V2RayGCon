using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace V2RayGCon.Views.WinForms
{
    public partial class FormLog : Form
    {
        #region Sigleton
        static FormLog _instant;
        public static FormLog GetForm()
        {
            if (_instant == null || _instant.IsDisposed)
            {
                _instant = new FormLog();
            }
            return _instant;
        }
        #endregion

        Service.Setting setting;
        int maxNumberLines;

        FormLog()
        {
            // for debug debug
            // throw new System.ArgumentException("hello");

            setting = Service.Setting.Instance;
            maxNumberLines = setting.maxLogLines;

            InitializeComponent();

            this.FormClosed += (s, e) =>
            {
                setting.OnLog -= LogReceiver;
            };

            Lib.UI.SetFormLocation<FormLog>(this, Model.Data.Enum.FormLocations.BottomLeft);

#if DEBUG
            this.Icon = Properties.Resources.icon_light;
#endif

            this.Show();

            rtBoxLogger.Text = setting.logCache;

            setting.OnLog += LogReceiver;
        }

        void LogReceiver(object sender, VgcApis.Models.StrEvent e)
        {
            Task.Factory.StartNew(() =>
            {
                var content = e.Data;
                try
                {
                    rtBoxLogger.Invoke((MethodInvoker)delegate
                    {
                        if (rtBoxLogger.Lines.Length >= maxNumberLines - 1)
                        {
                            rtBoxLogger.Lines = rtBoxLogger.Lines.Skip(rtBoxLogger.Lines.Length - maxNumberLines).ToArray();
                        }
                        rtBoxLogger.AppendText(content + "\r\n");
                    });
                }
                catch { }
            });
        }

        private void rtBoxLogger_TextChanged(object sender, System.EventArgs e)
        {
            // set the current caret position to the end
            rtBoxLogger.SelectionStart = rtBoxLogger.Text.Length;
            // scroll it automatically
            rtBoxLogger.ScrollToCaret();
        }
    }
}
