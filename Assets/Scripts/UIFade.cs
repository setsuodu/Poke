using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIFade : MonoBehaviour
{
    private Graphic g;
    public float delay, duration;
    public Color fromColor, toColor;

    void Awake() // 各种初始化
    {
        g = GetComponent<Graphic>(); // UI.Graphic 视觉UI组件的基类。
        g.GetComponent<CanvasRenderer>().SetColor(fromColor);
    }

    void Start() // 执行
    {
        if (Application.isPlaying)
        {
            StartCoroutine(Fade(delay));
        }
    }

    IEnumerator Fade(float _time)
    {
        yield return new WaitForSeconds(_time);
        g.CrossFadeColor(toColor, duration, false, true);
    }
}
