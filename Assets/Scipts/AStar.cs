using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int target, GridManager gridManager)
    {
        // Sử dụng PriorityQueue cho openList (C# 10+ có sẵn, nếu dùng version cũ hơn cần tự implement)
        PriorityQueue<Node, int> openList = new PriorityQueue<Node, int>();
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Node> nodeTracker = new Dictionary<Vector3Int, Node>(); // Theo dõi node để cập nhật

        Node startNode = new Node(start, 0, Heuristic(start, target), null);
        openList.Enqueue(startNode, startNode.FCost);
        nodeTracker.Add(start, startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList.Dequeue();
            closedList.Add(currentNode.position);

            if (currentNode.position == target)
                return RetracePath(currentNode);

            foreach (Vector3Int neighborPos in gridManager.GetNeighbors(currentNode.position))
            {
                if (closedList.Contains(neighborPos)) continue;

                int newGCost = currentNode.gCost + 1;
                Node neighborNode = nodeTracker.TryGetValue(neighborPos, out Node existingNode) ? existingNode : null;

                if (neighborNode == null)
                {
                    int hCost = Heuristic(neighborPos, target);
                    neighborNode = new Node(neighborPos, newGCost, hCost, currentNode);
                    openList.Enqueue(neighborNode, neighborNode.FCost);
                    nodeTracker[neighborPos] = neighborNode;
                }
                else if (newGCost < neighborNode.gCost)
                {
                    neighborNode.gCost = newGCost;
                    neighborNode.parent = currentNode;
                    openList.Enqueue(neighborNode, neighborNode.FCost); // Cần re-enqueue để cập nhật priority
                }
            }
        }
        return null;
    }

    // Các hàm helper giữ nguyên
    private static int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    private static List<Vector3Int> RetracePath(Node endNode)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Node currentNode = endNode;
        while (currentNode != null)
        {
            path.Add(currentNode.position);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        return path;
    }

    private class Node
    {
        public Vector3Int position;
        public int gCost;
        public int hCost;
        public Node parent;
        public int FCost => gCost + hCost;

        public Node(Vector3Int pos, int g, int h, Node p)
        {
            position = pos;
            gCost = g;
            hCost = h;
            parent = p;
        }
    }
}