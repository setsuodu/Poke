using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    Graphic g;

    void Awake()
    {
        g = GetComponent<Graphic>(); //UI.Graphic 所有视觉UI组件的基类。
    }

    [ContextMenu("FadeIn")]
    void FadeIn()
    {
        g.GetComponent<CanvasRenderer>().SetAlpha(0f);
        g.CrossFadeAlpha(1f, 4f, false);//second param is the time
    }

    [ContextMenu("FadeOut")]
    void Fadeout()
    {
        g.GetComponent<CanvasRenderer>().SetAlpha(1f);
        g.CrossFadeAlpha(0f, 4f, false);
    }
}
