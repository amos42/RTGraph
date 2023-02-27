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
        private int triggerValue = 0;
        public int TriggerValue {
            get { return triggerValue; }
            set { 
                triggerValue = value;
                this.Refresh();
            }
        }

        private int bufferCount = 50;
        public int BufferCount
        {
            get { return bufferCount; }
            set { 
                bufferCount = value;
                enqPos = 0;
                startPos = 0;
                genImage();
                this.Refresh();
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

        public Padding GraphMargin { get; set; } = new Padding(10, 100, 10, 100);

        private int startPos = 0;
        public int StartPos
        {
            get { return startPos; }
            set { 
                    startPos = value; 
                    this.Refresh(); 
            }
        }

        private Bitmap outBm;
        private int enqPos = 0;
        private int validPos = 0;

        private void genImage()
        {
            outBm = new Bitmap(1024, bufferCount, PixelFormat.Format8bppIndexed);
            var pal = outBm.Palette;
            for (int i = 0; i < pal.Entries.Length; i++)
            {
                pal.Entries[i] = Color.FromArgb(i, i, i);
            }
            outBm.Palette = pal;
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

        public void AddValueLine(byte[] values)
        {
            if (this.values != null)
            {
                Array.Copy(values, this.values, values.Length);
            }
            valueCnt++;

            Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);
            BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
            Marshal.Copy(values, 0, IntPtr.Add(bmpData.Scan0, bmpData.Stride * enqPos), values.Length);
            outBm.UnlockBits(bmpData);
            validPos = enqPos;
            enqPos++;
            if(enqPos >= bufferCount)
            {
                enqPos = 0;
                this.startPos ++;
            }

            this.Refresh();
        }

        private void RTGraphChartControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.None;

            e.Graphics.FillRectangle(Brushes.Gray, e.ClipRectangle);

            e.Graphics.DrawRectangle(Pens.Yellow, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            if (valueCnt > bufferCount)
            {
                //GraphicsUnit unit = GraphicsUnit.Pixel;
                //e.Graphics.DrawImage(outBm, e.ClipRectangle, outBm.GetBounds(ref unit), unit);
                int drawPos = this.Height * (validPos + 1) / (bufferCount - 1);
                e.Graphics.DrawImage(outBm, new Rectangle(GraphMargin.Left, 0, this.Width - (GraphMargin.Left + GraphMargin.Right), this.Height - drawPos),
                                            new Rectangle(0, validPos + 1, outBm.Width, outBm.Height - (validPos + 1)), GraphicsUnit.Pixel);
                e.Graphics.DrawImage(outBm, new Rectangle(GraphMargin.Left, this.Height - drawPos, this.Width - (GraphMargin.Left + GraphMargin.Right), drawPos), 
                                            new Rectangle(0, 0, outBm.Width, validPos + 1), GraphicsUnit.Pixel);
            } 
            else
            {
                e.Graphics.DrawImage(outBm, new Rectangle(GraphMargin.Left, 0, this.Width - (GraphMargin.Left + GraphMargin.Right), this.Height), new Rectangle(0, 0, outBm.Width, outBm.Height), GraphicsUnit.Pixel);
            }

            int width = this.Width - (GraphMargin.Left + GraphMargin.Right);
            int height = this.Height - (GraphMargin.Top + GraphMargin.Bottom);

            e.Graphics.DrawRectangle(Pens.Red, GraphMargin.Left, GraphMargin.Top, this.Width - (GraphMargin.Left + GraphMargin.Right), this.Height - (GraphMargin.Top + GraphMargin.Bottom));

            if (this.values != null)
            {
                int oldV = this.Height - GraphMargin.Bottom;
                for (int i = 0; i < this.values.Length; i++)
                {
                    int v = this.Height - GraphMargin.Bottom - this.values[i] * height / 255;
                    Pen pen = Pens.Blue;
                    if (triggerValue > 0 && values[i] > triggerValue)
                    {
                        pen = Pens.Red;
                    }
                    e.Graphics.DrawLine(pen, GraphMargin.Left + i * width / 1024, oldV, GraphMargin.Left + (i + 1) * width / 1024, v);
                    oldV = v;
                }
            }
            if (TriggerValue != 0) {
                int v2 = this.Height - GraphMargin.Bottom - TriggerValue * height / 255;
                e.Graphics.DrawLine(Pens.LightGray, GraphMargin.Left, v2, GraphMargin.Left + width, v2);
            }

        }

        private void RTGraphChartControl_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }


}

