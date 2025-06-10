using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Question", menuName = "Question", order = 0)]
public class QuestionManager : ScriptableObject
{
    [TextArea(2, 5)]
    [SerializeField] private string question;
    [SerializeField] private Sprite questionImage;
    [SerializeField] private string[] answer = new string[4];
    [SerializeField] private int correctAnswer;
    public string GetQuestion()
    {
        return question;
    }
    public Sprite GetQuestionImage()
    {
        return questionImage;
    }
    public string GetAnswer(int index)
    {
        return answer[index];
    }
    public int GetCorrectAnswerIndex()
    {
        return correctAnswer;
    }
}
