using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using MiniJSON;

namespace GoMap
{
	public class GOTile : MonoBehaviour
	{
		public Coordinates tileCenter;
		public float diagonalLenght;

		public List<Vector3> vertices;
		private object mapData;
		ParseJob job;
		GOMap map;
		IList buildingsIds = new List<object>();

		public IEnumerator ParseJson (string data) {

			job = new ParseJob();
			job.InData = data;
			job.Start();

			yield return StartCoroutine(job.WaitFor());
		}

		public IEnumerator LoadTileData(object m, Coordinates tilecenter, int zoom, Layer[] layers, bool delayedLoad)
		{

			#if !UNITY_WEBPLAYER

			map = (GOMap)m;

			Vector2 realPos = tileCenter.tileCoordinates (zoom);

			var tileurl = realPos.x + "/" + realPos.y;

			var baseUrl = "https://tile.mapzen.com/mapzen/vector/v1/";
			//			var baseUrl = "https://vector.mapzen.com/osm/"; oldapi
			List <string> layerNames = new List<string>();
			for (int i = 0; i < layers.ToList().Count; i++) {
				if (layers [i].disabled == false) {
					layerNames.Add(layers [i].json);
				}
			}
			layerNames.RemoveAll(str => String.IsNullOrEmpty(str));
//			var url = baseUrl + string.Join(",",layerNames.ToArray())+"/"+zoom+"/";
			var url = baseUrl + "all/"+zoom+"/";

			var completeurl = url + tileurl + ".json"; 

			if (map.mapzen_api_key != null && map.mapzen_api_key != "") {
				completeurl = completeurl + "?api_key=" + map.mapzen_api_key;
			}

			if (Application.isPlaying) { //Runtime build

				if (map.useCache && FileHandler.Exist(gameObject.name))
				{
					yield return StartCoroutine (ParseJson(FileHandler.LoadText (gameObject.name)));
				}
				else
				{
					Debug.Log (completeurl);
					var www = new WWW(completeurl);
					yield return www;
					if (www.error == null && www.text.Length > 0) {
						FileHandler.SaveText (gameObject.name, www.text);
					}else if (www.error != null && (www.error.Contains("429") || www.error.Contains("timed out"))) {
						Debug.LogWarning("Tile data reload "+www.error);
						yield return new WaitForSeconds(1);
						yield return StartCoroutine (LoadTileData(map,tilecenter,zoom,layers,delayedLoad));
						yield break;

					}else {
						Debug.LogWarning("Tile data missing "+www.error);
						((GOMap)m).tiles.Remove(this);
						GameObject.Destroy(this.gameObject);
						yield break;
					}
					yield return StartCoroutine (ParseJson(www.text));
				}

				mapData = job.OutData;

				yield return StartCoroutine(ParseTileData(map,tileCenter,zoom,layers,delayedLoad,layerNames));

			} else { //Editor build

				if (map.useCache && FileHandler.Exist(gameObject.name))
				{
					mapData = Json.Deserialize (FileHandler.LoadText (gameObject.name));
					GORoutine.start(ParseTileData(map,tileCenter,zoom,layers,delayedLoad,layerNames),this);
				}
				else
				{
					var www = new WWW(completeurl);
			#if UNITY_EDITOR
					ContinuationManager.Add(() => www.isDone, () => {

						if (!string.IsNullOrEmpty(www.error)) {
							Debug.LogWarning("Tile data missing "+www.error);
							System.Threading.Thread.Sleep(1000);
							GORoutine.start(LoadTileData(map,tilecenter,zoom,layers,delayedLoad),this);
						}
						else if (this != null){
							FileHandler.SaveText (gameObject.name, www.text);
							mapData = Json.Deserialize (FileHandler.LoadText (gameObject.name));
							GORoutine.start(ParseTileData(map,tileCenter,zoom,layers,delayedLoad,layerNames),this);
						}
					});
			#endif
					yield break;

				}
			}
				
			#else 
			yield return null;
			#endif
		}

		public IEnumerator ParseTileData(object m, Coordinates tilecenter, int zoom, Layer[] layers, bool delayedLoad, List <string> layerNames)
		{

//			transform.position = tilecenter.convertCoordinateToVector();
			foreach (Layer layer in layers) {

				IDictionary layerData = null;
				if (layerNames.Count () == 1) {
					layerData = (IDictionary)mapData;
				} else {
					if (mapData != null && ((IDictionary)mapData).Contains(layer.json)) {
						layerData = (IDictionary)((IDictionary)mapData) [layer.json];
					}
				}

				if (!layer.disabled) {
					yield return StartCoroutine( BuildTile (layerData,layer,delayedLoad));
				} 
			}
			yield return null;

		}

