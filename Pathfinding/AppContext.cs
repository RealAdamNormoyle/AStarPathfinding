using Pathfinding.AStar;
using System.Numerics;

namespace Pathfinding {
    internal class AppContext : ApplicationContext {

        private readonly GridRendererForm _gridRenderer;

        private readonly GridRendererContext _gridRendererContext = new();

        private readonly NodeGrid _nodeGrid;

        private Point _endCell = new Point();

        private PointF _currentPosition = new PointF();

        private Vector2[] _path = new Vector2[0];

        public AppContext() {
            _gridRenderer = new GridRendererForm("Grid Renderer", new Size(300, 300), 10, _gridRendererContext, OnPreRender, OnCellClicked);
            _gridRenderer.FormClosed += new FormClosedEventHandler(FormClosed);
            _gridRenderer.Show();

            _nodeGrid = new NodeGrid(10, 10);
        }

        private void OnCellClicked(Point position, MouseButtons button) {
            if (button == MouseButtons.Left) {
                _currentPosition = _endCell;
                _endCell = position;
                Vector2 point = new Vector2((int)_currentPosition.X, (int)_currentPosition.Y);
                AStarPathFinder.FindPath(point, new Vector2(position.X, position.Y), _nodeGrid, (path) => { _path = path; });
            } else if (button == MouseButtons.Right) {
                _nodeGrid.ToggleBlocked(new Vector2(position.X, position.Y));
            }
        }

        private void OnPreRender() {
            _gridRendererContext.RenderQueue.Add(new RenderQueueItem() {
                Position = _endCell,
                Size = new SizeF(1, 1),
                Circle = true,
                Fill = false,
                Color = Brushes.Red
            });

            _gridRendererContext.RenderQueue.Add(new RenderQueueItem() {
                Position = _currentPosition,
                Size = new SizeF(1, 1),
                Circle = true,
                Fill = false,
                Color = Brushes.Blue
            });

            for (int i = 0; i < _nodeGrid.Nodes.Length; i++) {
                if (_nodeGrid.Nodes[i].Blocked) {
                    Point point = new Point((int)_nodeGrid.Nodes[i].Position.X, (int)_nodeGrid.Nodes[i].Position.Y);
                    _gridRendererContext.RenderQueue.Add(new RenderQueueItem() {
                        Position = point,
                        Size = new SizeF(1f, 1f),
                        Circle = false,
                        Fill = true,
                        Color = Brushes.Black
                    });
                }
            }

            if (_path != null) {
                for (int i = 0; i < _path.Length; i++) {
                    _gridRendererContext.RenderQueue.Add(new RenderQueueItem() {
                        Position = new Point((int)_path[i].X, (int)_path[i].Y),
                        Size = new SizeF(0.3f, 0.3f),
                        Circle = true,
                        Fill = true,
                        Color = Brushes.Red
                    });
                }
            }
        }

        private void FormClosed(object? sender, FormClosedEventArgs e) {
            Application.Exit();
        }
    }
}
