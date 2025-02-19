using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    [Serializable]
    public class Question
    {
        public string GrammarQuestion;
        public string[] Choices;
        public int CorrectAnswerIndex;
    }

    public class PopupQuestion : MonoBehaviour
    {
        [SerializeField]
        private GameObject questionPanel;
        [SerializeField]
        private Text questionText;
        [SerializeField]
        private Button[] answerButtons;

        private Action<int> onAnswerSelected;

        public void ShowQuestion(global::Question question, Action<int> callback)
        {
            if (question == null || question.Choices == null || question.CorrectAnswerIndex < 0 || question.CorrectAnswerIndex >= question.Choices.Length)
            {
                Debug.LogError("Error: Invalid question data.");
                return;
            }

            questionPanel.SetActive(true);
            questionText.text = question.GrammarQuestion;
            onAnswerSelected = callback;

            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < question.Choices.Length)
                {
                    answerButtons[i].gameObject.SetActive(true);
                    answerButtons[i].GetComponentInChildren<Text>().text = question.Choices[i];
                    int index = i; // Capture index for listener
                    answerButtons[i].onClick.RemoveAllListeners();
                    answerButtons[i].onClick.AddListener(() => SelectAnswer(index));
                }
                else
                {
                    answerButtons[i].gameObject.SetActive(false);
                }
            }
        }

        private void SelectAnswer(int index)
        {
            questionPanel.SetActive(false);
            if (onAnswerSelected != null)
            {
                onAnswerSelected.Invoke(index);
            }
            else
            {
                Debug.LogError("Error: Callback 'onAnswerSelected' is null.");
            }
        }
    }
}
