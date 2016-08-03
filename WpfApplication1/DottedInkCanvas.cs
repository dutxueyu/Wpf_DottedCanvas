using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApplication1
{
    public class DottedInkCanvas : InkCanvas
    {
        /// <summary>
        /// 控制鼠标左键是否按下标志
        /// </summary>
        private bool IsButtonDown = false;

        public static DependencyProperty IsDottedModeProperty =
            DependencyProperty.Register("IsDottedMode", typeof(bool), typeof(DottedInkCanvas), new PropertyMetadata(true));
        public bool IsDottedMode
        {
            get { return (bool)GetValue(IsDottedModeProperty); }
            set { SetValue(IsDottedModeProperty, value); }
        }
        
        public static DependencyProperty DotSizeProperty =
           DependencyProperty.Register("DotSize", typeof(int), typeof(DottedInkCanvas), new PropertyMetadata(5));
        public int DotSize
        {
            get { return (int)GetValue(DotSizeProperty); }
            set { SetValue(DotSizeProperty, value); }
        }

        public static DependencyProperty StrokeColorProperty =
          DependencyProperty.Register("StrokeColor", typeof(Color), typeof(DottedInkCanvas), new PropertyMetadata(Colors.Black));
        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }

        /// <summary>
        /// 重写stroke类，用做画虚线
        /// </summary>
        internal class PathStrokeK : Stroke

        {

            private static double[] dashs = new double[] { 5.0, 3.0 };

            private static double Thickness = 3.0;

            public PathStrokeK(StylusPointCollection spc) : base(spc)
            {
            }
            public void AddPoints(IEnumerable<StylusPoint> stylusPoints)

            {
                foreach (StylusPoint point in stylusPoints)
                {
                    base.StylusPoints.Add(point);
                }
            }

            protected override void DrawCore(DrawingContext dc, DrawingAttributes da)

            {

                Brush brush = new SolidColorBrush(da.Color);

                StylusPointCollection stylusPoints = base.StylusPoints;

                if ((dashs == null) || (dashs.Length == 0))

                {

                    base.DrawCore(dc, da);

                }

                else if (stylusPoints.Count > 0)

                {

                    Pen pen = new Pen

                    {

                        Brush = brush,

                        Thickness = Thickness,

                        DashStyle = new DashStyle(dashs, 0.0),

                        DashCap = PenLineCap.Round,

                        LineJoin = PenLineJoin.Round,

                        MiterLimit = 0.0

                    };

                    PathGeometry geometry = new PathGeometry();

                    PathFigure figure = new PathFigure

                    {

                        StartPoint = (Point)stylusPoints[0],

                        IsClosed = false

                    };

                    for (int i = 1; i < stylusPoints.Count; i++)

                    {

                        figure.Segments.Add(new LineSegment((Point)stylusPoints[i], true));

                    }

                    geometry.Figures.Add(figure);

                    dc.DrawGeometry(null, pen, geometry);

                    dc.DrawGeometry(brush, null, new EllipseGeometry((Point)stylusPoints[0], Thickness / 2.0, Thickness / 2.0));

                    dc.DrawGeometry(brush, null, new EllipseGeometry((Point)stylusPoints[stylusPoints.Count - 1], Thickness / 2.0, Thickness / 2.0));

                }
            }
        }

        PathStrokeK stroke;
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            List<Point> list = new List<Point>();
            list.Add(e.GetPosition(this));
            stroke = new PathStrokeK(new StylusPointCollection(list));
            stroke.DrawingAttributes = new DrawingAttributes() { Color = StrokeColor, Height = this.DotSize, Width = this.DotSize};
           
            IsButtonDown = true;
            if (IsDottedMode)
            {
                this.Strokes.Add(stroke);
                this.DefaultDrawingAttributes = new DrawingAttributes() { Color = Colors.Transparent};
            }
           
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsButtonDown)
            {
                stroke.AddPoints(new StylusPointCollection(new List<Point> { e.GetPosition(this) }));
            }
            else
            {
                this.DefaultDrawingAttributes = new DrawingAttributes() { Color = StrokeColor, Height = this.DotSize, Width = this.DotSize };
            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            IsButtonDown = false;

        }
    }
}
