using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Register : UnitySingletonClass<Register>
{
    public Animator tips;
    public InputField regUser, regPwd, loginUser, loginPwd;
    public Text loginState;

    #region 注册

    IEnumerator RegisterData(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            loginState.text = "未填写用户名或密码";
            yield return new WaitForSeconds(2f);
            loginState.text = "";
            tips.SetBool("isOn", false);
            yield break;
        }

        WWWForm form = new WWWForm();
        form.AddField("action", "regist");
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW(Config.registUrl, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        loginState.text = www.text;

        switch (www.text)
        {
            case "success":
                loginState.text = "注册成功";
                Debug.Log("注册成功");
                break;
            case "exist":
                loginState.text = "用户名已被注册";
                Debug.Log("用户名已被注册");
                break;
            case "error_sql":
                loginState.text = "数据库链接失败";
                Debug.Log("数据库链接失败");
                break;
        }

        yield return new WaitForSeconds(2f);
        loginState.text = "";
        tips.SetBool("isOn", false);
    }

    public void doRegister()
    {
        StartCoroutine(RegisterData(regUser.text, regPwd.text));
        tips.SetBool("isOn", true);
    }

    public void doRegister(string username, string password)
    {
        StartCoroutine(RegisterData(username, password));
        tips.SetBool("isOn", true);
    }

    #endregion

    #region 登录

    IEnumerator LoginData(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            loginState.text = "未填写用户名或密码";
            yield return new WaitForSeconds(2f);
            loginState.text = "";
            tips.SetBool("isOn", false);
            yield break;
        }

        //GameManager.instance.AsyncScene("1.MapScene");
        //StartCoroutine(GameManager.instance.SetUserBase(loginUser.text, loginPwd.text));

        WWWForm form = new WWWForm();
        form.AddField("action", "login");
        form.AddField("username", username);
        form.AddField("password", password);

        WWW www = new WWW(Config.registUrl, form);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        loginState.text = www.text;

        switch (www.text)
        {
            case "success":
                loginState.text = "登录成功";
                Debug.Log("登录成功");
                //GameManager.instance.LoadScene("Loading");
                yield return new WaitForSeconds(2f);
                PanelManager.instance.dialogueCtrl();
                break;
            case "error":
                loginState.text = "用户名或密码错误";
                Debug.Log("用户名或密码错误");
                break;
            default:
                Debug.Log("default");
                break;
        }

        yield return new WaitForSeconds(2f);
        loginState.text = "";
        tips.SetBool("isOn", false);
    }

    public void doLogin()
    {
        StartCoroutine(LoginData(loginUser.text, loginPwd.text));
        tips.SetBool("isOn", true);
    }

    public void doLogin(string username, string password)
    {
        StartCoroutine(LoginData(username, password));
        tips.SetBool("isOn", true);
    }

    #endregion

}