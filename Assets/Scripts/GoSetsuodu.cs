using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoMap;
using System;
using LitJson;

public class GoSetsuodu : MonoBehaviour
{
    #region Spawn Pokemon
    
    public List<GameObject> prefabList;
    public List<Vector3> spawnLocation;

    void Start()
    {
        StartCoroutine(GetSpawnJson());
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            SpawnManager go = Instantiate(prefabList[i]).GetComponent<SpawnManager>();
            go.demo_CenterWorldCoordinates.latitude = spawnLocation[i].x;
            go.demo_CenterWorldCoordinates.longitude = spawnLocation[i].z;
            go.demo_CenterWorldCoordinates.altitude = spawnLocation[i].y;
            //go.CoordinateWorld();
        }
    }

    //150个Pokemon做成ABs，打包zip。
    //前面Loading界面下载页进行版本比对，下载，解压进本地目录。
    //游戏过程中不进行下载，始终从本地目录WWW.FromCacheOrDownload方法加载。
    //GetSpawnJson后根据名字、坐标instanciate。
    IEnumerator GetSpawnJson()
    {
        WWW www = new WWW(Config.serverUrl);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
        }
        Debug.Log(www.text);
        
        JsonData jd = JsonMapper.ToObject(www.text);
        /*
        for (int i = 0; i < jd.Count; i++)
        {
            SpawnPokemon spawnPokemon = new SpawnPokemon();
            spawnPokemon.name = jd[i]["name"].ToString();
            spawnPokemon.latitude = float.Parse(jd[i]["latitude"].ToString());
            spawnPokemon.Longitude = float.Parse(jd[i]["Longitude"].ToString());
        }*/
        /*
        if(prefabList.Count < jd.Count)
        {
            for (int i = 0; i < jd.Count; i++)
            {
                GameObject go = Resources.Load(jd[i]["name"].ToString()) as GameObject;
                prefabList.Add(go);
                SpawnLocation.Add(Parse(jd[i]["location"].ToString()));
                Debug.Log("name=" + jd[i]["name"]);
                Debug.Log("location=" + jd[i]["location"]);
            }
        }*/
    }

    //json中String GPS坐标转 Vector3
    public static Vector3 Parse(string name)
    {
        string[] s = name.Split(',');
        return new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));
    }

    #endregion

    #region 序列化

    [Serializable]
    public class SpawnPokemon
    {
        public string name;
        public float latitude;
        public float Longitude;
    }

    #endregion
}
