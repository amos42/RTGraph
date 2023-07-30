using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RTGraph;
using static RTGraphProtocol.PacketReceivedEventArgs;
using static RTGraph.RTGraphParameter;
using System.IO;
using System.Data;

namespace RTGraphProtocol
{
    public class PacketEventArgs : EventArgs
    {
        public RTGraphPacket Packet { get; set; }
        public IPEndPoint TargetIPEndPoint { get; set; } = null;

        public PacketEventArgs(RTGraphPacket packet, IPEndPoint targetEP = null)
        {
            this.Packet = packet;
            this.TargetIPEndPoint = targetEP;
        }
    }

    public class PacketReceivedEventArgs : PacketEventArgs
    {
        public enum ReceiveTypeEnum
        {
            Dummy,
            Connected,
            PingReceived,
            Disconnected,
            GrabStateChanged,
            GrabModeChanged,
            GrabDataReceivced,
            ParameterReceived,
            CalDataReceived
        }

        public ReceiveTypeEnum Type { get; set; } = 0;
        public int Result { get; set; } = 0;

        public PacketReceivedEventArgs(RTGraphPacket packet, ReceiveTypeEnum type, int result = 0, IPEndPoint targetEP = null) : base(packet, targetEP)
        {
            this.Type = type;
            this.Result = result;
        }
    }

