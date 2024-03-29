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
using DeviceSimulator;
using RTGraphProtocol;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        RTGraphComm comm = new RTGraphComm();
        LogForm logForm = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comm.ErrorEvent += new ErrorEventHandler(CommError);
        }

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => {
                if (e.Type == PacketReceivedEventArgs.ReceiveTypeEnum.GrabDataReceivced)
                {
                    chart1.SetValueLine(0, e.Packet.Data, 2, e.Packet.Data.Length - 2);
                }

                if (logForm != null) logForm.AddItem(LogControl.LogTypeEnum.Recv, null, e.Packet.Data);
            }));

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.CloseComm();
        }

        private void SocketOpenBtn_Click(object sender, EventArgs e)
        {
            if (SocketOpenBtn.Tag == null)
            {
                comm.HostIP = textBox1.Text;
                comm.SendPort = Int32.Parse(textBox2.Text);
                comm.RecvPort = Int32.Parse(textBox3.Text);
                comm.PacketReceived += new PacketReceivedEventHandler(ReceivePacket);
                comm.OpenComm();

                SocketOpenBtn.Text = "Socket Close";
                SocketOpenBtn.Tag = comm;
                panel1.Enabled = true;
            }
            else
            {
                comm.CloseComm();
                comm.PacketReceived -= ReceivePacket;
                SocketOpenBtn.Text = "Socket Open";
                SocketOpenBtn.Tag = null;
                panel1.Enabled = false;
            }
        }

        private void CommError(object sender, ErrorEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => {
                MessageBox.Show(e.GetException().Message);
            }));
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (ConnectBtn.Tag == null)
            {
                comm.Connect();

                ConnectBtn.Text = "Disconnect";
                ConnectBtn.Tag = comm;
            }
            else
            {
                comm.Disconnect();

                ConnectBtn.Text = "Cconnect";
                ConnectBtn.Tag = null;
            }
        }

        private void CaptureBtn_Click(object sender, EventArgs e)
        {
            if (CaptureBtn.Tag == null)
            {
                comm.StartCapture();

                CaptureBtn.Text = "Stop Capture";
                CaptureBtn.Tag = "active";
            }
            else
            {
                comm.StopCapture();

                CaptureBtn.Text = "Start Capture";
                CaptureBtn.Tag = null;
            }
        }

        private void LogFormDisposed(object sender, EventArgs e)
        {
            logForm = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (logForm == null)
            {
                logForm = new LogForm();
                logForm.Disposed += new EventHandler(LogFormDisposed);
            }
            if (!logForm.Visible) logForm.Show(this);
            if (logForm.WindowState == FormWindowState.Minimized) logForm.WindowState = FormWindowState.Normal;
            logForm.Top = this.Top;
            logForm.Left = this.Right;
        }
    }
}
