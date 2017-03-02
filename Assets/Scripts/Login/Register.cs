using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Register : MonoBehaviour
{
    public string RegURL = "";
    public string Pname = "";
    public string Ppwd = "";
    public bool isClicked;
    //public string TempMessage = "--Stage--";

    public Text m_content;

    void Start()
    {
        //RegURL = "http://127.0.0.1/checkreg.php";
        RegURL = "http://www.setsuodu.com/checkreg.php";
    }

    void OnGUI()
    {
        Pname = GUI.TextField(new Rect(0, 0, 100, 50), Pname);
        Ppwd = GUI.TextField(new Rect(0, 60, 100, 50), Ppwd);

        if (isClicked == false)
        {
            if (GUI.Button(new Rect(0, 150, 100, 100), "Register"))
            {
                isClicked = true;
                StartCoroutine(RegisterData());
            }
        }
    }

    WWWForm form;
    WWW download;

    IEnumerator RegisterData()
    {
        form = new WWWForm();
        form.AddField("pname", Pname);
        form.AddField("ppwd", Ppwd);

        download = new WWW(RegURL, form);
        yield return download;

        Debug.Log("OK - - " + download.text);
        m_content.text = download.text;
    }
}