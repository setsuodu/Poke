using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CatchManager : MonoBehaviour
{
    public GameObject BagView;
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

        }
        else
        {
            ARModeImage.sprite = ARMode[1];
            gyroController.AttachGyro();
            cameraAsBackground.videoTexture(true);
        }
    }

    public void RunAway()
    {
        Application.LoadLevel("MapScene");
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
