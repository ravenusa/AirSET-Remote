using System;
using System.Drawing;
using System.Windows.Forms;

namespace AirSET.Agent
{
    public partial class FormStatus : Form
    {
        private AgentListener listener;

        private static readonly System.Collections.Generic.List<string> LogHistory = new System.Collections.Generic.List<string>();

        public FormStatus(AgentListener agentListener)
        {
            InitializeComponent();
            this.listener = agentListener;
            this.Text = "AirSET Agent - Status";

            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            }
            catch { }

            lblHostname.Text = "Hostname : " + System.Net.Dns.GetHostName();
            lblStatus.Text = "Status : Running (Listening Port 3623)";
            lblStatus.ForeColor = Color.DarkGreen;

            // Muat log sebelumnya
            lock (LogHistory)
            {
                foreach (var l in LogHistory)
                {
                    txtLog.AppendText(l + Environment.NewLine);
                }
            }

            listener.OnLogReceived += (log) =>
            {
                lock (LogHistory)
                {
                    LogHistory.Add(log);
                }

                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        txtLog.AppendText(log + Environment.NewLine);
                        txtLog.SelectionStart = txtLog.Text.Length;
                        txtLog.ScrollToCaret();
                    }));
                }
            };
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
            base.OnFormClosing(e);
        }
    }
}