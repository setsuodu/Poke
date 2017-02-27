using UnityEngine;
using System.Collections;

public class CompassManage : MonoBehaviour
{
	void Start ()
    {
		Input.location.Start ();
		Input.compass.enabled = true;
	}
	
	void Update ()
    {
		transform.rotation = Quaternion.Euler(0, 0, transform.eulerAngles.y - Input.compass.trueHeading);

		//transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,Input.compass.magneticHeading, 0),Time.deltaTime*2);
	}
}
