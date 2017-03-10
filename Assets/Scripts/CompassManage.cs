using UnityEngine;
using System.Collections;

public class CompassManage : MonoBehaviour
{
	void Start ()
    {
		Input.compass.enabled = true;
	}
	
	void Update ()
    {
		transform.localRotation = Quaternion.Euler(0, 0, transform.eulerAngles.y - Input.compass.trueHeading);
	}
}
