using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CatchManager : MonoBehaviour
{
    public GameObject BagView;
    public GameObject SwitchFX;
    public Sprite[] ARMode = new Sprite[2];
    public Image ARModeImage;
    private bool isARModel = false;
    private GyroController gyroController;
    private CameraAsBackground cameraAsBackground;

    void Start ()
    {
        gyroController = Camera.main.GetComponent<GyroController>();
        cameraAsBackground = GameObject.FindObjectOfType<CameraAsBackground>();
    }

    public void SwitchARMode ()
    {
        isARModel = !isARModel;
        if (!isARModel)
        {
            ARModeImage.sprite = ARMode[0];
            gyroController.DetachGyro();
            cameraAsBackground.videoTexture(false);
            //切回普通模式，创建特效
            GameObject fx = Instantiate<GameObject>(SwitchFX);
            fx.transform.SetParent(Camera.main.transform);
            Destroy(fx, 2f);
        }
        else
        {
            ARModeImage.sprite = ARMode[1];
            gyroController.AttachGyro();
            cameraAsBackground.videoTexture(true);
        }
    }

    public void RunAway(string sc)
    {
        isARModel = false;
        Application.LoadLevel(sc);
    }

    public void BagOpen()
    {
        BagView.SetActive(true);
    }

    public void BagClose()
    {
        BagView.SetActive(false);
    }

    WebCamTexture webcamTexture;

    Texture2D exactCamData()
    {
        // get the sample pixels
        Texture2D snap = new Texture2D(Screen.width, Screen.height);
        //snap.SetPixels(webcamTexture.GetPixels((int)offset.x, (int)offset.y, w, w));
        snap.Apply();
        return snap;
    }

    public void TakePhoto()
    {
        Debug.Log(Application.dataPath);
        System.IO.File.WriteAllBytes(Application.dataPath + "/test.png", exactCamData().EncodeToPNG());
    }
}
