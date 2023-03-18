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
using static RTGraphProtocol.PacketReceivedEventArgs;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        private RTGraphComm comm = new RTGraphComm("127.0.0.1", 11000, 12000);
        private int continusMode = 0;
        private bool isActive = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comm.ErrorEvent += new ErrorEventHandler(commError);
            comm.PacketReceived += new PacketReceivedEventHandler(receivePacket);
            comm.DeviceParameter.PropertyChanged += new PropertyChangedEventHandler(parameterChanged);

            var cfg = new AppConfig("network");
            var hostIP = cfg.GetValue("HostIP");
            var sendPort = cfg.GetValue("SendPort");
            var recvPort = cfg.GetValue("RecvPort");

            if (!String.IsNullOrEmpty(hostIP)) comm.HostIP = hostIP;
            if (!String.IsNullOrEmpty(sendPort)) comm.SendPort = Int32.Parse(sendPort);
            if (!String.IsNullOrEmpty(recvPort)) comm.RecvPort = Int32.Parse(recvPort);

            isActive = true;

            deviceCommOpen(true);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            isActive = false;

            comm.ErrorEvent -= commError;
            comm.DeviceParameter.PropertyChanged -= parameterChanged;
            comm.CloseComm();
        }

        private void commError(object sender, ErrorEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => {
                MessageBox.Show(e.GetException().Message);
            }));
        }

        private void parameterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RTGraphParameter.ThresholdLevel)) {
                this.Invoke(new MethodInvoker(() => {
                    chart1.TriggerValue = comm.DeviceParameter.ThresholdLevel;
                }));
            } 
            else if (e.PropertyName == nameof(RTGraphParameter.TriggerSource))
            {
                this.Invoke(new MethodInvoker(() => {
                    setGraphDrawMode(comm.DeviceParameter.TriggerSource);
                }));
            }
        }

        private void setGraphDrawMode(RTGraphParameter.TriggerSourceEnum triggerSource)
        {
            if (continusMode != 0 && triggerSource == RTGraphParameter.TriggerSourceEnum.ImageTrigger)
            {
                continusMode = 0;
                continuesToolStripMenuItem.Checked = true;
                triggerModeToolStripMenuItem1.Checked = false;
                toolStripDropDownButton4.Text = "Continues Mode";
                chart1.IndexedMode = false;
                chart1.BufferCount = 100;
                calibrationToolStripMenuItem.Enabled = (btnGrab.CheckState == CheckState.Checked);
            }
            else if (continusMode != 1 && triggerSource == RTGraphParameter.TriggerSourceEnum.ExternalTrigger)
            {
                continusMode = 1;
                continuesToolStripMenuItem.Checked = false;
                triggerModeToolStripMenuItem1.Checked = true;
                toolStripDropDownButton4.Text = "Trigger Mode";
                chart1.IndexedMode = true;
                chart1.BufferCount = 300;
                chart1.Clear();
                calibrationToolStripMenuItem.Enabled = false;
            }
        }

        private void deviceCommOpen(bool isOpen)
        {
            if (isOpen)
            {
                comm.OpenComm();

                btnConnect.Enabled = true;

                toolStripDropDownButton1.Image = Properties.Resources.on;
                openToolStripMenuItem.Checked = true;
            }
            else
            {
                comm.CloseComm();

                btnConnect.Enabled = false;
                setConnectState(false);

                toolStripDropDownButton1.Image = Properties.Resources.off;
                openToolStripMenuItem.Checked = false;
            }
        }

        private void deviceConnect(int connectState)
        {
            if (connectState == 1)
            {
                comm.Connect();

                btnConnect.Text = "Connecting...";
                btnConnect.CheckState = CheckState.Indeterminate;

                connectionTimer.Start();
            }
            else if (connectState == 2)
            {
                setConnectState(true);

                connectionTimer.Stop();
            }
            else
            {
                comm.Disconnect();

                setConnectState(false);

                connectionTimer.Stop();
            }
        }

        private void doGrab(int connectState)
        {
            if (connectState == 1)
            {
                comm.StartCapture();
                btnGrab.Text = "Grab Starting...";
                btnGrab.CheckState = CheckState.Indeterminate;
            }
            else if (connectState == 2)
            {
                setGrabState(true);
            }
            else
            {
                comm.StopCapture();
                setGrabState(false);
            }
        }

        private void setConnectState(bool connected)
        {
            if (connected)
            {
                btnConnect.Text = "Disconnect";
                btnConnect.CheckState = CheckState.Checked;
                btnGrab.Enabled = true;
                toolStripDropDownButton2.Enabled = true;
                toolStripDropDownButton4.Enabled = true;
            }
            else
            {
                btnConnect.Text = "Connect";
                btnConnect.CheckState = CheckState.Unchecked;
                btnGrab.Enabled = false;
                setGrabState(false);
                toolStripDropDownButton2.Enabled = false;
                toolStripDropDownButton4.Enabled = false;
            }
        }

        private void setGrabState(bool grab)
        {
            if (grab)
            {
                btnGrab.Text = "Stop Grab";
                btnGrab.CheckState = CheckState.Checked;
                calibrationToolStripMenuItem.Enabled = (continusMode == 0);
            }
            else
            {
                btnGrab.Text = "Start Grab";
                btnGrab.CheckState = CheckState.Unchecked;
                calibrationToolStripMenuItem.Enabled = false;
            }
        }

        private void receivePacket(object sender, PacketReceivedEventArgs e)
        {
            if (!isActive) return;

            if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.Connected)
            {
                this.Invoke(new MethodInvoker(() => {
                    connectionTimer.Stop();
                    setConnectState(true);
                }));
            }
            else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.Disconnected)
            {
                this.Invoke(new MethodInvoker(() => {
                    setConnectState(false);
                }));
            }
            else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrabStarted)
            {
                this.Invoke(new MethodInvoker(() => {
                    setGrabState(true);
                }));
            }
            else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrabStopped)
            {
                this.Invoke(new MethodInvoker(() => {
                    setGrabState(false);
                }));
            }
            else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrabDataReceivced)
            {
                if (e.Packet.Option == 0x2 && continusMode == 0)
                {
                    int pos = (short)(e.Packet.Data[0] | ((int)e.Packet.Data[1] << 8));
                    chart1.SetValueLine(0, e.Packet.Data, 2, e.Packet.Data.Length - 2, pos, false);
                    this.Invoke(new MethodInvoker(() => {
                        if (!refreshTimer.Enabled) refreshTimer.Start();
                    }));
                } 
                else if (e.Packet.Option == 0x3 && continusMode == 1)
                {
                    int pos = (short)(e.Packet.Data[0] | ((int)e.Packet.Data[1] << 8));
                    chart1.SetValueLine(0, e.Packet.Data, 2, e.Packet.Data.Length - 2, pos, false);
                    this.Invoke(new MethodInvoker(() => {
                        if (!refreshTimer.Enabled) refreshTimer.Start();
                    }));
                }
            }
        }

        private void btnGrab_Click(object sender, EventArgs e)
        {
            if (btnGrab.CheckState == CheckState.Unchecked)
            {
                doGrab(1);
            }
            else if (btnGrab.CheckState == CheckState.Checked)
            {
                doGrab(0);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            deviceCommOpen(!openToolStripMenuItem.Checked);
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (new NetworkSettingForm(comm).ShowDialog() == DialogResult.OK)
            {
                if (comm.Opened) comm.CloseComm();
                comm.OpenComm();
            }
        }

        private void btnConnec_Click(object sender, EventArgs e)
        {
            if (btnConnect.CheckState == CheckState.Unchecked)
            {
                deviceConnect(1);
            }
            else if (btnConnect.CheckState == CheckState.Checked)
            {
                deviceConnect(0);
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
            if (comm.DeviceParameter.TriggerSource != RTGraphParameter.TriggerSourceEnum.ImageTrigger)
            {
                if(btnGrab.CheckState != CheckState.Checked)
                {
                    comm.ChangeGrapMode(0);
                }
                else
                {
                    MessageBox.Show("Grab이 진행 중일 땐 모드를 변경할 수 없습니다.");
                }
            }
        }

        private void triggerModeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (comm.DeviceParameter.TriggerSource != RTGraphParameter.TriggerSourceEnum.ExternalTrigger)
            {
                if (btnGrab.CheckState != CheckState.Checked)
                {
                    comm.ChangeGrapMode(1);
                }
                else
                {
                    MessageBox.Show("Grab이 진행 중일 땐 모드를 변경할 수 없습니다.");
                }
            }
        }

        private void connectionTimer_Tick(object sender, EventArgs e)
        {
            connectionTimer.Stop();
            if (!comm.Connected)
            {
                MessageBox.Show("장치로부터 응답이 없습니다.");
                deviceConnect(0);
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            refreshTimer.Stop();
            chart1.Refresh();
        }
    }
}
