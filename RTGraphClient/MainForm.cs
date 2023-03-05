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
        RTGraphComm comm = new RTGraphComm();

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comm.ErrorEvent += new ErrorEventHandler(CommError);
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
    }
}
