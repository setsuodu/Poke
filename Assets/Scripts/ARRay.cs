using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ARRay : MonoBehaviour
{
    public List<Transform> POIList;
    public Transform Canvas;
    public Image HUD;
    Vector2 ScreenPos;
    public List<Transform> shownList;

    void Start ()
    {
	
	}
	
	void Update ()
    {
        if (POIList.Count > Canvas.childCount)
        {
            for (int i = 0; i < POIList.Count; i++)
            {
                Image go = Instantiate(HUD) as Image;
                go.transform.parent = Canvas;
                go.transform.localScale = new Vector3(1, 1, 1);
                shownList.Add(go.transform);
                //go.transform.localPosition = Camera.main.WorldToScreenPoint(POIList[0].position);
            }
        }

        for (int i = 0; i < shownList.Count; i++)
        {
            shownList[i].position = new Vector3(Camera.main.WorldToScreenPoint(POIList[i].position).x, Camera.main.WorldToScreenPoint(POIList[i].position).y, 0);
        }
        //Debug.Log(Camera.main.WorldToScreenPoint(POIList[i].position));
    }
}
