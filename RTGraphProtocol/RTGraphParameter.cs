using System;
using System.ComponentModel;
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

    public class RTGraphParameter : INotifyPropertyChanged
    {
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
                image_selector = value;
                OnPropertyChanged();
            } 
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("image tirgger or external trigger")]
        [RefreshProperties(RefreshProperties.All)]
        public RTGraphParameterTriggerSource TriggerSource
        {
            get { return trigger_source; }
            set {
                trigger_source = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("100 <= time <= 255")]
        public byte ExposureTime
        {
            get { return exposure_time; }
            set
            {
                if (value >= 100 && value <= 255)
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
                line_rate = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("감도")]
        public short Gain
        {
            get { return gain; }
            set
            {
                gain = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short RCH
        {
            get { return rch; }
            set
            {
                rch = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TRE1
        {
            get { return tre1; }
            set
            {
                tre1 = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TRE2
        {
            get { return tre2; }
            set
            {
                tre2 = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TSL
        {
            get { return tsl; }
            set
            {
                tsl = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TDE
        {
            get { return tde; }
            set
            {
                tde = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public short TWD
        {
            get { return twd; }
            set
            {
                twd = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short Start
        {
            get { return start; }
            set
            {
                start = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short Size
        {
            get { return size; }
            set
            {
                size = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short Level
        {
            get { return level; }
            set
            {
                level = value;
                OnPropertyChanged();
            }
        }

        [CategoryAttribute("Detect Condition")]
        public short MinSize
        {
            get { return min_size; }
            set
            {
                min_size = value;
                OnPropertyChanged();
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

        public RTGraphParameter(byte[] packet = null, int startIdx = 0)
        {
            if (packet != null)
            {
                Parse(packet, startIdx);
            }
        }

        public void Parse(byte[] packet, int startIdx = 0)
        {
            if (packet.Length - startIdx >= 27)
            {
                ImageSelector = (RTGraphParameterImageSelector)packet[startIdx + 0];
                TriggerSource = (RTGraphParameterTriggerSource)packet[startIdx + 1];
                ExposureTime = packet[startIdx + 2];
                LineRate = getShortValu(packet, 3);
                Gain = getShortValu(packet, 5);
                RCH = getShortValu(packet, 7);
                TRE1 = getShortValu(packet, 9);
                TRE2 = getShortValu(packet, 11);
                TSL = getShortValu(packet, 13);
                TDE = getShortValu(packet, 15);
                TWD = getShortValu(packet, 17);
                Start = getShortValu(packet, 19);
                Size = getShortValu(packet, 21);
                Level = getShortValu(packet, 23);
                MinSize = getShortValu(packet, 25);
            }
        }

        public byte[] serialize(byte[] packet = null, int startIdx = 0)
        {
            if (packet == null)
            {
                packet = new byte[27];
            }

            if (packet.Length - startIdx >= 27)
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

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
