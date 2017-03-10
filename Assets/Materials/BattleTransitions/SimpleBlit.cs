using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SimpleBlit : UnitySingletonClass<SimpleBlit>
{
    public Material TransitionMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (TransitionMaterial != null)
            Graphics.Blit(src, dst, TransitionMaterial);
    }
    
    #region Classic Pokemon Loading Effect

    public Texture2D[] loadPatterns;
    public float value;
    bool turnOn;

    public Action<float> TransitionMat;

    void SetTransitionMat(float _value)
    {
        TransitionMaterial.SetFloat("_Cutoff", _value);
    }

    void OnEnable()
    {
        TransitionMat += SetTransitionMat; //多播委托，装载
    }

    void OnDisable()
    {
        TransitionMat -= SetTransitionMat; //多播委托，卸载
    }

    void Start()
    {
        TransitionMat(value); //value = 0
    }

    void Update()
    {
        if(turnOn && value < 1)
        {
            value += Time.deltaTime;
            TransitionMat(value); //名称+() 表示执行Action
        }
    }

    public void Capture()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        turnOn = true;
        int t = UnityEngine.Random.Range(0, loadPatterns.Length);
        TransitionMaterial.SetTexture("_TransitionTex", loadPatterns[t]);
        yield return new WaitUntil(() => value >= 1);
        SceneManager.LoadSceneAsync("2.CatchScene");
    }

    #endregion
}
