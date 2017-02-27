using Assets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

namespace GoMap
{
	[ExecuteInEditMode]
	[System.Serializable]
	public class GOMap : MonoBehaviour 
	{
		public LocationManager locationManager;
		public int tileBuffer = 2;
		public int zoomLevel = 0;
		public bool useCache = true;
		public string mapzen_api_key = "";
		public Material tileBackground;
		[HideInInspector]
		public GameObject tempTileBackgorund;
		public Layer[] layers;
		public bool dynamicLoad = true;
		public GOTileEvent OnTileLoad;


		Vector2 Center_tileCoords;
		[HideInInspector]
		public List <GOTile> tiles = new List<GOTile>();

		//features ID
		[HideInInspector]
		public IList buildingsIds = new List<object>();

		void Awake () 
	    {
			locationManager.onOriginSet += OnOriginSet;
			locationManager.onLocationChanged += OnLocationChanged;

			if (zoomLevel == 0) {
				zoomLevel = locationManager.zoomLevel;	
			}

			if (mapzen_api_key == null || mapzen_api_key == "") {
				Debug.Log ("GOMap - Mapzen api key is missing, GET iT HERE: https://mapzen.com/developers");
			}
			#if UNITY_WEBPLAYER
				Debug.LogError ("GOMap is NOT supported in the webplayer! Please switch platform in the build settings window.");
			#endif		
	    }

		void Start() {
			if (tileBackground != null && Application.isMobilePlatform) {
				CreateTemporaryMapBackground ();
			}
		}

		public IEnumerator ReloadMap (Coordinates location, bool delayed) {

			if (!dynamicLoad) {
				yield break;
			}
				
			//Get SmartTiles
			List <Vector2> tileList = location.adiacentNTiles(zoomLevel,tileBuffer);
				
			List <GOTile> newTiles = new List<GOTile> ();

			// Create new tiles
			foreach (Vector2 tileCoords in tileList) {

				if (!isSmartTileAlreadyCreated (tileCoords, zoomLevel)) {
					
					GOTile adiacentSmartTile = createSmartTileObject (tileCoords, zoomLevel);
					adiacentSmartTile.tileCenter = new Coordinates (tileCoords, zoomLevel);
					adiacentSmartTile.diagonalLenght = adiacentSmartTile.tileCenter.diagonalLenght(zoomLevel);
					adiacentSmartTile.gameObject.transform.position = adiacentSmartTile.tileCenter.convertCoordinateToVector();

					newTiles.Add (adiacentSmartTile);

					if (tileBackground != null) {
						CreateTileBackground (adiacentSmartTile);
					}
				}
			}

			foreach (GOTile tile in newTiles) {

				if (OnTileLoad != null) {
					OnTileLoad.Invoke(tile);
				}

				#if !UNITY_WEBPLAYER
				if (tile != null && FileHandler.Exist (tile.gameObject.name) && useCache) {
					yield return tile.StartCoroutine(tile.LoadTileData(this, tile.tileCenter, zoomLevel,layers,delayed));
				} else {
					tile.StartCoroutine(tile.LoadTileData(this, tile.tileCenter, zoomLevel,layers,delayed));
				}
	
	
				#endif
			}
				
			//Destroy far tiles
			List <Vector2> tileListForDestroy = location.adiacentNTiles(zoomLevel,tileBuffer+1);
			yield return StartCoroutine (DestroyTiles(tileListForDestroy));

		}

		IEnumerator DestroyTiles (List <Vector2> list) {

			try {
				List <string> tileListNames = new List <string> ();
				foreach (Vector2 v in list) {
					tileListNames.Add (v.x + "-" + v.y + "-" + zoomLevel);
				}

				List <GOTile> toDestroy = new List<GOTile> ();
				foreach (GOTile tile in tiles) {
					if (!tileListNames.Contains (tile.name)) {
						toDestroy.Add (tile);
					}
				}
				for (int i = 0; i < toDestroy.Count; i++) {
					GOTile tile = toDestroy [i];
					tiles.Remove (tile);
					GameObject.Destroy (tile.gameObject,i);
				}
			} catch  {
				
			}


			yield return null;
		}

		void OnLocationChanged (Coordinates currentLocation) {
			StartCoroutine(ReloadMap (currentLocation,true));
		}

		void OnOriginSet (Coordinates currentLocation) {
			if (tileBackground != null && Application.isMobilePlatform) {
				DestroyTemporaryMapBackground ();
			}
			StartCoroutine(ReloadMap (currentLocation,false));
		}

