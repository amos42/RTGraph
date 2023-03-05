using System;

namespace RTGraph
{
    public enum PacketClass
    {
        CONN = 0x11,
        PARAM = 0x20,
        CAL = 0x30,
        CAPTURE = 0x40
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

    public class CamParam
    {
        // camera setting			
        public byte image_selector;     // real, vertical test image or horizontal test image	
        public byte trigger_source;     // image tirgger or external trigger	
        public byte exposure_time;      //	
        public short line_rate;
        public short gain;  // 감도			

        // trigger_timing			
        public short rch, tre1, tre2, tsl, tde, twd;

        // detect condition			
        public short start, size, level, min_size;

        private static short getShortValu(byte[] packet, int startIdx)
        {
            return (short)(packet[startIdx] | ((int)packet[startIdx + 1] << 8));
        }

        private static void setShortValue(byte[] packet, int startIdx, short value)
        {
            packet[startIdx] = (byte)(value & 0xff);
            packet[startIdx + 1] = (byte)(value >> 8);
        }

        public CamParam(byte[] packet = null, int startIdx = 0)
        {
            if (packet != null)
            {
                Parse(packet, startIdx);
            }
        }

        public void Parse(byte[] packet, int startIdx = 0)
        {
            image_selector = packet[startIdx + 0];
            trigger_source = packet[startIdx + 1];
            exposure_time = packet[startIdx + 2];
            line_rate = getShortValu(packet, 3);
            gain = getShortValu(packet, 5);
            rch = getShortValu(packet, 7);
            tre1 = getShortValu(packet, 9);
            tre2 = getShortValu(packet, 11);
            tsl = getShortValu(packet, 13);
            tde = getShortValu(packet, 15);
            twd = getShortValu(packet, 17);
            start = getShortValu(packet, 19);
            size = getShortValu(packet, 21);
            level = getShortValu(packet, 23);
            min_size = getShortValu(packet, 25);
        }

        public byte[] serialize(byte[] packet = null, int startIdx = 0)
        {
            if (packet == null)
            {
                packet = new byte[27];
            }

            packet[startIdx + 0] = image_selector;
            packet[startIdx + 1] = trigger_source;
            packet[startIdx + 2] = exposure_time;
            setShortValue(packet, startIdx + 3, line_rate);
            setShortValue(packet, startIdx + 5, gain);
            setShortValue(packet, startIdx + 7, rch);
            setShortValue(packet, startIdx + 9, tre1);
            setShortValue(packet, startIdx + 11, tre2);
            setShortValue(packet, startIdx + 13, tsl);
            setShortValue(packet, startIdx + 15, tde);
            setShortValue(packet, startIdx + 17, twd);
            setShortValue(packet, startIdx + 19, start);
            setShortValue(packet, startIdx + 21, size);
            setShortValue(packet, startIdx + 23, level);
            setShortValue(packet, startIdx + 25, min_size);

            return packet;
        }
    }

    public class RTGraphPacket
    {
        public PacketClass Class;
        public PacketSubClass SubClass;
        public PacketClassBit Control;
        public int Option;
        public byte[] data = null;
        //public CamParam camParam = null;

        public RTGraphPacket(PacketClass Class, PacketSubClass SubClass, PacketClassBit Control, int Option, byte[] data = null)
        {
            this.Class = Class;
            this.SubClass = SubClass;
            this.Control = Control;
            this.Option = Option;
            this.data = data;
        }

        public RTGraphPacket(PacketClass Class, PacketSubClass SubClass, PacketClassBit Control, int Option/*, CamParam camParam*/)
        {
            this.Class = Class;
            this.SubClass = SubClass;
            this.Control = Control;
            this.Option = Option;
            //this.camParam = camParam;
        }

        public RTGraphPacket(byte[] packet)
        {
            Class = (PacketClass)packet[0];
            SubClass = (PacketSubClass)packet[1];
            Control = (PacketClassBit)packet[2];
            Option = packet[3];

            int len = packet[4] | ((int)packet[5] << 8);
            if (len > 0)
            {
                //if (Class == PacketClass.PARAM)
                //{
                //    camParam = new CamParam(data, 6);
                //}
                //else
                //{
                    data = new byte[len];
                    for (int i = 0; i < len; i++)
                    {
                        data[i] = packet[6 + i];
                    }
                //}
            } 
        }

        public byte[] serialize(byte[] packet = null)
        {
            int len = 0;
            if(data != null)
            {
                len += data.Length;
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

            if (data != null)
            {
                for (int i = 0; i < len; i++)
                {
                    packet[6 + i] = data[i];
                }
            }
            //if (camParam != null)
            //{
            //    camParam.serialize(packet, 6);
            //}

            return packet;
        }

    }
}
