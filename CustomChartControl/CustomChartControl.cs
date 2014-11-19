using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;

namespace CustomChart
{
    public partial class CustomChartControl : DevExpress.XtraEditors.XtraUserControl, IDiagram
    {

        private Point axisXLocation = new Point(0, 0);
        private Point axisYLocation = new Point(0, 0);
        private int axisXHeight;
        private int axisYHeight;
        private int axisXWidth;
        private int axisYWidth;
        private double axisXper;
        private double axisYper;
        private Pen forePen;
        private SolidBrush foreBrush;
        private DrawModeType _DrawMode = DrawModeType.CustomMode;
        public DrawModeType DrawMode
        {
            get
            {
                return this._DrawMode;
            }
            set
            {
                this._DrawMode = value;
            }
        }
        private Padding _AxisMargin = new Padding(3, 3, 3, 3);
        public Padding AxisMargin
        {
            get
            {
                return this._AxisMargin;
            }
            set
            {
                this._AxisMargin = value;
                this.RaisePaintEvent(new object(), new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
            }
        }
        private Axis _AxisX;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Axis AxisX
        {
            get
            {
                return this._AxisX;
            }
        }
        private Axis _AxisY;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Axis AxisY
        {
            get
            {
                return this._AxisY;
            }
        }
        private GraphCollection _Graphs = new GraphCollection();
        public GraphCollection Graphs
        {
            get
            {
                return this._Graphs;
            }
        }
        public CustomChartControl()
        {
            InitializeComponent();
            _AxisX = new Axis(this);
            _AxisX.AxisViewChanged += _AxisXProperties_LabelSizeChanged;
            _AxisY = new Axis(this);
            _AxisY.AxisViewChanged += _AxisYProperties_LabelSizeChanged;
            _Graphs.BoundChanged += _Graphs_BoundChanged;
            this.forePen = DPens.GetPenFromColor(this.ForeColor);
            this.foreBrush = DBrushs.GetBrushFromColor(this.ForeColor);
        }

        void _Graphs_BoundChanged(object sender, EventArgs e)
        {
            if (this.AxisX.Auto)
            {
                this.AxisX.MinValue = this.Graphs.MinX;
                this.AxisX.MaxValue = this.Graphs.MaxX;
            }
            if (this.AxisY.Auto)
            {
                this.AxisY.MinValue = this.Graphs.MinY;
                this.AxisY.MaxValue = this.Graphs.MaxY;
            }
        }
        private bool isUpdating = false;
        public void BeginUpdate()
        {
            isUpdating = true;
        }
        public void EndUpdate(bool immediate = true)
        {
            isUpdating = false;
            if(immediate)
                this.RaisePaintEvent(new object(), new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        void _AxisYProperties_LabelSizeChanged(object sender, EventArgs e)
        {
            if(!isUpdating)
                this.RaisePaintEvent(new object(), new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }

        void _AxisXProperties_LabelSizeChanged(object sender, EventArgs e)
        {
             if(!isUpdating)
            this.RaisePaintEvent(new object(), new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            forePen = DPens.GetPenFromColor(this.ForeColor);
            foreBrush = DBrushs.GetBrushFromColor(this.ForeColor);
             if(!isUpdating)
            this.RaisePaintEvent(new object(), new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }
        private void CustomChartControl_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphicsContext currentContext = BufferedGraphicsManager.Current;
            BufferedGraphics myBuffer = currentContext.Allocate(e.Graphics, e.ClipRectangle);
            Graphics g = myBuffer.Graphics;
            g.Clear(this.BackColor);
            this.drawAxisLine(g);
            this.drawAxisXLabel(g);
            this.drawAxisYLabel(g);
            this.drawSeries(g);
            myBuffer.Render(e.Graphics);
            myBuffer.Dispose();
            g.Dispose();
        }
        private void drawAxisLine(Graphics g)
        {
            axisYLocation.X = this.Margin.Left;
            axisYLocation.Y = this.Margin.Top + this.AxisY.AxisLabelHeight / 2;
            axisYWidth = this.AxisY.AxisLabelWidth + this.AxisY.ThickLength + this.AxisY.ThickMargin;
            axisXLocation.X = this.Margin.Left + axisYWidth;
            axisXHeight = this.AxisX.AxisLabelHeight + this.AxisX.ThickLength + this.AxisX.ThickMargin;
            axisXLocation.Y = this.Height - this.Margin.Bottom - axisXHeight;
            axisXWidth = this.Width - axisXLocation.X - this.Margin.Right - this.AxisX.AxisLabelWidth / 2;
            axisYHeight = this.axisXLocation.Y - axisYLocation.Y;
            g.DrawLine(DPens.GetPenFromColor(AxisX.Color), this.axisXLocation.X, this.axisXLocation.Y, this.axisXLocation.X + this.axisXWidth, this.axisXLocation.Y);
            g.DrawLine(DPens.GetPenFromColor(AxisY.Color), this.axisYLocation.X + axisYWidth, this.axisYLocation.Y, this.axisYLocation.X + axisYWidth, this.axisYLocation.Y + this.axisYHeight);
        }

        private void drawAxisXLabel(Graphics g)
        {
            this.axisXper= this.axisXWidth / ((double)this.AxisX.MaxValue - (double)this.AxisX.MinValue+this.AxisX.SideMargin*2);
            int lengthbtthick = (int)(this.axisXper * this.AxisX.AxisLabelPer);
            int labelthick = (int)Math.Ceiling((double)this.AxisX.AxisLabelWidth / lengthbtthick);
            int i, x;
            switch (this.AxisX.ValueType)
            {
                case AxisValueType.Number:
                    {
                        string target;
                        for (i = 0; i <= this.AxisX.ThickCount; i++)
                        {
                            x = axisXLocation.X + (int)((i * this.AxisX.AxisLabelPer+this.AxisX.SideMargin)*this.axisXper);
                            g.DrawLine(DPens.GetPenFromColor(AxisX.Color), new Point(x, axisXLocation.Y), new Point(x, axisXLocation.Y + this.AxisX.ThickLength));
                            if (i % labelthick == 0)
                            {
                                target = ((double)this.AxisX.MinValue + this.AxisX.AxisLabelPer * i).ToString();
                                g.DrawString(target, this.AxisX.Font, this.foreBrush, new Point(x - (int)(g.MeasureString(target, this.AxisX.Font).Width / 2), axisXLocation.Y + this.AxisX.ThickMargin + this.AxisX.ThickLength));
                            }
                        }
                        break;
                    }
                case AxisValueType.DateTime:
                    {
                        //for (i = 0; i < count; i++)
                        //{
                        //    x = startX + i * xperlength;
                        //    g.DrawLine(forePen, new Point(x, axisXLocation.Y), new Point(x, axisXLocation.Y + 10));
                        //    if (i % labelthick == 0)
                        //    {
                        //        g.DrawString(((DateTime)this.AxisX.MinValue).AddMilliseconds(this.AxisX.axisLabelPer * i).ToString(), this.AxisX.Font, this.foreBrush, new Point(x - this.AxisX.axisLabelWidth / 2, axisXLocation.Y + 13));
                        //    }
                        //}
                        break;
                    }
                case AxisValueType.Argument:
                    {
                        //for (i = 0; i < count; i++)
                        //{
                        //    x = startX + i * xperlength;
                        //    g.DrawLine(forePen, new Point(x, axisXLocation.Y), new Point(x, axisXLocation.Y + 10));
                        //    //if (i % labelthick == 0)
                        //    //{
                        //    //    g.DrawString(((DateTime)this.AxisX.MinValue).AddMilliseconds(this.AxisX.axisLabelPer * i).ToString(), this.AxisX.Font, this.foreBrush, new Point(x - this.AxisX.axisLabelWidth / 2, axisXLocation.Y + 13));
                        //    //}
                        //}
                        break;
                    }
            }
        }
        private void drawAxisYLabel(Graphics g)
        {
            this.axisYper = this.axisYHeight / ((double)this.AxisY.MaxValue - (double)this.AxisY.MinValue+this.AxisY.SideMargin*2);
            int lengthbtthick = (int)(this.axisYper * this.AxisY.AxisLabelPer);
            int labelthick = (int)Math.Ceiling((double)this.AxisY.AxisLabelHeight / lengthbtthick);
            int i, y;
            switch (this.AxisY.ValueType)
            {
                case AxisValueType.Number:
                    {
                        string target;
                        for (i = 0; i <= this.AxisY.ThickCount; i++)
                        {
                            y = axisXLocation.Y - (int)((i * this.AxisY.AxisLabelPer + this.AxisY.SideMargin) * this.axisYper);
                            g.DrawLine(DPens.GetPenFromColor(AxisY.Color), new Point(axisXLocation.X, y), new Point(axisXLocation.X - this.AxisY.ThickLength, y));
                            if (i % labelthick == 0)
                            {
                                target = ((double)this.AxisY.MinValue + this.AxisY.AxisLabelPer * i).ToString();
                                g.DrawString(target, this.AxisY.Font, this.foreBrush, new Point((int)(axisXLocation.X - this.AxisY.ThickLength - this.AxisY.ThickMargin - g.MeasureString(target, AxisY.Font).Width), y - (int)(this.AxisY.AxisLabelHeight / 2)));
                            }
                        }
                        break;
                    }
                case AxisValueType.DateTime:
                    {
                        //for (i = 0; i < count; i++)
                        //{
                        //    x = startX + i * xperlength;
                        //    g.DrawLine(forePen, new Point(x, axisXLocation.Y), new Point(x, axisXLocation.Y + 10));
                        //    if (i % labelthick == 0)
                        //    {
                        //        g.DrawString(((DateTime)this.AxisX.MinValue).AddMilliseconds(this.AxisX.axisLabelPer * i).ToString(), this.AxisX.Font, this.foreBrush, new Point(x - this.AxisX.axisLabelWidth / 2, axisXLocation.Y + 13));
                        //    }
                        //}
                        break;
                    }
                case AxisValueType.Argument:
                    {
                        //for (i = 0; i < count; i++)
                        //{
                        //    x = startX + i * xperlength;
                        //    g.DrawLine(forePen, new Point(x, axisXLocation.Y), new Point(x, axisXLocation.Y + 10));
                        //    //if (i % labelthick == 0)
                        //    //{
                        //    //    g.DrawString(((DateTime)this.AxisX.MinValue).AddMilliseconds(this.AxisX.axisLabelPer * i).ToString(), this.AxisX.Font, this.foreBrush, new Point(x - this.AxisX.axisLabelWidth / 2, axisXLocation.Y + 13));
                        //    //}
                        //}
                        break;
                    }
            }
        }
        private void drawSeries(Graphics g)
        {
            foreach (var graph in this._Graphs)
            {
                graph.Draw(g, this);
            }
        }
        private void CustomChartControl_SizeChanged(object sender, EventArgs e)
        {
             if(!isUpdating)
            this.RaisePaintEvent(new object(), new PaintEventArgs(this.CreateGraphics(), this.ClientRectangle));
        }
        public Point PointToDiagram(double x, double y)
        {
            return new Point((int)((x - (double)this.AxisX.MinValue) * this.axisXper), (int)((y - (double)this.AxisY.MinValue) * this.axisYper));
        }
        public Point PointToDiagram(string x, double y)
        {
            return new Point();
        }
        public Point PointToDiagram(DateTime x, double y)
        {
            return new Point();
        }
        public Point PointToDiagram(SPoint p)
        {
            return new Point((int)((p.X - (double)this.AxisX.MinValue + this.AxisX.SideMargin) * this.axisXper) + this.axisXLocation.X, this.axisXLocation.Y - (int)((p.Y - (double)this.AxisY.MinValue + this.AxisY.SideMargin) * this.axisYper));
        }
    }
    public enum DrawModeType
    {
        CustomMode,
        AutoMode
    }
    public enum AxisValueType
    {
        Argument,
        Number,
        DateTime
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Axis
    {
        private Control Parent;
        private Color _Color;
        [DefaultValue(typeof(Color),"DarkGray")]
        public Color Color 
        {
            get
            {
                return this._Color.IsEmpty?Color.DarkGray:this._Color;
            }
            set
            {
                this._Color = value;
                if (this.AxisViewChanged != null)
                    this.AxisViewChanged(this, new EventArgs());
            }
        }
        [Browsable(false)]
        public int AxisLabelWidth { get; private set; }
        [Browsable(false)]
        public int AxisLabelHeight { get; private set; }
        [Browsable(false)]
        public double AxisLabelPer { get; private set; }
        [Browsable(false)]
        public int ThickCount { get; private set; }
        private int _ThickLength = 10;
        [DefaultValue(10)]
        public int ThickLength
        {
            get
            {
                return this._ThickLength;
            }
            set
            {
                if (this._ThickLength != value)
                {
                    this._ThickLength = value;
                    if (this.AxisViewChanged != null)
                        this.AxisViewChanged(this, new EventArgs());
                }
            }
        }
        private int _ThickMargin = 3;
        [DefaultValue(3)]
        public int ThickMargin
        {
            get
            {
                return this._ThickMargin;
            }
            set
            {
                if (this._ThickMargin != value)
                {
                    this._ThickMargin = value;
                    if (this.AxisViewChanged != null)
                        this.AxisViewChanged(this, new EventArgs());
                }
            }
        }
        private Font _Font;
        public Font Font
        {
            get
            {
                return this._Font;
            }
            set
            {
                if (this._Font != value)
                {
                    this._Font = value;
                    refreshAxisLabelSize();
                }
            }
        }
        private bool _Auto = true;
        [DefaultValue(true)]
        public bool Auto
        {
            get
            {
                return _Auto;
            }
            set
            {
                this._Auto = value;
            }
        }
        private AxisValueType _ValueType = AxisValueType.Number;
        [DefaultValue(AxisValueType.Number)]
        public AxisValueType ValueType
        {
            get
            {
                return this._ValueType;
            }
            set
            {
                if (value != this._ValueType)
                {
                    this._MinValue = null;
                    this._MaxValue = null;
                    this._ValueType = value;
                    refreshAxisLabelSize();
                }
            }
        }
        private bool _AutoSideMargin = true;
        [DefaultValue(true)]
        public bool AutoSideMargin
        {
            get
            {
                return this._AutoSideMargin;
            }
            set
            {
                this._AutoSideMargin = value;
                refreshAxisLabelSize();
            }
        }
        private object _MinValue = 0;
        [TypeConverter(typeof(System.ComponentModel.StringConverter))]
        public object MinValue
        {
            get
            {
                switch (this.ValueType)
                {
                    case AxisValueType.Argument: return _MinValue == null ? _MinValue = "" : _MinValue;
                    case AxisValueType.DateTime: return _MinValue == null ? _MinValue = DateTime.Now : (DateTime)_MinValue;
                    case AxisValueType.Number: return _MinValue == null ? _MinValue = 0 : Convert.ToDouble(this._MinValue);
                    default: return _MinValue;
                }
            }
            set
            {
                switch (this.ValueType)
                {
                    case AxisValueType.DateTime:
                        {
                            DateTime time;
                            if (DateTime.TryParse(value.ToString(), out time) && (DateTime)MinValue != time)
                            {
                                _MinValue = time;
                                refreshAxisLabelSize();
                            }
                            break;
                        }
                    case AxisValueType.Number:
                        {
                            double result;
                            if (double.TryParse(value.ToString(), out result) && (double)MinValue != result)
                            {
                                _MinValue = result;
                                refreshAxisLabelSize();
                            }
                            break;
                        }
                    default:
                        {
                            if (this.MinValue.ToString() != value.ToString())
                            {
                                this._MinValue = value;
                                refreshAxisLabelSize();
                            } 
                            break;
                        }
                }
            }
        }
        private object _MaxValue = 10;
        [TypeConverter(typeof(System.ComponentModel.StringConverter))]
        public object MaxValue
        {
            get
            {
                switch (this.ValueType)
                {
                    case AxisValueType.Argument: return _MaxValue == null ? _MaxValue = "" : _MaxValue.ToString();
                    case AxisValueType.DateTime: return _MaxValue == null ? _MaxValue = ((DateTime)this.MinValue).AddMinutes(10) : (DateTime)_MaxValue;
                    case AxisValueType.Number: return _MaxValue == null ? _MaxValue = 0 : Convert.ToDouble(this._MaxValue);
                    default: return _MaxValue;
                }
            }
            set
            {
                switch (this.ValueType)
                {
                    case AxisValueType.DateTime:
                        {
                            DateTime time;
                            if (DateTime.TryParse(value.ToString(), out time) && (DateTime)MaxValue != time)
                            {
                                _MaxValue = time;
                                refreshAxisLabelSize();
                            }
                            break;
                        }
                    case AxisValueType.Number:
                        {
                            double result;
                            if (double.TryParse(value.ToString(), out result) && (double)MaxValue != result)
                            {
                                _MaxValue = result;
                                refreshAxisLabelSize();
                            }
                            break;
                        }
                    default:
                        {
                            if (this.MaxValue.ToString() != value.ToString())
                            {
                                this._MaxValue = value;
                                refreshAxisLabelSize();
                            }
                            break;
                        }
                }
                refreshAxisLabelSize();
            }
        }
        private double _SideMargin = 0;
        [DefaultValue(0)]
        public double SideMargin
        {
            get
            {
                return this._SideMargin;
            }
            set
            {
                this._SideMargin = value;
            }
        }
        public Axis(Control parent)
        {
            this.Parent = parent;
            this.Font = new Font(parent.Font, FontStyle.Regular);
        }
        public event EventHandler AxisViewChanged;
        private void refreshAxisLabelSize()
        {
            switch (this.ValueType)
            {
                case AxisValueType.Number:
                    {
                        var g = Parent.CreateGraphics();
                        var range = ((double)this.MaxValue - (double)this.MinValue);
                        var minsize = g.MeasureString(this.MinValue.ToString(), this.Font);
                        var maxsize = g.MeasureString(this.MaxValue.ToString(), this.Font);
                        if(range<0)
                            throw new Exception("最大值小于或等于最小值");
                        if(range==0)
                        {
                            this.AxisLabelPer = 0;
                            this.ThickCount = 1;
                            this.AxisLabelWidth = (int)Math.Max(minsize.Width, maxsize.Width);
                            this.AxisLabelHeight = (int)Math.Max(minsize.Height, maxsize.Height);
                            if(this.SideMargin==0)
                                this.SideMargin = 1;
                            break;
                        }
                        if (this.AutoSideMargin)
                            SideMargin = 0;
                        int pc = (int)Math.Floor(Math.Log10(range));
                        int e = (int)(range * Math.Pow(10, -pc));
                        while (e < 8)
                        {
                            pc--;
                            e = (int)(range * Math.Pow(10, -pc));
                        }
                        this.AxisLabelPer = (double)(e / 14 + 1) * Math.Pow(10, pc);
                        this.ThickCount = (int)(range / this.AxisLabelPer);
                        var perf = (double)this.MinValue + (double)this.AxisLabelPer;
                        var persize = g.MeasureString((perf).ToString(), this.Font);
                        this.AxisLabelWidth = (int)Math.Max(Math.Max(minsize.Width, maxsize.Width), persize.Width);
                        this.AxisLabelHeight = (int)Math.Max(Math.Max(minsize.Height, maxsize.Height), persize.Height);
                        break;
                    }
                case AxisValueType.DateTime:
                    {
                        var g = Parent.CreateGraphics();
                        this.AxisLabelPer = ((DateTime)this.MaxValue - (DateTime)this.MinValue).TotalMilliseconds / 10;
                        var minsize = g.MeasureString(this.MinValue.ToString(), this.Font);
                        var maxsize = g.MeasureString(this.MaxValue.ToString(), this.Font);
                        var perf = ((DateTime)this.MinValue).AddMilliseconds(this.AxisLabelPer);
                        var persize = g.MeasureString(perf.ToString(), this.Font);
                        this.AxisLabelWidth = (int)Math.Max(Math.Max(minsize.Width, maxsize.Width), persize.Width);
                        this.AxisLabelHeight = (int)Math.Max(Math.Max(minsize.Height, maxsize.Height), persize.Height);
                        break;
                    }
                case AxisValueType.Argument:
                    {
                        var g = Parent.CreateGraphics();
                        var minsize = g.MeasureString(this.MinValue.ToString(), this.Font);
                        var maxsize = g.MeasureString(this.MaxValue.ToString(), this.Font);
                        this.AxisLabelWidth = (int)Math.Max(minsize.Width, maxsize.Width);
                        this.AxisLabelHeight = (int)Math.Max(minsize.Height, maxsize.Height);
                        break;
                    }
            }
            if (this.AxisViewChanged != null)
                this.AxisViewChanged(this, new EventArgs());
        }
    }
    public abstract class GraghPiece
    {
        private double _MinX = double.MaxValue;
        public double MinX
        {
            get
            {
                return _MinX;
            }
            private set
            {
                if(_MinX!=value)
                {
                    this._MinX = value;
                }
            }
        }
        private double _MinY = double.MaxValue;
        public double MinY
        {
            get
            {
                return _MinY;
            }
            private set
            {
                if(_MinY!=value)
                {
                    this._MinY = value;
                }
            }
        }
        private double _MaxX = double.MinValue;
        public double MaxX
        {
            get
            {
                return _MaxX;
            }
            private set
            {
                if(_MaxX!=value)
                {
                    this._MaxX = value;
                }
            }
        }
        private double _MaxY = double.MinValue;
        public double MaxY
        {
            get
            {
                return _MaxY;
            }
            private set
            {
                if(_MaxY!=value)
                {
                    this._MaxY = value;
                }
            }
        }
        private List<SPoint> _Points = new List<SPoint>();
        public int PointsCount
        {
            get
            {
                return this._Points.Count;
            }
        }
        public SPoint PointAt(int index)
        {
            return this._Points[index];
        }
        public void AddPoint(SPoint p)
        {
            this._Points.Add(p);
            p.ValueChanged -= PointValueChanged;
            p.ValueChanged += PointValueChanged;
            this.appendBound(p);
        }
        private void PointValueChanged(object sender, EventArgs e)
        {
            this.appendBound(sender as SPoint);
        }
        public void DeletePoint(SPoint p)
        {
            this._Points.Remove(p);
            p.ValueChanged -= PointValueChanged;
            refreshBound();
        }
        public void ClearPoint()
        {
            foreach(var p in this._Points)
            {
                this._Points.Remove(p);
                p.ValueChanged -= PointValueChanged;
            }
            resetBound();
        }
        private void appendBound(SPoint p)
        {
            if (p.X > this.MaxX)
                this.MaxX = p.X;
            if (p.X < this.MinX)
                this.MinX = p.X;
            if (p.Y > this.MaxY)
                this.MaxY = p.Y;
            if (p.Y < this.MinY)
                this.MinY = p.Y;
        }
        private void resetBound()
        {
            this.MaxX = double.MinValue;
            this.MaxY = double.MinValue;
            this.MinX = double.MaxValue;
            this.MinY = double.MaxValue;
        }
        private void refreshBound()
        {
            resetBound();
            foreach (var p in _Points)
            {
                appendBound(p);
            }
        }
        protected Color _DrawColor;
        public Color DrawColor
        {
            get
            {
                return this._DrawColor;
            }
            set
            {
                this._DrawColor = value;
                this.brush = DBrushs.GetBrushFromColor(value);
                this.pen = DPens.GetPenFromColor(value);
            }
        }
        protected SolidBrush brush;
        protected Pen pen;
        public abstract void Draw(Graphics g, IDiagram drawboard);
        public GraghPiece(Color drawcolor)
        {
            this.DrawColor = drawcolor;
        }
        public GraghPiece()
        {
            Random maker = new Random();
            this.DrawColor = Color.FromArgb(maker.Next(100)+155,maker.Next(255),maker.Next(255),maker.Next(255));
        }
    }
    public interface IDiagram
    {
        Point PointToDiagram(double x, double y);
        Point PointToDiagram(string x, double y);
        Point PointToDiagram(DateTime x, double y);
        Point PointToDiagram(SPoint p);
    }
    public enum FillType
    {
        Fill,
        Edging
    }
    public class Polygon : GraghPiece
    {
        public FillType DrawType = FillType.Edging;
        public override void Draw(Graphics g, IDiagram drawboard)
        {
            Point[] ps = new Point[PointsCount];
            int i;
            for (i = 0; i < PointsCount;i++ )
            {
                ps[i] = drawboard.PointToDiagram(PointAt(i));
            }
            if (ps.Length >1)
            {
                switch (DrawType)
                {
                    case FillType.Fill: g.FillPolygon(brush, ps); break;
                    case FillType.Edging: g.DrawPolygon(pen, ps); break;
                }
            }
        }
    }
    public class SPoint
    {
        public event EventHandler ValueChanged;
        private double _X;
        public double X
        {
            get
            {
                return this._X;
            }
            set
            {
                if (_X != value)
                {
                    this._X = value;
                    if (this.ValueChanged != null)
                        this.ValueChanged(this, new EventArgs());
                }
            }
        }
        private double _Y;
        public double Y
        {
            get
            {
                return this._Y;
            }
            set
            {
                if (_Y != value)
                {
                    this._Y = value;
                    if (this.ValueChanged != null)
                        this.ValueChanged(this, new EventArgs());
                }
            }
        }
        public SPoint(double x, double y)
        {
            this._X = x;
            this._Y = y;
        }
    }
    public static class DPens
    {
        private static Dictionary<Color, Pen> pens = new Dictionary<Color, Pen>();
        public static Pen GetPenFromColor(Color color)
        {
            Pen result;
            if (!pens.TryGetValue(color, out result))
                pens.Add(color, result = new Pen(color));
            return result;
        }
        public static bool Clear()
        {
            pens.Clear();
            return true;
        }
        public static bool Remove(Color color)
        {
            return pens.Remove(color);
        }
    }
    public static class DBrushs
    {
        private static Dictionary<Color, SolidBrush> brushs = new Dictionary<Color, SolidBrush>();
        public static SolidBrush GetBrushFromColor(Color color)
        {
            SolidBrush result;
            if (!brushs.TryGetValue(color, out result))
                brushs.Add(color, result = new SolidBrush(color));
            return result;
        }
        public static bool Clear()
        {
            brushs.Clear();
            return true;
        }
        public static bool Remove(Color color)
        {
            return brushs.Remove(color);
        }
    }
    public class ItemAddedEventArgs<T> : EventArgs
    {
        public T Item;
        public ItemAddedEventArgs(T item)
        {
            Item = item;
        }
    }
    public class ItemChangedEventArgs<T> : EventArgs
    {
        public T PreItem;       
        public T NewItem;
        public ItemChangedEventArgs(T pitem,T nitem)
        {
            PreItem = pitem;
            NewItem = nitem;
        }
    }
    public class GraphCollection : IList<GraghPiece>, ICollection<GraghPiece>, IEnumerable<GraghPiece>, IEnumerable
    {
        private double _MinX = double.MaxValue;
        public double MinX
        {
            get
            {
                return _MinX;
            }
            private set
            {
                if (_MinX != value)
                {
                    this._MinX = value;
                }
            }
        }
        private double _MinY = double.MaxValue;
        public double MinY
        {
            get
            {
                return _MinY;
            }
            private set
            {
                if (_MinY != value)
                {
                    this._MinY = value;
                }
            }
        }
        private double _MaxX = double.MinValue;
        public double MaxX
        {
            get
            {
                return _MaxX;
            }
            private set
            {
                if (_MaxX != value)
                {
                    this._MaxX = value;
                }
            }
        }
        private double _MaxY = double.MinValue;
        public double MaxY
        {
            get
            {
                return _MaxY;
            }
            private set
            {
                if (_MaxY != value)
                {
                    this._MaxY = value;
                }
            }
        }
        public event EventHandler BoundChanged;
        private List<GraghPiece> data = new List<GraghPiece>();
        public int IndexOf(GraghPiece item)
        {
            return data.IndexOf(item);
        }
        public void Insert(int index, GraghPiece item)
        {
            data.Insert(index, item);
            this.appendBound(item);
        }

        public void RemoveAt(int index)
        {
            var item = data[index];
            data.RemoveAt(index);
            if (item.MaxX < this._MaxX && item.MaxY < this._MaxY && item.MinX > this.MinX && item.MinY > this.MinY)
                return;
            this.refreshBound();
        }

        public GraghPiece this[int index]
        {
            get
            {
                return data[index];
            }
            set
            {
                data[index] = value;
            }
        }
        public void Add(GraghPiece item)
        {
            data.Add(item);
            this.appendBound(item);
        }
        public void Clear()
        {
            data.Clear();
            this.resetBound();
        }

        public bool Contains(GraghPiece item)
        {
            return data.Contains(item);
        }

        public void CopyTo(GraghPiece[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return data.Count; }
        }
        public bool Remove(GraghPiece item)
        {
            var result = data.Remove(item);
            if (item.MaxX < this._MaxX && item.MaxY < this._MaxY && item.MinX > this.MinX && item.MinY > this.MinY)
                return result;
            this.refreshBound();
            return result;
        }

        public IEnumerator<GraghPiece> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.AsEnumerable<GraghPiece>().GetEnumerator();
        }


        public bool IsReadOnly
        {
            get { return false ; }
        }
        private void appendBound(GraghPiece gp)
        {
            bool changed = false;
            if (this._MinX > gp.MinX)
            {
                changed = true;
                this._MinX = gp.MinX;
            }
            if (this._MaxX < gp.MaxX)
            {
                changed = true;
                this._MaxX = gp.MaxX;
            }
            if (this._MinY > gp.MinY)
            {
                changed = true;
                this._MinY = gp.MinY;
            }
            if (this._MaxY < gp.MaxY)
            {
                changed = true;
                this._MaxY = gp.MaxY;
            }
            if (changed&&BoundChanged != null)
                BoundChanged(this, new EventArgs());
        }
        private void resetBound()
        {
            this._MaxX = double.MinValue;
            this._MaxY = double.MinValue;
            this._MinX = double.MaxValue;
            this._MinY = double.MaxValue;
            if (BoundChanged != null)
                BoundChanged(this, new EventArgs());
        }
        private void refreshBound()
        {
            this._MaxX = double.MinValue;
            this._MaxY = double.MinValue;
            this._MinX = double.MaxValue;
            this._MinY = double.MaxValue;
            foreach (var p in data)
            {
                appendBound(p);
            }
            if (BoundChanged != null)
                BoundChanged(this, new EventArgs());
        }
    }
}
