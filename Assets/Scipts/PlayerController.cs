using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1500f;  // Tốc độ di chuyển của nhân vật
    private Rigidbody2D rb;  // Tham chiếu đến Rigidbody2D để xử lý vật lý
    private Animator animator;  // Tham chiếu đến Animator để cập nhật animation
    private Vector2 movement;  // Vector lưu trữ hướng di chuyển

    private void Awake()
    {
        // Lấy các component Rigidbody2D và Animator khi khởi tạo
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update được gọi mỗi frame
    void Update()
    {
        // Xử lý đầu vào từ người chơi
        HandleMovement();

        // Cập nhật trạng thái animation
        UpdateAnimation();
    }

    // Xử lý di chuyển của người chơi
    private void HandleMovement()
    {
        // Nhận giá trị đầu vào từ bàn phím (WASD hoặc phím mũi tên)
        movement.x = Input.GetAxis("Horizontal");
        movement.y = Input.GetAxis("Vertical");

        // Cập nhật vận tốc của Rigidbody2D dựa trên hướng di chuyển
        rb.velocity = movement.normalized * moveSpeed * Time.deltaTime;
    }

    // Cập nhật animation dựa trên hướng di chuyển
    private void UpdateAnimation()
    {
        // Truyền giá trị trục x và y vào Animator để thay đổi animation
        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);

        // Kiểm tra nếu nhân vật đang di chuyển (magnitude > 0)
        animator.SetBool("isMoving", movement.magnitude > 0);
    }
    public void SpeedUp()
    {
        StartCoroutine(SpeedBoostCoroutine());
    }
    private IEnumerator SpeedBoostCoroutine()
    {
        float originalSpeed = moveSpeed; // Lưu lại tốc độ gốc
        moveSpeed *= 2f; // Tăng tốc độ lên gấp đôi

        yield return new WaitForSeconds(10f); // Chờ 10 giây

        moveSpeed = originalSpeed; // Khôi phục tốc độ ban đầu
    }
}
