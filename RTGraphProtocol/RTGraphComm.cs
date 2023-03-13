using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RTGraph;

namespace RTGraphProtocol
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public RTGraphPacket Packet { get; set; }
        public int Type { get; set; } = 0;
        public int Result { get; set; } = 0;
        public IPEndPoint TargetIPEndPoint {get; set;} = null;

        public PacketReceivedEventArgs(RTGraphPacket packet, int type, int result = 0, IPEndPoint targetEP = null) 
        {
            this.Packet = packet;
            this.Type = type;
            this.Result = result;
            this.TargetIPEndPoint = targetEP;
        }
    }

    public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);

    public class RTGraphComm : UDPComm
    {
        public bool Connected { get; set; } = false;
        public RTGraphParameter DeviceParameter { get; set; } = new RTGraphParameter();
        public byte[] CalibrationData { get; set; } = new byte[1024];

        // 파라미터 세팅 시, 장치로부터 응답이 올 때까지 잠시 저장해 놓는 파라미터 값
        private RTGraphParameter pendingParam = null;

        // 패킷을 받았을 때 발생
        public event PacketReceivedEventHandler PacketReceived;
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

        protected override void processPacket(byte[] byteData, IPEndPoint endpt)
        {
            int type = 0;

            var packet = new RTGraphPacket(byteData);
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
                            type = 1;
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
                            type = 2;
                            Connected = false;
                        }
                    }
                }
            }
            else if (packet.Class == PacketClass.PARAM)
            {
                if (packet.SubClass == PacketSubClass.RES)
                {
                    if (packet.Option == 0x00)
                    {
                        // default
                        DeviceParameter.Parse(packet.Data);
                    }
                    else if (packet.Option == 0x01)
                    {
                        // Load
                        DeviceParameter.Parse(packet.Data);
                    }
                    else if (packet.Option == 0x02 || packet.Option == 0x03)
                    {
                        // apply, save success
                        if (packet.Data?[0] == 0)
                        {
                            DeviceParameter.Assign(pendingParam);
                        }
                    }
                }
            }
            else if (packet.Class == PacketClass.CAL)
            {
                if (packet.SubClass == PacketSubClass.RES)
                {
                    if (packet.Option == 0x00)
                    {
                        Array.Copy(packet.Data, CalibrationData, 1024);
                        RaiseCalEvent();
                    }
                    else if (packet.Option == 0x01)
                    {
                        Array.Copy(packet.Data, CalibrationData, 1024);
                        RaiseCalEvent();
                    }
                }
            }
            else if (packet.Class == PacketClass.CAPTURE)
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
                    type = 10;
                }
            }

            RaisePacketReceivedEvent(packet, type, 0, endpt);
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

        public void RaisePacketReceivedEvent(RTGraphPacket packet, int type, int result, IPEndPoint targetIPEndPoint = null)
        {
            if (PacketReceived != null)
            {
                PacketReceived(this, new PacketReceivedEventArgs(packet, type, result, targetIPEndPoint));
            }
        }

        public void Connect()
        {
            var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            var data = packet.serialize();
            udpSender.Send(data, data.Length);
        }

        public void Disconnect()
        {
            var packet = new RTGraphPacket(PacketClass.CONN, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            var data = packet.serialize();
            udpSender.Send(data, data.Length);
        }

        public void StartCapture()
        {
            var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            var data = packet.serialize();
            udpSender.Send(data, data.Length);
        }

        public void StopCapture()
        {
            var packet = new RTGraphPacket(PacketClass.CAPTURE, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
            var data = packet.serialize();
            udpSender.Send(data, data.Length);
        }

        public void SendPacket(RTGraphPacket packet, IPEndPoint targetIPEndPoint = null)
        {
            var data = packet.serialize();
            SendStream(data, targetIPEndPoint);
        }

        public void SendPacket(PacketClass cls, PacketSubClass subCls, PacketClassBit pktBit, int opts, byte[] data = null, IPEndPoint targetIPEndPoint = null)
        {
            var packet = new RTGraphPacket(cls, subCls, pktBit, opts, data);
            SendPacket(packet, targetIPEndPoint);
        }

        public void RequestParam(bool isDefault = false)
        {
            SendPacket(PacketClass.PARAM, PacketSubClass.REQ, PacketClassBit.FIN, isDefault? 0x0 : 0x1);
        }

        public void RequesCalibration(int opts = 0)
        {
            SendPacket(PacketClass.CAL, PacketSubClass.REQ, PacketClassBit.FIN, opts);
        }

        public void ApplyParam(RTGraphParameter camParam, bool isSave = false)
        {
            pendingParam = camParam.Clone() as RTGraphParameter;
            SendPacket(PacketClass.PARAM, PacketSubClass.REQ, PacketClassBit.FIN, isSave ? 0x2 : 0x3, pendingParam.serialize());
        }

        public void ApplyCalibration(bool isSave, bool calEnable)
        {
            var data = new byte[1];
            data[0] = (byte)(calEnable ? 0x1 : 0x0);
            SendPacket(PacketClass.CAL, PacketSubClass.REQ, PacketClassBit.FIN, isSave ? 0x2 : 0x3, data);
        }
    }
}
