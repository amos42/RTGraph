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
using RTGraphProtocol;

namespace DeviceSimulator
{
    public partial class MainForm : Form
    {
        RTGraphComm comm = new RTGraphComm();
        Dictionary<IPEndPoint, bool> endPtrDic;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            bool foward = true;
            RTGraphPacket packet = e.Packet;
            IPEndPoint ep = e.TargetIPEndPoint;
            if (comm.SendPort > 0) ep.Port = comm.SendPort;

            addLogItem(1, "", packet.serialize());

            if (packet.Class == PacketClass.CONN)
            {
                if (packet.SubClass == PacketSubClass.REQ)
                {
                    if (packet.Option == 0x01)
                    {
                        endPtrDic.Add(ep, true);  // 접속하자마자 바로 캡춰 패킷 전송
                        applyTimer();
                    }
                    else if (packet.Option == 0x00)
                    {
                        endPtrDic.Remove(ep);
                        applyTimer();
                    }
                }
            }
            else if (packet.Class == PacketClass.PARAM)
            {
                if (packet.SubClass == PacketSubClass.REQ)
                {
                    if (packet.Option == 0x00)
                    {
                    }
                    else if (packet.Option == 0x01)
                    {
                        comm.camParam.image_selector = 2;
                        comm.camParam.trigger_source = 1;
                        packet = new RTGraphPacket(PacketClass.PARAM, PacketSubClass.RES, PacketClassBit.FIN, 0x1, comm.camParam.serialize());
                        comm.SendPacket(packet, e.TargetIPEndPoint);
                        //comm.SendPacket(PacketClass.PARAM, PacketSubClass.RES, PacketClassBit.FIN, 0x1, comm.camParam.serialize(), e.TargetIPEndPoint);
                        addLogItem(0, null, packet.serialize());
                        foward = false;
                    }
                }
            }
            else if (packet.Class == PacketClass.CAPTURE)
            {
                if (packet.SubClass == PacketSubClass.REQ)
                {
                    if (packet.Option == 0x00)
                    {
                        if (endPtrDic.TryGetValue(ep, out bool value) && !value)
                        {
                            endPtrDic[ep] = true;
                            applyTimer();
                        }
                    }
                    else if (packet.Option == 0x01)
                    {
                        if (endPtrDic.TryGetValue(ep, out bool value) && value)
                        {
                            endPtrDic[ep] = false;
                            applyTimer();
                        }
                    }
                }
            }

            if (foward)
            {
                packet.SubClass = PacketSubClass.RES;
                packet.data = new byte[1] { 0x01 }; // of 0xFF
                comm.SendPacket(packet, e.TargetIPEndPoint);
            }
        }

        private void socketOpen(bool open)
        {
            if (open)
            {
                comm.RecvPort = Int32.Parse(textBox2.Text);
                comm.SendPort = Int32.Parse(textBox3.Text);
                comm.PacketReceived += new PacketReceivedEventHandler(ReceivePacket);
                comm.OpenComm();

                SocketOpenBtn.Text = "Socket Close";
                SocketOpenBtn.Tag = comm;
            }
            else
            {
                comm.CloseComm();
                comm.PacketReceived -= new PacketReceivedEventHandler(ReceivePacket);
                SocketOpenBtn.Text = "Socket Open";
                SocketOpenBtn.Tag = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            endPtrDic = new Dictionary<IPEndPoint, bool>();

            socketOpen(true);
        }

        private void addLogItem(int direction, string message, byte[] byteData = null)
        {
            this.Invoke(new Action(() => {
                logControl1.AddItem(direction, message, byteData);
            }));
        }

        private void applyTimer()
        {
            int cnt = 0;
            if (endPtrDic.Any())
            {
                foreach(var ena in endPtrDic)
                {
                    if (ena.Value) cnt++;
                }
            }

            if (cnt > 0)
            {
                this.Invoke(new Action(() =>
                {
                    if (!timer1.Enabled)
                    {
                        timer1.Start();
                    }
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    if (timer1.Enabled)
                    {
                        timer1.Stop();
                    }
                }));
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (var xx in endPtrDic)
            {
                if (!xx.Value) continue;
                
                var data = new byte[1024];

                int limit = trackBar2.Value;
                float divder = limit * limit / (float)255;

                int v = trackBar1.Value;
                for (int i = 0; i < 1024; i++)
                {
                    int dist = Math.Abs(i - v);
                    if (v != 0 && dist <= limit)
                    {
                        data[i] = (byte)(255 - dist * dist / divder);
                    } 
                    else
                    {
                        data[i] = 0;
                    }
                }

                var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.NTY, PacketClassBit.FIN, 0x02, data);
                comm.SendPacket(packet, xx.Key);
                addLogItem(0, "", packet.serialize());
            }
        }

        private void SocketOpenBtn_Click(object sender, EventArgs e)
        {
            if (!comm.Opened)
            {
                socketOpen(true);

                SocketOpenBtn.Text = "Socket Close";
                SocketOpenBtn.Tag = comm;
            }
            else
            {
                socketOpen(false);

                SocketOpenBtn.Text = "Socket Open";
                SocketOpenBtn.Tag = null;
            }

        }
    }
}
