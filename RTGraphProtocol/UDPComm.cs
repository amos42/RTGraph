using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using RTGraph;
using System.IO;

namespace RTGraphProtocol
{
    public class StreamReceivedEventArgs : EventArgs
    {
        public byte[] Stream { get; set; }
        public IPEndPoint TargetIPEndPoint {get; set;} = null;

        public StreamReceivedEventArgs(byte[] stream, IPEndPoint targetEP = null) 
        {
            this.Stream = stream;
            this.TargetIPEndPoint = targetEP;
        }
    }

    public delegate void StreamReceivedEventHandler(object sender, StreamReceivedEventArgs e);

    public class UDPComm
    {
        public bool Opened { get; set; } = false;

        public string HostIP { get; set; } = null;
        public int SendPort { get; set; } = 0;
        public int RecvPort { get; set; } = 0;

        public event ErrorEventHandler ErrorEvent;
        public event StreamReceivedEventHandler StreamReceivedEvent;

        protected UdpClient udpClient;
        protected UdpClient udpSender = null;
        protected IPEndPoint targetIPEndPoint;

        public UDPComm()
        {
        }

        public UDPComm(string hostIP, int sendPort, int recvPort)
        {
            this.HostIP = hostIP;
            this.SendPort = sendPort;
            this.RecvPort = recvPort;
        }

        protected void raiseEvent(byte[] stream, IPEndPoint targetIPEndPoint = null)
        {
            if (StreamReceivedEvent != null)
            {
                StreamReceivedEvent(this, new StreamReceivedEventArgs(stream, targetIPEndPoint));
            }
        }

        protected void raiseErrorEvent(Exception ex)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(this, new ErrorEventArgs(ex));
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
                    raiseErrorEvent(ex);
                }
            }
        }

        public void OpenComm()
        {
            try
            {
                udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, RecvPort));
                udpSender = new UdpClient();
                if (HostIP != null)
                {
                    udpSender.Connect(new IPEndPoint(IPAddress.Parse(HostIP), SendPort));
                }
                targetIPEndPoint = new IPEndPoint(IPAddress.Any, RecvPort);

                udpClient.BeginReceive(new AsyncCallback(receiveText), udpClient);

                Opened = true;
            }
            catch (Exception ex)
            {
                CloseComm();
                //asyncResult = null;
                if (ErrorEvent != null)
                {
                    ErrorEvent(this, new ErrorEventArgs(ex));
                }
            }
        }

        public void CloseComm()
        {
            if (udpClient != null)
            {
                udpClient.Close();
                udpClient = null;
            }
            if (udpSender != null)
            {
                udpSender.Close();
                udpSender = null;
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

