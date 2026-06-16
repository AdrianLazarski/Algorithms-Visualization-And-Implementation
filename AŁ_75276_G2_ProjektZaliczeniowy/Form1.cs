using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AŁ_75276_G2_ProjektZaliczeniowy
{
    public class Form1 : Form
    {
        // Event handlers for view interactions
        public event EventHandler? GenerateGraphClicked;
        public event EventHandler? RunDijkstraClicked;
        public event EventHandler? SortClicked;
        public event EventHandler? CompressClicked;
        public event EventHandler? DecompressClicked;

        // UI Components
        private TabControl? _tabControl;
        private TabPage? _dijkstraTab;
        private TabPage? _sortTab;
        private TabPage? _lzwTab;
        private Panel? _drawingPanel;
        private Button? _btnGenerateGraph;
        private Button? _btnRunDijkstra;
        private Label? _lblCost;

        // Graph visual state
        private Graph? _demoGraph;
        private Dictionary<Vertex, Rectangle> _vertexVisuals = new Dictionary<Vertex, Rectangle>();
        private Dictionary<Edge, Tuple<Point, Point>> _edgeVisuals = new Dictionary<Edge, Tuple<Point, Point>>();
        private List<Vertex> _currentPath = new List<Vertex>();
        private Vertex? _currentlyAnalyzedVertex;
        private List<Vertex> _visitedVertices = new List<Vertex>();

        // Sort algorithm components
        private TextBox? _txtSortInput;
        private TextBox? _txtSortOutput;
        private Button? _btnSort;
        private Label? _lblSortTime;

        // LZW algorithm components
        private TextBox? _txtLzwInput;
        private TextBox? _txtLzwOutputCodes;
        private TextBox? _txtLzwOutputAlphabet;
        private TextBox? _txtLzwInputCodes;
        private TextBox? _txtLzwInputAlphabet;
        private TextBox? _txtLzwDecompressed;
        private Button? _btnCompress;
        private Button? _btnDecompress;

        public Form1()
        {
            Text = "Algorithms";
            Size = new Size(1000, 750);
            StartPosition = FormStartPosition.CenterScreen;

            BuildUserInterface();

            new DijkstraOrchestrator(this);
            new SortOrchestrator(this);
            new LzwOrchestrator(this);
        }

        private void BuildUserInterface()
        {
            _tabControl = new TabControl { Dock = DockStyle.Fill };
            _dijkstraTab = new TabPage { Text = "1. Dijkstra Algorithm (DEMO)" };
            _sortTab = new TabPage { Text = "2. Quick Sort" };
            _lzwTab = new TabPage { Text = "3. LZW Compression" };

            // Dijkstra Layout
            TableLayoutPanel layoutDijkstra = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            layoutDijkstra.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            layoutDijkstra.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            FlowLayoutPanel menuDijkstra = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(10) };
            Label titleDijkstra = new Label { Text = "Dijkstra Algorithm", Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = true, Margin = new Padding(0, 0, 0, 20) };

            _btnGenerateGraph = new Button { Text = "Generate Edge Weights", Size = new Size(260, 40), Margin = new Padding(0, 0, 0, 10) };
            _btnRunDijkstra = new Button { Text = "RUN DEMO", Size = new Size(260, 40), BackColor = Color.DodgerBlue, ForeColor = Color.White, Font = new Font("Arial", 10, FontStyle.Bold) };

            _btnGenerateGraph.Click += (s, e) => GenerateGraphClicked?.Invoke(this, EventArgs.Empty);
            _btnRunDijkstra.Click += (s, e) => RunDijkstraClicked?.Invoke(this, EventArgs.Empty);

            _lblCost = new Label { Text = "Optimal path cost: -", Font = new Font("Arial", 11, FontStyle.Bold), ForeColor = Color.Red, AutoSize = true, Margin = new Padding(0, 20, 0, 20) };

            Label titleLegend = new Label { Text = "Legend:", Font = new Font("Arial", 10, FontStyle.Bold), Margin = new Padding(0, 15, 0, 5) };
            Label legend1 = new Label { Text = "■ Gray - Unvisited", ForeColor = Color.DarkGray, AutoSize = true };
            Label legend2 = new Label { Text = "■ Orange - Current", ForeColor = Color.Orange, AutoSize = true };
            Label legend3 = new Label { Text = "■ Green - Visited", ForeColor = Color.Green, AutoSize = true };
            Label legend4 = new Label { Text = "■ Red - Shortest Path", ForeColor = Color.Red, AutoSize = true };

            menuDijkstra.Controls.AddRange(new Control[] { titleDijkstra, _btnGenerateGraph, _btnRunDijkstra, _lblCost, titleLegend, legend1, legend2, legend3, legend4 });

            _drawingPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(10) };
            _drawingPanel.Paint += OnPaintGraph;

            layoutDijkstra.Controls.Add(menuDijkstra, 0, 0);
            layoutDijkstra.Controls.Add(_drawingPanel, 1, 0);
            _dijkstraTab.Controls.Add(layoutDijkstra);

            // Sorting Layout
            FlowLayoutPanel layoutSort = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(30) };
            Label titleSort = new Label { Text = "Quick Sort", Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = true };
            Label descSort = new Label { Text = "Enter comma-separated numbers:", AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            _txtSortInput = new TextBox { Text = "135, 16, 19, 2, 35, 99, 87, 87, 73", Width = 500, Font = new Font("Arial", 11) };
            _btnSort = new Button { Text = "Sort", Size = new Size(150, 35), Margin = new Padding(0, 10, 0, 20) };
            Label resultDescSort = new Label { Text = "Sort result:", AutoSize = true };
            _txtSortOutput = new TextBox { ReadOnly = true, Width = 500, Font = new Font("Arial", 11), BackColor = Color.FromArgb(245, 245, 245) };
            _lblSortTime = new Label { Text = "Execution time: - ms", Font = new Font("Arial", 11, FontStyle.Bold), ForeColor = Color.DodgerBlue, Margin = new Padding(0, 15, 0, 0), AutoSize = true };

            _btnSort.Click += (s, e) => SortClicked?.Invoke(this, EventArgs.Empty);

            layoutSort.Controls.AddRange(new Control[] { titleSort, descSort, _txtSortInput, _btnSort, resultDescSort, _txtSortOutput, _lblSortTime });
            _sortTab.Controls.Add(layoutSort);

            // LZW Layout
            TableLayoutPanel layoutLzw = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(20) };
            layoutLzw.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            layoutLzw.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            FlowLayoutPanel panelCompress = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            Label titleCompress = new Label { Text = "LZW Compression", Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = true };
            Label descCompress = new Label { Text = "Source text:", AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            _txtLzwInput = new TextBox { Text = "CBCBDBABDABCACDBCBCCDCDAAABAABCADDDBAABDDAAACCADCCDAACAABDDDBDCADCBABADBDBDAABADCACBABCABABBBDAADDBDADBAADAABCCDBCBDDCAAADBBDCDCDDABDAAADADDAACCBBBBCAACABDCCDBDBDADACADAABBAADCBCAACBDDCDCCDABCCBBCDDBCDADBBCDBDCDDDCBCDBDDCBDAB", Width = 400, Multiline = true, Height = 60, Font = new Font("Arial", 11) };
            _btnCompress = new Button { Text = "Compress", Size = new Size(150, 35), Margin = new Padding(0, 10, 0, 10) };
            Label outCodesDesc = new Label { Text = "Generated codes:", AutoSize = true };
            _txtLzwOutputCodes = new TextBox { ReadOnly = true, Width = 400, Multiline = true, Height = 60, Font = new Font("Arial", 11), BackColor = Color.FromArgb(245, 245, 245) };
            Label outAlphabetDesc = new Label { Text = "Base alphabet (required for decompression):", AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            _txtLzwOutputAlphabet = new TextBox { ReadOnly = true, Width = 400, Font = new Font("Arial", 11), BackColor = Color.FromArgb(245, 245, 245) };

            _btnCompress.Click += (s, e) => CompressClicked?.Invoke(this, EventArgs.Empty);
            panelCompress.Controls.AddRange(new Control[] { titleCompress, descCompress, _txtLzwInput, _btnCompress, outCodesDesc, _txtLzwOutputCodes, outAlphabetDesc, _txtLzwOutputAlphabet });

            FlowLayoutPanel panelDecompress = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown };
            Label titleDecompress = new Label { Text = "LZW Decompression", Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = true };
            Label inCodesDesc = new Label { Text = "Input codes (comma or space separated):", AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            _txtLzwInputCodes = new TextBox { Width = 400, Multiline = true, Height = 60, Font = new Font("Arial", 11) };
            Label inAlphabetDesc = new Label { Text = "Base alphabet:", AutoSize = true, Margin = new Padding(0, 10, 0, 5) };
            _txtLzwInputAlphabet = new TextBox { Width = 400, Font = new Font("Arial", 11) };
            _btnDecompress = new Button { Text = "Decompress", Size = new Size(150, 35), Margin = new Padding(0, 10, 0, 10) };
            Label outDecompressDesc = new Label { Text = "Reconstructed text:", AutoSize = true };
            _txtLzwDecompressed = new TextBox { ReadOnly = true, Width = 400, Multiline = true, Height = 60, Font = new Font("Arial", 11), BackColor = Color.FromArgb(245, 245, 245) };

            _btnDecompress.Click += (s, e) => DecompressClicked?.Invoke(this, EventArgs.Empty);
            panelDecompress.Controls.AddRange(new Control[] { titleDecompress, inCodesDesc, _txtLzwInputCodes, inAlphabetDesc, _txtLzwInputAlphabet, _btnDecompress, outDecompressDesc, _txtLzwDecompressed });

            layoutLzw.Controls.Add(panelCompress, 0, 0);
            layoutLzw.Controls.Add(panelDecompress, 1, 0);
            _lzwTab.Controls.Add(layoutLzw);

            _tabControl.TabPages.AddRange(new TabPage[] { _dijkstraTab, _sortTab, _lzwTab });
            Controls.Add(_tabControl);
        }

        public void SetGraphForDrawing(Graph graph)
        {
            _demoGraph = graph;
            _vertexVisuals.Clear();
            _edgeVisuals.Clear();
            _currentPath.Clear();
            _visitedVertices.Clear();
            _currentlyAnalyzedVertex = null;

            if (_lblCost != null) _lblCost.Text = "Optimal path cost: -";

            foreach (var vertex in graph.GetVertices())
            {
                _vertexVisuals.Add(vertex, new Rectangle((int)vertex.X, (int)vertex.Y, 50, 40));
            }

            foreach (var edge in graph.GetEdges())
            {
                var startPoint = new Point((int)edge.Source.X + 25, (int)edge.Source.Y + 20);
                var endPoint = new Point((int)edge.Target.X + 25, (int)edge.Target.Y + 20);
                _edgeVisuals.Add(edge, new Tuple<Point, Point>(startPoint, endPoint));
            }
            _drawingPanel?.Invalidate();
        }

        public void ResetAlgorithmState()
        {
            _currentPath.Clear();
            _visitedVertices.Clear();
            _currentlyAnalyzedVertex = null;
            _drawingPanel?.Invalidate();
        }

        public void SetCostText(string text)
        {
            if (_lblCost != null) _lblCost.Text = text;
        }

        public void UpdateDijkstraState(Vertex? current, List<Vertex> visited, List<Vertex> currentPath)
        {
            _currentlyAnalyzedVertex = current;
            _visitedVertices = new List<Vertex>(visited);
            _currentPath = new List<Vertex>(currentPath);
            _drawingPanel?.Invalidate();
        }

        public void ShowDijkstraResult(List<Vertex>? path, int cost, string report)
        {
            if (path != null)
            {
                _currentPath = path;
                _currentlyAnalyzedVertex = null;
                SetCostText($"Optimal path cost: {cost}");
                _drawingPanel?.Invalidate();
                MessageBox.Show(report, "Final Report - Dijkstra", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                SetCostText("Optimal path cost: unreachable");
                ShowError("Path not found.");
            }
        }

        public string GetSortInput() => _txtSortInput?.Text ?? string.Empty;
        public void SetSortOutput(string result) { if (_txtSortOutput != null) _txtSortOutput.Text = result; }
        public void SetSortTime(string time) { if (_lblSortTime != null) _lblSortTime.Text = time; }

        public string GetCompressionInput() => _txtLzwInput?.Text ?? string.Empty;
        public void SetCompressionOutput(string codes, string alphabet)
        {
            if (_txtLzwOutputCodes != null) _txtLzwOutputCodes.Text = codes;
            if (_txtLzwOutputAlphabet != null) _txtLzwOutputAlphabet.Text = alphabet;
            if (_txtLzwInputCodes != null) _txtLzwInputCodes.Text = codes;
            if (_txtLzwInputAlphabet != null) _txtLzwInputAlphabet.Text = alphabet;
        }

        public string GetDecompressionCodes() => _txtLzwInputCodes?.Text ?? string.Empty;
        public string GetDecompressionAlphabet() => _txtLzwInputAlphabet?.Text ?? string.Empty;
        public void SetDecompressionOutput(string result) { if (_txtLzwDecompressed != null) _txtLzwDecompressed.Text = result; }

        public void ShowError(string message) => MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private void OnPaintGraph(object? sender, PaintEventArgs e)
        {
            if (_drawingPanel == null) return;
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            foreach (var pair in _edgeVisuals)
            {
                Edge edge = pair.Key;
                Point p1 = pair.Value.Item1;
                Point p2 = pair.Value.Item2;
                Pen pen = new Pen(Color.DarkGray, 2);

                if (_currentPath.Count > 0)
                {
                    for (int i = 0; i < _currentPath.Count - 1; i++)
                    {
                        if ((_currentPath[i] == edge.Source && _currentPath[i + 1] == edge.Target) ||
                            (_currentPath[i] == edge.Target && _currentPath[i + 1] == edge.Source))
                        {
                            pen = new Pen(Color.Red, 4);
                            break;
                        }
                    }
                }

                graphics.DrawLine(pen, p1, p2);
                string weightText = edge.Weight.ToString();
                Font weightFont = new Font("Arial", 10, FontStyle.Bold);
                Point textPoint = new Point((p1.X + p2.X) / 2 - 5, (p1.Y + p2.Y) / 2 - 8);
                graphics.DrawString(weightText, weightFont, Brushes.Blue, textPoint);
                pen.Dispose();
            }

            foreach (var pair in _vertexVisuals)
            {
                Vertex vertex = pair.Key;
                Rectangle rect = pair.Value;
                Brush brush = Brushes.LightGray;

                if (vertex == _currentlyAnalyzedVertex)
                {
                    brush = Brushes.Orange;
                }
                else if (_currentPath.Contains(vertex))
                {
                    brush = Brushes.Red;
                }
                else if (_visitedVertices.Contains(vertex))
                {
                    brush = Brushes.LightGreen;
                }

                graphics.FillEllipse(brush, rect);
                graphics.DrawEllipse(Pens.Black, rect);

                Font vertexFont = new Font("Arial", 9, FontStyle.Bold);
                StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                graphics.DrawString(vertex.Name, vertexFont, Brushes.Black, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), format);
            }
        }
    }

    /// <summary>
    /// Represents a single point in the graph architecture.
    /// </summary>
    public class Vertex
    {
        public string Name { get; }
        public double X { get; }
        public double Y { get; }

        public Vertex(string name, double x, double y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Represents a directed physical connection between two vertices.
    /// </summary>
    public class Edge
    {
        public Vertex Source { get; }
        public Vertex Target { get; }
        public int Weight { get; }

        public Edge(Vertex source, Vertex target, int weight)
        {
            Source = source;
            Target = target;
            Weight = weight;
        }
    }

    /// <summary>
    /// Central repository aggregating network topology.
    /// </summary>
    public class Graph
    {
        private readonly List<Vertex> _vertices = new List<Vertex>();
        private readonly List<Edge> _edges = new List<Edge>();

        public void AddVertex(Vertex v) => _vertices.Add(v);

        public void AddEdge(Vertex source, Vertex target, int weight) =>
            _edges.Add(new Edge(source, target, weight));

        public List<Vertex> GetVertices() => _vertices;
        public List<Edge> GetEdges() => _edges;

        public List<Edge> GetNeighbors(Vertex v) =>
            _edges.Where(e => e.Source == v || e.Target == v).ToList();
    }

    public class GraphGenerator
    {
        private readonly Random _random = new Random();

        public Graph GenerateLayeredGraph()
        {
            var graph = new Graph();

            var start = new Vertex("START", 30, 200);
            var end = new Vertex("META", 550, 200);
            var a = new Vertex("A", 180, 80);
            var b = new Vertex("B", 180, 320);
            var c = new Vertex("C", 380, 50);
            var d = new Vertex("D", 380, 200);
            var e = new Vertex("E", 380, 350);

            graph.AddVertex(start);
            graph.AddVertex(a);
            graph.AddVertex(b);
            graph.AddVertex(c);
            graph.AddVertex(d);
            graph.AddVertex(e);
            graph.AddVertex(end);

            graph.AddEdge(start, a, _random.Next(10, 35));
            graph.AddEdge(start, b, _random.Next(10, 35));
            graph.AddEdge(a, c, _random.Next(5, 25));
            graph.AddEdge(a, d, _random.Next(20, 40));
            graph.AddEdge(b, d, _random.Next(20, 40));
            graph.AddEdge(b, e, _random.Next(5, 25));
            graph.AddEdge(c, end, _random.Next(10, 30));
            graph.AddEdge(d, end, _random.Next(15, 35));
            graph.AddEdge(e, end, _random.Next(10, 30));

            return graph;
        }
    }

    public class DijkstraStepEventArgs : EventArgs
    {
        public Vertex CurrentVertex { get; }
        public List<Vertex> VisitedVertices { get; }
        public List<Vertex> CurrentPath { get; }

        public DijkstraStepEventArgs(Vertex current, List<Vertex> visited, List<Vertex> path)
        {
            CurrentVertex = current;
            VisitedVertices = visited;
            CurrentPath = path;
        }
    }

    public class DijkstraResult
    {
        public List<Vertex>? Path { get; }
        public int TotalCost { get; }
        public List<string> AlternativePaths { get; }

        public DijkstraResult(List<Vertex>? path, int cost, List<string> alternatives)
        {
            Path = path;
            TotalCost = cost;
            AlternativePaths = alternatives;
        }
    }

    /// <summary>
    /// Executes Dijkstra's shortest path algorithm asynchronously to allow UI updates.
    /// </summary>
    public class DijkstraAlgorithm
    {
        public event EventHandler<DijkstraStepEventArgs>? StepCompleted;

        public async Task<DijkstraResult> FindShortestPathAsync(Graph graph, Vertex source, Vertex target)
        {
            var distances = new Dictionary<Vertex, int>();
            var previousNodes = new Dictionary<Vertex, Vertex?>();
            var unvisited = new List<Vertex>();
            var visited = new List<Vertex>();
            var alternativesReport = new List<string>();

            foreach (var v in graph.GetVertices())
            {
                distances[v] = int.MaxValue;
                previousNodes[v] = null;
                unvisited.Add(v);
            }
            distances[source] = 0;

            while (unvisited.Count > 0)
            {
                unvisited.Sort((w1, w2) => distances[w1].CompareTo(distances[w2]));
                var current = unvisited[0];
                unvisited.Remove(current);

                if (distances[current] == int.MaxValue) break;

                var tempPath = new List<Vertex>();
                var tempVertex = current;
                while (tempVertex != null && previousNodes.ContainsKey(tempVertex))
                {
                    tempPath.Add(tempVertex);
                    tempVertex = previousNodes[tempVertex];
                }
                if (tempPath.Count > 0 || tempVertex == source) tempPath.Add(source);
                tempPath.Reverse();

                visited.Add(current);
                StepCompleted?.Invoke(this, new DijkstraStepEventArgs(current, visited, tempPath));

                // Intentional delay for UI visualization
                await Task.Delay(1000);

                if (current == target) break;

                foreach (var edge in graph.GetNeighbors(current))
                {
                    var neighbor = (edge.Source == current) ? edge.Target : edge.Source;
                    if (!unvisited.Contains(neighbor)) continue;

                    int potentialDistance = distances[current] + edge.Weight;

                    if (potentialDistance < distances[neighbor])
                    {
                        distances[neighbor] = potentialDistance;
                        previousNodes[neighbor] = current;
                    }
                    else
                    {
                        alternativesReport.Add($"Path to {neighbor.Name} via {current.Name} rejected (Proposed: {potentialDistance} >= Current: {distances[neighbor]})");
                    }
                }
            }

            var finalPath = new List<Vertex>();
            var backtrackPointer = target;

            while (backtrackPointer != null && previousNodes.ContainsKey(backtrackPointer))
            {
                finalPath.Add(backtrackPointer);
                backtrackPointer = previousNodes[backtrackPointer];
            }

            if (finalPath.Count > 0 || backtrackPointer == source) finalPath.Add(source);
            finalPath.Reverse();

            int finalCost = distances[target] == int.MaxValue ? 0 : distances[target];
            return new DijkstraResult(finalPath.Count > 1 ? finalPath : null, finalCost, alternativesReport);
        }
    }

    public interface ISorter
    {
        void Sort(int[] array);
    }

    /// <summary>
    /// In-place QuickSort implementation.
    /// </summary>
    public class QuickSort : ISorter
    {
        public void Sort(int[] array)
        {
            if (array == null || array.Length == 0) return;
            ExecuteSort(array, 0, array.Length - 1);
        }

        private void ExecuteSort(int[] array, int leftIndex, int rightIndex)
        {
            if (leftIndex < rightIndex)
            {
                int pivotIndex = Partition(array, leftIndex, rightIndex);
                ExecuteSort(array, leftIndex, pivotIndex - 1);
                ExecuteSort(array, pivotIndex + 1, rightIndex);
            }
        }

        private int Partition(int[] array, int leftIndex, int rightIndex)
        {
            int pivotValue = array[rightIndex];
            int pointer = leftIndex - 1;

            for (int i = leftIndex; i < rightIndex; i++)
            {
                if (array[i] < pivotValue)
                {
                    pointer++;
                    Swap(array, pointer, i);
                }
            }
            Swap(array, pointer + 1, rightIndex);
            return pointer + 1;
        }

        private void Swap(int[] array, int index1, int index2)
        {
            int temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }

    /// <summary>
    /// Dictionary state manager for LZW compression/decompression operations.
    /// </summary>
    public class LzwDictionary
    {
        private readonly List<string> _entries = new List<string>();

        public void Add(string value) => _entries.Add(value);
        public bool Contains(string value) => _entries.Contains(value);

        // 1-based indexing standard for LZW mapping
        public int GetIndex(string value) => _entries.IndexOf(value) + 1;
        public string GetValue(int index) => _entries[index - 1];

        public void Initialize(string text) =>
            text.Distinct().ToList().ForEach(c => Add(c.ToString()));
    }

    public class InputSanitizer
    {
        public IEnumerable<int> ParseCodes(string source) =>
            Regex.Matches(source, @"\d+")
                 .Cast<Match>()
                 .Select(m => int.Parse(m.Value));

        public string PrepareText(string input) =>
            input?.Trim().Replace("\r", string.Empty).Replace("\n", string.Empty) ?? string.Empty;
    }

    public class LzwCompressor
    {
        private string _currentSequence = string.Empty;
        private readonly List<int> _outputCodes = new List<int>();

        public List<int> Compress(string input, LzwDictionary dictionary)
        {
            input.ToList().ForEach(c => ProcessStep(c.ToString(), dictionary));
            EmitLast(dictionary);
            return _outputCodes;
        }

        private void ProcessStep(string nextChar, LzwDictionary dictionary)
        {
            if (dictionary.Contains(_currentSequence + nextChar))
            {
                _currentSequence += nextChar;
            }
            else
            {
                HandleNewSequence(nextChar, dictionary);
            }
        }

        private void HandleNewSequence(string nextChar, LzwDictionary dictionary)
        {
            _outputCodes.Add(dictionary.GetIndex(_currentSequence));
            dictionary.Add(_currentSequence + nextChar);
            _currentSequence = nextChar;
        }

        private void EmitLast(LzwDictionary dictionary)
        {
            if (!string.IsNullOrEmpty(_currentSequence))
                _outputCodes.Add(dictionary.GetIndex(_currentSequence));
        }
    }

    public class LzwDecompressor
    {
        private string _previousEntry = string.Empty;
        private readonly List<string> _parts = new List<string>();

        public string Decompress(List<int> codes, LzwDictionary dictionary)
        {
            InitializeState(codes[0], dictionary);
            codes.Skip(1).ToList().ForEach(c => DecodeStep(c, dictionary));
            return string.Join(string.Empty, _parts);
        }

        private void InitializeState(int firstCode, LzwDictionary dictionary)
        {
            _previousEntry = dictionary.GetValue(firstCode);
            _parts.Add(_previousEntry);
        }

        private void DecodeStep(int code, LzwDictionary dictionary)
        {
            string entry = RetrieveEntry(code, dictionary);
            _parts.Add(entry);
            dictionary.Add(_previousEntry + entry[0]);
            _previousEntry = entry;
        }

        private string RetrieveEntry(int code, LzwDictionary dictionary)
        {
            // Handles the LZW edge case where the code is not yet in the dictionary
            try { return dictionary.GetValue(code); }
            catch { return _previousEntry + _previousEntry[0]; }
        }
    }

    public class DijkstraOrchestrator
    {
        private readonly Form1 _view;
        private readonly GraphGenerator _generator;
        private Graph? _currentGraph;

        public DijkstraOrchestrator(Form1 view)
        {
            _view = view;
            _generator = new GraphGenerator();

            _view.GenerateGraphClicked += HandleGenerateGraph;
            _view.RunDijkstraClicked += HandleRunDijkstra;

            HandleGenerateGraph(this, EventArgs.Empty);
        }

        private void HandleGenerateGraph(object? sender, EventArgs e)
        {
            _currentGraph = _generator.GenerateLayeredGraph();
            _view.SetGraphForDrawing(_currentGraph);
        }

        private async void HandleRunDijkstra(object? sender, EventArgs e)
        {
            if (_currentGraph == null || _currentGraph.GetVertices().Count == 0) return;

            _view.ResetAlgorithmState();
            _view.SetCostText("Optimal path cost: calculating...");

            var source = _currentGraph.GetVertices().First();
            var target = _currentGraph.GetVertices().Last();
            var algorithm = new DijkstraAlgorithm();

            // Bridge algorithm progress directly back to the UI thread safely.
            algorithm.StepCompleted += (s, args) =>
            {
                _view.Invoke((MethodInvoker)delegate {
                    _view.UpdateDijkstraState(args.CurrentVertex, args.VisitedVertices, args.CurrentPath);
                });
            };

            var result = await algorithm.FindShortestPathAsync(_currentGraph, source, target);
            string report = GenerateReport(result);

            _view.ShowDijkstraResult(result.Path, result.TotalCost, report);
        }

        private string GenerateReport(DijkstraResult result)
        {
            if (result.Path == null) return "No path found.";

            var sb = new StringBuilder();
            sb.AppendLine("GRAPH ANALYSIS SUMMARY");
            sb.AppendLine("==========================");
            sb.AppendLine($"Winning path: {string.Join(" -> ", result.Path.Select(v => v.Name))}");
            sb.AppendLine($"Winning path cost: {result.TotalCost}\n");

            sb.AppendLine("Alternative paths explored (rejected):");
            if (result.AlternativePaths.Count > 0)
            {
                foreach (string alt in result.AlternativePaths) sb.AppendLine("- " + alt);
            }
            else
            {
                sb.AppendLine("- No alternatives explored.");
            }
            return sb.ToString();
        }
    }

    public class SortOrchestrator
    {
        private readonly Form1 _view;
        private readonly ISorter _sorter;

        public SortOrchestrator(Form1 view)
        {
            _view = view;
            _sorter = new QuickSort();
            _view.SortClicked += HandleSort;
        }

        private void HandleSort(object? sender, EventArgs e)
        {
            try
            {
                int[] data = _view.GetSortInput()
                                  .Split(',')
                                  .Select(t => int.Parse(t.Trim()))
                                  .ToArray();

                var stopwatch = Stopwatch.StartNew();
                _sorter.Sort(data);
                stopwatch.Stop();

                _view.SetSortOutput(string.Join(", ", data));
                _view.SetSortTime($"Execution time: {stopwatch.Elapsed.TotalMilliseconds:F4} ms");
            }
            catch
            {
                _view.ShowError("Invalid format. Please use comma-separated integers.");
            }
        }
    }

    public class LzwOrchestrator
    {
        private readonly Form1 _view;
        private readonly InputSanitizer _sanitizer = new InputSanitizer();

        public LzwOrchestrator(Form1 view)
        {
            _view = view;
            _view.CompressClicked += HandleCompression;
            _view.DecompressClicked += HandleDecompression;
        }

        private void HandleCompression(object? sender, EventArgs e)
        {
            try
            {
                string rawText = _view.GetCompressionInput();
                if (string.IsNullOrEmpty(rawText)) return;

                string cleanText = _sanitizer.PrepareText(rawText);

                var dictionary = new LzwDictionary();
                dictionary.Initialize(cleanText);

                var compressor = new LzwCompressor();
                List<int> codes = compressor.Compress(cleanText, dictionary);

                string alphabet = string.Join(string.Empty, cleanText.Distinct());
                _view.SetCompressionOutput(string.Join(" ", codes), alphabet);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Compression error: {ex.Message}");
            }
        }

        private void HandleDecompression(object? sender, EventArgs e)
        {
            try
            {
                string codesStr = _view.GetDecompressionCodes();
                string alphabet = _view.GetDecompressionAlphabet();

                if (string.IsNullOrEmpty(codesStr) || string.IsNullOrEmpty(alphabet)) return;

                var dictionary = new LzwDictionary();
                dictionary.Initialize(alphabet);

                var codes = new List<int>(_sanitizer.ParseCodes(codesStr));

                var decompressor = new LzwDecompressor();
                string result = decompressor.Decompress(codes, dictionary);

                _view.SetDecompressionOutput(result);
            }
            catch (Exception ex)
            {
                _view.ShowError($"Decompression error: {ex.Message}");
            }
        }
    }
}