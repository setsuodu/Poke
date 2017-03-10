using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoCatch : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "target" && hit.transform.name == gameObject.name)
                {
                    PlayerPrefs.SetString("name", gameObject.name);
                    Debug.Log(PlayerPrefs.GetString("name"));
                    SimpleBlit.instance.Capture();
                }
            }
        }
    }
}
