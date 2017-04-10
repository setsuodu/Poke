using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelManager : UnitySingletonClass<PanelManager>
{
    public GameObject loginPanel, watingPanel, loadingPanel, privacyPanel, dialoguePanel;

    public void LoadScene(string sc)
    {
        SceneManager.LoadScene(sc);
    }

    public void dialogueCtrl()
    {
        dialoguePanel.active = !dialoguePanel.active;
    }
}
