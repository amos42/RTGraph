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
        private int continusMode = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comm.ErrorEvent += new ErrorEventHandler(CommError);
            comm.PacketReceived += new PacketReceivedEventHandler(ReceivePacket);
            comm.DeviceParameter.PropertyChanged += new PropertyChangedEventHandler(ParameterChanged);

            var cfg = new ConfigUtil("network");
            var hostIP = cfg.GetValue("HostIP");
            var sendPort = cfg.GetValue("SendPort");
            var recvPort = cfg.GetValue("RecvPort");

            if (!String.IsNullOrEmpty(hostIP)) comm.HostIP = hostIP;
            if (!String.IsNullOrEmpty(sendPort)) comm.SendPort = Int32.Parse(sendPort);
            if (!String.IsNullOrEmpty(recvPort)) comm.RecvPort = Int32.Parse(recvPort);

            DeviceCommOpen(true);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.ErrorEvent -= new ErrorEventHandler(CommError);
            comm.DeviceParameter.PropertyChanged -= new PropertyChangedEventHandler(ParameterChanged);
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
                    chart1.TriggerValue = comm.DeviceParameter.Level;
                }));
            } 
            else if (e.PropertyName == nameof(RTGraphParameter.TriggerSource))
            {
                this.Invoke(new Action(() => {
                    setGraphDrawMode(comm.DeviceParameter.TriggerSource);
                }));
            }
        }

        private void setGraphDrawMode(RTGraphParameterTriggerSource triggerSource)
        {
            if (continusMode != 0 && triggerSource == RTGraphParameterTriggerSource.ImageTrigger)
            {
                continusMode = 0;
                continuesToolStripMenuItem.Checked = true;
                triggerModeToolStripMenuItem1.Checked = false;
                toolStripDropDownButton4.Text = "Continues Mode";
            }
            else if (continusMode != 1 && triggerSource == RTGraphParameterTriggerSource.ExternalTrigger)
            {
                continusMode = 1;
                continuesToolStripMenuItem.Checked = false;
                triggerModeToolStripMenuItem1.Checked = true;
                toolStripDropDownButton4.Text = "Trigger Mode";
                chart1.Clear();
            }
        }

        private void DeviceCommOpen(bool isOpen)
        {
            if (isOpen)
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
                SetConnectState(false);

                toolStripDropDownButton1.Image = Properties.Resources.off;
                openToolStripMenuItem.Checked = false;
            }
        }

        private void DeviceConnect(int connectState)
        {
            if (connectState == 1)
            {
                comm.Connect();

                toolStripButton3.Text = "Cconnecting...";
                toolStripButton3.CheckState = CheckState.Indeterminate;

                timer1.Start();
            }
            else if (connectState == 2)
            {
                SetConnectState(true);

                timer1.Stop();
            }
            else
            {
                comm.Disconnect();

                SetConnectState(false);

                timer1.Stop();
            }
        }

        private void SetConnectState(bool connected)
        {
            if (connected)
            {
                toolStripButton3.Text = "Disconnect";
                toolStripButton3.CheckState = CheckState.Checked;
                toolStripDropDownButton2.Enabled = true;
                toolStripDropDownButton3.Enabled = true;
                toolStripDropDownButton4.Enabled = true;
            }
            else
            {
                toolStripButton3.Text = "Cconnect";
                toolStripButton3.CheckState = CheckState.Unchecked;
                toolStripDropDownButton2.Enabled = false;
                toolStripDropDownButton3.Enabled = false;
                toolStripDropDownButton4.Enabled = false;
            }

        }

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            this.Invoke(new Action(() => {
                if (e.Type == 1)
                {
                    timer1.Stop();
                    SetConnectState(true);
                }
                else if (e.Type == 2)
                {
                    SetConnectState(false);
                }
                else if (e.Type == 10)
                {
                    if (continusMode == 0)
                    {
                        if (e.Packet.Option == 0x2) { 
                            chart1.AddValueLine(-1, e.Packet.data, 2, e.Packet.data.Length - 2);
                        }
                    } 
                    else if (continusMode == 1)
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
            if (!comm.Connected)
            {
                MessageBox.Show("장치로부터 응답이 없습니다.");
                DeviceConnect(0);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeviceCommOpen(!openToolStripMenuItem.Checked);
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
                DeviceConnect(1);
            }
            else if (toolStripButton3.CheckState == CheckState.Checked)
            {
                DeviceConnect(0);
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

        private void startGrabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comm.StartCapture();
        }

        private void stopGrabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comm.StopCapture();
        }

        private void continuesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comm.DeviceParameter.TriggerSource != RTGraphParameterTriggerSource.ImageTrigger)
            {
                var param = comm.DeviceParameter.Clone() as RTGraphParameter;
                param.TriggerSource = RTGraphParameterTriggerSource.ImageTrigger;
                comm.ApplyParam(param);
            }
        }

        private void triggerModeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (comm.DeviceParameter.TriggerSource != RTGraphParameterTriggerSource.ExternalTrigger)
            {
                var param = comm.DeviceParameter.Clone() as RTGraphParameter;
                param.TriggerSource = RTGraphParameterTriggerSource.ExternalTrigger;
                comm.ApplyParam(param);
            }
        }
    }
}
