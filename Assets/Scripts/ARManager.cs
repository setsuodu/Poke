using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ARManager : MonoBehaviour
{
    public List<PlaceInfo> places = new List<PlaceInfo>();
    public PlaceInfo location = new PlaceInfo();
    public GameObject POI;

    public void ShowPlaces()
    {
        ClearPlace();

        for (int i = 0; i < places.Count; i++)
        {
            GameObject newPlace = Instantiate<GameObject>(POI);
            newPlace.transform.parent = transform;
            newPlace.GetComponent<SpawnManager>().demo_CenterWorldCoordinates = new Coordinates(places[i].Latitude, places[i].Longitude, 0);
            newPlace.GetComponent<SpawnManager>().currentLocation = new Coordinates(places[i].Latitude, places[i].Longitude, 0);
            
            newPlace.transform.LookAt(Camera.main.transform.parent);

            newPlace.gameObject.GetComponentInChildren<Text>().text = places[i].Name + " " + places[i].Distance + "米";
        }
    }

    private void ClearPlace()
    {
        GameObject[] oldPlaces = GameObject.FindGameObjectsWithTag("Place");
        for (int i = 0; i < oldPlaces.Length; i++)
        {
            Destroy(oldPlaces[i].gameObject);
        }
    }

    [ContextMenu("Destroy")]
    public void fresh()
    {
        GameObject[] oldPlaces = GameObject.FindGameObjectsWithTag("Place");
        for (int i = 0; i < oldPlaces.Length; i++)
        {
            Destroy(oldPlaces[i].gameObject);
        }
    }

    public Text CameraTrans;
    void Update()
    {
        CameraTrans.text = Camera.main.transform.eulerAngles.ToString();
    }
}
