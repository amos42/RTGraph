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
using System.Reflection;

namespace DeviceSimulator
{
    public partial class MainForm : Form
    {
        static string[] dirStr = { "발신", "수신" };

        UdpClient udpServer;
        IPEndPoint targetIPEndPoint;
        IPEndPoint myIPEndPoint;
        UdpClient udpSender = null;
        Dictionary<IPEndPoint, IPEndPoint> endPtrDic;

        public MainForm()
        {
            InitializeComponent();

            listView1.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(listView1, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            endPtrDic = new Dictionary<IPEndPoint, IPEndPoint>();
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

                listView1.Items.Add(item);
                item.EnsureVisible();
            }));

        }

        private void receiveText(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                var byteData = (result.AsyncState as UdpClient)?.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                addItem(1, byteData);

                var packet = new RTGraphPacket(byteData);

                if (packet.Class == PacketClass.CAPTURE)
                {
                    if (packet.Option == 0x00)
                    {
                        endPtrDic.Add(targetIPEndPoint, targetIPEndPoint);

                        this.Invoke(new Action(() =>
                        {
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
                var data = new byte[1024];

                int v = trackBar1.Value;
                for(int i = 0; i < 1024; i++)
                {
                    if (v != 0 && Math.Abs(i - v) < 16)
                    {
                        data[i] = (byte)(255 - (i - v) * (i - v));
                    } 
                    else
                    {
                        data[i] = 0;
                    }
                }

                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.NTY, PacketClassBit.FIN, 0x02, data);
                var packetStream = packet.serialize();

                udpSender.Send(packetStream, packetStream.Length, xx.Value);
                addItem(0, packetStream);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            udpServer = new UdpClient(Int32.Parse(textBox2.Text));
            udpSender = udpServer;

            targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));

            udpServer.BeginReceive(new AsyncCallback(receiveText), udpServer);
        }
    }
}
