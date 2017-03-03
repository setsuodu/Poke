using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register : UnitySingletonClass<Register>
{
    private string RegUrl, loginUrl;

    void Start()
    {
        RegUrl = "http://www.setsuodu.com/checkreg.php";
        loginUrl = "http://www.setsuodu.com/login.php";
    }

    IEnumerator RegisterData(string user, string pwd)
    {
        WWWForm form = new WWWForm();
        form.AddField("pname", user);
        form.AddField("ppwd", pwd);

        WWW www = new WWW(RegUrl, form);
        yield return www;
        if (www.text != null)
        {
            Debug.Log("Register OK : " + www.text);
        }
    }

    public void doRegister(string user, string pwd)
    {
        StartCoroutine(RegisterData(user, pwd));
    }

    IEnumerator LoginData(string user, string pwd)
    {
        WWWForm form = new WWWForm();
        form.AddField("pname", user);
        form.AddField("ppwd", pwd);

        WWW www = new WWW(loginUrl, form);
        yield return www;
        Debug.Log("Login OK : " + www.text);
        if (www.text == "LoginRight")
        {
            yield return new WaitForSeconds(2f);
            PanelManager.instance.dialogueCtrl();
        }
    }

    public void doLogin(string user, string pwd)
    {
        StartCoroutine(LoginData(user, pwd));
    }
}