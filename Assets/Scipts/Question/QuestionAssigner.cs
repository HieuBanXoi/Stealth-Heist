using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionAssigner : MonoBehaviour
{
    [SerializeField] private QuestionManager[] availableQuestions; // Danh sách các câu hỏi có thể gán
    private List<QuestionManager> unusedQuestions = new List<QuestionManager>(); // Danh sách câu hỏi chưa sử dụng

    void Start()
    {
        ResetUnusedQuestions();
    }

    void ResetUnusedQuestions()
    {
        unusedQuestions.Clear();
        if (availableQuestions != null && availableQuestions.Length > 0)
        {
            unusedQuestions.AddRange(availableQuestions);
        }
    }

    public QuestionManager GetRandomQuestion()
    {
        if (unusedQuestions.Count == 0)
        {
            // Nếu đã hết câu hỏi, reset lại danh sách
            ResetUnusedQuestions();
        }

        if (unusedQuestions.Count > 0)
        {
            // Chọn ngẫu nhiên một câu hỏi từ danh sách chưa sử dụng
            int randomIndex = Random.Range(0, unusedQuestions.Count);
            QuestionManager selectedQuestion = unusedQuestions[randomIndex];

            // Xóa câu hỏi khỏi danh sách chưa sử dụng
            unusedQuestions.RemoveAt(randomIndex);

            return selectedQuestion;
        }

        return null;
    }
}