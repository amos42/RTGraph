﻿using System;
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

        private void SendPacket(object sender, PacketEventArgs e)
        {
            addLogItem(0, null, e.Packet.Serialize());
        }

        private void ReceivePacket(object sender, PacketReceivedEventArgs e)
        {
            bool foward = true;
            RTGraphPacket packet = e.Packet;
            IPEndPoint ep = e.TargetIPEndPoint;
            if (comm.SendPort > 0) ep.Port = comm.SendPort;

            addLogItem(LogControl.LogTypeEnum.Recv, "", packet.Serialize());

            if (packet.Class == PacketClass.CONN)
            {
                if (packet.SubClass == PacketSubClass.REQ)
                {
                    if (packet.Option == 0x01)
                    {
                        endPtrDic.Add(ep, false);  // 접속하자마자 바로 캡춰 패킷 전송하려면 true로
                        applyTimer();

                        var data = new byte[RTGraphParameter.PARAMETERS_PACKET_SIZE + 1];
                        packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.RES, PacketClassBit.FIN, 0x1, comm.DeviceParameter.Serialize(data, 1));
                        comm.SendPacket(packet, e.TargetIPEndPoint);
                        //comm.SendPacket(PacketClass.PARAM, PacketSubClass.RES, PacketClassBit.FIN, 0x1, comm.camParam.Serialize(), e.TargetIPEndPoint);
                        //addLogItem(0, null, packet.Serialize());
                        foward = false;
                    }
                    else if (packet.Option == 0x00)
                    {
                        endPtrDic.Remove(ep);
                        applyTimer();

                        var data = new byte[1] { 0x0 };
                        packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.RES, PacketClassBit.FIN, 0x0, data);
                        comm.SendPacket(packet, e.TargetIPEndPoint);
                        //addLogItem(0, null, packet.Serialize());
                        foward = false;
                    }
                }
            }
            else if (packet.Class == PacketClass.PARAM)
            {
                if (packet.SubClass == PacketSubClass.REQ)
                {
                    if (packet.Option == 0x01 || packet.Option == 0x00)
                    {
                        var data = comm.DeviceParameter.Serialize(null, 1);
                        data[0] = RTGraphParameter.MASK_GROUP_ALL;
                        packet = new RTGraphPacket(PacketClass.PARAM, PacketSubClass.RES, PacketClassBit.FIN, 0x1, data);
                        comm.SendPacket(packet, e.TargetIPEndPoint);
                        // comm.SendPacket(PacketClass.PARAM, PacketSubClass.RES, PacketClassBit.FIN, 0x1, data, e.TargetIPEndPoint);
                        //addLogItem(0, null, packet.Serialize());
                        foward = false;
                    }
                    else if (packet.Option == 0x02 || packet.Option == 0x03)
                    {
                        comm.DeviceParameter.Parse(packet.Data, 1);
                        if (packet.Option == 0x02)
                        {
                            var cfg = new AppConfig("camParam");
                            cfg.SetAllValues(comm.DeviceParameter);
                            cfg.Save();
                        }
                    }
                }
            }
            else if (packet.Class == PacketClass.CAL)
            {
                if (packet.SubClass == PacketSubClass.REQ)
                {
                    if (packet.Option == 0x00)
                    {
                        var data = new byte[1024 + 1];
                        var rnd = new Random();
                        for (int i = 1; i < 1024+1;i++)
                        {
                            data[i] = (byte)rnd.Next(10, 50);
                        }
                        data[0] = 0;
                        packet = new RTGraphPacket(PacketClass.CAL, PacketSubClass.RES, PacketClassBit.FIN, 0x0, data);
                        comm.SendPacket(packet, e.TargetIPEndPoint);
                        //addLogItem(0, null, packet.Serialize());
                        foward = false;
                    }
                    else if (packet.Option == 0x01)
                    {
                        var data = new byte[1024 + 1];
                        Array.Copy(comm.CalibrationData, 0, data, 1, 1024);
                        data[0] = 0;
                        packet = new RTGraphPacket(PacketClass.CAL, PacketSubClass.RES, PacketClassBit.FIN, 0x1, data);
                        comm.SendPacket(packet, e.TargetIPEndPoint);
                        //addLogItem(0, null, packet.Serialize());
                        foward = false;
                    }
                    else if (packet.Option == 0x02 || packet.Option == 0x03)
                    {
                        Array.Copy(packet.Data, comm.CalibrationData, 1024);
                        if (packet.Option == 0x02)
                        {
                            var cfg = new AppConfig("calibration");
                            cfg.SetArrayValue("Data", comm.CalibrationData);
                            cfg.Save();
                        }
                    }
                }
            }
            else if (packet.Class == PacketClass.GRAB)
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
                    else if (packet.Option == 0x03)
                    {
                        comm.DeviceParameter.TriggerSource = (RTGraphParameter.TriggerSourceEnum)packet.Data?[0];
                        comm.DeviceParameter.TriggerSource = (RTGraphParameter.TriggerSourceEnum)packet.Data?[0];
                    }
                }
            }

            if (foward)
            {
                packet.SubClass = PacketSubClass.RES;
                packet.Data = new byte[1] { 0x00 }; // of 0xFF
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
                comm.PacketSended += new PacketSendedEventHandler(SendPacket);
                comm.OpenComm();

                socketOpenBtn.Text = "Socket Close";
                socketOpenBtn.Tag = comm;
            }
            else
            {
                comm.CloseComm();
                comm.PacketReceived -= ReceivePacket;
                comm.PacketSended -= SendPacket;
                socketOpenBtn.Text = "Socket Open";
                socketOpenBtn.Tag = null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = comm.DeviceParameter;
            comm.DeviceParameter.PropertyChanged += new PropertyChangedEventHandler(propertyGrid1_SelectedGridPropertyChanged);

            endPtrDic = new Dictionary<IPEndPoint, bool>();

            socketOpen(true);

            var cfg = new AppConfig("camParam");
            cfg.GetAllValues(comm.DeviceParameter);

            var cfg2 = new AppConfig("calibration");
            cfg2.GetArrayValue("Data", comm.CalibrationData);
        }

        private void addLogItem(LogControl.LogTypeEnum logType, string message, byte[] byteData = null)
        {
            this.Invoke(new MethodInvoker(() => {
                logControl1.AddItem(logType, message, byteData);
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
                this.Invoke(new MethodInvoker(() => {
                    if (!grabSendTimer.Enabled) grabSendTimer.Start();
                }));
            }
            else
            {
                this.Invoke(new MethodInvoker(() => {
                    if (grabSendTimer.Enabled) grabSendTimer.Stop();
                }));
            }
        }

        private static UInt16 idx = 0;

        private void grabSendTimer_Tick(object sender, EventArgs e)
        {
            foreach (var xx in endPtrDic)
            {
                if (!xx.Value) continue;

                var data = new byte[1024 + 2];
                data[0] = (byte)(idx & 0xff);
                data[1] = (byte)(idx >> 8);
                idx++;

                int limit = trackBar2.Value;
                float divder = limit * limit / (float)255;

                int v = trackBar1.Value;
                for (int i = 0; i < 1024; i++)
                {
                    int dist = Math.Abs(i - v);
                    if (v != 0 && dist <= limit)
                    {
                        data[2 + i] = (byte)(255 - dist * dist / divder);
                    }
                    else
                    {
                        data[2 + i] = 0;
                    }
                }

                var packet = new RTGraphPacket(PacketClass.GRAB, PacketSubClass.NTY, PacketClassBit.FIN, 0x02, data);
                // for(int i = 0; i < 20; i++) {
                    comm.SendPacket(packet, xx.Key);
                // }
                //addLogItem(0, "", packet.Serialize());
            }
        }

        private void SocketOpenBtn_Click(object sender, EventArgs e)
        {
            if (!comm.Opened)
            {
                socketOpen(true);

                socketOpenBtn.Text = "Socket Close";
                socketOpenBtn.Tag = comm;
            }
            else
            {
                socketOpen(false);

                socketOpenBtn.Text = "Socket Open";
                socketOpenBtn.Tag = null;
            }
        }

        private void propertyGrid1_SelectedGridPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() => {
                propertyGrid1.Refresh();
            }));
        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            // propertyGrid1.Refresh();
        }
    }
}
