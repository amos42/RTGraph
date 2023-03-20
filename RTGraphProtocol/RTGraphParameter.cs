using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace RTGraph
{
    public class RTGraphParameter : INotifyPropertyChanged, ICloneable
    {
        public enum ImageSelectorEnum
        {
            RealImage,
            HorizontalPattern,
            VerticalPattern,
            ImageStop
        }

        public enum TriggerSourceEnum
        {
            ImageTrigger,
            ExternalTrigger
        }

        public enum ExposureLevelEnum
        {
            Level_0,
            Level_10,
            Level_20,
            Level_30,
            Level_40,
            Level_50,
            Level_60,
            Level_70,
            Level_80,
            Level_90,
            Level_100
        }

        public enum GainLevelEnum
        {
            Level_0,
            Level_1,
            Level_2,
            Level_3,
            Level_4,
            Level_5,
            Level_6,
            Level_7
        }

        public const int MASK_GROUP_1 = 0x01;
        public const int MASK_GROUP_2 = 0x02;
        public const int MASK_GROUP_3 = 0x04;
        public const int MASK_GROUP_4 = 0x08;
        public const int MASK_GROUP_ALL = MASK_GROUP_1 | MASK_GROUP_2 | MASK_GROUP_3 | MASK_GROUP_4;

        public int group_1_refCnt = 0;
        public int group_2_refCnt = 0;
        public int group_3_refCnt = 0;
        public int group_4_refCnt = 0;

        public const int PARAMETERS_PACKET_SIZE = 26;

        // camera setting			
        private ImageSelectorEnum image_selector = ImageSelectorEnum.RealImage;
        private TriggerSourceEnum trigger_source = TriggerSourceEnum.ImageTrigger;
        private UInt16 line_scan_rate = 30000;
        private ExposureLevelEnum exposure_level = 0;
        private GainLevelEnum gain_level = 0;  // 감도			

        // trigger timing
        private UInt16 tde = 0;
        private UInt16 tch = 0;
        private UInt16 tre1 = 0;
        private UInt16 tre2 = 0;
        private UInt16 tsl = 0;
        private UInt16 tpw = 0;

        // detect condition			
        private UInt16 roi_start = 0; // 0 ~ 1023
        private UInt16 roi_end = 1023; // 0 ~ 1023
        private byte threshold_level = 0; // 0 ~ 255
        private UInt16 threshold_width = 1; // 1 ~ 1024

        // calibration
        private bool calibration_enable;

        public event PropertyChangedEventHandler PropertyChanged;

        // camera setting			
        [CategoryAttribute("Camera Setting"), DescriptionAttribute("real image, vertical test image, horizontal test image")]
        [RefreshProperties(RefreshProperties.All)]
        public ImageSelectorEnum ImageSelector { 
            get { return image_selector; }
            set {
                if (image_selector != value)
                {
                    image_selector = value;
                    group_1_refCnt++;
                    OnPropertyChanged();
                }
            } 
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("image tirgger or external trigger")]
        [RefreshProperties(RefreshProperties.All)]
        public TriggerSourceEnum TriggerSource
        {
            get { return trigger_source; }
            set {
                if (trigger_source != value) {
                    trigger_source = value;
                    group_1_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Camera Setting")]
        public UInt16 LineScanRate
        {
            get { return line_scan_rate; }
            set
            {
                if (line_scan_rate != value && value >= 5000 && value <= 30000)
                {
                    line_scan_rate = value;
                    group_1_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Camera Setting")]
        public ExposureLevelEnum ExposureLevel
        {
            get { return exposure_level; }
            set
            {
                if (exposure_level != value)
                {
                    exposure_level = value;
                    group_1_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Camera Setting"), DescriptionAttribute("감도")]
        public GainLevelEnum GainLevel
        {
            get { return gain_level; }
            set
            {
                if (gain_level != value) {
                    gain_level = value;
                    group_1_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public UInt16 TDE
        {
            get { return tde; }
            set
            {
                if (tde != value)
                {
                    tde = value;
                    group_2_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public UInt16 TCH
        {
            get { return tch; }
            set
            {
                if (tch != value) {
                    tch = value;
                    group_2_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public UInt16 TRE1
        {
            get { return tre1; }
            set
            {
                if (tre1 != value) {
                    tre1 = value;
                    group_2_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public UInt16 TRE2
        {
            get { return tre2; }
            set
            {
                if (tre2 != value) {
                    tre2 = value;
                    group_2_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public UInt16 TSL
        {
            get { return tsl; }
            set
            {
                if (tsl != value)
                {
                    tsl = value;
                    group_2_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Trigger Timing")]
        public UInt16 TPW
        {
            get { return tpw; }
            set
            {
                if (tpw != value)
                {
                    tpw = value;
                    group_2_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public UInt16 ROIStart
        {
            get { return roi_start; }
            set
            {
                if (roi_start != value && roi_start >= 0 && roi_start < 1024)
                {
                    roi_start = value;
                    group_3_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public UInt16 ROIEnd
        {
            get { return roi_end; }
            set
            {
                if (roi_end != value && roi_end >= 0 && roi_end < 1024)
                {
                    roi_end = value;
                    group_3_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public byte ThresholdLevel
        {
            get { return threshold_level; }
            set
            {
                if (threshold_level != value)
                {
                    threshold_level = value;
                    group_3_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Detect Condition")]
        public UInt16 ThresholdWidth
        {
            get { return threshold_width; }
            set
            {
                if (threshold_width != value && value >= 1 && value <= 1024)
                {
                    threshold_width = value;
                    group_3_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        [CategoryAttribute("Calibration")]
        public bool CalibrationEnable
        {
            get { return calibration_enable; }
            set
            {
                if (calibration_enable != value)
                {
                    calibration_enable = value;
                    group_4_refCnt++;
                    OnPropertyChanged();
                }
            }
        }

        private static UInt16 getUInt16Value(byte[] packet, int startIdx)
        {
            return (UInt16)(packet[startIdx] | ((UInt16)packet[startIdx + 1] << 8));
        }

        private static void setUInt16Value(byte[] packet, int startIdx, UInt16 value)
        {
            packet[startIdx] = (byte)(value & 0xff);
            packet[startIdx + 1] = (byte)(value >> 8);
        }

        public RTGraphParameter(RTGraphParameter src)
        {
            Assign(src);
            group_1_refCnt = 0;
            group_2_refCnt = 0;
            group_3_refCnt = 0;
            group_4_refCnt = 0;
        }

        public RTGraphParameter(byte[] packet = null, int startIdx = 0)
        {
            if (packet != null)
            {
                Parse(packet, startIdx);
                group_1_refCnt = 0;
                group_2_refCnt = 0;
                group_3_refCnt = 0;
                group_4_refCnt = 0;
            }
        }

        public void Assign(RTGraphParameter src)
        {
            ImageSelector = src.image_selector;
            TriggerSource = src.trigger_source;
            LineScanRate = src.line_scan_rate;
            ExposureLevel = src.exposure_level;
            GainLevel = src.gain_level;
            TDE = src.tde;
            TCH = src.tch;
            TRE1 = src.tre1;
            TRE2 = src.tre2;
            TSL = src.tsl;
            TPW = src.tpw;
            ROIStart = src.roi_start;
            ROIEnd = src.roi_end;
            ThresholdLevel = src.threshold_level;
            ThresholdWidth = src.threshold_width;
            CalibrationEnable = src.calibration_enable;
        }

        public void Parse(byte[] packet, int startIdx = 0, int groupMask = MASK_GROUP_ALL)
        {
            if (packet.Length - startIdx >= PARAMETERS_PACKET_SIZE)
            {
                if ((groupMask & MASK_GROUP_1) != 0)
                {
                    ImageSelector = (ImageSelectorEnum)packet[startIdx + 0];
                    TriggerSource = (TriggerSourceEnum)packet[startIdx + 1];
                    LineScanRate = getUInt16Value(packet, startIdx + 2);
                    ExposureLevel = (ExposureLevelEnum)packet[startIdx + 4];
                    GainLevel = (GainLevelEnum)packet[startIdx + 5];
                }
                if ((groupMask & MASK_GROUP_2) != 0)
                {
                    TDE = getUInt16Value(packet, startIdx + 6);
                    TCH = getUInt16Value(packet, startIdx + 8);
                    TRE1 = getUInt16Value(packet, startIdx + 10);
                    TRE2 = getUInt16Value(packet, startIdx + 12);
                    TSL = getUInt16Value(packet, startIdx + 14);
                    TPW = getUInt16Value(packet, startIdx + 16);
                }
                if ((groupMask & MASK_GROUP_3) != 0)
                {
                    ROIStart = getUInt16Value(packet, startIdx + 18);
                    ROIEnd = getUInt16Value(packet, startIdx + 20);
                    ThresholdLevel = packet[startIdx + 22];
                    ThresholdWidth = getUInt16Value(packet, startIdx + 23);
                }
                if ((groupMask & MASK_GROUP_4) != 0)
                {
                    CalibrationEnable = packet[startIdx + 25] != 0;
                }
            }
        }

        public byte[] Serialize(byte[] packet = null, int startIdx = 0, int groupMask = MASK_GROUP_ALL)
        {
            if (packet == null)
            {
                packet = new byte[startIdx + PARAMETERS_PACKET_SIZE];
            }

            if (packet.Length - startIdx >= PARAMETERS_PACKET_SIZE)
            {
                if ((groupMask & MASK_GROUP_1) != 0)
                {
                    packet[startIdx + 0] = (byte)image_selector;
                    packet[startIdx + 1] = (byte)trigger_source;
                    setUInt16Value(packet, startIdx + 2, line_scan_rate);
                    packet[startIdx + 4] = (byte)exposure_level;
                    packet[startIdx + 5] = (byte)gain_level;
                }
                if ((groupMask & MASK_GROUP_2) != 0)
                {
                    setUInt16Value(packet, startIdx + 6, tde);
                    setUInt16Value(packet, startIdx + 8, tch);
                    setUInt16Value(packet, startIdx + 10, tre1);
                    setUInt16Value(packet, startIdx + 12, tre2);
                    setUInt16Value(packet, startIdx + 14, tsl);
                    setUInt16Value(packet, startIdx + 16, tpw);
                }
                if ((groupMask & MASK_GROUP_3) != 0)
                {

                    setUInt16Value(packet, startIdx + 18, roi_start);
                    setUInt16Value(packet, startIdx + 20, roi_end);
                    packet[startIdx + 22] = threshold_level;
                    setUInt16Value(packet, startIdx + 23, threshold_width);
                }
                if ((groupMask & MASK_GROUP_4) != 0)
                {
                    packet[startIdx + 25] = (byte)(calibration_enable ? 1 : 0);
                }
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
