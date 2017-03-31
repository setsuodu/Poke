using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoShared;
using System;
using LitJson;

public class GoSetsuodu : MonoBehaviour
{
    #region Spawn Pokemon
    
    public List<SpawnPokemon> spawnPokemonList;

    void Start()
    {
        StartCoroutine(GetSpawnJson());
    }

    [ContextMenu("Spawn")]
    public void Spawn()
    {
        GameObject go = Instantiate(Resources.Load<GameObject>("Pikachu"));
        go.transform.SetParent(this.transform);
        go.name = "Pikachu";
        //Coordinates coordinates = new Coordinates(Input.location.lastData.latitude, Input.location.lastData.longitude, 0);
        //go.transform.localPosition = coordinates.convertCoordinateToVector();
        go.transform.localPosition = new Vector3(0,0,0);
        go.transform.localEulerAngles = new Vector3(0, 180, 0);
        go.transform.localScale = new Vector3(50, 50, 50);
    }

    //150个Pokemon做成ABs，打包zip。
    //前面Loading界面下载页进行版本比对，下载，解压进本地目录。
    //游戏过程中不进行下载，始终从本地目录WWW.FromCacheOrDownload方法加载。← 从这里开始实现
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

        for (int i = 1; i < jd.Count; i++) //int从1开始，避免目前的重名载入不实例化bug
        {
            SpawnPokemon spawnPokemon = new SpawnPokemon();
            spawnPokemon.name = jd[i]["name"].ToString();
            spawnPokemon.latitude = float.Parse(jd[i]["latitude"].ToString());
            spawnPokemon.longitude = double.Parse(jd[i]["longitude"].ToString());
            spawnPokemonList.Add(spawnPokemon);
            GameObject go = Instantiate(Resources.Load<GameObject>(spawnPokemon.name));
            go.transform.SetParent(this.transform);
            go.name = spawnPokemon.name;
            Coordinates coordinates = new Coordinates(spawnPokemon.latitude, spawnPokemon.longitude, 0);
            go.transform.localPosition = coordinates.convertCoordinateToVector();
            go.transform.localEulerAngles = new Vector3(0,180,0);
            go.transform.localScale = new Vector3(50,50,50);
        }
        Spawn();
    }

    #endregion

    #region 序列化

    [Serializable]
    public class SpawnPokemon
    {
        public string name;
        public double latitude;
        public double longitude;
    }

    #endregion
}
