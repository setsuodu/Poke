using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using cn.sharesdk.unity3d; //导入ShareSDK

public class ShareDemo : MonoBehaviour
{
    private ShareSDK ssdk;
    QQUser qqUser = new QQUser();
    AuthInfo authInfo = new AuthInfo();
    public Text message;

    void Start()
    {
        ssdk = GetComponent<ShareSDK>();
        ssdk.shareHandler += ShareResultHandler; //分享回调事件
        ssdk.authHandler += AuthResultHandler; //授权回调事件
        ssdk.showUserHandler += GetUserInfoResultHandler; //用户信息事件
    }

    void QQRegist()
    {
        string regName = Md5Sum(authInfo.token); 

        //注册 -> 登录，账户为token，密码为token的MD5值
        Register.instance.doRegister(authInfo.token, regName);
        Register.instance.doLogin(authInfo.token, regName);
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
        ssdk.ShowShareContentEditor(PlatformType.QQ, content);                 //指定平台直接分享
    }

    // 分享结果回调
    void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {   
        if (state == ResponseState.Success) //成功
        {
            message.text = "share result :";
            message.text = MiniJSON.jsonEncode(result);
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
        ssdk.Authorize(PlatformType.QQ);
    }

    //授权结果回调
    void AuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
    {
        if (state == ResponseState.Success)
        {
            message.text = "authorize success !";
            ssdk.GetUserInfo(type); //授权成功的话，获取用户信息
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
                    Hashtable info = ssdk.GetAuthInfo(PlatformType.QQ);
                    authInfo = JsonUtility.FromJson<AuthInfo>(MiniJSON.jsonEncode(info));
                    Debug.Log("userGender " + authInfo.userGender);
                    QQRegist();
                    break;
                case PlatformType.WeChat:
                    //TODO...
                    Debug.Log("WeChat " + MiniJSON.jsonEncode(result));
                    break;
                case PlatformType.SinaWeibo:
                    //TODO...
                    break;
            }
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

    struct AuthInfo //QQ用户信息结构体
    {
        public string openID; //同一个QQ号码在不同的应用中有不同的OpenID。//""
        public int expiresIn; //7776000
        public string userGender; //m
        public string tokenSecret; //""
        public string userID; //9629982F7B21F80465B028970A9D0123
        public string unionID; //""
        public int expiresTime; //-1
        public string userName; //せつ うとう
        public string token; //D85D6F250385F69C614A157166F4A05C
        public string userIcon; //http://q.qlogo.cn/qqapp/1105917797/9629982F7B21F80465B028970A9D0123/40
    }

    //MD5加密
    public static string Md5Sum(string strToEncrypt)
    {
        byte[] bs = UTF8Encoding.UTF8.GetBytes(strToEncrypt);
        System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();

        byte[] hashBytes = md5.ComputeHash(bs);

        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }
    #endregion
}