    public delegate void PacketSendedEventHandler(object sender, PacketEventArgs e);
    public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);

    public class RTGraphComm : UDPComm
    {
        public class GrabDataItem
        {
            public int Position;
            public byte[] Data;

            public GrabDataItem(byte[] data, int startIndex, int pos)
            {
                int len = data.Length - startIndex;
                this.Data = new byte[len];
                Array.Copy(data, startIndex, this.Data, 0, len);

                this.Position = pos;
            }
        }

        public enum GrabModeEnum {
            ContinuousMode,
            TriggerMode
        };

        public enum GrabStateEnum
        {
            Stop,
            Start
        };

        public const int TriggerGrabPacketCount = 250;

        public bool Connected { get; set; } = false;
        public RTGraphParameter DeviceParameter { get; set; } = new RTGraphParameter();
        public byte[] CalibrationData { get; set; } = new byte[1024];

        public Queue<GrabDataItem> GrabDataQueue { get; set; } = new Queue<GrabDataItem>();
        public readonly Object thisBlock = new Object();

        public DateTime LatestPacketSendTime { get; set; }
        public DateTime LatestPacketRecvTime { get; set; }

        public GrabModeEnum GrabMode { get; set; }
        public GrabStateEnum GrabState { get; set; }

        private class SendedPacket
        {
            public byte[] stream;
            public DateTime sendTime;
            public int retryCount;

            public SendedPacket(byte[] stream)
            {
                this.stream = stream;
                sendTime = DateTime.Now;
                retryCount = 0;
            }
        }
        private Dictionary<int, SendedPacket> needResponsePackets = new Dictionary<int, SendedPacket>();

        // 파라미터 세팅 시, 장치로부터 응답이 올 때까지 잠시 저장해 놓는 파라미터 값
        private GrabModeEnum pendingGrabMode;
        private RTGraphParameter pendingParam = null;
        private byte[] pendingCalibrationData = null;

        // 패킷을 받았을 때 발생
        public event PacketReceivedEventHandler PacketReceived;
        // 패킷을 전송했을 때 발생
        public event PacketSendedEventHandler PacketSended;
        // Calibration 값이 변경되었을 때 발생
        public event EventHandler CalibrationChanged;
        // 통신 상태가 변경되었을 때 발생
        public event EventHandler StateChanged;

        public RTGraphComm()
        {
        }

        public RTGraphComm(string hostIP, int sendPort, int recvPort) : base(hostIP, sendPort, recvPort)
        {
        }

        byte[] stream = null;

        protected override void processPacket(byte[] byteData, IPEndPoint endpt)
        {
            int idx = 0;
            byte[] data = byteData;
            if(stream != null)
            {
                data = new byte[stream.Length + byteData.Length];
                Array.Copy(stream, 0, data, 0, stream.Length);
                Array.Copy(byteData, 0, data, stream.Length, byteData.Length);
                stream = null;
            }

            while (idx < data.Length)
            {
                ReceiveTypeEnum type = 0;

                var packet = new RTGraphPacket();
                int len = packet.SetPacketFromStream(data, idx);
                if (len <= 0)
                {
                    stream = new byte[data.Length - idx];
                    Array.Copy(data, idx, stream, 0, data.Length - idx);
                    break;
                }

                idx += len;

                if (packet.SubClass == PacketSubClass.RES)
                {
                    needResponsePackets.Remove((int)packet.Class << 8 + packet.Option);
                }

                if (packet.Class == PacketClass.CONN)
                {
                    if (packet.SubClass == PacketSubClass.RES)
                    {
                        if (packet.Option == 0x01)
                        {
                            // 연결
                            bool result;
                            if (packet.Data?.Length > 0)
                            {
                                result = packet.Data[0] == 0;
                            }
                            else
                            {
                                result = true;
                            }

                            if (result)
                            {
                                type = ReceiveTypeEnum.Connected;
                                Connected = true;
                                RaiseStateEvent();

                                if (packet.Data?.Length + 1 >= RTGraphParameter.PARAMETERS_PACKET_SIZE)
                                {
                                    DeviceParameter.Parse(packet.Data, 1);
                                }
                            }
                        }
                        else if (packet.Option == 0x00)
                        {
                            // 연결 종료
                            bool result = packet.Data[0] == 0;
                            if (result)
                            {
                                GrabDataQueue.Clear();
                                type = ReceiveTypeEnum.Disconnected;
                                Connected = false;
                            }
                        }
                    }
                }
                else if (packet.Class == PacketClass.PING)
                {
                    if (packet.SubClass == PacketSubClass.RES)
                    {
                        type = ReceiveTypeEnum.PingReceived;
                    }
                }
                else if (packet.Class == PacketClass.PARAM)
                {
                    if (packet.SubClass == PacketSubClass.RES)
                    {
                        if (packet.Option == 0x00)
                        {
                            // default
                            DeviceParameter.Parse(packet.Data, 1);
                        }
                        else if (packet.Option == 0x01)
                        {
                            // Load
                            DeviceParameter.Parse(packet.Data, 1);
                        }
                        else if (packet.Option == 0x02 || packet.Option == 0x03)
                        {
                            // apply, save success
                            if (packet.Data?[0] == 0)
                            {
                                DeviceParameter.Assign(pendingParam);
                            }
                        }

                        type = ReceiveTypeEnum.ParameterReceived;
                    }
                }
                else if (packet.Class == PacketClass.CAL)
                {
                    if (packet.SubClass == PacketSubClass.RES)
                    {
                        if (packet.Option == 0x01)
                        {
                            if (packet.Data?[0] == 0)
                            {
                                Array.Copy(packet.Data, 1, CalibrationData, 0, 1024);
                                RaiseCalEvent();
                            }
                        }
                        else if (packet.Option == 0x02 || packet.Option == 0x03)
                        {
                            if (packet.Data?[0] == 0)
                            {
                                if (pendingCalibrationData != null)
                                {
                                    Array.Copy(pendingCalibrationData, CalibrationData, 1024);
                                    RaiseCalEvent();
                                }
                            }
                        }

                        type = ReceiveTypeEnum.CalDataReceived;
                    }
                }
                else if (packet.Class == PacketClass.GRAB)
                {
                    if (packet.SubClass == PacketSubClass.RES)
                    {
                        if (packet.Option == 0x00)
                        {
                            // 캡춰 시작
                            if (packet.Data?[0] == 0)
                            {
                                GrabState = GrabStateEnum.Start;
                                type = ReceiveTypeEnum.GrabStateChanged;
                            }
                        }
                        else if (packet.Option == 0x01)
                        {
                            // 캡춰 끝
                            if (packet.Data?[0] == 0)
                            {
                                GrabState = GrabStateEnum.Stop;
                                GrabDataQueue.Clear();
                                type = ReceiveTypeEnum.GrabStateChanged;
                            }
                        }
                        else if (packet.Option == 0x02)
                        {
                            // 현재 모드 정보
                            if (packet.Data?[0] == 0)
                            {
                                var oldGrabMode = GrabMode;
                                var oldGrabState = GrabState;
                                GrabMode = packet.Data[2] == 0 ? GrabModeEnum.ContinuousMode : GrabModeEnum.TriggerMode;
                                GrabState = packet.Data[1] == 0 ? GrabStateEnum.Stop : GrabStateEnum.Start;

                                if (GrabState != oldGrabState)
                                {
                                    RaisePacketReceivedEvent(packet, ReceiveTypeEnum.GrabStateChanged, 0, endpt);
                                }

                                if (GrabMode != oldGrabMode)
                                {
                                    type = ReceiveTypeEnum.GrabModeChanged;
                                }
                            }
                        }
                        else if (packet.Option == 0x03)
                        {
                            // 모드 변경
                            if (packet.Data?[0] == 0)
                            {
                                GrabDataQueue.Clear();
                                GrabMode = pendingGrabMode;
                                type = ReceiveTypeEnum.GrabModeChanged;
                            }
                        }
                    }
                    else if (packet.SubClass == PacketSubClass.NTY)
                    {
                        type = ReceiveTypeEnum.GrabDataReceivced;

                        if (packet.Data != null && packet.Data.Length > 2)
                        {
                            if (GrabState == GrabStateEnum.Start)
                            {
                                if (packet.Option == 0x2 && GrabMode == GrabModeEnum.ContinuousMode)
                                {
                                    int pos = (short)(packet.Data[0] | ((int)packet.Data[1] << 8));
                                    GrabDataQueue.Enqueue(new GrabDataItem(packet.Data, 2, pos));
                                }
                                else if (packet.Option == 0x3 && GrabMode == GrabModeEnum.TriggerMode)
                                {
                                    int pos = (short)(packet.Data[0] | ((int)packet.Data[1] << 8));
                                    lock (thisBlock)
                                    {
                                        GrabDataQueue.Enqueue(new GrabDataItem(packet.Data, 2, pos));
                                    }
                                }
                            }
                        }
                    }
                }

                // if (type != 0)
                {
                    LatestPacketRecvTime = DateTime.Now;
                    RaisePacketReceivedEvent(packet, type, 0, endpt);
                }
            }
        }

        private void RaiseStateEvent()
        {
            if (StateChanged != null)
            {
                StateChanged(this, new EventArgs());
            }
        }

        private void RaiseCalEvent()
        {
            if (CalibrationChanged != null)
            {
                CalibrationChanged(this, new EventArgs());
            }
        }

        private void RaisePacketReceivedEvent(RTGraphPacket packet, ReceiveTypeEnum type, int result, IPEndPoint targetIPEndPoint = null)
        {
            if (PacketReceived != null)
            {
                PacketReceived(this, new PacketReceivedEventArgs(packet, type, result, targetIPEndPoint));
            }
        }

        private void RaisePacketSendEvent(RTGraphPacket packet, IPEndPoint targetIPEndPoint = null)
        {
            if (PacketSended != null)
            {
                PacketSended(this, new PacketEventArgs(packet, targetIPEndPoint));
            }
        }

        public byte[] SendPacket(RTGraphPacket packet, IPEndPoint targetIPEndPoint = null)
        {
            if (!Opened) return null;

            var data = packet.Serialize();
            SendStream(data, targetIPEndPoint);

            LatestPacketSendTime = DateTime.Now;

            RaisePacketSendEvent(packet, targetIPEndPoint);

            return data;
        }

        public byte[] SendPacket(PacketClass cls, PacketSubClass subCls, PacketClassBit pktBit, int opts, byte[] data = null, IPEndPoint targetIPEndPoint = null)
        {
            var packet = new RTGraphPacket(cls, subCls, pktBit, opts, data);
            return SendPacket(packet, targetIPEndPoint);
        }

        public void Connect()
        {
            var stream = SendPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            //var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            //var data = packet.Serialize();
            //udpSender.Send(data, data.Length);
            needResponsePackets.Add((int)PacketClass.CONN << 8 + 0x01, new SendedPacket(stream));
        }

        public void Disconnect()
        {
            var stream = SendPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            //var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            //var data = packet.Serialize();
            //udpSender.Send(data, data.Length);
            needResponsePackets.Add((int)PacketClass.CONN << 8 + 0x00, new SendedPacket(stream));
        }

        public void SendPing()
        {
            SendPacket(PacketClass.PING, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
        }

        public void RequestParam(bool isDefault = false)
        {
            int opt = isDefault ? 0x0 : 0x1;
            var stream = SendPacket(PacketClass.PARAM, PacketSubClass.REQ, PacketClassBit.FIN, opt);
            needResponsePackets.Add((int)PacketClass.PARAM << 8 + opt, new SendedPacket(stream));
        }

        public void ApplyParam(RTGraphParameter camParam, bool isSave = false, byte groupMask = RTGraphParameter.MASK_GROUP_ALL)
        {
            pendingParam = camParam.Clone() as RTGraphParameter;
            var data = pendingParam.Serialize(null, 1);
            data[0] = groupMask;
            var stream = SendPacket(PacketClass.PARAM, PacketSubClass.REQ, PacketClassBit.FIN, isSave ? 0x2 : 0x3, data);
            needResponsePackets.Add((int)PacketClass.PARAM << 8 + 0x3, new SendedPacket(stream));
        }

        public void RequestGrabInfo()
        {
            var stream = SendPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x02);
            //var packet = new RTGraphPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x02);
            //var data = packet.Serialize();
            //udpSender.Send(data, data.Length);
            needResponsePackets.Add((int)PacketClass.GRAB << 8 + 0x02, new SendedPacket(stream));
        }

        public void ChangeGrabMode(byte mode)
        {
            pendingGrabMode = (GrabModeEnum)mode;
            var data = new byte[1] { mode };
            var stream = SendPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x3, data);
            needResponsePackets.Add((int)PacketClass.GRAB << 8 + 0x3, new SendedPacket(stream));
        }

        public void StartCapture()
        {
            var stream = SendPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            //var packet = new RTGraphPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            //var data = packet.Serialize();
            //udpSender.Send(data, data.Length);
            needResponsePackets.Add((int)PacketClass.GRAB << 8 + 0x00, new SendedPacket(stream));
        }

        public void StopCapture()
        {
            var stream = SendPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            //var packet = new RTGraphPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            //var data = packet.Serialize();
            //udpSender.Send(data, data.Length);
            needResponsePackets.Add((int)PacketClass.GRAB << 8 + 0x01, new SendedPacket(stream));
        }

        public void RequesCalibration()
        {
            var stream = SendPacket(PacketClass.CAL, PacketSubClass.REQ, PacketClassBit.FIN, 0x1);
            needResponsePackets.Add((int)PacketClass.CAL << 8 + 0x1, new SendedPacket(stream));
        }

        public void ApplyCalibration(byte[] calData, bool isSave, bool calEnable)
        {
            //var data = new byte[1];
            //data[0] = (byte)(calEnable ? 0x1 : 0x0);
            pendingCalibrationData = calData.Clone() as byte[];
            int opt = isSave ? 0x2 : 0x3;
            var stream = SendPacket(PacketClass.CAL, PacketSubClass.REQ, PacketClassBit.FIN, opt, calData);
            needResponsePackets.Add((int)PacketClass.CAL << 8 + opt, new SendedPacket(stream));
        }

        public void TickProcess()
        {
            if (!Opened) return;

            List<int> removeList = null;

            foreach (var packet in needResponsePackets)
            {
                if (DateTime.Now - packet.Value.sendTime > TimeSpan.FromMilliseconds(500))
                {
                    if (packet.Value.retryCount < 3)
                    {
                        SendStream(packet.Value.stream);
                        packet.Value.retryCount++;
                        packet.Value.sendTime = DateTime.Now;
                    }
                    else
                    {
                        if (removeList == null) 
                        {
                            removeList = new List<int>();
                        }
                        removeList.Add(packet.Key);
                        raiseErrorEvent(new Exception("No Response Packet"));
                    }
                }
            }

            if (removeList != null) 
            {
                foreach (var key in removeList)
                {
                    needResponsePackets.Remove(key);
                }
            }
        }
    }
}
