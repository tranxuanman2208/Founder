using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Windows.Media.Imaging;

namespace WpfMapDijkstra.Views
{
    public partial class BasicPage : UserControl
    {
        enum Mode { None, AddNode, Connect, SetStart, SetEnd, DeleteNode, DeleteEdge }
        Mode currentMode = Mode.None;

        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();

        Node startNode = null;
        Node endNode = null;

        Node tempA = null;

        private double zoomFactor = 1.0;

        // Pan
        private Point lastMousePos;
        private bool isPanning = false;

        public BasicPage()
        {
            InitializeComponent();
            MapCanvas.MouseLeftButtonDown += MapCanvas_MouseLeftButtonDown;
            MapCanvas.MouseMove += MapCanvas_MouseMove;
            MapCanvas.MouseLeftButtonUp += MapCanvas_MouseLeftButtonUp;
            MapCanvas.MouseWheel += MapCanvas_MouseWheel;
        }

        private void btnChangeBackground_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bitmap = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                ImageBrush brush = new ImageBrush(bitmap)
                {
                    Stretch = Stretch.Uniform 
                };

                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new ScaleTransform(zoomFactor, zoomFactor));
                tg.Children.Add(new TranslateTransform(0, 0));
                brush.Transform = tg;

                MapCanvas.Background = brush;
                txtInfo.Text = "Đã thay đổi bản đồ: " + System.IO.Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor += 0.1;
            ApplyBackgroundZoom();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            zoomFactor -= 0.1;
            if (zoomFactor < 0.1) zoomFactor = 0.1;
            ApplyBackgroundZoom();
        }

        private void ApplyBackgroundZoom()
        {
            if (MapCanvas.Background is ImageBrush brush && brush.Transform is TransformGroup tg)
            {
                var st = tg.Children.OfType<ScaleTransform>().FirstOrDefault();
                if (st != null)
                {
                    st.ScaleX = zoomFactor;
                    st.ScaleY = zoomFactor;
                }
            }
        }

        private void MapCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MapCanvas.Background is ImageBrush brush && brush.Transform is TransformGroup tg)
            {
                var st = tg.Children.OfType<ScaleTransform>().FirstOrDefault();
                var tt = tg.Children.OfType<TranslateTransform>().FirstOrDefault();
                if (st != null && tt != null)
                {
                    Point mousePos = e.GetPosition(MapCanvas);
                    double oldZoom = zoomFactor;
                    zoomFactor += e.Delta > 0 ? 0.1 : -0.1;
                    if (zoomFactor < 0.1) zoomFactor = 0.1;

                    double scaleChange = zoomFactor / oldZoom;
                    tt.X = (tt.X - mousePos.X) * scaleChange + mousePos.X;
                    tt.Y = (tt.Y - mousePos.Y) * scaleChange + mousePos.Y;
                    st.ScaleX = zoomFactor;
                    st.ScaleY = zoomFactor;
                }
            }
        }

        private void MapCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            lastMousePos = e.GetPosition(MapCanvas);

            if (currentMode == Mode.None)
            {
                isPanning = true;
                MapCanvas.CaptureMouse();
            }
            else
            {
                HandleCanvasClick(lastMousePos);
            }
        }

        private void MapCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning && MapCanvas.Background is ImageBrush brush && brush.Transform is TransformGroup tg)
            {
                var tt = tg.Children.OfType<TranslateTransform>().FirstOrDefault();
                if (tt != null)
                {
                    Point pos = e.GetPosition(MapCanvas);
                    Vector delta = pos - lastMousePos;
                    tt.X += delta.X;
                    tt.Y += delta.Y;
                    lastMousePos = pos;
                }
            }
        }

        private void MapCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isPanning)
            {
                isPanning = false;
                MapCanvas.ReleaseMouseCapture();
            }
        }

        private void HandleCanvasClick(Point p)
        {
            if (currentMode == Mode.AddNode)
            {
                AddNode(p);
                return;
            }

            Node n = GetNodeAtPoint(p);
            if (n == null) return;

            if (currentMode == Mode.SetStart)
            {
                startNode = n;
                n.Ui.Fill = Brushes.Green;
                txtInfo.Text = "Start: " + n.Name;
                return;
            }

            if (currentMode == Mode.SetEnd)
            {
                endNode = n;
                n.Ui.Fill = Brushes.Red;
                txtInfo.Text = "End: " + n.Name;
                return;
            }

            if (currentMode == Mode.DeleteNode)
            {
                DeleteNode(n);
                return;
            }

            if (currentMode == Mode.Connect || currentMode == Mode.DeleteEdge)
            {
                if (tempA == null)
                {
                    tempA = n;
                    txtInfo.Text = "Chọn đỉnh thứ 2";
                    return;
                }

                if (tempA == n) return;

                if (currentMode == Mode.Connect)
                    AddEdge(tempA, n);
                else
                    DeleteEdge(tempA, n);

                tempA = null;
            }
        }

        private void btnAddNode_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.AddNode;
            txtInfo.Text = "Chế độ: Thêm đỉnh";
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.Connect;
            txtInfo.Text = "Chế độ: Nối 2 đỉnh";
            tempA = null;
        }

        private void btnSetStart_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.SetStart;
            txtInfo.Text = "Chọn Start";
        }

        private void btnSetEnd_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.SetEnd;
            txtInfo.Text = "Chọn End";
        }

        private void btnDeleteNode_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.DeleteNode;
            txtInfo.Text = "Chế độ: Xóa đỉnh";
        }

        private void btnDeleteEdge_Click(object sender, RoutedEventArgs e)
        {
            currentMode = Mode.DeleteEdge;
            txtInfo.Text = "Chế độ: Xóa đường (chọn 2 đỉnh)";
            tempA = null;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            foreach (var n in nodes)
                n.Ui.Fill = Brushes.LightBlue;

            foreach (var ed in edges)
            {
                ed.Ui.Stroke = Brushes.Black;
                ed.Ui.StrokeThickness = 2;
            }

            txtInfo.Text = "Đã đặt lại.";
        }

        private Node AddNode(Point p)
        {
            int idx = nodes.Count + 1;

            Ellipse el = new Ellipse()
            {
                Width = 26,
                Height = 26,
                Fill = Brushes.LightBlue,
                Stroke = Brushes.DarkBlue,
                StrokeThickness = 2
            };
            MapCanvas.Children.Add(el);
            Canvas.SetLeft(el, p.X - 13);
            Canvas.SetTop(el, p.Y - 13);

            TextBlock label = new TextBlock()
            {
                Text = "Đ" + idx,
                FontWeight = FontWeights.Bold
            };
            MapCanvas.Children.Add(label);
            Canvas.SetLeft(label, p.X - 8);
            Canvas.SetTop(label, p.Y - 10);

            Node n = new Node()
            {
                Ui = el,
                Label = label,
                X = p.X,
                Y = p.Y,
                Name = "Đ" + idx
            };

            nodes.Add(n);
            return n;
        }

        private Node GetNodeAtPoint(Point p)
        {
            return nodes.FirstOrDefault(n =>
                Math.Sqrt((n.X - p.X) * (n.X - p.X) + (n.Y - p.Y) * (n.Y - p.Y)) < 15);
        }

        private void AddEdge(Node a, Node b)
        {
            if (edges.Any(e => (e.A == a && e.B == b) || (e.A == b && e.B == a)))
            {
                MessageBox.Show("Hai đỉnh đã nối");
                return;
            }

            double w = 1;
            double.TryParse(txtWeight.Text, out w);

            Line line = new Line()
            {
                X1 = a.X,
                Y1 = a.Y,
                X2 = b.X,
                Y2 = b.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            MapCanvas.Children.Add(line);

            TextBlock txt = new TextBlock()
            {
                Text = w.ToString(),
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold
            };
            MapCanvas.Children.Add(txt);
            Canvas.SetLeft(txt, (a.X + b.X) / 2);
            Canvas.SetTop(txt, (a.Y + b.Y) / 2);

            edges.Add(new Edge() { A = a, B = b, Weight = w, Ui = line, Label = txt });
        }

        private void DeleteEdge(Node a, Node b)
        {
            Edge ed = edges.FirstOrDefault(e => (e.A == a && e.B == b) || (e.A == b && e.B == a));
            if (ed == null)
            {
                MessageBox.Show("Không có đường");
                return;
            }

            MapCanvas.Children.Remove(ed.Ui);
            if (ed.Label != null)
                MapCanvas.Children.Remove(ed.Label);
            edges.Remove(ed);
        }

        private void DeleteNode(Node n)
        {
            foreach (var ed in edges.Where(e => e.A == n || e.B == n).ToList())
            {
                MapCanvas.Children.Remove(ed.Ui);
                if (ed.Label != null)
                    MapCanvas.Children.Remove(ed.Label);
                edges.Remove(ed);
            }

            MapCanvas.Children.Remove(n.Ui);
            MapCanvas.Children.Remove(n.Label);

            nodes.Remove(n);
        }

        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (startNode == null || endNode == null)
            {
                MessageBox.Show("Chưa chọn Start hoặc End");
                return;
            }

            var prev = new Dictionary<Node, Node>();
            var dist = nodes.ToDictionary(n => n, n => double.PositiveInfinity);
            dist[startNode] = 0;

            var pq = new List<Node>(nodes);

            while (pq.Any())
            {
                Node u = pq.OrderBy(n => dist[n]).First();
                pq.Remove(u);

                if (u == endNode) break;

                foreach (var edge in edges.Where(edge => edge.A == u || edge.B == u))
                {
                    Node v = (edge.A == u) ? edge.B : edge.A;
                    double alt = dist[u] + edge.Weight;

                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            foreach (var e2 in edges)
            {
                e2.Ui.Stroke = Brushes.Black;
                e2.Ui.StrokeThickness = 2;
            }

            Node cur = endNode;
            List<Node> order = new List<Node>() { cur };
            while (prev.ContainsKey(cur))
            {
                Node from = prev[cur];
                Edge ed = edges.First(edge =>
                    (edge.A == from && edge.B == cur) ||
                    (edge.A == cur && edge.B == from));

                ed.Ui.Stroke = Brushes.Blue;
                ed.Ui.StrokeThickness = 4;

                cur = from;
                order.Add(cur);
            }
            order.Reverse();
            txtOutput.Text = "Đường đi ngắn nhất:\n" + string.Join(" → ", order.Select(o => o.Label.Text));
            txtInfo.Text = "Hoàn tất";
        }

        private void btnConnectAll_Click(object sender, RoutedEventArgs e)
        {
            if (startNode == null)
            {
                MessageBox.Show("Chưa chọn Start");
                return;
            }

            foreach (var ed in edges)
            {
                ed.Ui.Stroke = Brushes.Gray;
                ed.Ui.StrokeThickness = 1;
            }

            var prev = new Dictionary<Node, Node>();
            var dist = nodes.ToDictionary(n => n, n => double.PositiveInfinity);
            dist[startNode] = 0;
            var pq = new List<Node>(nodes);

            while (pq.Any())
            {
                Node u = pq.OrderBy(n => dist[n]).First();
                pq.Remove(u);

                foreach (var edge in edges.Where(edge => edge.A == u || edge.B == u))
                {
                    Node v = (edge.A == u) ? edge.B : edge.A;
                    double alt = dist[u] + edge.Weight;

                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }

            txtOutput.Text = "Đường đi từ Start:\n";
            foreach (var n in nodes.Where(n => n != startNode))
            {
                Node cur = n;
                List<Node> path = new List<Node>() { cur };
                while (prev.ContainsKey(cur))
                {
                    Node from = prev[cur];
                    Edge ed = edges.First(edge =>
                        (edge.A == from && edge.B == cur) ||
                        (edge.A == cur && edge.B == from));
                    ed.Ui.Stroke = Brushes.Blue;
                    ed.Ui.StrokeThickness = 3;
                    cur = from;
                    path.Add(cur);
                }
                path.Reverse();
                txtOutput.Text += string.Join(" → ", path.Select(x => x.Label.Text)) + "\n";
            }

            txtInfo.Text = "Đường từ Start đến tất cả đỉnh.";
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            MapCanvas.Children.Clear();
            edges.Clear();
            nodes.Clear();
            startNode = endNode = null;
            txtOutput.Text = "";
            txtInfo.Text = "Đã xóa tất cả.";
        }
    }

    public class Node
    {
        public Ellipse Ui;
        public TextBlock Label;
        public double X, Y;
        public string Name;
    }

    public class Edge
    {
        public Node A, B;
        public double Weight;
        public Line Ui;
        public TextBlock Label;
    }
}
