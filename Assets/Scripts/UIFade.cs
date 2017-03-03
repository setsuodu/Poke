using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum FadeType
{
    ColorMode,
    AlphaMode,
}

public class UIFade : MonoBehaviour
{
    private Graphic g;
    public FadeType fadeType = FadeType.ColorMode;
    public float delay, duration;
    public Color fromColor, toColor;
    public bool isReset;

    //如果是按钮类，使用委托让其完全显示才interactalbe
    private delegate bool ListenValueHandler(float value);
    public bool listener1(float value)
    {
        if (value == 1 && GetComponent<Button>())
        {
            GetComponent<Button>().interactable = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    void Awake()
    {
        g = GetComponent<Graphic>(); // UI.Graphic 视觉UI组件的基类。
        GetComponent<Button>();
        if (GetComponent<Button>())
        {
            GetComponent<Button>().interactable = false;
        }
    }

    void OnEnable()
    {
        if (!isReset) return;
        switch (fadeType)
        {
            case FadeType.ColorMode:
                g.GetComponent<CanvasRenderer>().SetColor(fromColor);
                break;
            case FadeType.AlphaMode:
                g.GetComponent<CanvasRenderer>().SetAlpha(0f); //该赋值必须在Start完成，不能Awake
                break;
        }

        if (Application.isPlaying)
        {
            StartCoroutine(Fade(delay));
        }
    }

    void Start() // 执行
    {
        ListenValueHandler listener = listener1;

        switch (fadeType)
        {
            case FadeType.ColorMode:
                g.GetComponent<CanvasRenderer>().SetColor(fromColor);
                break;
            case FadeType.AlphaMode:
                g.GetComponent<CanvasRenderer>().SetAlpha(0f); //该赋值必须在Start完成，不能Awake
                break;
        }

        if (Application.isPlaying)
        {
            StartCoroutine(Fade(delay));
        }
    }

    IEnumerator Fade(float _time)
    {
        yield return new WaitForSeconds(_time);
        switch (fadeType)
        {
            case FadeType.ColorMode:
                g.CrossFadeColor(toColor, duration, false, true);
                break;
            case FadeType.AlphaMode:
                g.CrossFadeAlpha(1f, duration, false);
                break;
        } 
        //等待duration时间结束，调用委托
        yield return new WaitForSeconds(duration);
        listener1(g.GetComponent<CanvasRenderer>().GetAlpha());
    }
}
