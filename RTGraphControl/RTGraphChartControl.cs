using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
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

        private int valideAreaStart = -1;
        public int ValideAreaStart
        {
            get { return valideAreaStart; }
            set
            {
                if (valideAreaStart != value && value <= valideAreaEnd)
                {
                    valideAreaStart = value;
                    this.Refresh();
                }
            }
        }

        private int valideAreaEnd = 1024;
        public int ValideAreaEnd
        {
            get { return valideAreaEnd; }
            set
            {
                if (valideAreaEnd != value && value >= valideAreaStart)
                {
                    valideAreaEnd = value;
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
                    //startPos = 0;
                    genImage();
                    this.Refresh();
                }
            }
        }

        private int valueCount = 1024;
        public int ValueCount
        {
            get { return valueCount; }
            set
            {
                if (value != valueCount && value > 0)
                {
                    valueCount = value;
                    Values.ForEach(items => {
                        items.SetSize(value);
                    });
                    enqPos = 0;
                    //startPos = 0;
                    genImage();
                    this.Refresh();
                }
            }
        }

        private int valueCnt = 0;

        public List<GraphItem> Values { get; } = new List<GraphItem>();

        public int ValuesCount {
            get { return Values.Count; }
            set
            {
                if (value != Values.Count && value > 1)
                {
                    if (value > Values.Count)
                    {
                        int cnt = value - Values.Count;
                        for (int i = 0; i < cnt; i++)
                        {
                            Values.Add(new GraphItem(valueCount));
                        }
                    }
                    else
                    {
                        int cnt = Values.Count - value;
                        for (int i = 0; i < cnt; i++)
                        {
                            Values.RemoveAt(value);
                        }
                    }
                    this.Refresh();
                }
            }
        }

        public bool IndexedMode { get; set; } = false;

        public bool StretchMode { get; set; } = true;

        private bool osdVisible = false;
        public bool OSDVisible { 
            get { return osdVisible;  } 
            set
            {
                if (value != osdVisible) 
                { 
                    osdVisible = value;
                    this.Refresh();
                }
            } 
        }

        private bool axisVisible = false;
        public bool AxisVisible
        {
            get { return axisVisible; }
            set
            {
                if (value != axisVisible)
                {
                    axisVisible = value;
                    this.Refresh();
                }
            }
        }

        private bool bufferAxisVisible = false;
        public bool BufferAxisVisible
        {
            get { return bufferAxisVisible; }
            set
            {
                if (value != bufferAxisVisible)
                {
                    bufferAxisVisible = value;
                    this.Refresh();
                }
            }
        }

        Padding graphMargin = new Padding(10, 100, 10, 100);
        public Padding GraphMargin {
            get { return graphMargin; }
            set
            {
                graphMargin = value;
                this.Refresh();
            } 
        }

        public event ErrorEventHandler ErrorEvent;

        private Queue<KeyValuePair<int, byte[]>> pendingGraphQueue = new Queue<KeyValuePair<int, byte[]>>();
        private Bitmap outBm = null;
        private int enqPos = 0;
        private int validPos = 0;
        private readonly Object thisBlock = new Object();

        private void genImage()
        {
            if (bufferCount > 0)
            {
                outBm = new Bitmap(valueCount, bufferCount, PixelFormat.Format8bppIndexed);
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

            genImage();

            this.Values.Add(new GraphItem(valueCount, Color.Blue, Color.Lime));

            BackColor = Color.Black;
        }

        public void SetValuesCapacity(int size)
        {
            int cnt = 0;
            this.Values.ForEach(items =>
            {
                if (items.SetSize(size) == true)
                {
                    cnt++;
                }
            });
            if(cnt > 0)
            {
                this.Refresh();
            }
        }

        public void Clear(bool isRefresh = true)
        {
            if (outBm != null)
            {
                Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);
                lock (thisBlock)
                {
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
                }

                validPos = enqPos;
                enqPos = 0;
                valueCnt = 0;
                //this.startPos = 0;

                if (isRefresh) this.Refresh();
            }
        }

        private void applyBitmap(int index, byte[] line)
        {
            if (outBm == null) return;

            Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);

            BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
            if (IndexedMode)
            {
                Marshal.Copy(line, 0, IntPtr.Add(bmpData.Scan0, bmpData.Stride * index), line.Length);
            }
            else
            {
                valueCnt++;
                Marshal.Copy(line, 0, IntPtr.Add(bmpData.Scan0, bmpData.Stride * enqPos), line.Length);
                validPos = enqPos;
                enqPos++;
                if (enqPos >= bufferCount)
                {
                    enqPos = 0;
                    //this.startPos++;
                }
            }
            outBm.UnlockBits(bmpData);
        }

        private void _applyBitmapOne(BitmapData bmpData, int idx, byte[] data, int stIdx = 0)
        {
            if (IndexedMode)
            {
                if (idx < bmpData.Height)
                {
                    if (data != null)
                    {
                        Marshal.Copy(data, stIdx, IntPtr.Add(bmpData.Scan0, bmpData.Stride * idx), data.Length);
                    }
                }
            }
            else
            {
                if (data != null)
                {
                    valueCnt++;
                    Marshal.Copy(data, stIdx, IntPtr.Add(bmpData.Scan0, bmpData.Stride * enqPos), data.Length);
                    validPos = enqPos;
                    enqPos++;
                    if (enqPos >= bufferCount)
                    {
                        enqPos = 0;
                        //this.startPos++;
                    }
                }
            }
        }


        private void applyBitmap(Queue<KeyValuePair<int, byte[]>> graphQueue)
        {
            if (outBm == null) return;

            Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);

            BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
            while (graphQueue.Count > 0)
            {
                var values = graphQueue.Dequeue();
                _applyBitmapOne(bmpData, values.Key, values.Value);
            }
            outBm.UnlockBits(bmpData);
        }

        private void applyBitmapOne(int idx, byte[] data, int stIdx = 0)
        {
            if (outBm == null) return;

            Rectangle rect = new Rectangle(0, 0, outBm.Width, outBm.Height);

            BitmapData bmpData = outBm.LockBits(rect, ImageLockMode.WriteOnly, outBm.PixelFormat);
            _applyBitmapOne(bmpData, idx, data, stIdx);
            outBm.UnlockBits(bmpData);
        }

        public void SetValueLine(int idx, byte[] values, int startIdx = 0, int length = -1, int pos = -1, bool isRefresh = true)
        {
            if (idx >= this.Values.Count) return;
            if (idx == 0 && IndexedMode && pos >= this.BufferCount) return;

            if (this.Values[idx]?.Items != null)
            {
                lock (thisBlock)
                {
                    if (length <= 0) length = values.Length;
                    length = Math.Min(this.Values[idx].Items.Length, Math.Min(length, values.Length - startIdx));
                    Array.Copy(values, startIdx, this.Values[idx].Items, 0, length);
                    this.Values[idx].Index = pos;

                    applyBitmapOne(pos, values, startIdx);
                }

                //var itm = this.Values[idx].Items.Clone() as byte[];
                //if (itm != null)
                //{
                //    pendingGraphQueue.Enqueue(new KeyValuePair<int, byte[]>(pos, itm));
                //}
                //else
                //{
                //    raiseErrorEvent(new Exception("value is not array"));
                //}
            }

            if (isRefresh) this.Refresh();
        }

        private DateTime oldTime = DateTime.Now;
        private int oldIdx = 0;

        private void RTGraphChartControl_Paint(object sender, PaintEventArgs e)
        {
            // half 모드에서는 시작 좌표가 0.5이다.
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;

            float startX = START_COORD_POS + GraphMargin.Left;
            float startY = START_COORD_POS + GraphMargin.Top;
            int width = this.ClientSize.Width - (GraphMargin.Left + GraphMargin.Right) - (bufferAxisVisible ? 20 : 0);
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
                applyBitmap(pendingGraphQueue);

                const int errorTerm = 2; // 이유를 알 수 없는 좌표 보정 값. 원인 분석 필요
                if (valueCnt <= bufferCount)
                {
                    if (StretchMode)
                    {
                        e.Graphics.DrawImage(outBm, new RectangleF(startX - errorTerm, START_COORD_POS, width + errorTerm, this.ClientSize.Height), new Rectangle(0, 0, outBm.Width, outBm.Height), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        var scrOnePixel = width / 1024f;
                        var virHeight = height / scrOnePixel;
                        e.Graphics.DrawImage(outBm, new RectangleF(startX - errorTerm, START_COORD_POS, width + errorTerm, this.ClientSize.Height), new Rectangle(0, 0, outBm.Width, (int)virHeight), GraphicsUnit.Pixel);
                    }
                }
                else
                {
                    if (StretchMode)
                    {
                        int drawPos = this.ClientSize.Height * validPos / outBm.Height;
                        e.Graphics.DrawImage(outBm, new RectangleF(startX - errorTerm, START_COORD_POS, width + errorTerm, this.ClientSize.Height - drawPos),
                                                    new Rectangle(0, validPos + 1, outBm.Width, outBm.Height - (validPos + 1)), GraphicsUnit.Pixel);
                        e.Graphics.DrawImage(outBm, new RectangleF(startX - errorTerm, START_COORD_POS + this.ClientSize.Height - drawPos, width + errorTerm, drawPos),
                                                    new Rectangle(0, 0, outBm.Width, validPos), GraphicsUnit.Pixel);
                    }
                    else
                    {

                    }
                }
            }

            float borderStartX = startX - borderLineWidth / 2;
            float borderStartY = startY - borderLineWidth / 2;
            int borderWidth = width + borderLineWidth;
            int borderHeight = height + borderLineWidth;

            var areaPen = new Pen(Color.Gray, borderLineWidth);
            e.Graphics.DrawRectangle(areaPen, borderStartX, borderStartY, borderWidth, borderHeight);

            width--; height--;
            float graphBaseY = startY + height;

            if (axisVisible)
            {
                for (int i = 16; i < 256; i += 16)
                {
                    float v = graphBaseY - i * height / 255;
                    e.Graphics.DrawLine(Pens.Gray, startX, v, startX + width, v);
                }
                for (int i = 16; i < 1024; i += 16)
                {
                    float v = startX + i * width / 1023;
                    e.Graphics.DrawLine(Pens.Gray, v, startY, v, startY + height);
                }
            }

            if (valideAreaStart >= 0 && valideAreaStart <= 1023)
            {
                float v = startX + valideAreaStart * width / 1023;
                e.Graphics.DrawLine(Pens.DarkGray, v, startY, v, startY + height);
            }
            if (valideAreaEnd >= 0 && valideAreaEnd <= 1023)
            {
                float v = startX + valideAreaEnd * width / 1023;
                e.Graphics.DrawLine(Pens.DarkGray, v, startY, v, startY + height);
            }

            if (bufferAxisVisible)
            {
                var virHeight = (float)this.ClientSize.Height / bufferCount;
                for (int i = 10; i < bufferCount; i += 10)
                {
                    var h = i * virHeight;
                    int s;
                    if (i % 100 == 0)
                        s = 20;
                    else if (i % 50 == 0)
                        s = 10;
                    else
                        s = 5;
                    e.Graphics.DrawLine(Pens.LightGray, this.ClientSize.Width - s, h, this.ClientSize.Width, h);
                }
            }

            this.Values.ForEach(items => {
                var gpen = new Pen(items.GraphColor, items.LineWidth);
                var tpen = new Pen(items.TriggerColor, items.LineWidth);

                float oldV = graphBaseY - items.Items[0] * height / 255;
                int cnt = items.Items.Length;
                for (int i = 1; i < cnt; i++)
                {
                    float v = graphBaseY - items.Items[i] * height / 255;
                    Pen pen;
                    pen = (triggerValue > 0 && items.Items[i] > triggerValue) ? tpen : gpen;
                    e.Graphics.DrawLine(pen, startX + (i - 1) * width / (cnt - 1), oldV, startX + i * width / (cnt - 1), v);
                    oldV = v;
                }
            });

            if (TriggerValue > 0)
            {
                float v2 = graphBaseY - TriggerValue * height / 255;
                var trigPen = new Pen(Color.Red, 1);
                trigPen.DashStyle = DashStyle.Dash;
                e.Graphics.DrawLine(trigPen, startX, v2, startX + width - 1, v2);
            }

            if (OSDVisible)
            {
                var diff = DateTime.Now.Subtract(oldTime);
                int idx = this.Values[0].Index;

                float vv = (float)(idx - oldIdx) / diff.Ticks * TimeSpan.TicksPerSecond;

                oldTime = DateTime.Now;
                oldIdx = idx;

                e.Graphics.DrawString($"{vv.ToString("N")}/s", SystemFonts.DefaultFont, Brushes.Red, new PointF(10, 10));
            }
        }

        protected void raiseErrorEvent(Exception ex)
        {
            if (ErrorEvent != null)
            {
                ErrorEvent(this, new ErrorEventArgs(ex));
            }
        }

        private void RTGraphChartControl_Resize(object sender, EventArgs e)
        {
            this.Refresh();
        }
    }

    public class GraphItem : ICloneable
    {
        public int Index { get; set; } = 0;

        private byte[] items = null;
        public byte[] Items
        {
            get { return items; }
        }

        public int Size
        {
            get { return items != null ? items.Length : 0; }
            set
            {
                if (value > 0 && (items == null || value != items.Length))
                {
                    items = new byte[value];
                } 
                else
                {
                    items = null;
                }
            }
        }

        public Color GraphColor { get; set; } = Color.White;
        public Color TriggerColor { get; set; } = Color.Red;
        public int LineWidth { get; set; } = 1;

        public bool SetSize(int size, bool isClear = true)
        {
            bool result = false;
            if (size > 0)
            {
                if (Items == null || size != Items.Length)
                {
                    items = new byte[size];
                    if (isClear)
                    {
                        Array.Clear(Items, 0, size);
                    }
                    result = true;
                }
            }
            else
            {
                if (size != Items?.Length)
                {
                    result = true;
                }
                items = null;
            }
            return result;
        }

        public GraphItem(int size, Color graphColor, Color triggerColor, int lineWidth = 1)
        {
            SetSize(size);
            GraphColor = graphColor;
            TriggerColor = triggerColor;
            LineWidth = lineWidth;
        }

        public GraphItem(Color graphColor, Color triggerColor, int lineWidth = 1) : this(0, graphColor, triggerColor, lineWidth)
        {            
        }

        public GraphItem(int size) : this(size, Color.White, Color.Red)
        {
        }

        public GraphItem() : this(0, Color.White, Color.Red)
        {
        }

        public object Clone()
        {
            var item =  new GraphItem(this.GraphColor, this.TriggerColor, this.LineWidth);
            item.items = this.items.Clone() as byte[];
            item.Index = this.Index;
            return item;
        }
    }
}

