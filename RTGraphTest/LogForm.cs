using DeviceSimulator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RTGraph
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void AddItem(LogControl.LogTypeEnum logType, string message, byte[] byteData = null)
        {
            logControl1.AddItem(logType, message, byteData);
        }
    }
}
