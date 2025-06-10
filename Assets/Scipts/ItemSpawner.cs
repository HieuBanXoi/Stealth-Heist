using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject itemPrefab; // Prefab của item
        public int quantity = 1; // Số lượng item cần spawn
        public float minDistanceBetweenSameType = 2f; // Khoảng cách tối thiểu giữa các item cùng loại
    }

    [SerializeField] private SpawnableItem[] spawnableItems; // Danh sách các item có thể spawn
    [SerializeField] private float spawnRadius = 5f; // Bán kính spawn item
    [SerializeField] private LayerMask obstacleLayer; // Layer chứa các vật cản
    [SerializeField] private float minDistanceFromObstacles = 1f; // Khoảng cách tối thiểu từ vật cản
    [SerializeField] private float minDistanceBetweenItems = 2f; // Khoảng cách tối thiểu giữa các item

    private List<Vector3> spawnedPositions = new List<Vector3>(); // Danh sách vị trí đã spawn
    private List<GameObject> spawnedItems = new List<GameObject>(); // Danh sách các item đã spawn
    private GridManager gridManager; // Tham chiếu đến GridManager
    private QuestionAssigner questionAssigner; // Tham chiếu đến QuestionAssigner

    void Start()
    {
        // Kiểm tra và lấy tham chiếu đến GridManager
        gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            Debug.LogError("Không tìm thấy GridManager trong scene!");
            enabled = false;
            return;
        }

        // Kiểm tra và lấy tham chiếu đến QuestionAssigner
        questionAssigner = FindObjectOfType<QuestionAssigner>();
        if (questionAssigner == null)
        {
            Debug.LogError("Không tìm thấy QuestionAssigner trong scene!");
            enabled = false;
            return;
        }

        // Kiểm tra dữ liệu đầu vào
        if (spawnableItems == null || spawnableItems.Length == 0)
        {
            Debug.LogError("Không có item nào được cấu hình để spawn!");
            enabled = false;
            return;
        }

        // Kiểm tra từng item trong danh sách
        foreach (var item in spawnableItems)
        {
            if (item == null || item.itemPrefab == null)
            {
                Debug.LogError("Có item null trong danh sách spawnableItems!");
                enabled = false;
                return;
            }
        }

        SpawnItems();
    }

    void SpawnItems()
    {
        // Xóa danh sách vị trí cũ
        spawnedPositions.Clear();
        spawnedItems.Clear();

        // Spawn từng loại item với số lượng đã chỉ định
        foreach (SpawnableItem item in spawnableItems)
        {
            if (item == null || item.itemPrefab == null) continue;

            int remainingQuantity = item.quantity;

            // Kiểm tra xem item có phải là key không
            bool isKey = item.itemPrefab.GetComponent<KeyManager>() != null;

            if (isKey)
            {
                // Đối với chìa khóa, thử cho đến khi spawn đủ
                while (remainingQuantity > 0)
                {
                    int initialRemaining = remainingQuantity;

                    for (int i = 0; i < remainingQuantity; i++)
                    {
                        if (!SpawnItem(item))
                        {
                            remainingQuantity = i;
                            break;
                        }
                    }

                    // Nếu không spawn đủ, xóa các key đã spawn và thử lại
                    if (remainingQuantity > 0)
                    {
                        Debug.LogWarning($"Không thể spawn đủ {item.itemPrefab.name}. Đã spawn {item.quantity - remainingQuantity}/{item.quantity}. Thử lại...");
                        // Xóa các key đã spawn trong lần thử này
                        for (int i = spawnedItems.Count - 1; i >= 0; i--)
                        {
                            if (spawnedItems[i] != null && spawnedItems[i].name.Contains(item.itemPrefab.name))
                            {
                                Destroy(spawnedItems[i]);
                                spawnedItems.RemoveAt(i);
                                spawnedPositions.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            else

            {
                // Đối với các vật phẩm khác, chỉ thử một lần
                for (int i = 0; i < remainingQuantity; i++)
                {
                    if (!SpawnItem(item))
                    {
                        remainingQuantity = i;
                        break;
                    }
                }

                if (remainingQuantity > 0)
                {
                    Debug.LogWarning($"Không thể spawn đủ {item.itemPrefab.name}. Đã spawn {item.quantity - remainingQuantity}/{item.quantity} item.");
                }
            }
        }
    }

    bool SpawnItem(SpawnableItem item)
    {
        if (item == null || item.itemPrefab == null) return false;

        // Tìm vị trí hợp lệ để spawn
        Vector3 spawnPosition = FindValidSpawnPosition(item);
        if (spawnPosition != Vector3.zero)
        {
            // Spawn item tại vị trí đã tìm được
            GameObject spawnedItem = Instantiate(item.itemPrefab, spawnPosition, Quaternion.identity);
            if (spawnedItem == null) return false;

            // Nếu là key, gán câu hỏi ngẫu nhiên
            KeyManager keyManager = spawnedItem.GetComponent<KeyManager>();
            if (keyManager != null)
            {
                QuestionManager question = questionAssigner.GetRandomQuestion();
                if (question != null)
                {
                    keyManager.SetQuestion(question);
                }
            }

            spawnedPositions.Add(spawnPosition);
            spawnedItems.Add(spawnedItem);
            return true;
        }
        return false;
    }

    Vector3 FindValidSpawnPosition(SpawnableItem item)
    {
        if (item == null || item.itemPrefab == null) return Vector3.zero;

        int maxAttempts = item.itemPrefab.GetComponent<KeyManager>() != null ? 100 : 50; // Tăng số lần thử cho chìa khóa
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            // Tạo vị trí ngẫu nhiên trong bán kính
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPosition = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);

            // Kiểm tra xem vị trí có hợp lệ không
            if (IsValidPosition(randomPosition, item))
            {
                return randomPosition;
            }

            attempts++;
        }

        return Vector3.zero; // Không tìm thấy vị trí hợp lệ
    }

    bool IsValidPosition(Vector3 position, SpawnableItem currentItem)
    {
        if (currentItem == null || currentItem.itemPrefab == null) return false;
        if (gridManager == null || gridManager.groundTilemap == null || gridManager.wallTilemap == null) return false;

        // Kiểm tra xem vị trí có nằm trên tilemap không
        Vector3Int cellPosition = gridManager.groundTilemap.WorldToCell(position);
        if (!gridManager.groundTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // Kiểm tra xem vị trí có nằm trên wall không
        if (gridManager.wallTilemap.HasTile(cellPosition))
        {
            return false;
        }

        // Kiểm tra khoảng cách với vật cản
        Collider2D[] obstacles = Physics2D.OverlapCircleAll(position, minDistanceFromObstacles, obstacleLayer);
        if (obstacles.Length > 0)
        {
            return false;
        }

        // Kiểm tra khoảng cách với các item đã spawn
        for (int i = 0; i < spawnedPositions.Count; i++)
        {
            if (spawnedItems[i] == null) continue;

            float minDistance = minDistanceBetweenItems;

            // Nếu là item cùng loại, sử dụng khoảng cách tối thiểu giữa các item cùng loại
            if (spawnedItems[i].name.Contains(currentItem.itemPrefab.name))
            {
                minDistance = currentItem.minDistanceBetweenSameType;
            }

            if (Vector3.Distance(position, spawnedPositions[i]) < minDistance)
            {
                return false;
            }
        }

        return true;
    }

    // Vẽ gizmo để dễ nhìn trong editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}