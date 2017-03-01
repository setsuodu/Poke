using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FadeType
{
    UIFadeIn,
    UIFadeOut
}

public class UIFade : MonoBehaviour
{
    private Graphic g;
    public FadeType myFadeType = FadeType.UIFadeIn; //选择Fade类型
    public float delay, duration;

    void Awake() // 各种初始化
    {
        g = GetComponent<Graphic>(); // UI.Graphic 视觉UI组件的基类。

        switch (myFadeType)
        {
            case FadeType.UIFadeIn:
                g.GetComponent<CanvasRenderer>().SetAlpha(0f);
                break;
            case FadeType.UIFadeOut:
                g.GetComponent<CanvasRenderer>().SetAlpha(1f);
                break;
        }
    }

    void Start() // 执行
    {
        if (Application.isPlaying)
        {
            StartCoroutine(FadeIn(delay));
        }
    }

    IEnumerator FadeIn(float _time)
    {
        yield return new WaitForSeconds(_time);
        g.CrossFadeAlpha(1f, duration, false);
    }

    IEnumerator Fadeout(float _time)
    {
        yield return new WaitForSeconds(_time);
        g.CrossFadeAlpha(0f, duration, false);
    }

    #region 递归遍历子物体
    void Example(GameObject obj)
    {
        Recursive(obj);
    }

    private void Recursive(GameObject parentGameObject)
    {
        //Do something you want...
        foreach (Transform child in parentGameObject.transform)
        {
            Recursive(child.gameObject);
            g = child.GetComponent<Graphic>();
        }
    }
    #endregion
}
