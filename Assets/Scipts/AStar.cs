using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<Vector3Int> FindPath(Vector3Int start, Vector3Int target, GridManager gridManager)
    {
        List<Node> openList = new List<Node>(); // Danh sách các node cần kiểm tra
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>(); // Danh sách các node đã kiểm tra

        Node startNode = new Node(start, 0, Heuristic(start, target), null); // Tạo node bắt đầu
        openList.Add(startNode);

        while (openList.Count > 0) // Lặp cho đến khi không còn node nào để kiểm tra
        {
            Node currentNode = GetLowestFNode(openList); // Lấy node có FCost thấp nhất
            openList.Remove(currentNode); // Loại bỏ node khỏi danh sách mở
            closedList.Add(currentNode.position); // Đánh dấu node đã kiểm tra

            if (currentNode.position == target) // Nếu đạt mục tiêu, truy vết đường đi
                return RetracePath(currentNode);

            foreach (Vector3Int neighborPos in gridManager.GetNeighbors(currentNode.position)) // Lặp qua các ô kề bên
            {
                if (closedList.Contains(neighborPos)) continue; // Nếu đã kiểm tra, bỏ qua

                int newGCost = currentNode.gCost + 1; // Tính lại gCost
                Node neighborNode = openList.Find(n => n.position == neighborPos); // Tìm node lân cận trong danh sách mở

                if (neighborNode == null) // Nếu chưa có trong danh sách mở
                {
                    int hCost = Heuristic(neighborPos, target); // Tính heuristic
                    neighborNode = new Node(neighborPos, newGCost, hCost, currentNode); // Tạo node mới
                    openList.Add(neighborNode); // Thêm vào danh sách mở
                }
                else if (newGCost < neighborNode.gCost) // Nếu đường đi hiện tại tốt hơn đường đi trước đó
                {
                    neighborNode.gCost = newGCost; // Cập nhật gCost
                    neighborNode.parent = currentNode; // Cập nhật parent để truy vết đường đi mới tốt hơn
                }
            }
        }
        return null; // Không tìm thấy đường đi
    }

    private static Node GetLowestFNode(List<Node> nodes) // Lấy node có FCost thấp nhất
    {
        Node lowestNode = nodes[0]; // Mặc định node đầu tiên là nhỏ nhất
        foreach (Node node in nodes)
        {
            if (node.FCost < lowestNode.FCost || // Kiểm tra nếu node hiện tại có FCost nhỏ hơn
               (node.FCost == lowestNode.FCost && node.hCost < lowestNode.hCost)) // Nếu bằng nhau, chọn node có hCost thấp hơn
            {
                lowestNode = node;
            }
        }
        return lowestNode;
    }

    private static int Heuristic(Vector3Int a, Vector3Int b) // Hàm Heuristic (Khoảng cách Manhattan)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Tính tổng khoảng cách theo trục x và y
    }

    private static List<Vector3Int> RetracePath(Node endNode) // Truy vết lại đường đi từ điểm đích đến điểm bắt đầu
    {
        List<Vector3Int> path = new List<Vector3Int>(); // Danh sách lưu đường đi
        Node currentNode = endNode;

        while (currentNode != null) // Lặp cho đến khi không còn node cha (tức đã về đến điểm xuất phát)
        {
            path.Add(currentNode.position); // Thêm node vào danh sách đường đi
            currentNode = currentNode.parent; // Đi lùi về node cha
        }
        path.Reverse(); // Đảo ngược danh sách để có đường đi đúng thứ tự
        return path;
    }

    private class Node
    {
        public Vector3Int position; // Vị trí của node trong grid
        public int gCost; // Khoảng cách từ điểm bắt đầu đến node hiện tại
        public int hCost; // Heuristic (ước tính khoảng cách đến điểm đích)
        public Node parent; // Node cha để truy vết đường đi
        public int FCost => gCost + hCost; // Tổng chi phí (gCost + hCost)

        public Node(Vector3Int pos, int g, int h, Node p)
        {
            position = pos;
            gCost = g;
            hCost = h;
            parent = p;
        }
    }
}
