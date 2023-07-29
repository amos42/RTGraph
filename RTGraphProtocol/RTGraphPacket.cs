using System;
using System.ComponentModel;

namespace RTGraph
{
    public enum PacketClass
    {
        CONN = 0x11,
        PING = 0x15,
        PARAM = 0x20,
        CAL = 0x30,
        GRAB = 0x40
    }

    public enum PacketSubClass
    {
        REQ = 0x01,
        RES = 0x02,
        EVT = 0x03,
        NTY = 0x04,
        IND = 0x05,
        CNF = 0x06,
        BSY = 0x07,
        ERR = 0x7F
    }

    public enum PacketClassBit
    {
        FIN = 0x01,
        FRG = 0x02,
        RST = 0x04
    }

    public class RTGraphPacket
    {
        public PacketClass Class;
        public PacketSubClass SubClass;
        public PacketClassBit Control;
        public int Option;
        public byte[] Data = null;
        //public CamParam camParam = null;

        public RTGraphPacket() { }

        public RTGraphPacket(PacketClass Class, PacketSubClass SubClass, PacketClassBit Control, int Option, byte[] data = null)
        {
            this.Class = Class;
            this.SubClass = SubClass;
            this.Control = Control;
            this.Option = Option;
            this.Data = data;
        }

        public RTGraphPacket(PacketClass Class, PacketSubClass SubClass, PacketClassBit Control, int Option/*, CamParam camParam*/)
        {
            this.Class = Class;
            this.SubClass = SubClass;
            this.Control = Control;
            this.Option = Option;
            //this.camParam = camParam;
        }

        public RTGraphPacket(byte[] stream)
        {
            SetPacketFromStream(stream);
        }

        public int SetPacketFromStream(byte[] stream, int idx = 0)
        {
            Class = (PacketClass)stream[idx++];
            SubClass = (PacketSubClass)stream[idx++];
            Control = (PacketClassBit)stream[idx++];
            Option = stream[idx++];

            int len = stream[idx++] | ((int)stream[idx++] << 8);
            if (len > 0)
            {
                //if (Class == PacketClass.PARAM)
                //{
                //    camParam = new CamParam(data, 6);
                //}
                //else
                //{
                Data = new byte[len];
                for (int i = 0; i < len; i++)
                {
                    Data[i] = stream[6 + i];
                }
                //}
            }

            return idx + len;
        }

        public byte[] Serialize(byte[] packet = null)
        {
            int len = 0;
            if(Data != null)
            {
                len += Data.Length;
            }
            //if(camParam != null)
            //{
            //    len += 26;
            //}

            if (packet == null)
            {
                packet = new byte[6 + len];
            }

            packet[0] = (byte)Class;
            packet[1] = (byte)SubClass;
            packet[2] = (byte)Control;
            packet[3] = (byte)Option;
            packet[4] = (byte)(len & 0xff);
            packet[5] = (byte)(len >> 8);

            if (Data != null)
            {
                for (int i = 0; i < len; i++)
                {
                    packet[6 + i] = Data[i];
                }
            }
            //if (camParam != null)
            //{
            //    camParam.Serialize(packet, 6);
            //}

            return packet;
        }

    }
}
