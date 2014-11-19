using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CustomChart
{
    public partial class Form1 : DevExpress.XtraEditors.XtraForm
    {
        public Form1()
        {
            InitializeComponent();
            var p = new Polygon();
            p.DrawType = FillType.Fill;
            p.AddPoint(new SPoint(0, 0));
            p.AddPoint(new SPoint(1, 0));
            p.AddPoint(new SPoint(0.5, 1));
            this.customChartControl1.Graphs.Add(p);
        }
    }
}
