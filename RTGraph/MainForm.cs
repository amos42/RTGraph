using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Net;
using System.Net.Sockets;
using MetroFramework.Forms;

namespace RTGraph
{
    public partial class MainForm : MetroForm
    {
        private double x = 0;
        UdpClient udpListener;
        //IPEndPoint targetIPEndPoint;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Series[0].ChartType = SeriesChartType.Line;

            // (1) UdpClient 객체 성성
            udpListener = new UdpClient();
            udpListener.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5555));

            //string msg = "안녕하세요";
            //byte[] datagram = Encoding.UTF8.GetBytes(msg);

            // (2) 데이타 송신
            //cli.Send(datagram, datagram.Length, "127.0.0.1", 7777);
            //Console.WriteLine("[Send] 127.0.0.1:7777 로 {0} 바이트 전송", datagram.Length);

            // (3) 데이타 수신
            //IPEndPoint epRemote = new IPEndPoint(IPAddress.Any, 0);
            //byte[] bytes = cli.Receive(ref epRemote);
            //Console.WriteLine("[Receive] {0} 로부터 {1} 바이트 수신", epRemote.ToString(), bytes.Length);

            // (4) UdpClient 객체 닫기
            //cli.Close();

            udpListener.BeginReceive(new AsyncCallback(receiveText), udpListener);

        }

        private void receiveText(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                var client = result.AsyncState as UdpClient;
                if (client?.Client != null)
                {
                    var targetIPEndPoint = new IPEndPoint(IPAddress.Any, 8282);
                    var byteData = client.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득

                    var packet = new RTGraphPacket(byteData);
                    if (packet.Class == PacketClass.CAPTURE)
                    {
                        if (packet.SubClass == PacketSubClass.RES)
                        {
                            if (packet.Option == 0x00)
                            {
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
                                chart1.BeginInit();
                                x = new Random().NextDouble();
                                chart1.Series[0].Points.Clear();
                                for (int i = 0; i < 100; i++)
                                {
                                    chart1.Series[0].Points.AddXY(x, 3 * Math.Sin(5 * x) + 5 * Math.Cos(3 * x));

                                    if (chart1.Series[0].Points.Count > 100)
                                    {
                                        chart1.Series[0].Points.RemoveAt(0);
                                    }
                                    x += 0.1;

                                    chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
                                    chart1.ChartAreas[0].AxisX.Maximum = x;
                                }
                                chart1.EndInit();
                            }));
                        }
                    }
                }

                udpListener.BeginReceive(new AsyncCallback(receiveText), udpListener);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //chart1.Series[0].Points.AddXY(x, 3 * Math.Sin(5 * x) + 5 * Math.Cos(3 * x));

            //if(chart1.Series[0].Points.Count > 100)
            //{
            //    chart1.Series[0].Points.RemoveAt(0);
            //}

            //chart1.ChartAreas[0].AxisX.Minimum = chart1.Series[0].Points[0].XValue;
            //chart1.ChartAreas[0].AxisX.Maximum= x;

            //x += 0.1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            var data = packet.serialize();

            udpListener.Send(data, data.Length);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            udpListener.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            new ConfigForm().ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            var data = packet.serialize();

            udpListener.Send(data, data.Length);
        }

        private void chart1_DoubleClick(object sender, EventArgs e)
        {
            chart1.Annotations[0].Y = chart1.ChartAreas[0].InnerPlotPosition.Y + 5;
        }
    }
}
