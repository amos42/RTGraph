using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RTGraph;
using System.IO;

namespace RTGraphProtocol
{
    public class UDPCommventArgs : EventArgs
    {
        public byte[] stream { get; set; }
        public IPEndPoint TargetIPEndPoint {get; set;} = null;

        public UDPCommventArgs(byte[] stream, IPEndPoint targetEP = null) 
        {
            this.stream = stream;
            this.TargetIPEndPoint = targetEP;
        }
    }

    public delegate void UDPCommEventHandler(object sender, UDPCommventArgs e);

    public class UDPComm
    {
        public bool Opened { get; set; } = false;

        public string HostIP { get; set; } = null;
        public int SendPort { get; set; } = 0;
        public int RecvPort { get; set; } = 0;

        public event ErrorEventHandler ErrorEvent;
        public event UDPCommEventHandler StreamReceivedEvent;

        protected UdpClient udpClient;
        protected UdpClient udpSender = null;
        protected IPEndPoint targetIPEndPoint;

        protected void RaiseEvent(byte[] stream, IPEndPoint targetIPEndPoint = null)
        {
            if (StreamReceivedEvent != null)
            {
                StreamReceivedEvent(this, new UDPCommventArgs(stream, targetIPEndPoint));
            }
        }

        protected virtual void processPacket(byte[] byteData, IPEndPoint endpt)
        {
        }

        protected virtual void receiveText(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                var client = result.AsyncState as UdpClient;
                if (client?.Client == null) { return; }

                try
                {
                    var targetEP = new IPEndPoint(IPAddress.Any, RecvPort);
                    var byteData = client.EndReceive(result, ref targetEP); // 버퍼에 있는 데이터 취득

                    processPacket(byteData, targetEP);

                    if (udpClient.Client != null)
                    {
                        //asyncResult = udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                        udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);
                    }
                }
                catch (Exception ex)
                {
                    //asyncResult = null;
                    if (ErrorEvent != null)
                    {
                        ErrorEvent(this, new ErrorEventArgs(ex));
                    }
                }
            }
        }

        public void OpenComm()
        {
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, RecvPort));
            udpSender = new UdpClient();
            if(HostIP != null)
            {
                udpSender.Connect(new IPEndPoint(IPAddress.Parse(HostIP), SendPort));
            }
            targetIPEndPoint = new IPEndPoint(IPAddress.Any, RecvPort);

            udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

            Opened = true;
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

            Opened = false;
        }

        public void SendStream(byte[] stream, IPEndPoint targetIPEndPoint = null)
        {
            if (HostIP != null)
            {
                udpSender.Send(stream, stream.Length);
            }
            else
            {
                if (targetIPEndPoint == null) targetIPEndPoint = this.targetIPEndPoint;
                if (SendPort > 0) targetIPEndPoint.Port = SendPort;
                udpSender.Send(stream, stream.Length, targetIPEndPoint);
            }
        }
    }
}