		private IEnumerator BuildTile(IDictionary mapData, Layer layer, bool delayedLoad)
		{ 

			GameObject parent = new GameObject ();
			parent.name = layer.name;
			parent.transform.parent = this.transform;
			parent.SetActive (!layer.startInactive);

			if (mapData == null) {
				Debug.LogWarning ("Map Data is null!");
				#if !UNITY_WEBPLAYER
				FileHandler.Remove (gameObject.name);
				#endif
				yield break;
			}

			IList features = (IList)mapData ["features"];
			if (features == null)
				yield break;

			foreach (IDictionary geo in features) {
				
				IDictionary geometry = (IDictionary)geo ["geometry"];
				IDictionary properties = (IDictionary)geo ["properties"];
				string type = (string)geometry ["type"];
				string kind = (string)properties ["kind"];

				var id = properties ["id"]; 
				if (idCheck (id,layer) == false && layer.json == "buildings") {
					continue;
				}

				if (layer.useOnly.Length > 0 && !layer.useOnly.Contains (kind)) {
					continue;
				}
				if (layer.avoid.Length > 0 && layer.avoid.Contains (kind)) {
					continue;
				}

				if (type == "MultiLineString" || (type == "Polygon" && !layer.isPolygon)) {
					IList lines = new List<object>();
					lines = (IList)geometry ["coordinates"];
					foreach (IList coordinates in lines) {
						if (delayedLoad)
							yield return StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
						else
							StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
					}
				} 

				else if (type == "LineString") {
					IList coordinates = (IList)geometry ["coordinates"];
					if (delayedLoad)
						yield return StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
					else
						StartCoroutine (CreateLine (parent, kind, type, coordinates, properties, layer, delayedLoad));
				} 

				else if (type == "Polygon") {
					
					List <object> shapes = new List<object>();
					shapes = (List <object>)geometry["coordinates"];

					IList subject = null;
					List<object> clips = null;
					if (shapes.Count == 1) {
						subject = (List<object>)shapes[0];
					} else if (shapes.Count > 1) {
						subject = (List<object>)shapes[0];
						clips = shapes.GetRange (1, shapes.Count - 1);
					} else {
						continue;
					}
						
					if (delayedLoad)
					yield return StartCoroutine (CreatePolygon (parent, kind, type, subject,clips, properties, layer, delayedLoad));
					else
					StartCoroutine (CreatePolygon (parent, kind, type, subject, clips, properties, layer, delayedLoad));			
				}

				if (type == "MultiPolygon") {

					GameObject multi = new GameObject ("MultiPolygon");
					multi.transform.parent = parent.transform;

					IList shapes = new List<object>();
					shapes = (IList)geometry["coordinates"];

					foreach (List<object> polygon in shapes) {

						IList subject = null;
						List<object> clips = null;
						if (polygon.Count > 0) {
							subject = (List<object>)polygon[0];
						} else if (polygon.Count > 1) {
							clips = polygon.GetRange (1, polygon.Count - 1);
						} else {
							continue;
						}

						if (delayedLoad)
							yield return StartCoroutine (CreatePolygon (multi, kind, type, subject,clips, properties, layer, delayedLoad));
						else
							StartCoroutine (CreatePolygon (multi, kind, type, subject, clips,properties, layer, delayedLoad));	
						//						}
					}

				}
			}
		}

		private IEnumerator CreateLine(GameObject parent, string kind, string type, IList coordinates, IDictionary properties, Layer layer, bool delayedLoad)
		{

			bool isBridge = properties.Contains ("is_bridge") && properties ["is_bridge"].ToString() == "True";
			bool isTunnel = properties.Contains ("is_tunnel") && properties ["is_tunnel"].ToString() == "True";
			bool isLink = properties.Contains ("is_link") && properties ["is_link"].ToString() == "True";

			if ((isBridge && !layer.useBridges) || (isTunnel && !layer.useTunnels) || (isLink && !layer.useBridges)) {
				yield break;
			}

			var l = new List<Vector3>();
			for (int i = 0; i < coordinates.Count; i++)
			{
				IList c = (IList)coordinates[i];
				Coordinates coords = new Coordinates ((double)c[1], (double)c[0],0);
				l.Add(coords.convertCoordinateToVector(layer.defaultRendering.distanceFromFloor));

			}

			GameObject road = new GameObject (layer.json);
			RoadPolygon roadPolygon = road.AddComponent<RoadPolygon>();
			road.transform.parent = parent.transform;
//			try
//			{
				
				Int64 sort;
				if (properties.Contains("sort_key")) {
					sort = (Int64)properties["sort_key"];
				} else sort = (Int64)properties["sort_rank"];
				
				roadPolygon.Initialize(l, kind,layer,(int)sort);

				Attributes attributes = road.AddComponent<Attributes>();
				attributes.useName = true;
				attributes.loadWithDictionary((Dictionary<string,object>)properties);
//			}
//			catch (Exception ex)
//			{
//				Debug.Log(ex);
//			}
			if (delayedLoad)
				yield return null;
		}

