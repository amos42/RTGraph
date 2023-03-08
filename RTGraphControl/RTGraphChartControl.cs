using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTGraph
{
    public partial class RTGraphChartControl : UserControl
    {
        // half 모드에서 시작 좌표
        public const float START_COORD_POS = 0.5f;

        private int borderLineWidth = 1;
        public int BorderLineWidth
        {
            get { return borderLineWidth; }
            set
            {
                if (borderLineWidth != value)
                {
                    borderLineWidth = value;
                    this.Refresh();
                }
            }
        }

        private int graphAreaMinHeight = 100;
        public int GraphAreaMinHeight
        {
            get { return graphAreaMinHeight; }
            set
            {
                if (graphAreaMinHeight != value && value >= 100)
                {
                    graphAreaMinHeight = value;
                    this.Refresh();
                }
            }
        }

        private int triggerValue = 0;
        public int TriggerValue {
            get { return triggerValue; }
            set {
                if (triggerValue != value)
                {
                    triggerValue = value;
                    this.Refresh();
                }
            }
        }

        private int bufferCount = 0;
        public int BufferCount
        {
            get { return bufferCount; }
            set {
                if (bufferCount != value)
                {
                    bufferCount = value;
                    enqPos = 0;
                    startPos = 0;
                    genImage();
                    this.Refresh();
                }
            }
        }

        private int valueCnt = 0;
        private byte[] values = null;
        //public byte[] Values {
        //    get { return values; }
        //    set {
        //        values = value;
        //        this.Refresh();
        //    }
        //}

        Padding graphMargin = new Padding(10, 100, 10, 100);
        public Padding GraphMargin {
            get { return graphMargin; }
            set
            {
                graphMargin = value;
                this.Refresh();
            } 
        }

        private int startPos = 0;
        public int StartPos
        {
            get { return startPos; }
            set {
                if (startPos != value)
                {
                    startPos = value;
                    this.Refresh();
                }
            }
        }

        private Bitmap outBm = null;
        private int enqPos = 0;
        private int validPos = 0;

        private void genImage()
        {
            if (bufferCount > 0)
            {
                outBm = new Bitmap(1024, bufferCount, PixelFormat.Format8bppIndexed);
                var pal = outBm.Palette;
                for (int i = 0; i < pal.Entries.Length; i++)
                {
                    pal.Entries[i] = Color.FromArgb(i, i, i);
                }
                outBm.Palette = pal;
            } 
            else
            {
                outBm = null;
            }
        }

        public RTGraphChartControl()
        {
            InitializeComponent();

            //byte[] xx = new byte[1024 * 256 * 3];
            //for (int i = 0; i < xx.Length; i++)
            //{
            //    xx[i] = (byte)(i & 0xff);
            //}

            genImage();

            //Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);
            //BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
            //Marshal.Copy(xx, 0, bmpData.Scan0, 1024*250);

            //outBm.UnlockBits(bmpData);

            this.values = new byte[1024];

            BackColor = Color.Black;
        }

        public void SetValuesCap(int size)
        {
            if (values?.Length != size)
            {
                values = new byte[size];
                Array.Clear(values, 0, size);
                this.Refresh();
            }
        }

        public void SetValuesLines(int size)
        {
            if (values?.Length != size)
            {
                values = new byte[size];
                Array.Clear(values, 0, size);
                this.Refresh();
            }
        }

        public void Clear()
        {
            if (outBm != null)
            {
                Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);
                BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
                for (int i = 0; i < outBm.Height; i++)
                {
                    var ptr = IntPtr.Add(bmpData.Scan0, bmpData.Stride * i);
                    for (int j = 0; j < outBm.Width; j++)
                    {
                        Marshal.WriteByte(ptr, j, 0);
                    }
                }
                outBm.UnlockBits(bmpData);

                validPos = enqPos;
                enqPos = 0;
                valueCnt = 0;
                this.startPos = 0;

                this.Refresh();
            }
        }

        public void AddValueLine(int idx, byte[] values, int startIdx, int length)
        {
            if (this.values != null)
            {
                length = Math.Min(this.values.Length, Math.Min(length, values.Length - startIdx));
                Array.Copy(values, startIdx, this.values, 0, length);
            }

            if (outBm != null)
            {
                Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);
                BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
                if (idx >= 0)
                {
                    Marshal.Copy(values, 0, IntPtr.Add(bmpData.Scan0, bmpData.Stride * idx), length);
                } 
                else
                {
                    valueCnt++;
                    Marshal.Copy(values, 0, IntPtr.Add(bmpData.Scan0, bmpData.Stride * enqPos), length);
                    validPos = enqPos;
                    enqPos++;
                    if (enqPos >= bufferCount)
                    {
                        enqPos = 0;
                        this.startPos++;
                    }
                }
                outBm.UnlockBits(bmpData);

                this.Refresh();
            }
        }

        private void RTGraphChartControl_Paint(object sender, PaintEventArgs e)
        {
            // half 모드에서는 시작 좌표가 0.5이다.
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            float startX = START_COORD_POS + GraphMargin.Left;
            float startY = START_COORD_POS + GraphMargin.Top;
            int width = this.ClientSize.Width - (GraphMargin.Left + GraphMargin.Right);
            int height = this.ClientSize.Height - (GraphMargin.Bottom + GraphMargin.Top);

            if (graphAreaMinHeight > 0 && height < graphAreaMinHeight)
            {
                height = graphAreaMinHeight;
                if (startY + height >= this.ClientSize.Height)
                {
                    startY = this.ClientSize.Height - height - borderLineWidth * 2;
                }
            }

            if (outBm != null)
            {
                const int errorTerm = 2; // 이유를 알 수 없는 좌표 보정 값. 원인 분석 필요
                if (valueCnt > bufferCount)
                {
                    int drawPos = this.ClientSize.Height * validPos / outBm.Height;
                    e.Graphics.DrawImage(outBm, new RectangleF(startX-errorTerm, START_COORD_POS, width+errorTerm, this.ClientSize.Height - drawPos),
                                                new Rectangle(0, validPos + 1, outBm.Width, outBm.Height - (validPos + 1)), GraphicsUnit.Pixel);
                    e.Graphics.DrawImage(outBm, new RectangleF(startX- errorTerm, START_COORD_POS + this.ClientSize.Height - drawPos, width+errorTerm, drawPos),
                                                new Rectangle(0, 0, outBm.Width, validPos), GraphicsUnit.Pixel);
                }
                else
                {
                    e.Graphics.DrawImage(outBm, new RectangleF(startX-errorTerm, START_COORD_POS, width+errorTerm, this.ClientSize.Height), new Rectangle(0, 0, outBm.Width, outBm.Height), GraphicsUnit.Pixel);
                }
            }

            float borderStartX = startX - borderLineWidth / 2;
            float borderStartY = startY - borderLineWidth / 2;
            int borderWidth = width + borderLineWidth;
            int borderHeight = height + borderLineWidth;

            var areaPen = new Pen(Color.LightGray, borderLineWidth);
            e.Graphics.DrawRectangle(areaPen, borderStartX, borderStartY, borderWidth, borderHeight);

            width--; height--;
            float graphBaseY = startY + height;

            if (this.values != null)
            {
                float oldV = graphBaseY - this.values[0] * height / 255;
                int cnt = this.values.Length;
                for (int i = 1; i < cnt; i++)
                {
                    float v = graphBaseY - this.values[i] * height / 255;
                    Pen pen;
                    if (triggerValue > 0 && values[i] > triggerValue)
                    {
                        pen = Pens.Lime;
                    } 
                    else
                    {
                        pen = Pens.Blue;
                    }
                    e.Graphics.DrawLine(pen, startX + (i - 1) * width / (cnt - 1), oldV, startX + i * width / (cnt - 1), v);
                    oldV = v;
                }
            }
            if (TriggerValue > 0) {
                float v2 = graphBaseY - TriggerValue * height / 255;
                var trigPen = new Pen(Color.Red, 1);
                trigPen.DashStyle = DashStyle.Dash;
                e.Graphics.DrawLine(trigPen, startX, v2, startX + width - 1, v2);
            }

        }

        private void RTGraphChartControl_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }


}

