using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Tiles;
using UnityEngine;
using static NVTT.Utilities;

namespace Pathfinding._Scripts 
{
    public static class Pathfinding 
    {
        private static readonly Color PathColor = new Color(0.65f, 0.35f, 0.35f);
        private static readonly Color OpenColor = new Color(.4f, .6f, .4f);
        private static readonly Color ClosedColor = new Color(0.35f, 0.4f, 0.5f);
        
        public static IEnumerator AStar(NodeBase startNode, NodeBase targetNode, float delay)
        {
            var toSearch = new List<NodeBase> { startNode };
            var processed = new List<NodeBase>();
            var nodesExpanded = 0;
            
            while (toSearch.Any()) 
            {
                var current = toSearch[0];
                foreach (var t in toSearch) 
                    if (t.F < current.F || t.F == current.F && t.H < current.H) current = t;

                processed.Add(current);
                toSearch.Remove(current);
                
                current.SetColor(ClosedColor);
                nodesExpanded++;

                if (current == targetNode) 
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    while (currentPathTile != startNode) 
                    {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                    }
                    
                    foreach (var tile in path) tile.SetColor(PathColor);
                    startNode.SetColor(PathColor);
                    Debug.Log("Total nodes expanded: " + nodesExpanded);
                    Debug.Log("Path Length: " + path.Count);
                    yield break;
                }

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !processed.Contains(t))) {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    if (!inSearch || costToNeighbor < neighbor.G) {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch) {
                            neighbor.SetH(neighbor.GetDistance(targetNode));
                            toSearch.Add(neighbor);
                            neighbor.SetColor(OpenColor);
                        }
                    }
                }

                yield return GetWfs(delay);
            }
            Debug.Log("Total nodes expanded: " + nodesExpanded);
            Debug.Log("Path not found");
            yield return null;
        }
        
        public static IEnumerator Dijkstra(NodeBase startNode, NodeBase targetNode, float delay) 
        {
            var toSearch = new List<NodeBase> { startNode };
            var processed = new List<NodeBase>();
            var nodesExpanded = 0;
            
            while (toSearch.Any()) 
            {
                var current = toSearch[0];
                foreach (var t in toSearch) 
                    if (t.G < current.G) current = t;

                processed.Add(current);
                toSearch.Remove(current);

                current.SetColor(ClosedColor);
                nodesExpanded++;

                if (current == targetNode) 
                {
                    var currentPathTile = targetNode;
                    var path = new List<NodeBase>();
                    while (currentPathTile != startNode) {
                        path.Add(currentPathTile);
                        currentPathTile = currentPathTile.Connection;
                    }

                    foreach (var tile in path) tile.SetColor(PathColor);
                    startNode.SetColor(PathColor);
                    Debug.Log("Total nodes expanded: " + nodesExpanded);
                    Debug.Log("Path Length: " + path.Count);
                    yield break;
                }

                foreach (var neighbor in current.Neighbors.Where(t => t.Walkable && !processed.Contains(t))) 
                {
                    var inSearch = toSearch.Contains(neighbor);

                    var costToNeighbor = current.G + current.GetDistance(neighbor);

                    if (!inSearch || costToNeighbor < neighbor.G) 
                    {
                        neighbor.SetG(costToNeighbor);
                        neighbor.SetConnection(current);

                        if (!inSearch) 
                        {
                            //neighbor.SetH(0); // set H to 0 to disable the heuristic in Dijkstra
                            toSearch.Add(neighbor);
                            neighbor.SetColor(OpenColor);
                        }
                    }
                }
                yield return GetWfs(delay);
            }
            Debug.Log("Total nodes expanded: " + nodesExpanded);
            Debug.Log("Path not found");
            yield return null;
        }
        public static IEnumerator DFS(NodeBase startNode, NodeBase targetNode, float delay)
        {
            var stack = new Stack<NodeBase>();
            var visited = new HashSet<NodeBase>();
            var cameFrom = new Dictionary<NodeBase, NodeBase>();
            var nodesExpanded = 0;
            
            stack.Push(startNode);
            visited.Add(startNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                nodesExpanded++;

                if (current == targetNode)
                {
                    // Reconstruct path from start to target using cameFrom
                    var path = new List<NodeBase> { targetNode };
                    while (cameFrom.ContainsKey(path[0]))
                    {
                        path.Insert(0, cameFrom[path[0]]);
                    }

                    foreach (var node in path)
                    {
                        node.SetColor(PathColor);
                    }

                    startNode.SetColor(PathColor);
                    Debug.Log("Total nodes expanded: " + nodesExpanded);
                    Debug.Log($"Path Length: {path.Count - 1}");
                    yield break;
                }

                foreach (var neighbor in current.Neighbors)
                {
                    if (!neighbor.Walkable || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    stack.Push(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    neighbor.SetColor(OpenColor);
                    
                    yield return GetWfs(delay);
                }

                current.SetColor(ClosedColor);
            }

            Debug.Log("Total nodes expanded: " + nodesExpanded);
            Debug.Log("Path not found");
            yield return null;
        }

        public static IEnumerator BFS(NodeBase startNode, NodeBase targetNode, float delay)
        {
            var queue = new Queue<NodeBase>();
            var visited = new HashSet<NodeBase>();
            var cameFrom = new Dictionary<NodeBase, NodeBase>();
            var nodesExpanded = 0;
            
            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                nodesExpanded++;

                if (current == targetNode)
                {
                    // Reconstruct path from start to target using cameFrom
                    var path = new List<NodeBase> { targetNode };
                    while (cameFrom.ContainsKey(path[0]))
                    {
                        path.Insert(0, cameFrom[path[0]]);
                    }

                    foreach (var node in path)
                    {
                        node.SetColor(PathColor);
                    }

                    startNode.SetColor(PathColor);
                    Debug.Log("Total nodes expanded: " + nodesExpanded);
                    Debug.Log($"Path Length: {path.Count - 1}");
                    yield break;
                }

                foreach (var neighbor in current.Neighbors)
                {
                    if (!neighbor.Walkable || visited.Contains(neighbor))
                    {
                        continue;
                    }

                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    neighbor.SetColor(OpenColor);

                    yield return GetWfs(delay);
                }

                current.SetColor(ClosedColor);
            }

            // If target not found, return null
            Debug.Log("Total nodes expanded: " + nodesExpanded);
            Debug.Log("Path not found");
            yield return null;
        }
    }
}