		private IEnumerator CreatePolygon(GameObject parent, string kind, string type, IList subject, List<object> clips, IDictionary properties, Layer layer, bool delayedLoad)
		{

			GameObject polygon = null;
			try
			{

			PolygonHandler poly = new PolygonHandler(subject,clips);

			RenderingOptions renderingOptions = null;
			foreach (RenderingOptions r in layer.renderingOptions) {
				if (r.kind == kind) {
					renderingOptions = r;
					break;
				}
			}

			float height = layer.defaultRendering.polygonHeight;
			float defaultY = layer.defaultRendering.distanceFromFloor ;
			Material material = GetMaterial(layer.defaultRendering,poly.center);
			Material roofMat = layer.defaultRendering.roofMaterial == null ? layer.defaultRendering.material : layer.defaultRendering.roofMaterial;

			if (renderingOptions != null) {
				height = renderingOptions.polygonHeight;
				material = GetMaterial(renderingOptions,poly.center);;
				defaultY = renderingOptions.distanceFromFloor;
				roofMat = renderingOptions.roofMaterial;

			}

			//Group buildings by center coordinates
			if (layer.json == "buildings") {
				GameObject centerContainer = findNearestCenter(poly.center,parent,material);
				parent = centerContainer;
				material = centerContainer.GetComponent<MatHolder> ().material;
			}


			Int64 sort;
			if (properties.Contains("sort_key")) {
				sort = (Int64)properties["sort_key"];
			} else sort = (Int64)properties["sort_rank"];
				
			if (material)
				material.renderQueue = -(int)sort;
			if (roofMat)
				roofMat.renderQueue = -(int)sort;

			if (defaultY == 0f) {
				defaultY = sort / 1000.0f;
			}

			if (layer.useRealHeight && properties.Contains("height")) {
				double h = (double)properties["height"];
				height = (float)h;
			}
			if (layer.useRealHeight && properties.Contains("min_height")) {
				double hm = (double)properties["min_height"];
				defaultY = (float)hm;
				height = (float)height-(float)hm;
			} 
				
			polygon = poly.CreateModel(layer,height);
				if (polygon == null)
					yield break;

			polygon.name = layer.name;
			polygon.transform.parent = parent.transform;

			if (layer.useRealHeight) {
//				GameObject roof = poly.CreateModel(layer,0);
				GameObject roof = poly.CreateRoof();
				roof.name = "roof";
				roof.transform.parent = polygon.transform;
				roof.GetComponent<MeshRenderer> ().material = roofMat;
				roof.transform.position = new Vector3 (roof.transform.position.x,height+0.1f,roof.transform.position.z);

				if (properties.Contains ("roof_material")) {
					roof.name = "roofmat_"+(string)properties["roof_material"];
				}

				if (properties.Contains ("roof_color")) {
					Debug.LogError ("Roof color: " + (string)properties["roof_color"]);
					polygon.name = "roofx";
				}

			}

			Vector3 pos = polygon.transform.position;
			pos.y = defaultY;
			polygon.transform.position = pos;
			polygon.transform.localPosition = pos;

			Attributes attributes = polygon.AddComponent<Attributes>();
			attributes.type = type;
			attributes.loadWithDictionary((Dictionary<string,object>)properties);

			polygon.GetComponent<Renderer>().material = material;

			if (layer.OnFeatureLoad != null) {
				layer.OnFeatureLoad.Invoke(poly.mesh2D,layer,kind,poly.center);
			}

			}
			catch (Exception ex)
			{
				GameObject.Destroy (polygon);
				Debug.Log(ex);
			}
			if (delayedLoad)
				yield return null;
		}

		void OnDestroy() {
			removeIds ();
			//			Debug.Log ("Destroy tile: "+gameObject.name);
		}

		private bool idCheck (object id, Layer layer) {
			if (map.buildingsIds.Contains (id)) {
//				Debug.Log ("id already created");
				return false;
			} else {
				//					Debug.Log ("id added");
				buildingsIds.Add(id);
				map.buildingsIds.Add(id);
				return true;
			}
		}

		private void removeIds () {
			foreach (object id in buildingsIds) {
				map.buildingsIds.Remove (id);
			}
		}

		private List <Vector3> buildingCenters = new List<Vector3>();
		private float mdc = 60; // Group buildings every 50meters
		private GameObject findNearestCenter (Vector3 center, GameObject parent, Material material) {

			string name = "Container " + center.x + " "+center.y+ " "+center.z;
			foreach (Vector3 c in buildingCenters) {
				float d = Mathf.Abs(Vector3.Distance (center, c));
				if (d <= mdc) {
					string n = "Container " + c.x + " "+c.y+ " "+c.z;
					Transform child = parent.transform.FindChild (n);
					if (child != null)
						return child.gameObject;
				}
			}

			GameObject container = new GameObject (name);
			//			container.transform.localPosition = center;
			container.transform.parent = parent.transform;
			container.AddComponent<MatHolder> ().material = material;
			buildingCenters.Add (center);
			return container;
		}

		private Material GetMaterial (RenderingOptions rendering, Vector3 center) {
		
			if (rendering.materials.Length > 0) {
				float seed = center.x * center.z * 100;
				System.Random rnd = new System.Random ((int)seed);
				int pick = rnd.Next (0, rendering.materials.Length);
				Material material = rendering.materials [pick];
				return material;
			} else
				return rendering.material;
			
		}
	}
}
