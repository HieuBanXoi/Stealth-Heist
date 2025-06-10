using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 1200f;  // Tốc độ di chuyển của nhân vật
    private Rigidbody2D rb;  // Tham chiếu đến Rigidbody2D để xử lý vật lý
    private Animator animator;  // Tham chiếu đến Animator để cập nhật animation
    private Vector2 movement;  // Vector lưu trữ hướng di chuyển
    private SpriteRenderer spriteRenderer; // Tham chiếu đến SpriteRenderer để điều chỉnh độ trong suốt
    private Color originalColor; // Lưu màu gốc của nhân vật
    public bool isInvisible = false; // Trạng thái tàng hình của nhân vật
    public bool isSpeedUp = false; // Trạng thái tăng tốc của nhân vật
    private void Awake()
    {
        // Lấy các component Rigidbody2D và Animator khi khởi tạo
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color; // Lưu màu gốc
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
        rb.velocity = movement.normalized * moveSpeed;
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
        if (!isSpeedUp)
        {
            StartCoroutine(SpeedBoostCoroutine());
        }
    }
    private IEnumerator SpeedBoostCoroutine()
    {
        float originalSpeed = moveSpeed; // Lưu lại tốc độ gốc
        moveSpeed *= 1.5f; // Tăng tốc độ
        isSpeedUp = true; // Đánh dấu nhân vật đang tăng tốc

        yield return new WaitForSeconds(10f); // Chờ 10 giây

        isSpeedUp = false; // Đánh dấu nhân vật không còn tăng tốc
        moveSpeed = originalSpeed; // Khôi phục tốc độ ban đầu
    }
    public void Invisible()
    {
        if (!isInvisible)
        {
            StartCoroutine(InvisibleCoroutine());
        }
    }
    private IEnumerator InvisibleCoroutine()
    {
        // Lưu màu gốc
        Color originalColor = spriteRenderer.color;

        // Làm nhân vật trong suốt
        Color transparentColor = originalColor;
        transparentColor.a = 0.3f; // Đặt độ trong suốt (alpha) thành 0.3
        spriteRenderer.color = transparentColor;
        isInvisible = true; // Đánh dấu nhân vật đang tàng hình

        yield return new WaitForSeconds(10f); // Chờ 10 giây

        // Khôi phục màu gốc
        spriteRenderer.color = originalColor;
        isInvisible = false; // Đánh dấu nhân vật không còn tàng hình
    }
}
