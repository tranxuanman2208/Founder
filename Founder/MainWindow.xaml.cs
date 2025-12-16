using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MapShortestPath
{
    public partial class MainWindow : Window
    {
        // Các chế độ chương trình
        // None        : không làm gì (chế độ mặc định → dùng để kéo bản đồ)
        // AddNode     : thêm đỉnh (node)
        // Connect     : nối 2 đỉnh
        // SetStart    : chọn điểm bắt đầu
        // SetEnd      : chọn điểm kết thúc
        // DeleteNode  : xóa đỉnh
        // DeleteEdge  : xóa đường nối
        enum Mode { None, AddNode, Connect, SetStart, SetEnd, DeleteNode, DeleteEdge }
        Mode currentMode = Mode.None;

        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();

        // Đỉnh Start và End
        Node startNode = null;
        Node endNode = null;
        Node tempA = null;

        public MainWindow()
        {
            InitializeComponent();
        }
        private void MapCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MapCanvas);
            HandleCanvasClick(p);
        }
        private void HandleCanvasClick(Point p)
        {
            // Thêm đỉnh
            if (currentMode == Mode.AddNode)
            {
                AddNode(p);
                return;
            }

            // Tìm xem có đỉnh nào được click không
            Node n = GetNodeAtPoint(p);
            if (n == null) return;

            // Chọn Start
            if (currentMode == Mode.SetStart)
            {
                startNode = n;
                n.Ui.Fill = Brushes.Green;
                txtInfo.Text = "Start: " + n.Name;
                return;
            }

            // Chọn End
            if (currentMode == Mode.SetEnd)
            {
                endNode = n;
                n.Ui.Fill = Brushes.Red;
                txtInfo.Text = "End: " + n.Name;
                return;
            }

            // Xóa đỉnh
            if (currentMode == Mode.DeleteNode)
            {
                DeleteNode(n);
                return;
            }

            // Nối hoặc xóa đường
            if (currentMode == Mode.Connect || currentMode == Mode.DeleteEdge)
            {
                // Chưa chọn đỉnh đầu tiên
                if (tempA == null)
                {
                    tempA = n;
                    txtInfo.Text = "Chọn đỉnh thứ 2";
                    return;
                }

                if (tempA == n) return; // Tránh bấm lại cùng đỉnh

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

            // Vẽ hình tròn
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

            // Vẽ label
            TextBlock label = new TextBlock()
            {
                Text = "Đ" + idx,
                FontWeight = FontWeights.Bold
            };
            MapCanvas.Children.Add(label);
            Canvas.SetLeft(label, p.X - 8);
            Canvas.SetTop(label, p.Y - 10);

            // Tạo và lưu Node
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
                Math.Sqrt((n.X - p.X) * (n.X - p.X) +
                          (n.Y - p.Y) * (n.Y - p.Y)) < 15);
        }
        private void AddEdge(Node a, Node b)
        {
            // Tránh nối trùng
            if (edges.Any(e => (e.A == a && e.B == b) ||
                               (e.A == b && e.B == a)))
            {
                MessageBox.Show("Hai đỉnh đã nối");
                return;
            }

            // Lấy trọng số
            double w = Math.Sqrt(Math.Pow((a.X-b.X),2) + Math.Pow((a.Y - b.Y),2));
            w = Math.Round(Math.Sqrt(w)/(double)10, 1);

            // Vẽ đường thẳng
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

            // Vẽ label trọng số
            TextBlock txt = new TextBlock()
            {
                Text = w.ToString(),
                Foreground = Brushes.Red,
                FontWeight = FontWeights.Bold
            };
            MapCanvas.Children.Add(txt);
            Canvas.SetLeft(txt, (a.X + b.X) / 2);
            Canvas.SetTop(txt, (a.Y + b.Y) / 2);

            edges.Add(new Edge()
            {
                A = a,
                B = b,
                Weight = w,
                Ui = line,
                Label = txt
            });
        }

        private void DeleteEdge(Node a, Node b)
        {
            Edge ed = edges.FirstOrDefault(e =>
                (e.A == a && e.B == b) ||
                (e.A == b && e.B == a));

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
            // Xóa tất cả cạnh liên quan
            foreach (var ed in edges.Where(e => e.A == n || e.B == n).ToList())
            {
                MapCanvas.Children.Remove(ed.Ui);
                if (ed.Label != null)
                    MapCanvas.Children.Remove(ed.Label);
                edges.Remove(ed);
            }

            // Xóa hình node + label
            MapCanvas.Children.Remove(n.Ui);
            MapCanvas.Children.Remove(n.Label);

            nodes.Remove(n);
        }
        
        //Tìm đường theo Dijkstra
        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            if (startNode == null || endNode == null)
            {
                MessageBox.Show("Chưa chọn Start hoặc End");
                return;
            }

            // prev : lưu đường đi
            // dist : lưu khoảng cách từ Start
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

            // Reset giao diện
            foreach (var e2 in edges)
            {
                e2.Ui.Stroke = Brushes.Black;
                e2.Ui.StrokeThickness = 2;
            }

            // Tô màu đường đi ngắn nhất
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

            txtOutput.Text = "Đường đi ngắn nhất:\n" +
                string.Join(" → ", order.Select(o => o.Label.Text));

            txtInfo.Text = "Hoàn tất";
        }

        private void btnConnectAll_Click(object sender, RoutedEventArgs e)
        {
            if (startNode == null)
            {
                MessageBox.Show("Chưa chọn Start");
                return;
            }

            // Reset giao diện
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

            // Duyệt mọi đỉnh → in đường đi
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

        // Sự kiện click chuột phải để xem chi tiết
        private void MapCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(MapCanvas);
            Node n = GetNodeAtPoint(p);

            if (n != null)
            {
                int index = nodes.IndexOf(n);
                char idChar = (char)('A' + index);
                string id = idChar.ToString();
                DetailWindow detail = new DetailWindow(id);
                detail.ShowDialog();
            }
        }
    }

    public class Node
    {
        public Ellipse Ui;    // Hình tròn vẽ trên canvas
        public TextBlock Label; // Tên hiển thị
        public double X, Y;     // Tọa độ
        public string Name;     // Định danh
    }

    public class Edge
    {
        public Node A, B;      // Hai đỉnh
        public double Weight;  // Trọng số
        public Line Ui;        // Đường thẳng vẽ trên canvas
        public TextBlock Label; // Hiển thị trọng số
    }
}
