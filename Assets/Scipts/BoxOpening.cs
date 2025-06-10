using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpening : MonoBehaviour
{
    private bool isPlayerNearBy = false; // Kiểm tra xem player có ở gần rương không
    private Animator animator; // Điều khiển animation của rương
    private GameManager gameManager; // Tham chiếu đến GameManager
    [SerializeField] private GameObject player; // Tham chiếu đến player
    [SerializeField] private string boxID; // ID của rương để phân biệt các rương khác nhau
    [SerializeField] private GameObject circleIndicator; // Biểu tượng chấm chỉ thị
    private bool isBoxOpen = false; // Trạng thái mở/đóng của rương

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>(); // Tìm GameManager trong scene
    }

    void Start()
    {
        animator = GetComponent<Animator>(); // Lấy component Animator của rương
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player"); // Tự động tìm player nếu chưa được gán
        }
        if (circleIndicator != null)
        {
            circleIndicator.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Nếu player ở gần và nhấn Space
        {
            if (isBoxOpen)
            {
                CloseBox();
            }
            else
            {
                if (isPlayerNearBy)
                {
                    OpenBox();
                }
            }
        }
    }

    private void OpenBox()
    {
        animator.SetBool("OpenBox", true);
        if (player != null)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Reset hiệu ứng tăng tốc
                if (playerController.isSpeedUp)
                {
                    playerController.moveSpeed = 5f;
                    playerController.isSpeedUp = false;
                }
                // Reset hiệu ứng tàng hình
                if (playerController.isInvisible)
                {
                    SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                    {
                        Color originalColor = spriteRenderer.color;
                        originalColor.a = 1f;
                        spriteRenderer.color = originalColor;
                    }
                    playerController.isInvisible = false;
                }
            }
            player.gameObject.SetActive(false);
        }
        GameManager.Instance.isPlayerHiding = true;
        isBoxOpen = true;
    }

    private void CloseBox()
    {
        animator.SetBool("OpenBox", false); // Tắt animation mở rương
        if (player != null)
        {
            player.gameObject.SetActive(true);
        }
        GameManager.Instance.isPlayerHiding = false; // Thông báo rằng rương đã đóng
        isBoxOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Nếu player chạm vào rương
        {
            isPlayerNearBy = true; // Cập nhật trạng thái player ở gần rương
            if (circleIndicator != null)
            {
                circleIndicator.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Nếu player rời khỏi rương
        {
            isPlayerNearBy = false; // Cập nhật trạng thái player rời đi
            if (circleIndicator != null)
            {
                circleIndicator.SetActive(false);
            }
        }
    }

    // Phương thức để lấy ID của rương
    public string GetBoxID()
    {
        return boxID;
    }

    // Phương thức để kiểm tra trạng thái của rương
    public bool IsBoxOpen()
    {
        return isBoxOpen;
    }
}
