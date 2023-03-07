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

        public void AddValueLine(byte[] values, int startIdx, int length)
        {
            if (this.values != null)
            {
                length = Math.Min(this.values.Length, Math.Min(length, values.Length - startIdx));
                Array.Copy(values, startIdx, this.values, 0, length);
            }
            valueCnt++;

            if (outBm != null)
            {
                Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);
                BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
                Marshal.Copy(values, 0, IntPtr.Add(bmpData.Scan0, bmpData.Stride * enqPos), length);
                outBm.UnlockBits(bmpData);
                validPos = enqPos;
                enqPos++;
                if (enqPos >= bufferCount)
                {
                    enqPos = 0;
                    this.startPos++;
                }
            }

            this.Refresh();
        }

        private void RTGraphChartControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;

            //e.Graphics.FillRectangle(Brushes.Gray, e.ClipRectangle);

            //e.Graphics.DrawRectangle(Pens.LightGray, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            int startX = GraphMargin.Left + 1;
            int startY = GraphMargin.Top + 1;
            int width = this.Width - (startX + GraphMargin.Right + 1);
            int height = this.Height - (startY + GraphMargin.Bottom + 1);
            if (graphAreaMinHeight > 0 && height < graphAreaMinHeight)
            {
                height = graphAreaMinHeight;
                if (startY + height > this.Height)
                {
                    startY = this.Height - height;
                }
            }

            int graphBaseY = startY + height - 1;

            if (outBm != null)
            {
                if (valueCnt > bufferCount)
                {
                    //GraphicsUnit unit = GraphicsUnit.Pixel;
                    //e.Graphics.DrawImage(outBm, e.ClipRectangle, outBm.GetBounds(ref unit), unit);
                    int drawPos = this.Height * (validPos + 1) / (bufferCount - 1);
                    e.Graphics.DrawImage(outBm, new Rectangle(startX, 0, width, this.Height - drawPos),
                                                new Rectangle(0, validPos + 1, outBm.Width, outBm.Height - (validPos + 1)), GraphicsUnit.Pixel);
                    e.Graphics.DrawImage(outBm, new Rectangle(startX, this.Height - drawPos, width, drawPos),
                                                new Rectangle(0, 0, outBm.Width, validPos + 1), GraphicsUnit.Pixel);
                }
                else
                {
                    e.Graphics.DrawImage(outBm, new Rectangle(startX, 0, width, this.Height), new Rectangle(0, 0, outBm.Width, outBm.Height), GraphicsUnit.Pixel);
                }
            }

            e.Graphics.DrawRectangle(Pens.LightGray, startX, startY, width, height);

            if (this.values != null)
            {
                int oldV = graphBaseY;
                for (int i = 0; i < this.values.Length; i++)
                {
                    int v = graphBaseY - this.values[i] * height / 255;
                    Pen pen;
                    if (triggerValue > 0 && values[i] > triggerValue)
                    {
                        pen = Pens.Red;
                    } 
                    else
                    {
                        pen = Pens.Blue;
                    }
                    e.Graphics.DrawLine(pen, startX + i * width / 1024, oldV, startX + (i + 1) * width / 1024, v);
                    oldV = v;
                }
            }
            if (TriggerValue > 0) {
                int v2 = graphBaseY - TriggerValue * height / 255;
                e.Graphics.DrawLine(Pens.Red, startX, v2, startX + width, v2);
            }

        }

        private void RTGraphChartControl_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }


}

