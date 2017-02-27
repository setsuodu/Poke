using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SearchManage : MonoBehaviour
{
	public Text txtInfo;
	public InputField inputQuery;
	private SearchEvent searchEvent;

	private AndroidJavaClass jcu;
	private AndroidJavaObject jou;
	private AndroidJavaObject query;
	private AndroidJavaObject poiSearch;
	private AndroidJavaObject lastPoint;

	private ARManager arManager;
    //private POIInstance poiInstance;
    public Text POINumber;

    void Start()
    {
		arManager = FindObjectOfType<ARManager> ();
        //poiInstance = FindObjectOfType<POIInstance>();
    }

	public void StartSearch ()
	{
		try
        {
			txtInfo.text = "start search...";

			jcu = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
			jou = jcu.GetStatic<AndroidJavaObject> ("currentActivity");
			txtInfo.text = txtInfo.text + "currentActivity get...";

			AndroidJavaObject amapHelper = new AndroidJavaObject("com.nsh.amapHelper.AMapSearchHelper");
			txtInfo.text = txtInfo.text + "helper get...";

			AndroidJavaObject query = amapHelper.Call<AndroidJavaObject>("getPoiSearch",inputQuery.text,"","0571");
			txtInfo.text = txtInfo.text + "query get...";

			query.Call ("setPageSize", 30);
			query.Call ("setPageNum", 1);
			txtInfo.text = txtInfo.text + "page set...";

			poiSearch = new AndroidJavaObject ("com.amap.api.services.poisearch.PoiSearch", jou, query);
			txtInfo.text = txtInfo.text + "poiSearch set...";

			lastPoint = new AndroidJavaObject ("com.amap.api.services.core.LatLonPoint", arManager.location.Latitude, arManager.location.Longitude);
            //lastPoint = new AndroidJavaObject("com.amap.api.services.core.LatLonPoint", poiInstance.location.Latitude, poiInstance.location.Longitude);
            txtInfo.text = txtInfo.text + "lastPoint set...";

			AndroidJavaObject bound = amapHelper.Call<AndroidJavaObject>("getBound", lastPoint, 1000, true);
			txtInfo.text = txtInfo.text + "bound set...";

			poiSearch.Call ("setBound", bound);
			txtInfo.text = txtInfo.text + "setBound set...";

			searchEvent = new SearchEvent ();
			searchEvent.poiSearched += OnPoiSearched;
			poiSearch.Call ("setOnPoiSearchListener", searchEvent);
			txtInfo.text = txtInfo.text + "setOnPoiSearchListener set...";

			poiSearch.Call ("searchPOIAsyn");
			txtInfo.text = txtInfo.text + "searchPOIAsyn set...";
		}
        catch (Exception ex)
        {
			txtInfo.text = txtInfo.text + "\r\n--------------------\r\n";
			txtInfo.text = txtInfo.text + ex.Message;
		}
	}

	private void OnPoiSearched (AndroidJavaObject result, int rCode)
	{
		try
        {
			if (rCode == 0)
            {
				if (result != null && result.Call<AndroidJavaObject> ("getQuery") != null)
                {
                    // 搜索poi的结果
					txtInfo.text = "reslut get.";

					txtInfo.text = txtInfo.text + ">>";
					txtInfo.text = txtInfo.text + "页面数：" + result.Call<int>("getPageCount").ToString();

					txtInfo.text = txtInfo.text +">>";
					AndroidJavaObject resultHelper = new AndroidJavaObject("com.nsh.amapHelper.AMapPoiResultHelper");
					txtInfo.text = txtInfo.text +"resulthelper get..";
					resultHelper.Call("setPoiResult", result);
					txtInfo.text = txtInfo.text + "resultHelper set...";

					int num = resultHelper.Call<int>("poiItemNumber");
					txtInfo.text = txtInfo.text + ">>记录数：" + num.ToString() + ">>";

					arManager.places.Clear();
                    //poiInstance.places.Clear();

                    for(int i = 0; i < num; i++)
                    {
                        txtInfo.text = txtInfo.text + "\r\n";
                        txtInfo.text = txtInfo.text + resultHelper.Call<String>("poinItemInfo", i);

                        PlaceInfo info = new PlaceInfo();
						info.Name = resultHelper.Call<String>("poinIntemName",i);
						info.Latitude = resultHelper.Call<double>("poinIntemLatitude",i);
						info.Longitude = resultHelper.Call<double>("poinIntemLongitude",i);
						info.Distance = resultHelper.Call<float>("poinIntemDistance",i);
						arManager.places.Add(info);
                        //poiInstance.places.Add(info);
                    }
                    POINumber.text = num.ToString();
                    arManager.ShowPlaces();
                    //poiInstance.ShowPlaces();
                }
                else
                {
					txtInfo.text = "没有找到相关数据。";
				}
			}
            else if (rCode == 27)
            {
				txtInfo.text = "搜索失败，请检查网络连接。";
			}
            else if (rCode == 32)
            {
				txtInfo.text = "key验证无效。";
			}
            else
            {
				txtInfo.text = "未知错误，请稍后重试。错误代码：" + rCode.ToString ();
			}

		}
        catch (Exception ex)
        {
			txtInfo.text = txtInfo.text + "\r\n----------------\r\n";
			txtInfo.text = txtInfo.text + ex.Message;
		}
	}
}
