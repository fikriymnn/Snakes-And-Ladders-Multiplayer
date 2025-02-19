using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public GameObject panelTutorial;
    public GameObject panelGrammar;

    public void OpenTutorialPanel()
    {
        panelTutorial.SetActive(true);
    }

    public void CloseTutorialPanel()
    {
        panelTutorial.SetActive(false);
    }

    public void OpenGrammarPanel()
    {
        panelGrammar.SetActive(true);
    }

    public void CloseGrammarPanel()
    {
        panelGrammar.SetActive(false);
    }
}