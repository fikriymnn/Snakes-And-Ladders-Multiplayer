using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public string GrammarQuestion;
    public string[] Choices;
    public int CorrectAnswerIndex;
}

public class QuestionManager : MonoBehaviour
{
    public List<Question> Questions = new List<Question>();
    private List<Question> availableQuestions;
    private int currentQuestionIndex;

    public void Init()
    {
        // Inisialisasi lain jika diperlukan, misalnya mengisi pertanyaan atau konfigurasi lainnya
        availableQuestions = new List<Question>(Questions);
        ShuffleQuestions();
    }


    private void Start()
    {
        ResetQuestions();
    }

    public void ResetQuestions()
    {
        availableQuestions = new List<Question>(Questions);
        ShuffleQuestions();
    }

    private void ShuffleQuestions()
    {
        for (int i = 0; i < availableQuestions.Count; i++)
        {
            int randomIndex = Random.Range(i, availableQuestions.Count);
            Question temp = availableQuestions[i];
            availableQuestions[i] = availableQuestions[randomIndex];
            availableQuestions[randomIndex] = temp;
        }
    }

    public Question GetRandomQuestion()
    {
        if (availableQuestions.Count == 0)
        {
            Debug.LogWarning("No questions available!");
            return null;
        }

        Question selectedQuestion = availableQuestions[0];
        availableQuestions.RemoveAt(0);
        return selectedQuestion;
    }
}
