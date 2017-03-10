using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class UIBlock : MonoBehaviour
{
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Debug.Log("是UI");
            return;
        }
        /*
        else
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.gameObject.tag != "Untagged")
                    {
                        //Debug.Log(hit.transform.name);
                    }
                }
            }
            
        }
        */
    }
}