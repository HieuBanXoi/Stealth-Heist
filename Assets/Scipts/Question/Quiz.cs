using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textQuiz; // Hiển thị câu hỏi
    [SerializeField] private GameObject[] buttonAnswer; // Danh sách các nút đáp án
    [SerializeField] private Sprite defaultAnswerSprite; // Ảnh mặc định cho đáp án
    [SerializeField] private Sprite correctAnswerSprite; // Ảnh cho đáp án đúng
    [SerializeField] private Sprite wrongAnswerSprite; // Ảnh cho đáp án sai

    private GameManager gameManager; // Tham chiếu đến GameManager
    private KeyManager currentKey; // Lưu key liên quan đến câu hỏi
    private QuestionManager currentQuestion; // Lưu câu hỏi hiện tại

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>(); // Tìm GameManager trong scene
    }

    public void ShowQuestion(QuestionManager question, KeyManager key) // Hiển thị câu hỏi và đáp án
    {
        currentQuestion = question; // Lưu câu hỏi hiện tại
        currentKey = key; // Lưu key liên quan đến câu hỏi

        textQuiz.text = currentQuestion.GetQuestion(); // Hiển thị câu hỏi lên giao diện
        for (int i = 0; i < buttonAnswer.Length; i++) // Gán đáp án vào các nút
        {
            TextMeshProUGUI buttonText = buttonAnswer[i].GetComponentInChildren<TextMeshProUGUI>(); // Lấy text trong button
            buttonText.text = currentQuestion.GetAnswer(i); // Gán nội dung đáp án
        }
    }

    public void OnSelectedAnswer(int index) // Xử lý khi người chơi chọn đáp án
    {
        if (index == currentQuestion.GetCorrectAnswerIndex()) // Kiểm tra nếu đáp án đúng
        {
            gameManager.AddKey(); // Thêm key vào GameManager
            currentKey.RemoveKey(); // Xóa key khỏi scene
        }
        else
        {
            currentKey.ResetKey(); // Reset key nếu trả lời sai
        }
        gameManager.ShowOffQuiz(); // Đóng bảng câu hỏi
    }
}
