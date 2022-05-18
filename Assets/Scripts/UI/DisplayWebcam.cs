///This receives input from webcams and displays them as a WebCamTexture on the specified GameObject, and generates buttons to switch between cameras. 
///Uses sample Unity code for webcams, https://docs.unity3d.com/ScriptReference/WebCamTexture-devices.html
///Sam Harper, 2021

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayWebcam : MonoBehaviour
{
    [SerializeField]
    private RawImage rI_WebCamImage;    //The RawImage on which the WebCamTexture will be displayed
    public Canvas canvas;               //The parent canvas
    public GameObject go_ButtonPrefab;  //The prefab on which the generated buttons are based
    GameObject[] go_camButtons;         //Array to store the generated camera buttons
    WebCamDevice[] devices;             //Array to store the detected devices
    WebCamTexture[] tex;                //Array to store the WebCamTextures
    public int NumberOfCamerasToUse;

    void Start()
    {
        //Get devices, buttons and textures
        devices = WebCamTexture.devices;
        go_camButtons = new GameObject[NumberOfCamerasToUse];
        tex = new WebCamTexture[devices.Length];

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].name == "FaceTime HD Camera") { }
            else
            {
                print("Webcam available: " + devices[i].name);                                      // for debugging purposes, prints available devices to the console
                go_camButtons[i] = Instantiate(go_ButtonPrefab, gameObject.transform);              //Instantiate premade buttons as child of current object
                go_camButtons[i].name = "Camera" + (i + 1) + "Button";                                    //Name button in inspector
                go_camButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "Camera " + (i + 1);  //Change button text to camera number
                                                                                                    //go_camButtons[i].transform.SetParent(canvas.transform, false);
                go_camButtons[i].transform.localPosition = new Vector3(314, 215 - i * 50, 0);
            }
        }
        //ClosureIndex workaround to ensure we don't get an Out Of Range Exception
        //This loop adds listeners to the created buttons
        for (int i = 0; i < go_camButtons.Length; i++)
        {
            int closureIndex = i;
            if (devices[closureIndex].name == "FaceTime HD Camera") { }
            else
            {
                tex[closureIndex] = new WebCamTexture(devices[closureIndex].name, 1280, 720, 30);
                go_camButtons[closureIndex].GetComponent<Button>().onClick.AddListener(() => CamSwitch(closureIndex));
            }
        }

        rI_WebCamImage.texture = tex[0];
    }

    //Switch the displayed WebCamTexture on the WebCamImage to the desired camera
    void CamSwitch(int index)
    {
        Debug.Log("Button " + index + "clicked");
        rI_WebCamImage.texture = tex[index];
        tex[index].Play();
    }
}