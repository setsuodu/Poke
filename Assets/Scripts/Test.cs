using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    public List<Transform> pos;
    public GameObject prefab;
    public List<GameObject> ShowList;

    void Start ()
    {
        GameObject go = Instantiate(prefab) as GameObject;
        go.transform.parent = transform;
        ShowList.Add(go);
	}
	
	void Update ()
    {
        ShowList[0].transform.position = Camera.main.WorldToScreenPoint(pos[0].position);
    }
}
