using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestOpening : MonoBehaviour
{
    private bool isPlayerNearBy = false; // Kiểm tra xem player có ở gần rương không
    private Animator animator; // Điều khiển animation của rương
    private GameManager gameManager; // Tham chiếu đến GameManager

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>(); // Tìm GameManager trong scene
    }

    void Start()
    {
        animator = GetComponent<Animator>(); // Lấy component Animator của rương
    }

    void Update()
    {
        if (isPlayerNearBy && Input.GetKeyDown(KeyCode.Space)) // Nếu player ở gần và nhấn Space
        {
            animator.SetBool("OpenBox", true); // Bật animation mở rương
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Nếu player chạm vào rương
        {
            isPlayerNearBy = true; // Cập nhật trạng thái player ở gần rương
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Nếu player rời khỏi rương
        {
            isPlayerNearBy = false; // Cập nhật trạng thái player rời đi
        }
    }
}
