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
    public partial class RTGraphChartControl : UserControl
    {
        public int TriggerValue { get; set; } = 0;

        public RTGraphChartControl()
        {
            InitializeComponent();
        }

        private void RTGraphChartControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);
            e.Graphics.DrawLine(Pens.Red, 0, 0, 100, 100);

        }
    }
}
