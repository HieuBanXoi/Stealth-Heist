using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI; // UI hiển thị khi thua game
    [SerializeField] private GameObject quizUI; // UI hiển thị khi có câu hỏi
    [SerializeField] private TextMeshProUGUI keysText; // Hiển thị số lượng chìa khóa
    [SerializeField] private Quiz quizScript; // Script điều khiển quiz
    public int numberOfKeys = 0; // Số chìa khóa thu thập được
    private bool isGameOver = false; // Trạng thái game có kết thúc hay chưa
    public static GameManager Instance { get; private set; } // Singleton để truy cập GameManager từ các script khác
    public bool isChestOpened = false; // Kiểm tra xem rương đã được mở chưa
    public bool isPlayerHiding = false; // Kiểm tra xem player có đang trốn không

    private void Awake()
    {
        // Kiểm tra xem đã có GameManager nào tồn tại chưa
        if (Instance == null)
        {
            Instance = this; // Nếu chưa có, đặt GameManager hiện tại làm Instance
        }
        else
        {
            Destroy(gameObject); // Nếu đã có, hủy GameManager mới để tránh trùng lặp
        }
    }

    void Start()
    {
        gameOverUI.SetActive(false); // Ẩn màn hình thua game lúc đầu
        quizUI.SetActive(false); // Ẩn UI câu hỏi lúc đầu
    }

    public void ShowQuiz(QuestionManager question, KeyManager key)
    {
        Time.timeScale = 0; // Tạm dừng game khi hiển thị câu hỏi
        quizUI.gameObject.SetActive(true); // Hiển thị UI câu hỏi
        quizScript.ShowQuestion(question, key); // Gọi hàm hiển thị câu hỏi từ script Quiz
    }

    public void ShowOffQuiz()
    {
        Time.timeScale = 1; // Tiếp tục game sau khi đóng câu hỏi
        quizUI.SetActive(false); // Ẩn UI câu hỏi
    }

    public void AddKey()
    {
        if (!isGameOver)
        {
            numberOfKeys += 1; // Tăng số lượng chìa khóa
            UpdateKeyNumber(); // Cập nhật số lượng hiển thị trên UI
        }
    }

    private void UpdateKeyNumber()
    {
        keysText.text = numberOfKeys.ToString(); // Cập nhật số lượng chìa khóa trên UI
    }

    public void GameOver()
    {
        isGameOver = true; // Đánh dấu game đã kết thúc
        Time.timeScale = 0; // Dừng game
        gameOverUI.SetActive(true); // Hiển thị UI thua game
    }

    public void Restart()
    {
        isGameOver = false; // Reset trạng thái game
        Time.timeScale = 1; // Tiếp tục thời gian
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load lại scene hiện tại để restart game
    }

    public bool IsGameOver()
    {
        return isGameOver; // Trả về trạng thái game over
    }
}
