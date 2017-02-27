using UnityEngine;
using System.Collections;
using GoMap;
using UnityEngine.UI;

public class MovePokemon : MonoBehaviour
{
	public SpawnManager spawnManager;
    public Coordinates spawnLocation;

    void Start()
    {
        spawnManager.onOriginSet += OnOriginSet;
        spawnManager.onLocationChanged += OnLocationChanged;
    }

    void OnOriginSet (Coordinates currentLocation)
    {
		//Position
		Vector3 currentPosition = currentLocation.convertCoordinateToVector ();
		transform.position = currentPosition;
    }


    void OnLocationChanged (Coordinates currentLocation)
    {
        //Position
		Vector3 lastPosition = transform.position;
        Vector3 currentPosition = currentLocation.convertCoordinateToVector ();
        Vector3 spawnPosition = spawnLocation.convertCoordinateToVector();

        if (lastPosition == Vector3.zero)
        {
			lastPosition = currentPosition;
		}
        //moveAvatar (spawnPosition, spawnPosition);
        transform.position = spawnPosition;
    }

	void moveAvatar (Vector3 lastPosition, Vector3 currentPosition)
    {
		StartCoroutine (move (lastPosition, currentPosition, 0.5f));
    }

	private IEnumerator move(Vector3 lastPosition, Vector3 currentPosition, float time)
    {
		float elapsedTime = 0;
		Vector3 targetDir = currentPosition - lastPosition;
		Quaternion finalRotation = Quaternion.LookRotation (targetDir);

		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(lastPosition, currentPosition, (elapsedTime / time));
			transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation,(elapsedTime / time));

			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}

	void rotateAvatar(Vector3 lastPosition)
    {
        //Orient Avatar
        Vector3 targetDir = transform.position - lastPosition;

		if (targetDir != Vector3.zero)
        {
			transform.rotation = Quaternion.Slerp(
				transform.rotation,
				Quaternion.LookRotation(targetDir),
				Time.deltaTime * 10.0f
			);
		}
	}

    void Update()
    {
        //transform.LookAt(Camera.main.transform); 
    }
}
