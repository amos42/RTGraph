using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

            var hostIP = ConfigurationManager.AppSettings["HostIP"];
            var sendPort = ConfigurationManager.AppSettings["SendPort"];
            var recvPort = ConfigurationManager.AppSettings["RecvPort"];

            if (!String.IsNullOrEmpty(hostIP)) comm.HostIP = hostIP;
            if (!String.IsNullOrEmpty(sendPort)) comm.SendPort = Int32.Parse(sendPort);
            if (!String.IsNullOrEmpty(recvPort)) comm.RecvPort = Int32.Parse(recvPort);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            comm.CloseComm();
        }

        private void CommError(object sender, ErrorEventArgs e)
        {
            this.Invoke(new Action(() => {
                MessageBox.Show(e.GetException().Message);

            }));
        }

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            this.Invoke(new Action(() => {
                if (e.Type == 10)
                {
                    chart1.AddValueLine(e.Packet.data, 2, e.Packet.data.Length - 2);
                }
            }));
        }

        private void SocketOpenBtn_Click(object sender, EventArgs e)
        {
            if (SocketOpenBtn.Tag == null)
            {
                comm.PacketReceived += new PacketReceivedEventHandler(ReceivePacket);
                comm.OpenComm();

                SocketOpenBtn.Text = "Socket Close";
                SocketOpenBtn.Tag = comm;
                panel1.Enabled = true;
            }
            else
            {
                comm.CloseComm();
                comm.PacketReceived -= new PacketReceivedEventHandler(ReceivePacket);
                SocketOpenBtn.Text = "Socket Open";
                SocketOpenBtn.Tag = null;
                panel1.Enabled = false;
            }
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

        private void button1_Click(object sender, EventArgs e)
        {
            new ParamForm(comm).ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new CalibForm(comm).ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (new NetworkSettingForm(comm).ShowDialog() == DialogResult.OK)
            {
                if (comm.Opened) comm.CloseComm();
                comm.OpenComm();
            }
        }
    }
}
