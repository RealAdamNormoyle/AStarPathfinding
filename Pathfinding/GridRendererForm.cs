using Timer = System.Windows.Forms.Timer;

namespace Pathfinding {
    internal class GridRendererForm : Form {

        private readonly Timer _refreshTimer;

        private readonly GridRendererContext _gridRendererContext;

        private readonly Bitmap _gridTexture;

        private readonly Bitmap[] _displayBuffers;

        private readonly int _gridWidthCells;

        private readonly int _cellWidth;

        private readonly Action _preRender;

        private readonly Action<Point, MouseButtons> _cellClicked;

        private int _backBuffer;

        public GridRendererForm(
            string title,
            Size windowSize,
            int gridSize,
            GridRendererContext gridRendererContext,
            Action preRender,
            Action<Point, MouseButtons> cellClicked) {

            _gridRendererContext = gridRendererContext;
            _gridWidthCells = gridSize;
            _cellWidth = windowSize.Width / gridSize;
            _gridTexture = new Bitmap(_cellWidth * _gridWidthCells, _cellWidth * _gridWidthCells);
            _refreshTimer = new Timer() { Enabled = true, Interval = 30 };
            _refreshTimer.Tick += (_, _) => { Render(); };
            _cellClicked = cellClicked;
            _preRender = preRender;
            _displayBuffers = new Bitmap[2]{
                new Bitmap(windowSize.Width, windowSize.Height),
                new Bitmap(windowSize.Width, windowSize.Height),
            };

            MouseClick += new MouseEventHandler(OnClicked);
            DoubleBuffered = true;
            Name = title;
            Text = title;
            Size = windowSize;
            Size offset = windowSize - DisplayRectangle.Size;
            Size = windowSize + offset;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            BackgroundImage = _gridTexture;
            BackgroundImageLayout = ImageLayout.Tile;
            GenerateGrid(Graphics.FromImage(_gridTexture), _cellWidth, _gridWidthCells);
        }

        private void OnClicked(object? sender, MouseEventArgs e) {
            Point cellPosition = new Point() {
                X = (int)MathF.Floor(e.X / (float)_cellWidth),
                Y = (int)MathF.Floor(e.Y / (float)_cellWidth)
            };

            _cellClicked(cellPosition, e.Button);
        }

        private void Render() {
            _gridRendererContext.RenderQueue.Clear();
            _preRender.Invoke();
            int frontBuffer = _backBuffer == 0 ? 1 : 0;
            BackgroundImage = _displayBuffers[frontBuffer];
            Refresh();

            Graphics graphics = Graphics.FromImage(_displayBuffers[_backBuffer]);
            _backBuffer = frontBuffer;
            graphics.Clear(Color.White);
            graphics.DrawImage(_gridTexture, new Point(0, 0));
            if (_gridRendererContext.RenderQueue == null) {
                graphics.Flush();
                return;
            }

            for (int i = 0; i < _gridRendererContext.RenderQueue.Count; i++) {
                RenderQueueItem entry = _gridRendererContext.RenderQueue[i];
                Point pos = new Point((int)MathF.Floor(entry.Position.X * _cellWidth), (int)MathF.Floor(entry.Position.Y * _cellWidth));
                pos.X += (int)((_cellWidth / 2) - ((entry.Size.Width * _cellWidth) / 2));
                pos.Y += (int)((_cellWidth / 2) - ((entry.Size.Height * _cellWidth) / 2));

                if (entry.Circle) {
                    if (entry.Fill) {
                        graphics.FillEllipse(entry.Color, new RectangleF(pos, entry.Size * _cellWidth));
                    } else {
                        graphics.DrawEllipse(new Pen(entry.Color), new RectangleF(pos, entry.Size * _cellWidth));
                    }
                } else {
                    if (entry.Fill) {
                        graphics.FillRectangle(entry.Color, new RectangleF(pos, entry.Size * _cellWidth));
                    } else {
                        graphics.DrawRectangle(new Pen(entry.Color), new RectangleF(pos, entry.Size * _cellWidth));
                    }
                }
            }

            graphics.Flush();
        }

        private static void GenerateGrid(Graphics graphics, int cellSize, int gridSize) {
            graphics.Clear(Color.White);

            for (int y = 0; y < gridSize; y += 2) {
                for (int x = 0; x < gridSize; x += 2) {
                    graphics.FillRectangle(Brushes.LightBlue, new Rectangle(cellSize * x, cellSize * y, cellSize, cellSize));
                    graphics.FillRectangle(Brushes.LightBlue, new Rectangle(cellSize * (x + 1), cellSize * (y + 1), cellSize, cellSize));
                }
            }

            graphics.Flush();
        }
    }

    public class GridRendererContext {

        public List<RenderQueueItem> RenderQueue = new();

    }

    public struct RenderQueueItem {

        public PointF Position;

        public Brush Color;

        public SizeF Size;

        public bool Circle;

        public bool Fill;

    }
}
