using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoMap;
using System;
using LitJson;

public class Spawn : MonoBehaviour
{
    /*
    [Serializable]
    public class Character
    {
        public string ID;
        public Vector3 SpawnLocation;
    }
    public List<Character> SpawnList;
    */
    public List<GameObject> SpawnList;
    public List<Vector3> SpawnLocation;

    [ContextMenu("Spawn")]
    public void SpawnPokemon()
    {
        for (int i = 0; i < SpawnList.Count; i++)
        {
            SpawnManager go = Instantiate(SpawnList[i]).GetComponent<SpawnManager>();
            go.demo_CenterWorldCoordinates.latitude = SpawnLocation[i].x;
            go.demo_CenterWorldCoordinates.longitude = SpawnLocation[i].z;
            go.demo_CenterWorldCoordinates.altitude = SpawnLocation[i].y;
            //go.CoordinateWorld();
        }
    }

    private float time, lastTime;

    void Update()
    {
        time = Time.fixedTime;
        if (time - lastTime == 2)
        {
            Debug.Log("hello");
            lastTime = time;
            StartCoroutine(GetJson());
        }

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.tag == "target")
                {
                    Debug.Log("pokemon");
                    Application.LoadLevel("CatchScene");
                }
            }
        }
    }

    IEnumerator GetJson()
    {
        WWW www = new WWW("http://www.setsuodu.com/json/spawn.json");
        while (!www.isDone) { yield return new WaitForEndOfFrame(); }
        if (www.error != null) { Debug.LogError(www.error); }
        //Debug.Log(www.text);

        JsonData jd = JsonMapper.ToObject(www.text);

        if(SpawnList.Count < jd.Count)
        {
            for (int i = 0; i < jd.Count; i++)
            {
                GameObject go = Resources.Load(jd[i]["name"].ToString()) as GameObject;
                SpawnList.Add(go);
                SpawnLocation.Add(Parse(jd[i]["location"].ToString()));
                Debug.Log("name=" + jd[i]["name"]);
                Debug.Log("location=" + jd[i]["location"]);
            }
        }
    }

    //json中String GPS坐标转 Vector3
    public static Vector3 Parse(string name)
    {
        string[] s = name.Split(',');
        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
    }
}
