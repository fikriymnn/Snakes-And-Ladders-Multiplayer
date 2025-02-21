using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public GameObject panelTutorial;
    public GameObject panelGrammar;
    public GameObject panelCaraMain;

    public void OpenTutorialPanel()
    {
        panelTutorial.SetActive(true);
        panelCaraMain.SetActive(false);
        panelGrammar.SetActive(false);
    }

    public void CloseTutorialPanel()
    {
        panelCaraMain.SetActive(false);
        panelGrammar.SetActive(false);
        panelTutorial.SetActive(false);
    }

    public void OpenGrammarPanel()
    {
        panelGrammar.SetActive(true);
        panelCaraMain.SetActive(false);
    }

    public void CloseGrammarPanel()
    {
        panelCaraMain.SetActive(false);
        panelGrammar.SetActive(false);
    }

    public void OpenCaraBermain()
    {
        panelCaraMain.SetActive(true);
        panelGrammar.SetActive(false);
    }

    public void CloseCaraBermain()
    {
        panelCaraMain.SetActive(false);
        panelGrammar.SetActive(false);
    }
}