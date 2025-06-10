using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour
{
    private GameManager gameManager;  // Tham chiếu đến GameManager để quản lý game
    private BoxCollider2D boxCollider2D;  // Collider để xác định khi nào người chơi chạm vào chìa khóa
    [SerializeField] private QuestionManager question;  // Câu hỏi sẽ hiển thị khi nhặt chìa khóa
    [SerializeField] public float timeToReturnKey = 5f;  // Thời gian chờ để chìa khóa xuất hiện lại
    private float timeValue;  // Giá trị thể hiện mức độ đầy của thanh thời gian (từ 0 đến 1)
    public float fillFraction = 1;  // Biến kiểm tra xem thời gian đã hết hay chưa
    public bool timeOver = true;  // Thanh thời gian hiển thị trên UI
    private Image timerImage;  // Renderer của chìa khóa để thay đổi độ trong suốt
    private SpriteRenderer spriteRenderer; // Renderer của chìa khóa để thay đổi độ trong suốt
    private Color color; // Màu của chìa khóa để thay đổi độ trong suốt

    void Awake()
    {
        // Lấy các component cần thiết
        boxCollider2D = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        timerImage = GetComponentInChildren<Image>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Start()
    {
        // Lưu màu gốc của chìa khóa
        color = spriteRenderer.color;
    }

    void Update()
    {
        // Nếu thời gian chưa hết, cập nhật thanh thời gian
        if (!timeOver)
        {
            timerImage.fillAmount = fillFraction;
            UpdateTimer();
        }
        else
        {
            // Khi hết thời gian, ẩn thanh thời gian và hiển thị chìa khóa
            timerImage.fillAmount = 0;
            color.a = 1f;
            spriteRenderer.color = color;
        }
    }

    // Khi người chơi nhặt được chìa khóa
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Vô hiệu hóa collider để tránh bị nhặt nhiều lần
            boxCollider2D.enabled = false;

            // Hiển thị câu hỏi từ GameManager
            gameManager.ShowQuiz(question, this);
        }
    }

    // Xóa chìa khóa khỏi game khi người chơi nhặt nó
    public void RemoveKey()
    {
        Destroy(this.gameObject);
    }

    // Ẩn chìa khóa và bắt đầu đếm ngược để xuất hiện lại
    public void ResetKey()
    {
        StartTimer();
        color.a = 0f;  // Làm chìa khóa trở nên vô hình
        spriteRenderer.color = color;
    }

    // Cập nhật bộ đếm thời gian cho chìa khóa
    private void UpdateTimer()
    {
        if (timeValue > 0)
        {
            // Giảm dần thanh thời gian dựa trên thời gian còn lại
            fillFraction = 1 - (timeValue / timeToReturnKey);
        }
        else
        {
            // Khi hết thời gian, bật lại collider và đánh dấu thời gian đã hết
            timeOver = true;
            boxCollider2D.enabled = true;
        }

        // Trừ dần thời gian mỗi frame
        timeValue -= Time.deltaTime;
    }

    // Bắt đầu bộ đếm thời gian cho chìa khóa
    private void StartTimer()
    {
        timeValue = timeToReturnKey;
        timeOver = false;
    }
    public void SetQuestion(QuestionManager newQuestion)
    {
        question = newQuestion;
    }
}
