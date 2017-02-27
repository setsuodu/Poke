using UnityEngine;
using System.Collections;

public class SearchEvent : AndroidJavaProxy {

	public SearchEvent ()
		: base ("com.nsh.amapHelper.AMapListenerHelper")
	{
	}

	void onPoiSearched (AndroidJavaObject result, int rCode)
	{
		if (poiSearched != null)
        {
			poiSearched (result, rCode);
		}
	}

	void onPoiItemSearched(AndroidJavaObject paramPoiItem,int paramInt){
	}

	public delegate void DelegateOnPoiSearched(AndroidJavaObject result,int rCode);
	public event DelegateOnPoiSearched poiSearched;
}
