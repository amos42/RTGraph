﻿using System;
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
            if (e.PropertyName == "Level") {
                this.Invoke(new Action(() =>
                {
                    chart1.TriggerValue = comm.camParam.Level;
                }));
            }
        }

        private void ConnectProcess(bool connected)
        {
            if (connected)
            {
                toolStripSplitButton1.Enabled = true;
                toolStripDropDownButton2.Enabled = true;
            } 
            else
            {
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
                    chart1.AddValueLine(e.Packet.data, 2, e.Packet.data.Length - 2);
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
            if (!toolStripButton3.Checked)
            {
                comm.Connect();

                toolStripButton3.Text = "Disconnect";
                toolStripButton3.Checked = true;

                timer1.Start();
            }
            else
            {
                comm.Disconnect();

                toolStripButton3.Text = "Connect";
                toolStripButton3.Checked = false;
            }
        }

        private void startCaptureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!startCaptureToolStripMenuItem.Checked)
            {
                comm.StartCapture();

                startCaptureToolStripMenuItem.Text = "Stop Capture";
                startCaptureToolStripMenuItem.Checked = true;
            }
            else
            {
                comm.StopCapture();

                startCaptureToolStripMenuItem.Text = "Start Capture";
                startCaptureToolStripMenuItem.Checked = false;
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
    }
}
