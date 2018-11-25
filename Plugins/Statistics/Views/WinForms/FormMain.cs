using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Statistics.Views.WinForms
{
    public partial class FormMain : Form
    {
        VgcApis.Models.IServersService vgcServers;
        Dictionary<string, VgcApis.Models.StatsSample> preSamples
            = new Dictionary<string, VgcApis.Models.StatsSample>();

        public FormMain(VgcApis.Models.IServersService vgcServers)
        {
            this.vgcServers = vgcServers;
            InitializeComponent();
            this.FormClosing += (s, a) => ReleaseUpdateTimer();
            VgcApis.Libs.UI.AutoSetFormIcon(this);
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Text = Properties.Resources.Name + " v" + Properties.Resources.Version;
            updateStatsTableTimer.Tick += UpdateStatsTable;
            updateStatsTableTimer.Start();
        }
        #region public methods

        #endregion

        #region private methods
        VgcApis.Models.StatsSample GetPreSample(string name)
        {
            if (preSamples.ContainsKey(name))
            {
                return preSamples[name];
            }
            return null;
        }

        void UpdateStatsTable(object sender, EventArgs args)
        {
            Task.Factory.StartNew(UpdateStatsTableWorker);
        }

        readonly object updateStatsTableLocker = new object();
        bool isUpdating = false;
        void UpdateStatsTableWorker()
        {
            lock (updateStatsTableLocker)
            {
                if (isUpdating)
                {
                    return;
                }
                isUpdating = true;
            }

            var contents = vgcServers
                .GetAllServersList()
                .Where(s => s.IsCoreRunning())
                .Select(s => GetterCoreInfo(s))
                .ToList();

            RefreshListViewControl(contents);

            isUpdating = false;
        }

        private void RefreshListViewControl(List<string[]> contents)
        {
            lvStatsTable.Invoke((MethodInvoker)delegate
            {
                lvStatsTable.BeginUpdate();
                try
                {
                    lvStatsTable.Items.Clear();
                    foreach (var content in contents)
                    {
                        lvStatsTable.Items.Add(
                            new ListViewItem(content));
                    }
                }
                finally
                {
                    lvStatsTable.EndUpdate();
                }
            });
        }

        string[] GetTotalFromSampleDatas(
            string name,
            VgcApis.Models.StatsSample data)
        {
            var empty = new string[] { "0", "0", "0", "0", "0" };
            if (data == null)
            {
                return empty;
            }

            var preData = GetPreSample(name);

            if (preData == null)
            {
                preSamples[name] = data;
                return empty;
            }

            const int MiB = 1024 * 1024;
            var td = Math.Max(0, data.statsDownlink / MiB);
            var tu = Math.Max(0, data.statsUplink / MiB);

            var speed = CalcSpeed(preData, data);
            preSamples[name] = data;

            return new string[] {
                "", // reserved for name
                speed[0],
                speed[1],
                td.ToString(),
                tu.ToString() };
        }

        string[] CalcSpeed(
            VgcApis.Models.StatsSample start,
            VgcApis.Models.StatsSample end)
        {
            var time = 1.0 * (end.stamp - start.stamp) / TimeSpan.TicksPerSecond;
            if (time < 0.001)
            {
                return new string[] { "0", "0" };
            }

            var d = 1.0 * (end.statsDownlink - start.statsDownlink) / time / 1024;
            var u = 1.0 * (end.statsUplink - start.statsUplink) / time / 1024;
            return new string[] {
                (Math.Max(0,(int)d)).ToString(),
                (Math.Max(0,(int)u)).ToString(),
            };
        }

        string[] GetterCoreInfo(VgcApis.Models.ICoreCtrl coreCtrl)
        {
            var curData = coreCtrl.Peek();
            var name = coreCtrl.GetName();
            var result = GetTotalFromSampleDatas(name, curData);
            result[0] = coreCtrl.GetName();
            return result;
        }

        System.Windows.Forms.Timer updateStatsTableTimer = new Timer
        {
            Interval = 2000,
        };

        void ReleaseUpdateTimer()
        {
            updateStatsTableTimer.Stop();
            updateStatsTableTimer.Tick -= UpdateStatsTable;
            updateStatsTableTimer.Dispose();
        }
        #endregion
    }
}
