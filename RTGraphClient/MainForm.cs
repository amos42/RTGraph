using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTGraphProtocol;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        private RTGraphComm comm = new RTGraphComm("127.0.0.1", 11000, 12000);
        private bool continusMode = true;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comm.ErrorEvent += new ErrorEventHandler(CommError);
            comm.PacketReceived += new PacketReceivedEventHandler(ReceivePacket);
            comm.camParam.PropertyChanged += new PropertyChangedEventHandler(ParameterChanged);

            var cfg = new ConfigUtil("network");
            var hostIP = cfg.GetValue("HostIP");
            var sendPort = cfg.GetValue("SendPort");
            var recvPort = cfg.GetValue("RecvPort");

            if (!String.IsNullOrEmpty(hostIP)) comm.HostIP = hostIP;
            if (!String.IsNullOrEmpty(sendPort)) comm.SendPort = Int32.Parse(sendPort);
            if (!String.IsNullOrEmpty(recvPort)) comm.RecvPort = Int32.Parse(recvPort);

            //SocketOpenBtn_Click(this, new EventArgs());
            openToolStripMenuItem_Click(this, new EventArgs());
            //comm.OpenComm();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.ErrorEvent -= new ErrorEventHandler(CommError);
            comm.camParam.PropertyChanged -= new PropertyChangedEventHandler(ParameterChanged);
            comm.CloseComm();
        }

        private void CommError(object sender, ErrorEventArgs e)
        {
            this.Invoke(new Action(() => {
                MessageBox.Show(e.GetException().Message);
            }));
        }

        private void ParameterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RTGraphParameter.Level)) {
                this.Invoke(new Action(() => {
                    chart1.TriggerValue = comm.camParam.Level;
                }));
            } 
            else if (e.PropertyName == nameof(RTGraphParameter.TriggerSource))
            {
                this.Invoke(new Action(() => {
                    setGraphDrawMode(comm.camParam.TriggerSource);
                }));
            }
        }

        private void setGraphDrawMode(RTGraphParameterTriggerSource triggerSource)
        {
            if (!continusMode && triggerSource == RTGraphParameterTriggerSource.ImageTrigger)
            {
                continusMode = true;
                continueModeToolStripMenuItem.Checked = true;
                triggerModeToolStripMenuItem.Checked = false;
                var modestr = continusMode ? "Cont." : "Trig.";
                var v = (toolStripSplitButton1.Tag != null) ? (CheckState)toolStripSplitButton1.Tag : CheckState.Unchecked;
                if (v == CheckState.Unchecked)
                {
                    toolStripSplitButton1.Text = $"Start Capture ({modestr})";
                }
                else
                {
                    toolStripSplitButton1.Text = $"Stop Capture ({modestr})";
                }
            }
            else if (continusMode && triggerSource == RTGraphParameterTriggerSource.ExternalTrigger)
            {
                continusMode = false;
                continueModeToolStripMenuItem.Checked = false;
                triggerModeToolStripMenuItem.Checked = true;
                var modestr = continusMode ? "Cont." : "Trig.";
                var v = (toolStripSplitButton1.Tag != null) ? (CheckState)toolStripSplitButton1.Tag : CheckState.Unchecked;
                if (v == CheckState.Unchecked)
                {
                    toolStripSplitButton1.Text = $"Start Capture ({modestr})";
                }
                else
                {
                    toolStripSplitButton1.Text = $"Stop Capture ({modestr})";
                }
                chart1.Clear();
            }
        }

        private void ConnectProcess(bool connected)
        {
            if (connected)
            {
                toolStripButton3.Text = "Disconnect";
                toolStripButton3.Checked = true;
                toolStripSplitButton1.Enabled = true;
                toolStripDropDownButton2.Enabled = true;
            } 
            else
            {
                toolStripButton3.Text = "Cconnect";
                toolStripButton3.Checked = false;
                toolStripSplitButton1.Enabled = false;
                toolStripDropDownButton2.Enabled = false;
            }

        }

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            this.Invoke(new Action(() => {
                if (e.Type == 1)
                {
                    timer1.Stop();
                    ConnectProcess(true);
                }
                else if (e.Type == 2)
                {
                    ConnectProcess(false);
                }
                else if (e.Type == 10)
                {
                    if (continusMode)
                    {
                        if (e.Packet.Option == 0x2) { 
                            chart1.AddValueLine(-1, e.Packet.data, 2, e.Packet.data.Length - 2);
                        }
                    } 
                    else
                    {
                        if (e.Packet.Option == 0x3)
                        {
                            int idx = (short)(e.Packet.data[0] | ((int)e.Packet.data[1] << 8));
                            chart1.AddValueLine(idx, e.Packet.data, 2, e.Packet.data.Length - 2);
                        }
                    }
                }
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            if (!comm.connected)
            {
                MessageBox.Show("장치로부터 응답이 없습니다.");
                toolStripButton3_Click(this, new EventArgs());
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!openToolStripMenuItem.Checked)
            {
                comm.OpenComm();

                toolStripButton3.Enabled = true;

                toolStripDropDownButton1.Image = Properties.Resources.on;
                openToolStripMenuItem.Checked = true;
            }
            else
            {
                comm.CloseComm();

                toolStripButton3.Enabled = false;
                ConnectProcess(false);

                toolStripDropDownButton1.Image = Properties.Resources.off;
                openToolStripMenuItem.Checked = false;
            }
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new NetworkSettingForm(comm).ShowDialog() == DialogResult.OK)
            {
                if (comm.Opened) comm.CloseComm();
                comm.OpenComm();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (toolStripButton3.CheckState == CheckState.Unchecked)
            {
                comm.Connect();

                toolStripButton3.Text = "Cconnecting...";
                toolStripButton3.CheckState = CheckState.Indeterminate;

                timer1.Start();
            }
            else
            {
                comm.Disconnect();

                toolStripButton3.Text = "Connect";
                toolStripButton3.Checked = false;
            }
        }

        private void parametersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ParamForm(comm).ShowDialog();
        }

        private void calibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new CalibForm(comm).ShowDialog();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            var v = (toolStripSplitButton1.Tag != null) ? (CheckState)toolStripSplitButton1.Tag : CheckState.Unchecked;
            var modestr = continusMode ? "Cont." : "Trig.";
            if (v == CheckState.Unchecked)
            {
                comm.StartCapture();

                toolStripSplitButton1.Text = $"Stop Capture ({modestr})";
                toolStripSplitButton1.Tag = CheckState.Checked;
            }
            else
            {
                comm.StopCapture();

                toolStripSplitButton1.Text = $"Start Capture ({modestr})";
                toolStripSplitButton1.Tag = CheckState.Unchecked;
            }
        }

        private void continueModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comm.camParam.TriggerSource != RTGraphParameterTriggerSource.ImageTrigger) { 
                var param = comm.camParam.Clone() as RTGraphParameter;
                param.TriggerSource = RTGraphParameterTriggerSource.ImageTrigger;
                comm.ApplyParam(param);
            }
        }

        private void triggerModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comm.camParam.TriggerSource != RTGraphParameterTriggerSource.ExternalTrigger)
            {
                var param = comm.camParam.Clone() as RTGraphParameter;
                param.TriggerSource = RTGraphParameterTriggerSource.ExternalTrigger;
                comm.ApplyParam(param);
            }
        }
    }
}
