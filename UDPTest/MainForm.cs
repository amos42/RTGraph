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
using System.Reflection;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        static string[] dirStr = { "발신", "수신" };

        UdpClient udpClient = null;
        IPEndPoint targetIPEndPoint;

        public MainForm()
        {
            InitializeComponent();

            listView1.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(listView1, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void addItem(int direction, byte[] byteData)
        {
            this.Invoke(new Action(() =>
            {
                var item = new ListViewItem(
                    new string[]
                    {
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            dirStr[direction],
                            BitConverter.ToString(byteData).Replace("-", " ")
                    }
                );

                listView1.BeginUpdate();
                try
                {
                    listView1.Items.Add(item);
                }
                finally
                {
                    listView1.EndUpdate();
                }

                
                item.EnsureVisible();
            }));

        }

        private void receiveText(IAsyncResult result)
        {
            var client = result.AsyncState as UdpClient;
            if (client?.Client == null) { return; }

            if (result.IsCompleted)
            {
                var byteData = client.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                addItem(1, byteData);

                udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            var data = packet.serialize();

            udpClient.Send(data, data.Length);

            addItem(0, data);

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            var data = packet.serialize();

            udpClient.Send(data, data.Length);

            addItem(0, data);

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
                udpClient.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555));

                targetIPEndPoint = new IPEndPoint(IPAddress.Any, 8282);
                udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                button4.Text = "Disconnect";
                button4.Tag = udpClient;
            }
            else
            {
                udpClient.Close();
                udpClient = null;
                button4.Text = "Cconnect";
                button4.Tag = null;
            }
        }
    }
}
