using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : UnitySingletonClass<PanelManager>
{
    public GameObject loginPanel, watingPanel, loadingPanel, privacyPanel, dialoguePanel;

    public void LoadScene(string sc)
    {
        Application.LoadLevel(sc);
    }

    public void WaitingCtrl(bool isStart)
    {
        loginPanel.active = !isStart;
        watingPanel.active = isStart;
    }

    public void dialogueCtrl()
    {
        dialoguePanel.active = !dialoguePanel.active;
    }
}
