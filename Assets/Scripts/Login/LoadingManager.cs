using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [Range(0,1f)]
    public float pValue;
    public Image slider;

    //委托，进度条到某点弹出Notice Mask
    //private delegate void ShowNoticeMask();

    void Start ()
    {
		
	}
	
	void Update ()
    {
        slider.fillAmount = pValue;
    }


}
