using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RTGraph
{
    public class RTGraphParameter : INotifyPropertyChanged
    {
        private byte _image_selector = 0;
        private byte _trigger_source = 0;
        private byte _exposure_time = 0;
        private short _line_rate = 0;

        // camera setting			
        [CategoryAttribute("Camera Setting"), DescriptionAttribute("real, vertical test image or horizontal test image")]
        [RefreshProperties(RefreshProperties.All)]
        public byte image_selector { 
            get { return _image_selector; }
            set {
                _image_selector = value;
                OnPropertyChanged();
            } 
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("image tirgger or external trigger")]
        [RefreshProperties(RefreshProperties.All)]
        public byte trigger_source
        {
            get { return _trigger_source; }
            set {
                _trigger_source = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Camera Setting")]
        public byte exposure_time { get; set; }      //	
        [CategoryAttribute("Camera Setting")]
        public short line_rate { get; set; }
        [CategoryAttribute("Camera Setting"), DescriptionAttribute("감도")]
        public short gain { get; set; }  // 감도			

        // trigger_timing		
        [CategoryAttribute("Trigger Timing")]
        public short rch { get; set; }
        [CategoryAttribute("Trigger Timing")]
        public short tre1 { get; set; }
        [CategoryAttribute("Trigger Timing")]
        public short tre2 { get; set; }
        [CategoryAttribute("Trigger Timing")]
        public short tsl { get; set; }
        [CategoryAttribute("Trigger Timing")]
        public short tde { get; set; }
        [CategoryAttribute("Trigger Timing")]
        public short twd { get; set; }

        // detect condition			
        [CategoryAttribute("Detect Condition")]
        public short start { get; set; }
        [CategoryAttribute("Detect Condition")]
        public short size { get; set; }
        [CategoryAttribute("Detect Condition")]
        public short level { get; set; }
        [CategoryAttribute("Detect Condition")]
        public short min_size { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private static short getShortValu(byte[] packet, int startIdx)
        {
            return (short)(packet[startIdx] | ((int)packet[startIdx + 1] << 8));
        }

        private static void setShortValue(byte[] packet, int startIdx, short value)
        {
            packet[startIdx] = (byte)(value & 0xff);
            packet[startIdx + 1] = (byte)(value >> 8);
        }

        public RTGraphParameter(byte[] packet = null, int startIdx = 0)
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
