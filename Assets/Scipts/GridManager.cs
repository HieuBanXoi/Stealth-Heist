using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    // Tilemap cho nền (Ground) và tường (Wall)
    public Tilemap groundTilemap;
    public Tilemap wallTilemap;

    // Layer chứa các Item để kiểm tra chướng ngại vật
    public LayerMask itemLayer;

    // Danh sách các ô có thể đi qua (không phải tường và không có item)
    private HashSet<Vector3Int> walkablePositions = new HashSet<Vector3Int>();

    void Awake()
    {
        // Duyệt qua tất cả các ô trong Tilemap Ground để xác định ô có thể đi được
        BoundsInt bounds = groundTilemap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin)
        {
            // Nếu ô có tile nền, không có tile tường và không có item → có thể đi được
            if (groundTilemap.HasTile(pos) && !wallTilemap.HasTile(pos) && !HasItem(pos))
            {
                walkablePositions.Add(pos);
            }
        }
    }

    // Kiểm tra xem tại một ô có item nào không
    bool HasItem(Vector3Int cellPosition)
    {
        // Chuyển vị trí của ô trong Tilemap thành vị trí thế giới
        Vector3 worldPos = groundTilemap.CellToWorld(cellPosition) + new Vector3(0.5f, 0.5f, 0); // Căn giữa ô
        float checkRadius = 0.3f; // Bán kính kiểm tra (có thể điều chỉnh theo kích thước item)

        // Kiểm tra xem có bất kỳ Collider nào thuộc `itemLayer` ở vị trí này không
        return Physics2D.OverlapCircle(worldPos, checkRadius, itemLayer) != null;
    }

    // Lấy danh sách các ô kề bên có thể đi được
    public List<Vector3Int> GetNeighbors(Vector3Int cellPosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        // Các hướng di chuyển (trái, phải, trên, dưới)
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),  // Phải
            new Vector3Int(-1, 0, 0), // Trái
            new Vector3Int(0, 1, 0),  // Trên
            new Vector3Int(0, -1, 0)  // Dưới
        };

        // Kiểm tra từng hướng di chuyển
        foreach (var dir in directions)
        {
            Vector3Int neighborPos = cellPosition + dir;
            if (walkablePositions.Contains(neighborPos)) // Nếu ô có thể đi được, thêm vào danh sách
            {
                neighbors.Add(neighborPos);
            }
        }

        return neighbors;
    }
}
