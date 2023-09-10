using System.Numerics;

namespace Pathfinding.AStar {
    internal class AStarPathFinder {

        private static readonly Vector2[] _neighbourOffsets = {
            new Vector2(1,0),
            new Vector2(-1,0),
            new Vector2(0,1),
            new Vector2(0,-1),
            new Vector2(1,1),
            new Vector2(-1,-1),
            new Vector2(1,-1),
            new Vector2(-1,-1)
        };

        public static void FindPath(Vector2 from, Vector2 to, NodeGrid grid, Action<Vector2[]> onComplete) {
            int startIndex = 0;
            for (int i = 0; i < grid.Nodes.Length; i++) {
                grid.Nodes[i].FScore = float.MaxValue;
                grid.Nodes[i].GScore = float.MaxValue;
                grid.Nodes[i].PreviousIndex = -1;
                grid.Nodes[i].Index = i;

                if (grid.Nodes[i].Position == from) {
                    startIndex = i;
                }
            }

            List<Node> nodes = new List<Node>(grid.Nodes);
            List<Node> visited = new List<Node>();

            float fscore = GetHeuristic(nodes[startIndex], to);
            nodes[startIndex].FScore = fscore;
            nodes[startIndex].GScore = 0;

            bool done = false;
            while (!done) {
                if (nodes.Count == 0) {
                    done = true;
                    continue;
                }

                Node currentNode = GetNextBestNode(nodes);
                if (currentNode.Position == to) {
                    done = true;
                    visited.Add(currentNode);
                    continue;
                }

                List<Node> neighbours = GetNeighbours(currentNode, grid.Nodes, grid.Width);
                for (int i = 0; i < neighbours.Count; i++) {
                    if (visited.Contains(neighbours[i])) {
                        continue;
                    }

                    if (neighbours[i].Blocked) {
                        continue;
                    }

                    float newGScore = currentNode.GScore + 1;
                    neighbours[i].GScore = newGScore;
                    neighbours[i].FScore = newGScore + GetHeuristic(neighbours[i], to);
                    neighbours[i].PreviousIndex = currentNode.Index;
                }

                visited.Add(currentNode);
                nodes.Remove(currentNode);
            }

            List<Vector2> pathOutput = new List<Vector2>();
            Node node = visited[visited.Count - 1];
            int index = node.Index;
            while (grid.Nodes[index].PreviousIndex >= 0) {
                pathOutput.Insert(0, grid.Nodes[index].Position);
                index = grid.Nodes[index].PreviousIndex;
            }

            pathOutput.Insert(0, from);
            onComplete.Invoke(pathOutput.ToArray());
        }

        private static List<Node> GetNeighbours(Node node, Node[] nodes, int width) {
            List<Node> neighbours = new();
            for (int i = 0; i < _neighbourOffsets.Length; i++) {
                Vector2 pos = node.Position + _neighbourOffsets[i];
                for (int j = 0; j < nodes.Length; j++) {
                    if (nodes[j].Position == pos) {
                        neighbours.Add(nodes[j]);
                    }
                }
            }

            return neighbours;
        }

        private static Node GetNextBestNode(List<Node> nodes) {
            float bestF = float.MaxValue;
            int bestN = 0;
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i].FScore < bestF) {
                    bestF = nodes[i].FScore;
                    bestN = i;
                }
            }

            return nodes[bestN];
        }

        private static float GetHeuristic(Node node, Vector2 end) {
            return Vector2.DistanceSquared(node.Position, end);
        }
    }
}
