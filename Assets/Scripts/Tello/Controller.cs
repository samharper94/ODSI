///This script is the main Tello controller script, and features code snippets from TelloController.cs
///Sam Harper, 2021

using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using TelloLib;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Controller : SingletonMonoBehaviour<Controller>
{
    
    private static bool isLoaded = false;
    [Tooltip("TextMeshPro asset to display the app status")]
    public TextMeshProUGUI t_AppStatus; //App status display
    [Tooltip("TextMeshPro assets to display Tello information")]
    public TextMeshProUGUI t_BatteryLevel, t_Speed, t_Height, t_FlyTime, t_BatteryStatus, t_ButtonInstructions; //Various text displays
    [Tooltip("Button assets to display the Tello controls")]
    public Button b_TakeOff, b_Land, b_Flip, b_AutonomousMission, b_Rotate; //On screen clickable buttons for Tello functions
    public float f_TelloHeightSimActual, f_TelloHeightSimTarget, f_SimVSpeed, lx, ly, rx, ry; //Self explanatory floats, l and r floats for Tello control
    public InputAction IA_Rotate, IA_MoveY, IA_MoveZ, IA_MoveX, IA_Exit;
    string s_flighttime;
    [Tooltip("TelloVideoTexture asset to display video feed from Tello")]
    public TelloVideoTexture telloVideoTexture;
    [Tooltip("TCPServer asset")]
    public TCPServer tcpServer;
    public float f_TelloSymbiosisLevel = 0, f_SymbioticIncrement = 1, f_SymbioticDecrement = 0.1f;
    public GameObject go_TelloSymbiosisNeedle;
    Thread th_clearpath, th_inspect;


    // VideoBitRate is used to set the bit rate for the streaming video returned by the Tello.
    public enum VideoBitRate
    {
        // VideoBitRateAuto sets the bitrate for streaming video to auto-adjust.
        VideoBitRateAuto = 0,

        // VideoBitRate1M sets the bitrate for streaming video to 1 Mb/s.
        VideoBitRate1M = 1,

        // VideoBitRate15M sets the bitrate for streaming video to 1.5 Mb/s
        VideoBitRate15M = 2,

        // VideoBitRate2M sets the bitrate for streaming video to 2 Mb/s.
        VideoBitRate2M = 3,

        // VideoBitRate3M sets the bitrate for streaming video to 3 Mb/s.
        VideoBitRate3M = 4,

        // VideoBitRate4M sets the bitrate for streaming video to 4 Mb/s.
        VideoBitRate4M = 5,

    };

    //Set up Tello video connections
    override protected void Awake()
    {
        if (!isLoaded)
        {
            DontDestroyOnLoad(this.gameObject);
            isLoaded = true;
        }

        base.Awake();

        Tello.onConnection += Tello_onConnection;
        Tello.onUpdate += Tello_onUpdate;
        Tello.onVideoData += Tello_onVideoData;

        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
    }

    //Enable input controls
    private void OnEnable()
    {
        IA_Rotate.Enable();
        IA_MoveY.Enable();
        IA_MoveZ.Enable();
        IA_MoveX.Enable();
        IA_Exit.Enable();
        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
    }

    //Disable input controls
    private void OnDisable()
    {
        IA_Rotate.Disable();
        IA_MoveY.Disable();
        IA_MoveZ.Disable();
        IA_MoveX.Disable();
        IA_Exit.Disable();
#if !UNITY_EDITOR
        CancelInvoke();
        StopAllCoroutines();
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        //Start video from Tello
        if (telloVideoTexture == null)
            telloVideoTexture = FindObjectOfType<TelloVideoTexture>();
        //Connect to Tello on a separate thread to avoid hanging the app
        Thread connectThread = new Thread(new ThreadStart(Tello.startConnecting));
        connectThread.Start();
        
        //Start listeners for Tello control buttons
        b_TakeOff.onClick.AddListener(takeoffbut);
        b_Land.onClick.AddListener(landbut);
        b_Flip.onClick.AddListener(flipbut);
        b_AutonomousMission.onClick.AddListener(StartInspection);
        b_Rotate.onClick.AddListener(Rotate);
        //Initialise Tello control floats
        lx = 0;
        ly = 0;
        rx = 0;
        ry = 0;
        //Auto generate on screen controls display
        t_ButtonInstructions.text = 
            "Rotate: " + IA_Rotate.GetBindingDisplayString() + "\n" +
            "Move Y Axis: " + IA_MoveY.GetBindingDisplayString() + "\n" +
            "Move Z Axis: " + IA_MoveZ.GetBindingDisplayString() + "\n" +
            "Move X Axis: " + IA_MoveX.GetBindingDisplayString();
        //Tello.TelloEduMission("mon");
    }

    public void StartClearPath()
    {
        th_clearpath = new Thread(new ThreadStart(TelloClearPath));
        th_clearpath.Start();
        //StartCoroutine(TelloInspection());

    }

    public void StartInspection()
    {
        th_inspect = new Thread(new ThreadStart(TelloInspect));
        th_inspect.Start();
    }

    //Take off Tello
    public void takeoffbut()
    {
        Tello.takeOff();
        t_AppStatus.text = "Command: Take off";
    }

    //Land Tello
    public void landbut()
    {
        Tello.land();
        t_AppStatus.text = "Command: Land";
    }

    //Make Tello do a backflip
    public void flipbut()
    {
        Tello.doFlip(2);
        t_AppStatus.text = "Command: Flip";
    }

    public void Rotate()
    {
        th_clearpath = new Thread(new ThreadStart(TelloClearPath));
        th_clearpath.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //Get various Tello state information and display on screen
        s_flighttime = Tello.state.flyTime.ToString();
        t_BatteryLevel.text = "Batt: " + Tello.state.batteryPercentage.ToString() + "%";
        t_Speed.text = "Vert. Speed: " + Tello.state.verticalSpeed;
        t_Height.text = "Height: " + Tello.state.height + "0 cm";
        t_FlyTime.text = "Flight Time: " + Tello.state.flyTime + "s";
        t_BatteryStatus.text = "Battery crit?: " + Tello.state.batteryLower + "Battery State: " + Tello.state.batteryState;
        //Read Tello control information from controller or keyboard
        lx = IA_Rotate.ReadValue<float>();
        ly = IA_MoveZ.ReadValue<float>();
        ry = IA_MoveY.ReadValue<float>();
        rx = IA_MoveX.ReadValue<float>();
        //If the game pad is connected, vibrate the controller depending on magnitude of input
        if (Tello.state.height > 0 && Gamepad.all.Count > 0)
        {
            Gamepad.current.SetMotorSpeeds(Mathf.Abs(ly), Mathf.Abs(ry));
        }
        //Prevents vibration when Tello is not in flight
        else if (Tello.state.height <= 0 && Gamepad.all.Count > 0)
        {
            Gamepad.current.ResetHaptics();
        }
        //Send control inputs to Tello
        Tello.controllerState.setAxis(lx, ly, rx, ry);
        //Reset inputs for next frame
        lx = ly = ry = rx = 0;
        
        //Quit application properly if the button is pressed
        if (IA_Exit.triggered)
        {
            Application.Quit();
        }
//        if (Tello.state == null)
//        {
//            if (f_TelloSymbiosisLevel > 0.0f)
//            {
//                f_TelloSymbiosisLevel -= f_SymbioticDecrement;
//            }
//            if (f_TelloSymbiosisLevel < f_SymbioticDecrement)
//            {
//                f_TelloSymbiosisLevel = 0.0f;
//            }
//        }
//        else
//        {
//            f_TelloSymbiosisLevel += f_SymbioticIncrement;
//        }
//
//        if (f_TelloSymbiosisLevel > 180.0f)
//        {
//            f_TelloSymbiosisLevel = 180.0f;
//        }
//
//        go_TelloSymbiosisNeedle.transform.localEulerAngles = new Vector3(0, 0, -f_TelloSymbiosisLevel);
    }

    //Not used, left in for Debug purposes
    private void Tello_onUpdate(int cmdId)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Tello_onUpdate : " + Tello.state);
        //text2.text = "Tello_onUpdate : " + Tello.state;
    }

    //This is run upon first Tello connection
    private void Tello_onConnection(Tello.ConnectionState newState)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("Tello_onConnection : " + newState);
        if (newState == Tello.ConnectionState.Connected)
        {
            Tello.queryAttAngle();
            Tello.setMaxHeight(100);
            t_AppStatus.text = "Tello connected";
            Tello.setPicVidMode(1); // 0: picture, 1: video
            Tello.setVideoBitRate((int)VideoBitRate.VideoBitRateAuto);
            //Tello.setEV(0);
            Tello.requestIframe();
        }
        else
        {
            //t_AppStatus.text = "Tello Disconnected";
        }
    }


    //This should debug the Tello video not appearing but doesn't seem to work
    private void Tello_onVideoData(byte[] data)
    {
        //Debug.Log("Tello_onVideoData: " + data.Length);
        if (telloVideoTexture != null)
        {
            telloVideoTexture.PutVideoData(data);
            //Debug.Log("There should be video");
        }
        else
            Debug.Log("No video from Tello");
    }

    //Makes Tello land if the application crashes/is quit while Tello is still in the air and closes the connection
    void OnApplicationQuit()
    {
        Tello.land();
        Tello.stopConnecting();
    }

    IEnumerator TelloInspection()
    {
        //Tello.TelloEduMission("mon");

        //Debug.Log("Marker detection enabled");

        //yield return new WaitForSeconds(0.5f);

        //Tello.TelloEduMission("mdirection 2");

        //Debug.Log("Cameras enabled");

        //yield return new WaitForSeconds(0.5f);

        Tello.takeOff();

        Debug.Log("Takeoff");

        yield return new WaitForSeconds(2f);

        //Tello.TelloEduMission("go 0 0 100 50 m4");

        //Debug.Log("Going to pad 4");

        //Tello.TelloEduMission("up 300");

        //Debug.Log("Going up");

        //yield return new WaitForSeconds(5f);

        //Tello.TelloEduMission("go 160 0 200 70 m1");

        //Debug.Log("Going to pad 1");

        //yield return new WaitForSeconds(8f);

        Tello.land();

        Debug.Log("Land");

        //yield return new WaitForSeconds(5);

        //waitThread.Start();
    }

    void TelloClearPath()
    {
        Tello.takeOff();
        //Debug.Log("Takeoff");

        while (Tello.state.height < 15)
        {
            Tello.controllerState.setAxis(0, 1f, 0, 0);
            
            //Debug.Log("Going up" + Tello.state.height);
        }
        Tello.controllerState.setAxis(1f, 0, 0, 0);
        //Debug.Log("Waiting 3 seconds");
        Thread.Sleep(3000);
        Tello.land();
        //Debug.Log("Land");
    }

    void TelloInspect()
    {
        Tello.takeOff();
        Debug.Log("Takeoff");

        //while (Tello.state.height < 15)
        //{
        //    Tello.controllerState.setAxis(0, 1f, 0, 0);
        //
        //    //Debug.Log("Going up" + Tello.state.height);
        //}
        Tello.controllerState.setAxis(0, 0, 0, 0);
        Debug.Log("Waiting 3 seconds");
        Thread.Sleep(3000);
        Tello.land();
        //Debug.Log("Land");
    }

}
