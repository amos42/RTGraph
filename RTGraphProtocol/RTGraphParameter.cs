using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace RTGraph
{
    public enum RTGraphParameterImageSelector
    {
        RealImage,
        VerticalTestImage,
        HorizontalTestImage
    }

    public enum RTGraphParameterTriggerSource
    {
        ImageTrigger,
        ExternalTrigger
    }

    public class RTGraphParameter : INotifyPropertyChanged, ICloneable
    {
        public const int PARAMETERS_PACKET_SIZE = 27;

        // camera setting			
        private RTGraphParameterImageSelector image_selector = RTGraphParameterImageSelector.RealImage;
        private RTGraphParameterTriggerSource trigger_source = RTGraphParameterTriggerSource.ImageTrigger;
        private byte exposure_time = 100;
        private short line_rate = 0;
        private short gain = 0;  // 감도			

        // trigger timing
        private short rch = 0;
        private short tre1 = 0;
        private short tre2 = 0;
        private short tsl = 0;
        private short tde = 0;
        private short twd = 0;

        // detect condition			
        private short start = 0;
        private short size = 0;
        private short level = 0;
        private short min_size = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        // camera setting			
        [CategoryAttribute("Camera Setting"), DescriptionAttribute("real image, vertical test image, horizontal test image")]
        [RefreshProperties(RefreshProperties.All)]
        public RTGraphParameterImageSelector ImageSelector { 
            get { return image_selector; }
            set {
                if (image_selector != value)
                {
                    image_selector = value;
                    OnPropertyChanged();
                }
            } 
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("image tirgger or external trigger")]
        [RefreshProperties(RefreshProperties.All)]
        public RTGraphParameterTriggerSource TriggerSource
        {
            get { return trigger_source; }
            set {
                if (trigger_source != value) {
                    trigger_source = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("100 <= time <= 255")]
        public byte ExposureTime
        {
            get { return exposure_time; }
            set
            {
                if (exposure_time != value && value >= 100 && value <= 255)
                {
                    exposure_time = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Camera Setting")]
        public short LineRate
        {
            get { return line_rate; }
            set
            {
                if (line_rate != value) {
                    line_rate = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("감도")]
        public short Gain
        {
            get { return gain; }
            set
            {
                if (gain != value) {
                    gain = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short RCH
        {
            get { return rch; }
            set
            {
                if (rch != value) {
                    rch = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TRE1
        {
            get { return tre1; }
            set
            {
                if (tre1 != value) {
                    tre1 = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TRE2
        {
            get { return tre2; }
            set
            {
                if (tre2 != value) {
                    tre2 = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TSL
        {
            get { return tsl; }
            set
            {
                if (tsl != value)
                {
                    tsl = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TDE
        {
            get { return tde; }
            set
            {
                if (tde != value)
                {
                    tde = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TWD
        {
            get { return twd; }
            set
            {
                if (twd != value)
                {
                    twd = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short Start
        {
            get { return start; }
            set
            {
                if (start != value && start >= 0 && start < 1024)
                {
                    start = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short Size
        {
            get { return size; }
            set
            {
                if (size != value && size >= 0 && size < 1024)
                {
                    size = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short Level
        {
            get { return level; }
            set
            {
                if (level != value)
                {
                    level = value;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short MinSize
        {
            get { return min_size; }
            set
            {
                if (min_size != value)
                {
                    min_size = value;
                    OnPropertyChanged();
                }
            }
        }

        private static short getShortValu(byte[] packet, int startIdx)
        {
            return (short)(packet[startIdx] | ((int)packet[startIdx + 1] << 8));
        }

        private static void setShortValue(byte[] packet, int startIdx, short value)
        {
            packet[startIdx] = (byte)(value & 0xff);
            packet[startIdx + 1] = (byte)(value >> 8);
        }

        public RTGraphParameter(RTGraphParameter src)
        {
            Assign(src);
        }

        public RTGraphParameter(byte[] packet = null, int startIdx = 0)
        {
            if (packet != null)
            {
                Parse(packet, startIdx);
            }
        }

        public void Assign(RTGraphParameter src)
        {
            ImageSelector = src.image_selector;
            TriggerSource = src.trigger_source;
            ExposureTime = src.exposure_time;
            LineRate = src.line_rate;
            Gain = src.gain;
            RCH = src.rch;
            TRE1 = src.tre1;
            TRE2 = src.tre2;
            TSL = src.tsl;
            TDE = src.tde;
            TWD = src.twd;
            Start = src.start;
            Size = src.size;
            Level = src.level;
            MinSize = src.min_size;
        }

        public void Parse(byte[] packet, int startIdx = 0)
        {
            if (packet.Length - startIdx >= PARAMETERS_PACKET_SIZE)
            {
                ImageSelector = (RTGraphParameterImageSelector)packet[startIdx + 0];
                TriggerSource = (RTGraphParameterTriggerSource)packet[startIdx + 1];
                ExposureTime = packet[startIdx + 2];
                LineRate = getShortValu(packet, startIdx + 3);
                Gain = getShortValu(packet, startIdx + 5);
                RCH = getShortValu(packet, startIdx + 7);
                TRE1 = getShortValu(packet, startIdx + 9);
                TRE2 = getShortValu(packet, startIdx + 11);
                TSL = getShortValu(packet, startIdx + 13);
                TDE = getShortValu(packet, startIdx + 15);
                TWD = getShortValu(packet, startIdx + 17);
                Start = getShortValu(packet, startIdx + 19);
                Size = getShortValu(packet, startIdx + 21);
                Level = getShortValu(packet, startIdx + 23);
                MinSize = getShortValu(packet, startIdx + 25);
            }
        }

        public byte[] serialize(byte[] packet = null, int startIdx = 0)
        {
            if (packet == null)
            {
                packet = new byte[PARAMETERS_PACKET_SIZE];
            }

            if (packet.Length - startIdx >= PARAMETERS_PACKET_SIZE)
            {
                packet[startIdx + 0] = (byte)image_selector;
                packet[startIdx + 1] = (byte)trigger_source;
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
            }

            return packet;
        }

        public object Clone()
        {
            return new RTGraphParameter(this);
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
