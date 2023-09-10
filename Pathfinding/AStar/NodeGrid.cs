using System.Numerics;

namespace Pathfinding.AStar {
    internal class NodeGrid {

        public readonly int Width;

        public readonly int Height;

        public readonly Node[] Nodes;

        public NodeGrid(int width, int height) {
            Width = width;
            Height = height;
            Nodes = new Node[Width * Height];

            for (int i = 0; i < Nodes.Length; i++) {
                Nodes[i] = new Node();
                Nodes[i].Position = new Vector2(i % Width, i / Width);
            }
        }

        public void ToggleBlocked(Vector2 position) {
            int index = ((int)position.Y * Width) + (int)position.X;
            if (index < 0 || index >= Nodes.Length) {
                return;
            }

            Nodes[index].Blocked = !Nodes[index].Blocked;
        }
    }

    internal class Node {

        public bool Blocked;

        public Vector2 Position;

        public int Index;

        public float GScore;

        public float FScore;

        public int PreviousIndex;

    }
}
