using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform pointA; // Điểm tuần tra A
    [SerializeField] private Transform pointB; // Điểm tuần tra B
    [SerializeField] private float speed; // Tốc độ di chuyển của enemy
    [SerializeField] private float chaseSpeed = 1.5f; // Tốc độ khi đuổi theo người chơi
    [SerializeField] private Transform player; // Tham chiếu đến player
    [SerializeField] private float defaultChaseRange; // Phạm vi phát hiện player
    [SerializeField] private float defaultReturnRange; // Khoảng cách để quay lại tuần tra
    [SerializeField] private float updatePathInterval = 1f; // Thời gian cập nhật đường đi
    [SerializeField] private GameObject exclamationMark; // Biểu tượng chấm than
    [SerializeField] private float increasedChaseRange = 20f; // Phạm vi phát hiện tăng khi trả lời sai
    [SerializeField] private float increasedChaseDuration = 10f; // Thời gian tăng phạm vi phát hiện

    private Animator animator; // Điều khiển animation của enemy
    private List<Vector3> patrolPoints; // Danh sách các điểm tuần tra (A và B)
    private int currentPatrolIndex = 0; // Chỉ mục điểm tuần tra hiện tại
    public bool isChasing = false; // Trạng thái đuổi theo player
    private List<Vector3> currentPath; // Đường đi A* hiện tại
    private int currentWaypoint; // Điểm đến tiếp theo trong đường đi
    private GridManager gridManager; // Tham chiếu đến GridManager
    private float chaseRange;
    private float returnRange;
    private bool isChaseRangeIncreased = false;

    void Start()
    {
        chaseRange = defaultChaseRange;
        returnRange = defaultReturnRange;
        animator = GetComponent<Animator>(); // Lấy component Animator
        gridManager = FindObjectOfType<GridManager>(); // Tìm GridManager trong scene
        exclamationMark.SetActive(false); // Ẩn biểu tượng chấm than khi bắt đầu
        patrolPoints = new List<Vector3> { pointA.position, pointB.position }; // Khởi tạo danh sách điểm tuần tra

        StartCoroutine(UpdatePath()); // Cập nhật đường đi theo thời gian
        StartCoroutine(Patrol()); // Bắt đầu tuần tra
    }

    void Update()
    {
        if (GameManager.Instance.isPlayerHiding || (player.GetComponent<PlayerController>() != null && player.GetComponent<PlayerController>().isInvisible))
        {
            chaseRange = 0;
            returnRange = 0;
            exclamationMark.SetActive(false);
        }
        else
        {
            chaseRange = defaultChaseRange;
            returnRange = defaultReturnRange;
        }

        if (GameManager.Instance.isWrongAnswer && !isChaseRangeIncreased) // Nếu trả lời sai và chưa tăng phạm vi
        {
            StartCoroutine(IncreaseChaseRange());
        }

        MoveAlongPath();
        if (isChasing)
        {
            exclamationMark.SetActive(true); // Hiển thị biểu tượng chấm than khi đuổi theo
            ChasePlayer(); // Đuổi theo người chơi
        }
        else
        {
            exclamationMark.SetActive(false); // Ẩn biểu tượng chấm than khi không đuổi theo
        }
        UpdateAnimation(); // Cập nhật animation
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || currentWaypoint >= currentPath.Count) return;

        float currentSpeed = isChasing ? speed * chaseSpeed : speed; // Sử dụng tốc độ cao hơn khi đuổi theo
        transform.position = Vector3.MoveTowards(transform.position, currentPath[currentWaypoint], currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, currentPath[currentWaypoint]) < 0.1f)
        {
            currentWaypoint++;
        }
    }

    private void ChasePlayer()
    {
        if (Vector3.Distance(transform.position, player.position) > returnRange) // Nếu player thoát khỏi phạm vi, quay lại tuần tra
        {
            isChasing = false;
            currentPath = null;
            StartCoroutine(Patrol());
        }
    }
    private IEnumerator UpdatePath()
    {
        while (true)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position); // Tính khoảng cách đến player
            bool isPlayerInvisible = player.GetComponent<PlayerController>() != null && player.GetComponent<PlayerController>().isInvisible;

            if (distanceToPlayer <= chaseRange && !isPlayerInvisible) // Nếu player trong phạm vi và không tàng hình, bắt đầu đuổi theo
            {
                updatePathInterval = 1f;
                isChasing = true;
                Vector3Int startCell = gridManager.groundTilemap.WorldToCell(transform.position);
                Vector3Int targetCell = gridManager.groundTilemap.WorldToCell(player.position);

                List<Vector3Int> pathCells = AStar.FindPath(startCell, targetCell, gridManager);

                if (pathCells != null && pathCells.Count > 0)
                {
                    currentPath = ConvertToWorldPath(pathCells);
                    currentWaypoint = 0;
                }
            }
            else if ((distanceToPlayer > returnRange || isPlayerInvisible) && isChasing) // Nếu player thoát ra khỏi returnRange hoặc tàng hình, quay lại tuần tra
            {
                updatePathInterval = 3f;
                isChasing = false;
                currentPath = null;
                yield return new WaitForSeconds(2f);
                StartCoroutine(Patrol());
            }

            yield return new WaitForSeconds(updatePathInterval);
        }
    }

    private IEnumerator Patrol()
    {
        while (!isChasing) // Khi không đuổi theo player
        {
            Vector3Int startCell = gridManager.groundTilemap.WorldToCell(transform.position); // Chuyển enemy về tọa độ cell
            Vector3Int targetCell = gridManager.groundTilemap.WorldToCell(patrolPoints[currentPatrolIndex]); // Chuyển điểm tuần tra về tọa độ cell

            List<Vector3Int> pathCells = AStar.FindPath(startCell, targetCell, gridManager); // Tìm đường đi A*

            if (pathCells != null && pathCells.Count > 0) // Nếu tìm thấy đường đi
            {
                currentPath = ConvertToWorldPath(pathCells);
                currentWaypoint = 0;
            }

            yield return new WaitUntil(() => currentPath == null || currentWaypoint >= currentPath.Count); // Đợi đến khi di chuyển xong

            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; // Chuyển đến điểm tuần tra tiếp theo
        }
    }

    private List<Vector3> ConvertToWorldPath(List<Vector3Int> cellPath)
    {
        List<Vector3> worldPath = new List<Vector3>();
        foreach (Vector3Int cell in cellPath)
        {
            worldPath.Add(gridManager.groundTilemap.GetCellCenterWorld(cell)); // Chuyển tọa độ cell thành tọa độ thế giới
        }
        return worldPath;
    }
    private void UpdateAnimation()
    {
        Vector3 moveDirection = (isChasing && currentPath != null && currentWaypoint < currentPath.Count) // Nếu đang đuổi theo
            ? (currentPath[currentWaypoint] - transform.position).normalized
            : (currentPath != null && currentWaypoint < currentPath.Count) // Nếu đang tuần tra
            ? (currentPath[currentWaypoint] - transform.position).normalized
            : Vector3.zero;

        animator.SetFloat("moveX", moveDirection.x); // Cập nhật hướng di chuyển X
        animator.SetFloat("moveY", moveDirection.y); // Cập nhật hướng di chuyển Y
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Màu đỏ hiển thị phạm vi đuổi theo
        Gizmos.DrawWireSphere(transform.position, defaultChaseRange);

        Gizmos.color = Color.blue; // Màu xanh hiển thị phạm vi quay lại tuần tra
        Gizmos.DrawWireSphere(transform.position, defaultReturnRange);
    }

    private IEnumerator IncreaseChaseRange()
    {
        isChaseRangeIncreased = true;
        float originalChaseRange = defaultChaseRange;
        float originalReturnRange = defaultReturnRange;
        defaultChaseRange = increasedChaseRange;
        defaultReturnRange = increasedChaseRange * 1.5f; // Return range is 1.5x the chase range
        chaseRange = increasedChaseRange;
        returnRange = defaultReturnRange;

        yield return new WaitForSeconds(increasedChaseDuration);

        defaultChaseRange = originalChaseRange;
        defaultReturnRange = originalReturnRange;
        chaseRange = originalChaseRange;
        returnRange = originalReturnRange;
        isChaseRangeIncreased = false;
    }
}
