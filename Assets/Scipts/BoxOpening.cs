using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOpening : MonoBehaviour
{
    private bool isPlayerNearBy = false; // Kiểm tra xem player có ở gần rương không
    private bool isHiding = false; // Kiểm tra xem player có ở gần rương không
    private Animator animator; // Điều khiển animation của rương
    private GameManager gameManager; // Tham chiếu đến GameManager
    [SerializeField] private GameObject player;

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
        if (Input.GetKeyDown(KeyCode.Space)) // Nếu player ở gần và nhấn Space
        {
            if (isHiding)
            {
                isHiding = false;
                animator.SetBool("OpenBox", false); // Bật animation mở rương
                player.gameObject.SetActive(true);
            }
            else
            {
                if (isPlayerNearBy)
                {
                    isHiding = true;
                    animator.SetBool("OpenBox", true); // Bật animation mở rương
                    player.gameObject.SetActive(false);
                }

            }
        }
    }

    public bool PlayerIsHiding()
    {
        return isHiding;
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
