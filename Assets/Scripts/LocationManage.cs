using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class LocationManage : MonoBehaviour
{
	public Text txtLocation;
	//public Text txtInfo;
	private AmapEvent amap;
	private AndroidJavaClass jcu;
	private AndroidJavaObject jou;
	private AndroidJavaObject mLocationClient;
	private AndroidJavaObject mLocationOption;
	private ARManager arManager;
    //private POIInstance poiInstance;
    private SearchManage searchMange;
	private PlaceInfo lastPlace;

	public void Start ()
	{
		lastPlace = new PlaceInfo ();
		lastPlace.Latitude = -100d;
		lastPlace.Longitude = -100d;
		arManager = GameObject.FindObjectOfType<ARManager> ();
        //poiInstance = GameObject.FindObjectOfType<POIInstance>();
        searchMange = GameObject.FindObjectOfType<SearchManage> ();

		StartLocation ();
	}

	public void StartLocation ()
	{
		try
        {
			txtLocation.text = "start location...";

			//txtInfo.text = txtInfo.text + "\r\n";
			jcu = new AndroidJavaClass ("com.unity3d.player.UnityPlayer"); 
			jou = jcu.GetStatic<AndroidJavaObject> ("currentActivity");
			txtLocation.text = "currentActivity get...";

			//txtInfo.text = txtInfo.text + "\r\n";
			mLocationClient = new AndroidJavaObject ("com.amap.api.location.AMapLocationClient", jou);
			txtLocation.text = "AMapLocationClient get...";

			//txtInfo.text = txtInfo.text + "\r\n";
			mLocationOption = new AndroidJavaObject ("com.amap.api.location.AMapLocationClientOption");
			txtLocation.text = "AMapLocationClientOption get...";

            //Hight_Accuracy | Battery_Saving | Device_Sensors
            //txtInfo.text = txtInfo.text + "\r\n";
            //AndroidJavaObject hightAccuracy = helper.CallStatic<AndroidJavaObject>("HightAccuracy");
            //txtInfo.text = txtInfo.text + "HightAccuracy get...";

//			txtInfo.text = txtInfo.text + "\r\n";
//			AndroidJavaObject hightAccuracy = new AndroidJavaClass("com.amap.api.location.AMapLocationClientOption$AMapLocationMode").GetStatic<AndroidJavaObject>("Hight_Accuracy");
//			txtInfo.text = txtInfo.text + "hightAccuracy...";
//
//			txtInfo.text = txtInfo.text + "\r\n";
//			mLocationOption.Call ("setLocationMode", hightAccuracy);
//			txtInfo.text = txtInfo.text + "setLocationMode...";

            AndroidJavaObject helper = new AndroidJavaObject ("com.nsh.amapHelper.AMapLocationModeHelper");
			txtLocation.text = "helper get...";

			//txtInfo.text = txtInfo.text + "\r\n";
			helper.Call ("setHA", mLocationOption);
			txtLocation.text = "mode set...";


			//txtInfo.text = txtInfo.text + "\r\n";
			mLocationClient.Call ("setLocationOption", mLocationOption);
			txtLocation.text = "setLocationOption...";

			amap = new AmapEvent ();
			amap.locationChanged += OnLocationChanged;

			//txtInfo.text = txtInfo.text + "\r\n";
			mLocationClient.Call ("setLocationListener", amap);
			txtLocation.text = "setLocationListener...";

			//txtInfo.text = txtInfo.text + "\r\n";
			mLocationClient.Call ("startLocation");
			txtLocation.text = "startLocation...";

		}
        catch (Exception ex)
        {
			txtLocation.text = txtLocation.text + ">>>>";
			txtLocation.text = txtLocation.text + ex.Message;
			EndLocation ();
		}
	}

	public void EndLocation ()
	{
		if (amap != null)
        {
			amap.locationChanged -= OnLocationChanged;
		}

		if (mLocationClient != null)
        {
			mLocationClient.Call ("stopLocation");
			mLocationClient.Call ("onDestroy");
		}
		txtLocation.text = "";
	}

	private void OnLocationChanged (AndroidJavaObject amapLocation)
	{
		if (amapLocation != null)
        {
			if (amapLocation.Call<int> ("getErrorCode") == 0)
            {
				txtLocation.text = ">>success:";
				try
                {
					arManager.location.Latitude = amapLocation.Call<double> ("getLatitude");
					arManager.location.Longitude = amapLocation.Call<double> ("getLongitude");
					arManager.location.Name = amapLocation.Call<string> ("getPoiName");


                    txtLocation.text = arManager.location.Name + ">>纬度:" + arManager.location.Latitude.ToString () + ">>经度:" + arManager.location.Longitude.ToString ();
					if(arManager.location.Latitude != lastPlace.Latitude||arManager.location.Longitude != lastPlace.Longitude)
					{
						searchMange.StartSearch();
						lastPlace.Latitude = arManager.location.Latitude;
						lastPlace.Longitude = arManager.location.Longitude;
					}

                    //poiInstance.location.Latitude = amapLocation.Call<double>("getLatitude");
                    //poiInstance.location.Longitude = amapLocation.Call<double>("getLongitude");
                    //poiInstance.location.Name = amapLocation.Call<string>("getPoiName");
                    //txtLocation.text = poiInstance.location.Name + ">>纬度:" + poiInstance.location.Latitude.ToString() + ">>经度:" + poiInstance.location.Longitude.ToString();
                    //if (poiInstance.location.Latitude != lastPlace.Latitude || poiInstance.location.Longitude != lastPlace.Longitude)
                    //{
                    //    searchMange.StartSearch();
                    //    lastPlace.Latitude = poiInstance.location.Latitude;
                    //    lastPlace.Longitude = poiInstance.location.Longitude;
                    //}
                }
                catch (Exception ex)
                {
					txtLocation.text = txtLocation.text + ">>>>" + ex.Message;
				}
			}
            else
            {
				txtLocation.text = ">>amaperror:";
				txtLocation.text = txtLocation.text + ">>getErrorCode:" + amapLocation.Call<int> ("getErrorCode").ToString ();
				txtLocation.text = txtLocation.text + ">>getErrorInfo:" + amapLocation.Call<string> ("getErrorInfo");
			}
		}
        else
        {
			txtLocation.text = "amaplocation is null.";
		}
	}
}
