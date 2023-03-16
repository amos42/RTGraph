using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RTGraph;
using static RTGraphProtocol.PacketReceivedEventArgs;
using static RTGraph.RTGraphParameter;

namespace RTGraphProtocol
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public enum ReceiveTypeEnum
        {
            Dummy,
            Connected,
            Disconnected,
            GrapStarted,
            GrapStopped,
            GrabDataReceivced,
            ParameterReceived
        }


        public RTGraphPacket Packet { get; set; }
        public ReceiveTypeEnum Type { get; set; } = 0;
        public int Result { get; set; } = 0;
        public IPEndPoint TargetIPEndPoint {get; set;} = null;

        public PacketReceivedEventArgs(RTGraphPacket packet, ReceiveTypeEnum type, int result = 0, IPEndPoint targetEP = null) 
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
        private TriggerSourceEnum pendingParamTriggerSource;
        private RTGraphParameter pendingParam = null;
        private byte[] pendingCalibrationData = null;

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
            ReceiveTypeEnum type = 0;

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
                            type = ReceiveTypeEnum.Disconnected;
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
                            type = ReceiveTypeEnum.GrapStarted;
                        }
                    }
                    else if (packet.Option == 0x01)
                    {
                        // 캡춰 끝
                        if (packet.Data?[0] == 0)
                        {
                            type = ReceiveTypeEnum.GrapStopped;
                        }
                    }
                    else if (packet.Option == 0x03)
                    {
                        // 모드 변경
                        if (packet.Data?[0] == 0)
                        {
                            DeviceParameter.TriggerSource = pendingParamTriggerSource;
                        }

                    }
                }
                else if (packet.SubClass == PacketSubClass.NTY)
                {
                    type = ReceiveTypeEnum.GrabDataReceivced;
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

        private void RaisePacketReceivedEvent(RTGraphPacket packet, ReceiveTypeEnum type, int result, IPEndPoint targetIPEndPoint = null)
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
            var packet = new RTGraphPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x00);
            var data = packet.serialize();
            udpSender.Send(data, data.Length);
        }

        public void StopCapture()
        {
            var packet = new RTGraphPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x01);
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

        public void RequesCalibration()
        {
            SendPacket(PacketClass.CAL, PacketSubClass.REQ, PacketClassBit.FIN, 1);
        }

        public void ApplyParam(RTGraphParameter camParam, bool isSave = false, byte groupMask = RTGraphParameter.MASK_GROUP_ALL)
        {
            pendingParam = camParam.Clone() as RTGraphParameter;
            var data = pendingParam.serialize(null, 1);
            data[0] = groupMask;
            SendPacket(PacketClass.PARAM, PacketSubClass.REQ, PacketClassBit.FIN, isSave ? 0x2 : 0x3, data);
        }

        public void RequestGrapInfo()
        {
            SendPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x2);
        }

        public void ChangeGrapMode(byte mode)
        {
            pendingParamTriggerSource = (RTGraphParameter.TriggerSourceEnum)mode;
            var data = new byte[1] { mode };
            SendPacket(PacketClass.GRAB, PacketSubClass.REQ, PacketClassBit.FIN, 0x3, data);
        }

        public void ApplyCalibration(byte[] calData, bool isSave, bool calEnable)
        {
            //var data = new byte[1];
            //data[0] = (byte)(calEnable ? 0x1 : 0x0);
            pendingCalibrationData = calData.Clone() as byte[];
            SendPacket(PacketClass.CAL, PacketSubClass.REQ, PacketClassBit.FIN, isSave ? 0x2 : 0x3, calData);
        }
    }
}
