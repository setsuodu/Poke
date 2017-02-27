using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustAnchor : UnitySingletonClass<AdjustAnchor>
{
    public List<Transform> childList;
    public GameObject placeholder;

    void Start()
    {

    }
	
	public void getOrder()
    {
        foreach (Transform child in this.transform)
        {
            childList.Add(child);
        }

        while (childList.Count < 4) //循环，自动补充placeholder
        {
            GameObject go = Instantiate(placeholder);
            go.transform.parent = this.transform;
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localScale = new Vector3(1, 1, 1);
            childList.Add(go.transform);
        }

        if (childList.Count > 4)
        {
            //处理uGUI的Scroll content自动加载后，自适应的通用方法
            GetComponent<RectTransform>().anchorMin = new Vector2(0, (4 - childList.Count) * 0.25f);
            GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        }
    }
}
