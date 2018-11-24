using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Statistics.Views.WinForms
{
    public partial class FormMain : Form
    {
        VgcApis.Models.IServersService vgcServers;
        public FormMain(VgcApis.Models.IServersService vgcServers)
        {
            this.vgcServers = vgcServers;
            InitializeComponent();
            this.FormClosing += (s, a) => ReleaseUpdateTimer();
            VgcApis.Libs.UI.AutoSetFormIcon(this);
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            this.Text = Properties.Resources.Name + " - v" + Properties.Resources.Version;
            updateStatsTableTimer.Tick += UpdateStatsTable;
            updateStatsTableTimer.Start();
        }
        #region public methods

        #endregion

        #region private methods
        readonly object updateStatsTableLocker = new object();
        bool isUpdating = false;
        void UpdateStatsTable(object sender, EventArgs args)
        {
            lock (updateStatsTableLocker)
            {
                if (isUpdating)
                {
                    return;
                }
                isUpdating = true;
            }

            var serverList = vgcServers
                .GetAllServersList()
                .Where(s => s.GetSampleData().Count > 0)
                .ToList();

            ShowSampleData(serverList);
            isUpdating = false;
        }

        void ShowSampleData(List<VgcApis.Models.ICoreCtrl> servList)
        {
            lvStatsTable.Items.Clear();
            foreach (var ctrl in servList)
            {
                lvStatsTable.Items.Add(new ListViewItem(Core2Line(ctrl)));
            }
        }

        string[] GetTotalFromSampleDatas(
            ConcurrentQueue<VgcApis.Models.StatsSample> data)
        {
            if (data == null || data.Count <= 0)
            {
                return new string[] { "0", "0", "0", "0", "0" };
            }

            var q = data.ToList();
            var count = q.Count;
            var first = q[count - 1];
            var middle = q[Math.Max(0, count - 4)];

            var cur = CalcSpeed(middle, first);
            int d = first.statsDownlink / 1024 / 1024;
            int u = first.statsUplink / 1024 / 1024;
            return new string[] {
                "", // reserved for name
                cur[0],
                cur[1],
                d.ToString(),
                u.ToString() };
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
                ((int)d).ToString(),
                ((int)u).ToString(),
            };
        }

        string[] Core2Line(VgcApis.Models.ICoreCtrl coreCtrl)
        {
            var data = coreCtrl.GetSampleData();
            var result = GetTotalFromSampleDatas(data);
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
