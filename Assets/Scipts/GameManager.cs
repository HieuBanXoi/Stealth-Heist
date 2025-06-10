using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverUI; // UI hiển thị khi thua game
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject keyText;
    [SerializeField] private GameObject gameWinUI; // UI hiển thị khi thắng game
    [SerializeField] private GameObject quizUI; // UI hiển thị khi có câu hỏi
    [SerializeField] private GameObject tutorialUI; // UI hiển thị khi có câu hỏi
    [SerializeField] private TextMeshProUGUI keysText; // Hiển thị số lượng chìa khóa
    [SerializeField] private Quiz quizScript; // Script điều khiển quiz
    [SerializeField] private string[] levelScenes; // Danh sách các scene level
    public int numberOfKeys = 0; // Số chìa khóa thu thập được
    private bool isGameOver = false; // Trạng thái game có kết thúc hay chưa
    public static GameManager Instance { get; private set; } // Singleton để truy cập GameManager từ các script khác
    public bool isPlayerHiding = false; // Kiểm tra xem player có đang trốn không
    public bool isWrongAnswer = false; // Kiểm tra xem câu trả lời có sai không
    public int currentLevelIndex = 0; // Chỉ số của level hiện tại
    // public AudioManager audioManager;

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
        // audioManager = FindObjectOfType<AudioManager>();
        gameOverUI.SetActive(false); // Ẩn màn hình thua game lúc đầu
        gameWinUI.SetActive(false); // Ẩn màn hình thắng game lúc đầu
        quizUI.SetActive(false); // Ẩn UI câu hỏi lúc đầu
        pauseMenuUI.SetActive(false); // Ẩn UI tạm dừng game lúc đầu
        currentLevelIndex = GetCurrentLevelIndex();
        // Chỉ hiển thị tutorial khi không phải map 4 hoặc 5
        if (currentLevelIndex < 3) // Vì index bắt đầu từ 0, nên map 4 là index 3, map 5 là index 4
        {
            Debug.Log("<3");
            tutorialUI.SetActive(true); // Hiển thị tutorial UI khi bắt đầu
            Time.timeScale = 0; // Tạm dừng game để hiển thị tutorial
        }
        else
        {
            Debug.Log(">=3");
            tutorialUI.SetActive(false);
            Time.timeScale = 1; // Tiếp tục game bình thường
        }


    }

    private int GetCurrentLevelIndex()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelScenes.Length; i++)
        {
            if (levelScenes[i] == currentScene)
            {
                return i;
            }
        }
        return 0;
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
            Debug.Log(numberOfKeys);
            numberOfKeys += 1; // Tăng số lượng chìa khóa
            UpdateKeyNumber(); // Cập nhật số lượng hiển thị trên UI
        }
    }

    private void UpdateKeyNumber()
    {
        keysText.text = numberOfKeys.ToString(); // Cập nhật số lượng chìa khóa trên UI
    }

    public void GameWin()
    {
        AudioManager.GetInstance().PlaySFX(AudioManager.GetInstance().winClip);
        pauseButton.SetActive(false);
        keyText.SetActive(false);
        gameWinUI.SetActive(true); // Hiển thị UI thắng game
        Time.timeScale = 0; // Dừng game
    }


    public void LoadNextLevel()
    {
        if (currentLevelIndex < levelScenes.Length - 1)
        {
            currentLevelIndex++;
            Time.timeScale = 1; // Tiếp tục thời gian
            SceneManager.LoadScene(levelScenes[currentLevelIndex]);
        }
        else
        {
            // Nếu đã hoàn thành tất cả các level
            LoadMenu();
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void GameOver()
    {
        AudioManager.GetInstance().PlaySFX(AudioManager.GetInstance().loseClip);
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

    public void HideTutorial()
    {
        tutorialUI.SetActive(false);
        Time.timeScale = 1; // Tiếp tục game sau khi ẩn tutorial
    }
}
