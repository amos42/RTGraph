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
        public event PacketReceivedEventHandler PacketReceived;
        public event EventHandler ParameterChanged;

        protected override void processPacket(byte[] byteData, IPEndPoint endpt)
        {
            int type = 0;

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
                    //this.Invoke(new Action(() => {
                    //    chart1.AddValueLine(packet.data, 2, packet.data.Length - 2);
                    //}));
                    type = 10;
                }
            }
            else if (packet.Class == PacketClass.CONN)
            {

            }

            RaisePacketReceivedEvent(packet, type, 0, endpt);

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

        public void SendPacket(PacketClass cls, PacketSubClass subCls, PacketClassBit pktBit, int opts, byte[] data, IPEndPoint targetIPEndPoint = null)
        {
            var packet = new RTGraphPacket(cls, subCls, pktBit, opts, data);
            SendPacket(packet, targetIPEndPoint);
        }
    }
}
