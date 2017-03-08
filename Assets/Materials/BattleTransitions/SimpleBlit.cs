using UnityEngine;
using System;
using System.Collections;

public class SimpleBlit : MonoBehaviour
{
    public Material TransitionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (TransitionMaterial != null)
            Graphics.Blit(src, dst, TransitionMaterial);
    }
    /*
    #region Classic Pokemon loading effect
    public Texture2D[] loadPatterns;
    public float value;

    void OnEnable()
    {
        TransitionMaterial.SetFloat("_Cutoff", 0);
    }

    void Update()
    {
        value = Mathf.Lerp(0, 1, Time.deltaTime);
    }

    public void Capture()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        int t = UnityEngine.Random.Range(0, loadPatterns.Length);
        TransitionMaterial.SetTexture("_TransitionTex", loadPatterns[t]);
        TransitionMaterial.SetFloat("_Cutoff", value);

        yield return new WaitForSeconds(1f);

        AsyncOperation async = Application.LoadLevelAsync("2.CatchScene");

        yield return async;
    }
    #endregion
    */

    private delegate void PrintString();
    static void PrintStr(PrintString print) { }

    void Start()
    {
        //方法装载进delegate
        PrintString method = Method1; //委托类型要先初始化才能使用，赋值，赋的值是PrintString结构相同的函数。
        method += Method2; //method是变量，可以再赋值。
        //执行delegate
        PrintStr(method);
    }

    static void Method1()
    {
        Debug.Log("Method1");
    }

    static void Method2()
    {
        Debug.Log("Method2");
    }
}
