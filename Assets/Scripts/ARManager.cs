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
            //GameObject newPlace = Instantiate<GameObject>(perfab);
            //newPlace.transform.parent = transform;

            //double posZ = places[i].Latitude - location.Latitude;
            //double posX = places[i].Longitude - location.Longitude;

            //float z = 0;
            //float x = 0;
            //float y = 0;

            //if (posZ > 0)
            //{
            //    z = 500f;
            //}
            //else
            //{
            //    z = -500f;
            //}

            //if (posX > 0)
            //{
            //    x = 500f;
            //}
            //else
            //{
            //    x = -500f;
            //}

            //z = z + (float)(posZ * 1000);
            //x = x + (float)(posX * 1000);
            //y = y + i * 20;

            //newPlace.transform.position = Camera.main.WorldToScreenPoint(new Vector3(x, y, z));
            //newPlace.transform.position = new Vector3(x, y, z);
            //newPlace.transform.LookAt(transform);
            //newPlace.transform.Rotate(new Vector3(0f, 180f, 0f));
            //newPlace.gameObject.GetComponentInChildren<Text>().text = places[i].Name + " " + places[i].Distance + "米";

            GameObject newPlace = Instantiate<GameObject>(POI);
            newPlace.transform.parent = transform;
            newPlace.GetComponent<SpawnManager>().demo_CenterWorldCoordinates = new Coordinates(places[i].Latitude, places[i].Longitude, 0);
            newPlace.GetComponent<SpawnManager>().currentLocation = new Coordinates(places[i].Latitude, places[i].Longitude, 0);
            //newPlace.GetComponent<MovePokemon>().spawnLocation = new Coordinates(places[i].Latitude, places[i].Longitude, 0);
            
            newPlace.transform.LookAt(Camera.main.transform.parent);
            //newPlace.transform.Rotate(new Vector3(0f, 0f, 0f));

            //float distance = Vector3.Distance(newPlace.transform.position, transform.position) / 100;
            //newPlace.transform.localScale = new Vector3(distance, distance, distance);
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
