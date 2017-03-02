using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using cn.sharesdk.unity3d; //导入ShareSDK

public class ShareDemo : MonoBehaviour
{
    private ShareSDK shareSdk;
    public Text message;
    QQUser qqUser = new QQUser();

    void Start()
    {
        shareSdk = GetComponent<ShareSDK>();
        shareSdk.shareHandler += ShareResultHandler; //分享回调事件
        shareSdk.authHandler += AuthResultHandler; //授权回调事件
        shareSdk.showUserHandler += GetUserInfoResultHandler; //用户信息事件
    }

    public void LoadScene(string sc)
    {
        Application.LoadLevel(sc);
    }

    public GameObject privacyPanel;

    public void ShowPrivacy()
    {
        privacyPanel.SetActive(true);
    }

    public void ClosePrivacy()
    {
        privacyPanel.SetActive(false);
    }

    #region 分享

    public void OnShareClick()
    {
        ShareContent content = new ShareContent();

        //这个地方要参考不同平台需要的参数    可以看ShareSDK提供的   分享内容参数表.docx
        content.SetText("快来和我一起玩这个游戏吧！");                            //分享文字
        content.SetImageUrl("https://f1.webshare.mob.com/code/demo/img/4.jpg");   //分享图片
        content.SetTitle("标题title");                                            //分享标题
        content.SetTitleUrl("http://www.qq.com");
        content.SetSite("Mob-ShareSDK");
        content.SetSiteUrl("http://www.mob.com");
        content.SetUrl("http://www.sina.com");                                    //分享网址
        content.SetComment("描述");
        content.SetMusicUrl("http://up.mcyt.net/md5/53/OTg1NjA5OQ_Qq4329912.mp3");//分享类型为音乐时用
        content.SetShareType(ContentType.Webpage);
        //shareSdk.ShowPlatformList(null, content, 100, 100);                      //弹出分享菜单选择列表
        shareSdk.ShowShareContentEditor(PlatformType.QQ, content);                 //指定平台直接分享
    }

    // 分享结果回调
    void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {   
        if (state == ResponseState.Success) //成功
        {
            message.text = "share result :";
            message.text = MiniJSON2.jsonEncode(result);
        }
        else if (state == ResponseState.Fail) //失败
        {
            message.text = "fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"];
        }
        else if (state == ResponseState.Cancel) //取消，按下返回键
        {
            message.text = "cancel !";
        }
    }

    #endregion

    #region 登陆授权

    public void OnAuthClick()
    {
        //请求QQ授权，请求这个授权是为了获取用户信息来第三方登录
        shareSdk.Authorize(PlatformType.QQ);
    }

    //授权结果回调
    void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            message.text = "authorize success !";
            shareSdk.GetUserInfo(type); //授权成功的话，获取用户信息
        }
        else if (state == ResponseState.Fail)
        {
            message.text = "fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"];
        }
        else if (state == ResponseState.Cancel)
        {
            message.text = "cancel !";
        }
    }

    //获取用户信息
    void GetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            //获取成功的话，可以写一个类放不同平台的结构体，用PlatformType来判断，用户的Json转化成结构体，来做第三方登录。
            switch (type)
            {
                case PlatformType.QQ:
                    message.text = (MiniJSON2.jsonEncode(result));  //Json

                    //完整的json内容作为string传入，得到整个UserInfo结构/类
                    qqUser = JsonUtility.FromJson<QQUser>(MiniJSON2.jsonEncode(result));
                    message.text = qqUser.nickname;
                    Debug.Log("nikename : " + qqUser.nickname);
                    //php数据库交互，如果数据库里没有，新建用户，→登陆。
                    //如果有，→登陆。
                    //ToDo it...

                    break;
            }
        }
        else if (state == ResponseState.Fail)
        {
            message.text = ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
        }
        else if (state == ResponseState.Cancel)
        {
            message.text = ("cancel !");
        }
    }

    #endregion

    #region JSON 序列化

    [Serializable]
    struct QQUser //QQ用户信息结构体
    {
        public string yellow_vip_level;
        public string msg;
        public string province;
        public string gender;
        public string is_yellow_year_vip;
        public int is_lost;
        public string nickname;
        public int ret;
        public string level;
        public string city;
        public string figureurl;
        public string figureurl_1;
        public string figureurl_2;
        public string figureurl_qq_1;
        public string figureurl_qq_2;
        public string vip;
        public string is_yellow_vip;
    }

    #endregion
}
