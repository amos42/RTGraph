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

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comm.ErrorEvent += new ErrorEventHandler(CommError);
            comm.PacketReceived += new PacketReceivedEventHandler(ReceivePacket);
            comm.DeviceParameter.PropertyChanged += new PropertyChangedEventHandler(ParameterChanged);

            var cfg = new AppConfig("network");
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
            comm.ErrorEvent -= CommError;
            comm.DeviceParameter.PropertyChanged -= ParameterChanged;
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
            if (e.PropertyName == nameof(RTGraphParameter.ThresholdLevel)) {
                this.Invoke(new Action(() => {
                    chart1.TriggerValue = comm.DeviceParameter.ThresholdLevel;
                }));
            } 
            else if (e.PropertyName == nameof(RTGraphParameter.TriggerSource))
            {
                this.Invoke(new Action(() => {
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
                calibrationToolStripMenuItem.Enabled = (btnGrab.CheckState == CheckState.Checked);
            }
            else if (continusMode != 1 && triggerSource == RTGraphParameter.TriggerSourceEnum.ExternalTrigger)
            {
                continusMode = 1;
                continuesToolStripMenuItem.Checked = false;
                triggerModeToolStripMenuItem1.Checked = true;
                toolStripDropDownButton4.Text = "Trigger Mode";
                chart1.Clear();
                calibrationToolStripMenuItem.Enabled = false;
            }
        }

        private void DeviceCommOpen(bool isOpen)
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

                btnConnect.Text = "Connecting...";
                btnConnect.CheckState = CheckState.Indeterminate;

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

        private void DoGrab(int connectState)
        {
            if (connectState == 1)
            {
                comm.StartCapture();
                btnGrab.Text = "Grap Starting...";
                btnGrab.CheckState = CheckState.Indeterminate;
            }
            else if (connectState == 2)
            {
                SetGrabState(true);
            }
            else
            {
                comm.StopCapture();
                SetGrabState(false);
            }
        }

        private void SetConnectState(bool connected)
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
                toolStripDropDownButton2.Enabled = false;
                toolStripDropDownButton4.Enabled = false;
            }
        }

        private void SetGrabState(bool grab)
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

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            this.Invoke(new Action(() => {
                if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.Connected)
                {
                    timer1.Stop();
                    SetConnectState(true);
                }
                else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.Disconnected)
                {
                    SetConnectState(false);
                }
                else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrapStarted)
                {
                    SetGrabState(true);
                }
                else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrapStopped)
                {
                    SetGrabState(false);
                }
                else if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrabDataReceivced)
                {
                    if (continusMode == 0)
                    {
                        if (e.Packet.Option == 0x2) { 
                            chart1.SetValueLine(0, e.Packet.Data, 2, e.Packet.Data.Length - 2);
                        }
                    } 
                    else if (continusMode == 1)
                    {
                        if (e.Packet.Option == 0x3)
                        {
                            int pos = (short)(e.Packet.Data[0] | ((int)e.Packet.Data[1] << 8));
                            chart1.SetValueLine(0, e.Packet.Data, 2, e.Packet.Data.Length - 2, pos);
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

        private void btnGrab_Click(object sender, EventArgs e)
        {
            if (btnGrab.CheckState == CheckState.Unchecked)
            {
                DoGrab(1);
            }
            else if (btnGrab.CheckState == CheckState.Checked)
            {
                DoGrab(0);
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

        private void btnConnec_Click(object sender, EventArgs e)
        {
            if (btnConnect.CheckState == CheckState.Unchecked)
            {
                DeviceConnect(1);
            }
            else if (btnConnect.CheckState == CheckState.Checked)
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
            if (comm.DeviceParameter.TriggerSource != RTGraphParameter.TriggerSourceEnum.ImageTrigger)
            {
                comm.ChangeGrapMode(0);
            }
        }

        private void triggerModeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (comm.DeviceParameter.TriggerSource != RTGraphParameter.TriggerSourceEnum.ExternalTrigger)
            {
                comm.ChangeGrapMode(1);
            }
        }
    }
}
