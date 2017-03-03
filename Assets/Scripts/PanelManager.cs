using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public void LoadScene(string sc)
    {
        Application.LoadLevel(sc);
    }

    public GameObject loginPanel, watingPanel, loadingPanel, privacyPanel, dialoguePanel;

    public void WaitingCtrl()
    {
        loginPanel.active = !loginPanel.active;
        watingPanel.active = !watingPanel.active;
        if (!loginPanel.active && watingPanel.active)
        {
            Debug.Log("Now");
        }
    }

    public void PrivacyCtrl()
    {
        privacyPanel.active = !privacyPanel.active;
    }
}
