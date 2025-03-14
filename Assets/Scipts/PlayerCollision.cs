using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;  // Tham chiếu đến GameManager để quản lý trạng thái trò chơi
    private void Awake()
    {
        // Tìm đối tượng GameManager trong scene
        gameManager = FindAnyObjectByType<GameManager>();
    }

    // Xử lý va chạm giữa người chơi và các đối tượng khác
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra nếu va chạm với đối tượng có tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Gọi hàm GameOver() từ GameManager khi người chơi chạm vào kẻ địch
            gameManager.GameOver();
        }
    }
}
