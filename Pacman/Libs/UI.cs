using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pacman.Libs
{
    public static class UI
    {
        public static void MsgBox(string content)
        {
            MessageBox.Show(content, Properties.Resources.Name);
        }

        public static void MsgBoxAsync(string content)
        {
            Task.Factory.StartNew(() => MsgBox(content));
        }
    }
}
