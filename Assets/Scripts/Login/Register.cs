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

    #region 注册

    IEnumerator RegisterData(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW(RegUrl, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.text))
        {
            Debug.Log("Register OK : " + www.text);
        }
    }

    public void doRegister(string username, string password)
    {
        StartCoroutine(RegisterData(username, password));
    }

    #endregion

    #region 登录

    IEnumerator LoginData(string username, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW(loginUrl, form);
        yield return www;
        Debug.Log("Login OK : " + www.text);

        switch (www.text)
        {
            case "LoginRight":
                yield return new WaitForSeconds(2f);
                PanelManager.instance.dialogueCtrl();
                yield break;
        }
    }

    public void doLogin(string username, string password)
    {
        StartCoroutine(LoginData(username, password));
    }

    #endregion

}