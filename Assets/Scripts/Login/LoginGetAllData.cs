using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginGetAllData : MonoBehaviour
{
    public string Server_url = "";
    public string Pname = "";
    public string Ppwd = "";
    public bool isClicked;
    public Text m_content;

    void Start()
    {
        //Server_url = "http://127.0.0.1/logingetdata.php";
        Server_url = "http://www.setsuodu.com/logingetdata.php";
    }

    void OnGUI()
    {
        Pname = GUI.TextField(new Rect(400, 0, 100, 50), Pname);
        Ppwd = GUI.TextField(new Rect(400, 60, 100, 50), Ppwd);

        if (isClicked == false)
        {
            if (GUI.Button(new Rect(400, 150, 100, 100), "LoginAllData"))
            {
                isClicked = true;
                StartCoroutine(RegisterData());
            }
        }
    }

    WWWForm form;
    WWW logindownload;

    IEnumerator RegisterData()
    {
        form = new WWWForm();

        form.AddField("pname", Pname);
        form.AddField("ppwd", Ppwd);

        logindownload = new WWW(Server_url, form);
        yield return logindownload;

        Debug.Log("GetOK: " + logindownload.text);
        m_content.text = logindownload.text;

        isClicked = false;
    }
}