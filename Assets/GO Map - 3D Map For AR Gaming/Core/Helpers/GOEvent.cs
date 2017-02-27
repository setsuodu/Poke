using Assets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GoMap {

	[Serializable]
	//Feature's mesh
	//Feature's layer
	//Features's kind
	//Mesh center

	public class GOEvent : UnityEvent <Mesh,Layer,string,Vector3> {


	}

	[Serializable]
	public class GOTileEvent : UnityEvent <GOTile> {


	}
		
}


