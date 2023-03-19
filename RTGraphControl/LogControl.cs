using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using RTGraph;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;

namespace DeviceSimulator
{
    public partial class LogControl : UserControl
    {
        private int listViewCapa = 100;
        public int ListViewCapa {
            get { return listViewCapa; }
            set
            {
                listViewCapa = value;
                applyListViewCapa();
            } 
        }

        private bool listViewAutoScroll = true;
        public bool ListViewAutoScroll {
            get { return listViewAutoScroll; }
            set
            {
                listViewAutoScroll = value;
                ((ToolStripMenuItem)contextMenuStrip1.Items[1]).Checked = listViewAutoScroll;
            } 
        }

        public enum LogTypeEnum {
            Send,
            Recv,
            Info,
            Error
        }

        private static string[] dirStr = { "Send", "Recv", "Info", "Error" };

        public LogControl()
        {
            InitializeComponent();

            listView1.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(listView1, true);
        }

        private void applyListViewCapa(bool isUpdating = false, int gap = 0)
        {
            if (!isUpdating) listView1.BeginUpdate();
            while (listView1.Items.Count >= listViewCapa + gap)
            {
                listView1.Items.RemoveAt(0);
            }
            if (!isUpdating) listView1.EndUpdate();
        }

        public void AddItem(LogTypeEnum logType, string message, byte[] byteData = null)
        {
            //this.Invoke(new Action(() =>
            {
                var sb = new StringBuilder();
                if (message != null) sb.Append(message);
                if (byteData != null)
                {
                    if (sb.Length > 0) sb.Append(" ");
                    sb.Append(BitConverter.ToString(byteData).Replace("-", " "));
                }

                var item = new ListViewItem(
                    new string[]
                    {
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            dirStr[(int)logType],
                            sb.ToString()
                    }
                );

                if (ListViewAutoScroll)
                {
                    listView1.BeginUpdate();
                    applyListViewCapa(true, 1);
                    listView1.Items.Add(item);
                    item.EnsureVisible();
                    listView1.EndUpdate();
                }
                else
                {
                    listView1.Items.Add(item);
                }
            }
            //));
        }

        private void LogControl_Load(object sender, EventArgs e)
        {
            listView1.SetBounds(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
        }

        private void LogControl_Resize(object sender, EventArgs e)
        {
            listView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void copyDataToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems?.Count > 0) {
                Clipboard.SetText(listView1.SelectedItems[0].SubItems[2].Text);
            }            
        }

        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewAutoScroll = !ListViewAutoScroll;
        }

        private void exportToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Path.GetDirectoryName(Application.ExecutablePath);
            var date = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var logFileName = Path.Combine(path, $"log_{date}.csv");
            try
            {
                var sw = new StreamWriter(logFileName, true);
                sw.WriteLine("Time,Type,Message");
                for (int i = 0; i < listView1.Items.Count; i++)
                {
                    sw.WriteLine($"{listView1.Items[i].Text},{listView1.Items[i].SubItems[1].Text},\"{listView1.Items[i].SubItems[2].Text}\"");
                }
                sw.Close();
            } 
            finally
            {
                MessageBox.Show($"Log Export Successful. ({logFileName})");
            }
        }
    }
}
