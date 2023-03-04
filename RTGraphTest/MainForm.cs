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
using System.Runtime.Remoting.Messaging;

namespace RTGraph
{
    public partial class MainForm : Form
    {
        UdpClient udpClient;
        UdpClient udpSender = null;
        IPEndPoint targetIPEndPoint;
        //IAsyncResult asyncResult = null;
        LogForm logForm = null;

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

                try
                {
                    var byteData = client.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                    if (logForm != null)
                    {
                        this.Invoke(new Action(() => {
                            if (logForm != null) logForm.AddItem(0, null, byteData);
                        }));
                    }

                    //chart1.ChartAreas[0].AxisX.Minimum = 0;
                    //chart1.ChartAreas[0].AxisX.Maximum = 20;

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
                            this.Invoke(new Action(() => {
                                chart1.AddValueLine(packet.data, 2, packet.data.Length - 2);
                            }));
                        }
                    }
                    else if (packet.Class == PacketClass.CONN)
                    {

                    }

                    if (udpClient.Client != null)
                    {
                        //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                        udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                    }
                }
                catch (Exception ex)
                {
                    //asyncResult = null;
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (udpClient != null)
            {
                //if (asyncResult != null)
                //{
                //    udpClient.EndReceive(asyncResult, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득
                //}
                udpClient.Close();
                udpClient = null;
            }
        }

        private void SocketOpenBtn_Click(object sender, EventArgs e)
        {
            if (SocketOpenBtn.Tag == null)
            {
                udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text))); 
                udpSender = new UdpClient();
                udpSender.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), Int32.Parse(textBox2.Text)));
                //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                SocketOpenBtn.Text = "Socket Close";
                SocketOpenBtn.Tag = udpClient;
                panel1.Enabled = true;
            }
            else
            {
                udpSender.Close();
                udpSender = null;
                udpClient.Close();
                udpClient = null;
                SocketOpenBtn.Text = "Socket Open";
                SocketOpenBtn.Tag = null;
                panel1.Enabled = false;
            }
        }

        private void ConnectBtn_Click(object sender, EventArgs e)
        {
            if (ConnectBtn.Tag == null)
            {
                var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
                var data = packet.serialize();
                udpSender.Send(data, data.Length);

                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                //udpClient = new UdpClient(targetIPEndPoint);
                //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                ConnectBtn.Text = "Disconnect";
                ConnectBtn.Tag = udpClient;
            }
            else
            {
                var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
                var data = packet.serialize();
                udpSender.Send(data, data.Length);

                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                //udpClient = new UdpClient(targetIPEndPoint);
                //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                ConnectBtn.Text = "Cconnect";
                ConnectBtn.Tag = null;
            }
        }

        private void CaptureBtn_Click(object sender, EventArgs e)
        {
            if (CaptureBtn.Tag == null)
            {
                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
                var data = packet.serialize();
                udpSender.Send(data, data.Length);

                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                // udpClient = new UdpClient(targetIPEndPoint);
                //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                CaptureBtn.Text = "Stop Capture";
                CaptureBtn.Tag = "active";
            }
            else
            {
                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
                var data = packet.serialize();
                udpSender.Send(data, data.Length);

                targetIPEndPoint = new IPEndPoint(IPAddress.Any, Int32.Parse(textBox3.Text));
                // udpClient = new UdpClient(targetIPEndPoint);
                //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

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
                logForm.Disposed += LogFormDisposed;
            }
            if (!logForm.Visible) logForm.Show(this);
            if (logForm.WindowState == FormWindowState.Minimized) logForm.WindowState = FormWindowState.Normal;
            logForm.Top = this.Top;
            logForm.Left = this.Right;
        }
    }
}
