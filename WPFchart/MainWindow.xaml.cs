using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Mathos.Parser;

namespace WPFchart
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            inputTextBox.Focus();
        }

        private decimal xMax;
        private decimal xMin;
        private decimal yMax;
        private decimal yMin;
        private Polyline line;
        private Polyline xAxis;
        private Polyline yAxis;

        private void PlotButtonClick(object sender, RoutedEventArgs e)
        {
            CreatePlot();
        }

        private void CreatePlot()
        {
            if (!string.IsNullOrWhiteSpace(inputTextBox.Text))
            {
                if (DrawChart())
                {
                    DrawAxes();
                }
            }
        }

        private bool DrawChart()
        {
            xMin = decimal.Parse(xMinTextBox.Text);
            xMax = decimal.Parse(xMaxTextBox.Text);
            yMin = decimal.Parse(yMinTextBox.Text);
            yMax = decimal.Parse(yMaxTextBox.Text);
            var smoothness = (decimal)smoothnessUpDown.Value;
            chartCanvas.Children.Remove(line);
            line = new Polyline
            {
                Stroke = Brushes.Red,
                StrokeThickness = (double)lineWidthUpDown.Value
            };
            string input = ParseInput();
            try
            {
                for (decimal x = xMin; x < xMax + smoothness; x += smoothness)
                {
                    var parser = new MathParser();
                    parser.LocalVariables.Add("x", x);
                    var y = (double)parser.Parse(input);
                    line.Points.Add(CurvePoint(new Point((double)x, y)));
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Error");
                return false;
            }          
            chartCanvas.Children.Add(line);
            return true;
        }

        private string ParseInput()
        {
            string input;
            if (inputTextBox.Text.IndexOf("-x") == 0)
            {
                input = inputTextBox.Text.Replace("-x", "(-1)*x");
            }
            else if (inputTextBox.Text.Contains("(-x"))
            {
                input = inputTextBox.Text.Replace("(-x", "((-1)*x");
            }
            else
            {
                input = inputTextBox.Text;
            }
            return input;
        }

        private void DrawAxes()
        {
            chartCanvas.Children.Remove(xAxis);
            chartCanvas.Children.Remove(yAxis);
            if (xMax - xMin > xMax)
            {
                decimal xV = -xMin * (decimal)chartCanvas.Width / (xMax - xMin);
                yAxis = new Polyline
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.4
                };
                yAxis.Points.Add(new Point((double)xV, 0));
                yAxis.Points.Add(new Point((double)xV, chartCanvas.Height));
                chartCanvas.Children.Add(yAxis);
            }
            if (yMax - yMin > yMax)
            {
                decimal yH = -yMin * (decimal)chartCanvas.Height / (yMax - yMin);
                xAxis = new Polyline
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 0.4
                };
                xAxis.Points.Add(new Point(0, chartCanvas.Height - (double)yH));
                xAxis.Points.Add(new Point(chartCanvas.Width, chartCanvas.Height - (double)yH));
                chartCanvas.Children.Add(xAxis);
            }
        }

        private Point CurvePoint(Point pt)
        {
            return new Point
            {
                X = (pt.X - (double)xMin) * chartCanvas.Width / ((double)xMax - (double)xMin),
                Y = chartCanvas.Height
                - (pt.Y - (double)yMin) * chartCanvas.Height / ((double)yMax - (double)yMin)
            };
        }

        private void LineWidthUpDownValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CreatePlot();
        }
    }
}
