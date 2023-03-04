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

        private static string[] dirStr = { "발신", "수신", "정보", "에러" };

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

        public void AddItem(int direction, string message, byte[] byteData = null)
        {
            //this.Invoke(new Action(() =>
            {
                if(byteData != null)
                {
                    message += " " + BitConverter.ToString(byteData).Replace("-", " ");
                }

                var item = new ListViewItem(
                    new string[]
                    {
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            dirStr[direction],
                            message
                    }
                );

                listView1.BeginUpdate();
                applyListViewCapa(true, 1);
                listView1.Items.Add(item);
                item.EnsureVisible();
                listView1.EndUpdate();
            }
            //));
        }

        private void LogControl_Load(object sender, EventArgs e)
        {
            listView1.SetBounds(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width, this.ClientRectangle.Height);
        }
    }
}
