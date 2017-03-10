using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoCatch : MonoBehaviour
{
    private AudioSource audio;
    public AudioClip roar, goCatch;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

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
                    audio.clip = goCatch;
                    audio.Play();
                    PlayerPrefs.SetString("name", gameObject.name);
                    SimpleBlit.instance.Capture();
                }
            }
        }
    }
}
