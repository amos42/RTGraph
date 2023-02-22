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
using RTGraph;

namespace DeviceSimulator
{
    public partial class MainForm : Form
    {
        UdpClient udpServer;
        IPEndPoint targetIPEndPoint;
        IPEndPoint myIPEndPoint;
        UdpClient udpSender = null;
        Dictionary<IPEndPoint, IPEndPoint> endPtrDic;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            endPtrDic = new Dictionary<IPEndPoint, IPEndPoint>();

            targetIPEndPoint = new IPEndPoint(IPAddress.Any, 0);

            udpServer = new UdpClient(5555);
            udpSender = udpServer;

            //var server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //server.Bind(new IPEndPoint(IPAddress.Any, 5555));

            //Console.WriteLine("Server Start");

            //myIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            //var remote = (EndPoint)(sender);

            //byte[] _data = new byte[1024];

            udpServer.BeginReceive(new AsyncCallback(receiveText), udpServer);

            // ReceiveFrom()
            //server.ReceiveFrom(_data, ref remote);
            //Console.WriteLine("{0} : \r\nServar Recieve Data : {1}", remote.ToString(),
            //    Encoding.Default.GetString(_data));

            // string --> byte[]
            //_data = Encoding.Default.GetBytes("Client SendTo Data");

            // SendTo()
            //server.SendTo(_data, _data.Length, SocketFlags.None, remote);

            // Close()
            //server.Close();
        }

        private void receiveText(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                var byteData = (result.AsyncState as UdpClient)?.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득

                var packet = new RTGraphPacket(byteData);

                if (packet.Class == PacketClass.CAPTURE)
                {
                    if (packet.Option == 0x00)
                    {
                        endPtrDic.Add(targetIPEndPoint, targetIPEndPoint);

                        this.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add("send start");
                            if (!timer1.Enabled)
                            {
                                timer1.Start();
                            }
                        }));
                    }
                    else if (packet.Option == 0x01)
                    {
                        endPtrDic.Remove(targetIPEndPoint);

                        this.Invoke(new Action(() =>
                        {
                            listBox1.Items.Add("send stop");

                            if (!endPtrDic.Any())
                            {
                                timer1.Stop();
                            }
                        }));
                    }

                }

                packet.SubClass = PacketSubClass.RES;
                packet.data = new byte[1] { 0x01 }; // of 0xFF
                var data = packet.serialize();
                udpSender.Send(data, data.Length, targetIPEndPoint);

                udpServer.BeginReceive(new AsyncCallback(receiveText), udpServer);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // (2) 데이타 송신
            //cli.Send(datagram, datagram.Length, "127.0.0.1", 7777);
            //Console.WriteLine("[Send] 127.0.0.1:7777 로 {0} 바이트 전송", datagram.Length);

            foreach (var xx in endPtrDic)
            {
                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.NTY, PacketClassBit.FIN, 0x02, new byte[] { 0, 1, 2, 3, 4 });
                var data = packet.serialize();

                udpSender.Send(data, data.Length, xx.Value);
                listBox1.Items.Add("0x1 0x2 0x3 0x4 0x5");
                listBox1.TopIndex = listBox1.Items.Count - 1;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (udpSender != null)
            {
                udpSender.Send(new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5 }, 5);
            }
            listBox1.Items.Add("0x1 0x2 0x3 0x4 0x5");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            udpSender = null;
            listBox1.Items.Add("send stop");
        }
    }
}
