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
    
        public PacketReceivedEventArgs(RTGraphPacket packet, int type, int result = 0) 
        {
            this.Packet = packet;
            this.Type = type;
            this.Result = result;
        }
    }

    public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);

    public class RTGraphComm
    {
        public string HostIP { get; set; } = "127.0.0.1";
        public int SendPort { get; set; } = 0;
        public int RecvPort { get; set; } = 0;

        public event PacketReceivedEventHandler PacketReceived;
        public event EventHandler ParameterChanged;

        private UdpClient udpClient;
        private UdpClient udpSender = null;
        private IPEndPoint targetIPEndPoint;


        private void receiveText(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                var client = result.AsyncState as UdpClient;
                if (client?.Client == null) { return; }

                try
                {
                    var byteData = client.EndReceive(result, ref targetIPEndPoint); // 버퍼에 있는 데이터 취득

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
                            //this.Invoke(new Action(() => {
                            //    chart1.AddValueLine(packet.data, 2, packet.data.Length - 2);
                            //}));
                            if (PacketReceived != null) PacketReceived(this, new PacketReceivedEventArgs(packet, 10, 0));
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

        public void OpenComm()
        {
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, RecvPort));
            udpSender = new UdpClient();
            udpSender.Connect(new IPEndPoint(IPAddress.Parse(HostIP), SendPort));
            targetIPEndPoint = new IPEndPoint(IPAddress.Any, RecvPort);

            udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
        }

        public void CloseComm()
        {
            if (udpClient != null)
            {
                udpSender.Close();
                udpSender = null;
                udpClient.Close();
                udpClient = null;
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

    }
}
