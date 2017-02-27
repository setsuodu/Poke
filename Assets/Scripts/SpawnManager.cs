using Assets;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.UI;
using GoMap;

    public class SpawnManager : MonoBehaviour
    {
        public enum DemoLocation
        {
            NoGPSTest,
            Custom
        };

        public enum MotionPreset
        {
            Walk,
            Bike,
            Car
        };

        public bool useLocationServices;

        public int zoomLevel = 17;
        public DemoLocation demoLocation;
        public Coordinates demo_CenterWorldCoordinates;
        [HideInInspector]
        public Vector2 demo_CenterWorldTile;

        public Coordinates currentLocation;

        [HideInInspector]
        public static Coordinates CenterWorldCoordinates;

        public float desiredAccuracy = 50;
        public float updateDistance = 0.1f;

        [HideInInspector]
        public float updateEvery = 1 / 1000f;

        public MotionPreset simulateMotion = MotionPreset.Walk;
        float demo_WASDspeed = 20;

        public static bool IsOriginSet;
        public static bool UseLocationServices;
        public static LocationServiceStatus status;

        public event OnOriginSet onOriginSet;
        public delegate void OnOriginSet(Coordinates origin);

        public event OnLocationChanged onLocationChanged;
        public delegate void OnLocationChanged(Coordinates current);

        void Start()
        {
            if (Application.isEditor || !Application.isMobilePlatform)
            {
                useLocationServices = false;
            }

            if (useLocationServices)
            {
                Input.location.Start(desiredAccuracy, updateDistance);
            }
            else
            { //Demo origin

                LoadDemoLocation();
            }

            UseLocationServices = useLocationServices;

            StartCoroutine(LocationCheck(updateEvery));

            StartCoroutine(LateStart(0.01f));

        }

        public void LoadDemoLocation()
        {
            switch (demoLocation)
            {
                case DemoLocation.NoGPSTest:
                    currentLocation = demo_CenterWorldCoordinates = null;
                    return;
                case DemoLocation.Custom:
                    currentLocation = demo_CenterWorldCoordinates;
                    break;
                default:
                    break;
            }
            SetOrigin(demo_CenterWorldCoordinates);
        }

        IEnumerator LateStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (!useLocationServices && demoLocation != DemoLocation.NoGPSTest)
            {
                adjust(); //This adjusts the current location just after the initialization
            }
        }

        IEnumerator LocationCheck(float repeatTime)
        {
            while (true)
            {
                status = Input.location.status;

                if (!useLocationServices)
                {
                    yield return new WaitForSeconds(repeatTime);
                }
                else if (status == LocationServiceStatus.Failed)
                {
                    yield return new WaitForSeconds(repeatTime);
                }
                else if (status == LocationServiceStatus.Stopped)
                {
                    yield return new WaitForSeconds(repeatTime);
                }
                else if (status == LocationServiceStatus.Initializing)
                {
                    yield return new WaitForSeconds(repeatTime);
                }
                else if (status == LocationServiceStatus.Running)
                {

                    if (Input.location.lastData.horizontalAccuracy > desiredAccuracy)
                    {
                        yield return new WaitForSeconds(repeatTime);
                    }
                    else
                    {
                        if (!IsOriginSet)
                        {
                            SetOrigin(new Coordinates(Input.location.lastData));
                        }
                        LocationInfo info = Input.location.lastData;
                        if (info.latitude != currentLocation.latitude || info.longitude != currentLocation.longitude)
                        {
                            currentLocation.updateLocation(Input.location.lastData);
                            if (onLocationChanged != null)
                            {
                                onLocationChanged(currentLocation);
                            }
                        }
                    }
                }

                if (!useLocationServices && Application.isEditor && demoLocation != DemoLocation.NoGPSTest)
                {
                    changeLocationWASD();
                }

                yield return new WaitForSeconds(repeatTime);

            }
        }

        void SetOrigin(Coordinates coords)
        {
            IsOriginSet = true;
            CenterWorldCoordinates = coords.tileCenter(zoomLevel);
            demo_CenterWorldTile = coords.tileCoordinates(zoomLevel);
            Coordinates.setWorldOrigin(CenterWorldCoordinates);
            if (onOriginSet != null)
            {
                onOriginSet(CenterWorldCoordinates);
            }
        }

        void adjust()
        {
            Vector3 current = currentLocation.convertCoordinateToVector();
            Vector3 v = current;
            currentLocation = Coordinates.convertVectorToCoordinates(v);
            v = current + new Vector3(0, 0, 0.4f);
            currentLocation = Coordinates.convertVectorToCoordinates(v);
            if (onLocationChanged != null)
            {
                onLocationChanged(currentLocation);
            }
        }

        void changeLocationWASD()
        {
            switch (simulateMotion)
            {
                case MotionPreset.Car:
                    demo_WASDspeed = 4;
                    break;
                case MotionPreset.Bike:
                    demo_WASDspeed = 2;
                    break;
                case MotionPreset.Walk:
                    demo_WASDspeed = 0.4f;
                    break;
                default:
                    break;
            }

            Vector3 current = currentLocation.convertCoordinateToVector();
            Vector3 v = current;

            if (Input.GetKey(KeyCode.W))
            {
                v = current + new Vector3(0, 0, demo_WASDspeed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                v = current + new Vector3(0, 0, -demo_WASDspeed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                v = current + new Vector3(-demo_WASDspeed, 0, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                v = current + new Vector3(demo_WASDspeed, 0, 0);
            }

            if (v != current)
            {
                currentLocation = Coordinates.convertVectorToCoordinates(v);
                if (onLocationChanged != null)
                {
                    onLocationChanged(currentLocation);
                }
            }
        }
}