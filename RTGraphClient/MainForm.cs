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
using DeviceSimulator;
using RTGraphProtocol;
using static RTGraphProtocol.PacketReceivedEventArgs;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        enum NotiTypeEnum
        {
            Info,
            Warning,
            Error
        }

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
            comm.PacketSended += new PacketSendedEventHandler(sendPacket);
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
            if (isActive)
            {
                if (btnConnect.CheckState == CheckState.Checked)
                {
                    deviceConnect(0);
                }

                isActive = false;
            }

            comm.ErrorEvent -= commError;
            comm.PacketReceived -= receivePacket;
            comm.PacketSended -= sendPacket;
            comm.DeviceParameter.PropertyChanged -= parameterChanged;
            comm.CloseComm();
        }

        private void commError(object sender, ErrorEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => {
                showNotiMessage(NotiTypeEnum.Error, e.GetException().Message);
            }));
        }

        private void parameterChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RTGraphParameter.ThresholdLevel)) {
                this.Invoke(new MethodInvoker(() => {
                    chart1.TriggerValue = comm.DeviceParameter.ThresholdLevel;
                }));
            }
            if (e.PropertyName == nameof(RTGraphParameter.ROIStart))
            {
                this.Invoke(new MethodInvoker(() => {
                    chart1.ValideAreaStart = comm.DeviceParameter.ROIStart;
                }));
            }
            if (e.PropertyName == nameof(RTGraphParameter.ROIEnd))
            {
                this.Invoke(new MethodInvoker(() => {
                    chart1.ValideAreaEnd = comm.DeviceParameter.ROIEnd;
                }));
            }
            if (e.PropertyName == nameof(RTGraphParameter.ThresholdLevel))
            {
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
                toolStripDropDownButton4.Text = "Continuous Mode";
                chart1.IndexedMode = false;
                chart1.BufferCount = 100;
                chart1.OSDVisible = true;
                // calibrationToolStripMenuItem.Enabled = (btnGrab.CheckState == CheckState.Checked); // 문제가 있어서 calibration 기능 막음.
                parametersToolStripMenuItem.Enabled = false; // 문제가 있어서 parameter 세팅은 continus 모드 start 된 상황에서만 활성화
            }
            else if (continusMode != 1 && triggerSource == RTGraphParameter.TriggerSourceEnum.ExternalTrigger)
            {
                continusMode = 1;
                continuesToolStripMenuItem.Checked = false;
                triggerModeToolStripMenuItem1.Checked = true;
                toolStripDropDownButton4.Text = "Trigger Mode";
                chart1.IndexedMode = true;
                chart1.BufferCount = 300;
                chart1.OSDVisible = false;
                chart1.Clear();
                calibrationToolStripMenuItem.Enabled = false;
                parametersToolStripMenuItem.Enabled = false; // 문제가 있어서 parameter 세팅은 continus 모드에서만 활성화
            }
        }

        private void deviceCommOpen(bool isOpen)
        {
            if (isOpen)
            {
                try
                {
                    comm.OpenComm();
                } 
                catch (Exception ex)
                {
                    showNotiMessage(NotiTypeEnum.Error, ex.Message);
                }
                finally
                {
                    btnConnect.Enabled = true;

                    toolStripDropDownButton1.Image = Properties.Resources.on;
                    openToolStripMenuItem.Checked = true;

                    showNotiMessage(NotiTypeEnum.Info, "Comm Device Opend");
                }
            }
            else
            {
                try
                {
                    comm.CloseComm();
                }
                catch (Exception ex)
                {
                    showNotiMessage(NotiTypeEnum.Error, ex.Message);
                }
                finally
                {
                    btnConnect.Enabled = false;
                    setConnectState(false);

                    toolStripDropDownButton1.Image = Properties.Resources.off;
                    openToolStripMenuItem.Checked = false;

                    showNotiMessage(NotiTypeEnum.Info, "Comm Device Closed");
                }
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
            if (connected && btnConnect.CheckState != CheckState.Checked)
            {
                btnConnect.Text = "Disconnect";
                btnConnect.CheckState = CheckState.Checked;
                btnGrab.Enabled = true;
                toolStripDropDownButton2.Enabled = true;
                toolStripDropDownButton4.Enabled = true;
                logControl1.AddItem(LogControl.LogTypeEnum.Info, "Connected");
            }
            else if (!connected && btnConnect.CheckState != CheckState.Unchecked)
            {
                btnConnect.Text = "Connect";
                btnConnect.CheckState = CheckState.Unchecked;
                btnGrab.Enabled = false;
                setGrabState(false);
                toolStripDropDownButton2.Enabled = false;
                toolStripDropDownButton4.Enabled = false;
                logControl1.AddItem(LogControl.LogTypeEnum.Info, "Disconnected");
            }
        }

        private void setGrabState(bool grab)
        {
            if (grab && btnGrab.CheckState != CheckState.Checked)
            {
                btnGrab.Text = "Stop Grab";
                btnGrab.CheckState = CheckState.Checked;
                // calibrationToolStripMenuItem.Enabled = (continusMode == 0); // 문제가 있어서 calibration 기능 막음.
                parametersToolStripMenuItem.Enabled = continusMode == 0; // 문제가 있어서 parameter 세팅은 continus 모드 start 된 상황에서만 활성화
                logControl1.AddItem(LogControl.LogTypeEnum.Info, "Grab Started");
            }
            else if (!grab && btnGrab.CheckState != CheckState.Unchecked)
            {
                btnGrab.Text = "Start Grab";
                btnGrab.CheckState = CheckState.Unchecked;
                calibrationToolStripMenuItem.Enabled = false;
                parametersToolStripMenuItem.Enabled = false; // 문제가 있어서 parameter 세팅은 continus 모드 start 된 상황에서만 활성화
                logControl1.AddItem(LogControl.LogTypeEnum.Info, "Grab Stopped");
            }
        }

        private void receivePacket(object sender, PacketReceivedEventArgs e)
        {
            if (!isActive) return;

            this.Invoke(new MethodInvoker(() => {
                logControl1.AddItem(LogControl.LogTypeEnum.Recv, BitConverter.ToString(e.Packet.Serialize()).Replace('-',' '));
            }));

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

        private void sendPacket(object sender, PacketEventArgs e)
        {
            if (!isActive) return;

            this.Invoke(new MethodInvoker(() => {
                logControl1.AddItem(LogControl.LogTypeEnum.Send, BitConverter.ToString(e.Packet.Serialize()).Replace('-', ' '));
            }));
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
                    showNotiMessage(NotiTypeEnum.Error, "You can't change the mode while a Grab is in progress.");
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
                    showNotiMessage(NotiTypeEnum.Error, "You can't change the mode while a Grab is in progress.");
                }
            }
        }

        private void connectionTimer_Tick(object sender, EventArgs e)
        {
            connectionTimer.Stop();
            if (!comm.Connected)
            {
                showNotiMessage(NotiTypeEnum.Error, "No response from the device.");
                logControl1.AddItem(LogControl.LogTypeEnum.Error, "No response from the device.");
                deviceConnect(0);
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            refreshTimer.Stop();
            chart1.Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            chart1.StretchMode = !chart1.StretchMode;
            if(chart1.StretchMode)
            {
                toolStripButton1.Text = "Stretch";
            } 
            else
            {
                toolStripButton1.Text = "1:1";
            }
        }

        private void toolStripButton2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.AxisVisible = toolStripButton2.Checked;
        }

        private void toolStripButton4_CheckedChanged(object sender, EventArgs e)
        {
            chart1.BufferAxisVisible = toolStripButton4.Checked;
        }

        private void chart1_ErrorEvent(object sender, ErrorEventArgs e)
        {
            logControl1.AddItem(LogControl.LogTypeEnum.Error, e.GetException().Message);
        }

        private void toolStripButton3_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !toolStripButton3.Checked;
        }

        private void showNotiMessage(NotiTypeEnum type, string message)
        {
            if (type == NotiTypeEnum.Info)
            {
                panel1.ForeColor = Color.Black;
            }
            else if (type == NotiTypeEnum.Warning)
            {
                panel1.ForeColor = Color.Yellow;
            }
            else if (type == NotiTypeEnum.Error)
            {
                panel1.ForeColor = Color.Red;
            }

            label1.Text = message;
            panel1.Visible = true;
            if (timer1.Enabled)
            {
                timer1.Stop();
            }
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            panel1.Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            panel1.Visible = false;
        }
    }
}
