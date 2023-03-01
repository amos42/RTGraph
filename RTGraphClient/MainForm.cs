using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        private double x = 0;
        UdpClient udpClient;
        IPEndPoint targetIPEndPoint;
        IAsyncResult asyncResult = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void receiveText(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                var client = result.AsyncState as UdpClient;
                if (client?.Client == null) { return; }

                var byteData = client.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                asyncResult = null;

                //chart1.ChartAreas[0].AxisX.Minimum = 0;
                //chart1.ChartAreas[0].AxisX.Maximum = 20;

                var packet = new RTGraphPacket(byteData);
                if (packet.Class == PacketClass.CAPTURE)
                {
                    if (packet.SubClass == PacketSubClass.RES)
                    {
                        if (packet.Option == 0x00)
                        {
                            asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                            // 캡춰 시작
                        }
                        else if (packet.Option == 0x01)
                        {
                            // 캡춰 끝

                        }
                    }
                    else if (packet.SubClass == PacketSubClass.NTY)
                    {
                        // 캡춰 데이터.
                        // packet.Option : 0x02 - continu mode, 0x03 - trigger mode
                        this.Invoke(new Action(() =>
                        {
                            chart1.AddValueLine(packet.data);
                        }));
                        asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                    }
                }
                else if (packet.Class == PacketClass.CONN)
                {
                    asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (udpClient != null)
            {
                if (asyncResult != null)
                {
                    udpClient.EndReceive(asyncResult, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                }
                udpClient.Close();
                udpClient = null;
            }
        }

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            //chart1.Annotations[0].Y = chart1.ChartAreas[0].InnerPlotPosition.Y + 5;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (button4.Tag == null)
            {
                udpClient = new UdpClient();
                udpClient.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), Int32.Parse(textBox2.Text)));

                button4.Text = "Socket Close";
                button4.Tag = udpClient;
            }
            else
            {
                if (asyncResult != null)
                {
                    udpClient.EndReceive(asyncResult, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                }
                udpClient.Close();
                udpClient = null;
                button4.Text = "Socket Open";
                button4.Tag = null;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Tag == null)
            {
                var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
                var data = packet.serialize();
                udpClient.Send(data, data.Length);
                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                button2.Text = "Disconnect";
                button2.Tag = udpClient;
            }
            else
            {
                var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
                var data = packet.serialize();
                udpClient.Send(data, data.Length);
                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                button2.Text = "Cconnect";
                button2.Tag = null;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Tag == null)
            {
                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
                var data = packet.serialize();
                udpClient.Send(data, data.Length);
                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                button1.Text = "Stop Capture";
                button1.Tag = "active";
            }
            else
            {
                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
                var data = packet.serialize();
                udpClient.Send(data, data.Length);
                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                button1.Text = "Start Capture";
                button1.Tag = null;
            }
        }

    }
}