		bool isSmartTileAlreadyCreated (Vector2 tileCoords, int Zoom) {
			
			string name = tileCoords.x+ "-" + tileCoords.y+ "-" + zoomLevel;
			return transform.Find (name);
		}

		GOTile createSmartTileObject (Vector2 tileCoords, int Zoom) {
		
			GameObject tileObj = new GameObject(tileCoords.x+ "-" + tileCoords.y+ "-" + zoomLevel);
			tileObj.transform.parent = gameObject.transform;
			GOTile tile = tileObj.AddComponent<GOTile> ();

			tiles.Add(tile);
			return tile;
		}

		public void dropPin(double lat, double lng, GameObject go) {

			Transform pins = transform.FindChild ("Pins");
			if (pins == null) {
				pins = new GameObject ("Pins").transform;
				pins.parent = transform;
			}

			Coordinates coordinates = new Coordinates (lat, lng,0);
			go.transform.localPosition = coordinates.convertCoordinateToVector(0);
			go.transform.parent = pins;	
		}

		#region Tile Background

		private void CreateTileBackground(GOTile tile) {

			MeshFilter filter = tile.gameObject.AddComponent<MeshFilter>();
			MeshRenderer renderer = tile.gameObject.AddComponent<MeshRenderer>();

			tile.vertices = tile.tileCenter.tileVertices (zoomLevel);
			Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
			poly.outside = tile.vertices;
			Mesh mesh = Poly2Mesh.CreateMesh (poly);

			Vector2[] uvs = new Vector2[mesh.vertices.Length];
			for (int i=0; i < uvs.Length; i++) {
				uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
			}
			mesh.uv = uvs;

			filter.mesh = mesh;
			renderer.material = tileBackground;

		}

		private void CreateTemporaryMapBackground () {
		
			tempTileBackgorund = new GameObject ("Temporary tile background");

			MeshFilter filter = tempTileBackgorund.AddComponent<MeshFilter>();
			MeshRenderer renderer = tempTileBackgorund.AddComponent<MeshRenderer>();

			float size = 1000;

			Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();
			poly.outside = new List<Vector3> {
				new Vector3(size, -0.1f, size),
				new Vector3(size, -0.1f, -size),
				new Vector3(-size, -0.1f, -size),
				new Vector3(-size,-0.1f ,size)
			};
			Mesh mesh = Poly2Mesh.CreateMesh (poly);

			Vector2[] uvs = new Vector2[mesh.vertices.Length];
			for (int i=0; i < uvs.Length; i++) {
				uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].z);
			}
			mesh.uv = uvs;

			filter.mesh = mesh;
			renderer.material = tileBackground;
		
		} 

		private void DestroyTemporaryMapBackground () {
			Debug.Log ("Destroy temp bknd");
			tempTileBackgorund = null;
			GameObject.DestroyImmediate (tempTileBackgorund);
		}

		#endregion


		#region Editor Map Builder

		public void BuildInsideEditor () {

			dynamicLoad = true;
			//This fixes the map origin
			locationManager.LoadDemoLocation ();

			//Wipe buildings id list
			buildingsIds = new List<object>();

			//Start load routine (This might take some time...)
			IEnumerator routine = ReloadMap (locationManager.demo_CenterWorldCoordinates.tileCenter (locationManager.zoomLevel), false);
			GORoutine.start (routine,this);

		}

		public void TestEditorWWW () {
			#if UNITY_EDITOR
			var www = new WWW("https://tile.mapzen.com/mapzen/vector/v1//buildings,landuse,water,roads/17/70076/48701.json");

			ContinuationManager.Add(() => www.isDone, () =>
				{
					if (!string.IsNullOrEmpty(www.error)) Debug.Log("WWW failed: " + www.error);
					Debug.Log("[GOMap Editor] Request success: " + www.text);
				});
			#endif
		}
		#endregion
	}

	[System.Serializable]
	public class Layer
	{
		public string name;
		public string json;
		public bool isPolygon;
		public bool useRealHeight = false;
		public RenderingOptions defaultRendering;
		public RenderingOptions [] renderingOptions;

		public string[] useOnly;
		public string[] avoid;
		public bool useTunnels = true;
		public bool useBridges = true;
		public bool useColliders = false;

		public bool startInactive;
		public bool disabled = false;

		public GOEvent OnFeatureLoad; 
	}

	[System.Serializable]
	public class RenderingOptions
	{
		public string kind;
		public Material material;
		public Material outlineMaterial;
		public Material roofMaterial;

		public Material[] materials;

		public float lineWidth;
		public float outlineWidth;
		public float polygonHeight;
		public float distanceFromFloor;

	}

}