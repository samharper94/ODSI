///My own TCP server code using .NET sockets
///This listens for messages from connected clients
///Sam Harper, 2021

using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TCPServer : MonoBehaviour
{
    //Initialise TCP Listeners for each communications channel
    private TcpListener spotTelemetrySocket, spotCommandSocket, huskyTelemetrySocket, huskyCommandSocket, jackalTelemetrySocket, jackalCommandSocket,
        HLSpotSocket, HLHuskySocket, HLTelloSocket, HLJackalSocket, HLGeneralSocket;
    //Initialise threads for each communications channel
    private Thread spotTelemetryThread, spotCommandThread, huskyTelemetryThread, huskyCommandThread, jackalTelemetryThread, jackalCommandThread/*,
        HLSpotThread, HLHuskyThread, HLTelloThread, HLJackalThread, HLGeneralThread*/;
    //Initialise TCP Clients for each robot
    private TcpClient spotTelemetryClient, spotCommandClient, huskyClient, jackalClient;
    //Initialise message text fields
    public TextMeshProUGUI GenericMessages, HuskyErrors;
    //Initialise Controller
    public Controller controller;
    //Initialise all UI buttons
    public Button b_SpotCmd, b_SpotAM, b_SpotBatt, b_HuskyCmd, b_HuskyArms, b_JackalCmd, b_SpotAllRooms, b_SpotCorrosion, b_AllCorrosion, b_AllCorrosionTelloJackal,
        b_SpotHome, b_SendHLTest;
    //Initialise strings for receiving messages from robots
    string spotTelemetryMessage, spotCommandMessage, huskyTelemetryMessage, huskyCommandMessage, jackalCommandMessage, jackalTelemetryMessage, path;
    //Initialise ints for port numbers
    public int SpotTelemetryPort, SpotCommandPort, HuskyTelemetryPort, HuskyCommandPort, JackalTelemetryPort, JackalCommandPort/*,
        HLSpotPort, HLHuskyPort, HLTelloPort, HLJackalPort, HLGeneralPort*/;
    //Initialise strings for displaying robot parameters
    string s_runtime, s_battperc, s_powerstate, s_chargestatus, 
        s_huskyuptime, s_ros_cont_loop_freq, s_huskymcucurr, s_huskydcl,
        s_huskydcr, s_huskybattv, s_huskylv, s_huskyrv, s_huskyldt,
        s_huskyrdt, s_huskylmt, s_huskyrmt, s_huskycapest, s_huskychrgest,
        s_jackaluptime, s_jackal_ros_cont_loop_freq, s_jackal_mcucurr, s_jackaldcl,
        s_jackaldcr, s_jackalbattv, s_jackallv, s_jackalrv, s_jackalldt,
        s_jackalrdt, s_jackallmt, s_jackalrmt, s_jackalcapest, s_jackalchrgest;
    //Initialise robot parameter text fields
    public TextMeshProUGUI t_runtime, t_battperc, t_powerstate, t_chargestatus, 
        t_huskyuptime, t_huskycurr, t_huskyv, t_huskycomponentt, t_huskybattcap,
        t_huskyuptime2, t_huskycurr2, t_huskyv2, t_huskycomponentt2, t_huskybattcap2,
        t_jackaluptime, t_jackalcurr, t_jackalv, t_jackalcomponentt, t_jackalbattcap,
        t_jackaluptime2, t_jackalcurr2, t_jackalv2, t_jackalcomponentt2, t_jackalbattcap2;
    //Initialise 3D space text fields
    public TextMeshPro t_runtime2, t_battperc2, t_powerstate2, t_chargestatus2;
    //Initialise symbiosis display
    public GameObject go_HuskySymbiosisNeedle, go_SpotSymbiosisNeedle, go_TelloSymbiosisNeedle;
    public float f_HuskySymbiosisLevel = 0.0f, f_SpotSymbiosisLevel = 0.0f, f_TelloSymbiosisLevel = 0.0f, f_SymbioticIncrement = 1.0f, f_SymbioticDecrement = 0.1f, currentTime, targetTime;
    bool boo_AllMissionStart = false, boo_HasTargetBeenSet = false, boo_MissionRun = false, boo_batterymissionstart = false;

    // Use this for initialization
    void Start()
    {
        s_runtime = "NOT CONNECTED";
        s_battperc = "NOT CONNECTED";
        s_powerstate = "NOT CONNECTED";
        s_chargestatus = "NOT CONNECTED";
        //Start a thread for listening so application doesn't hang. This runs until the application is closed.
        spotTelemetryThread = new Thread(new ThreadStart(SpotListenForIncomingTelemetry));
        spotTelemetryThread.IsBackground = true;
        spotTelemetryThread.Start();
        spotCommandThread = new Thread(new ThreadStart(SpotListenForIncomingCommands));
        spotCommandThread.IsBackground = true;
        spotCommandThread.Start();
        huskyTelemetryThread = new Thread(new ThreadStart(HuskyListenForIncomingTelemetry));
        huskyTelemetryThread.IsBackground = true;
        huskyTelemetryThread.Start();
        jackalTelemetryThread = new Thread(new ThreadStart(JackalListenForIncomingTelemetry)); ;
        jackalTelemetryThread.IsBackground = true;
        jackalTelemetryThread.Start();
        //Experimental HoloLens connection
        /*HLSpotThread = new Thread(new ThreadStart(HLSpotSocketStart));
        HLSpotThread.IsBackground = true;
        HLSpotThread.Start();
        HLHuskyThread = new Thread(new ThreadStart(HLHuskySocketStart));
        HLHuskyThread.IsBackground = true;
        HLHuskyThread.Start();
        HLTelloThread = new Thread(new ThreadStart(HLTelloSocketStart));
        HLTelloThread.IsBackground = true;
        HLTelloThread.Start();
        HLJackalThread = new Thread(new ThreadStart(HLJackalSocketStart));
        HLJackalThread.IsBackground = true;
        HLJackalThread.Start();
        HLGeneralThread = new Thread(new ThreadStart(HLGeneralSocketStart));
        HLGeneralThread.IsBackground = true;
        HLGeneralThread.Start();*/
        //huskyCommandThread = new Thread(new ThreadStart(HuskyListenForIncomingCommands));
        //huskyCommandThread.IsBackground = true;
        //huskyCommandThread.Start();
        b_SpotCmd.onClick.AddListener(SpotCmd);
        b_SpotAM.onClick.AddListener(SpotAM);
        b_SpotBatt.onClick.AddListener(SpotBatt);
        b_HuskyCmd.onClick.AddListener(HuskyCmd);
        b_HuskyArms.onClick.AddListener(HuskyArm);
        b_JackalCmd.onClick.AddListener(JackalCmd);
        b_SpotHome.onClick.AddListener(SpotHome);
        //b_SpotAllRooms.onClick.AddListener(SpotRooms);
        b_SpotCorrosion.onClick.AddListener(SpotCorrosion);
        b_AllCorrosion.onClick.AddListener(AllCorrosion);
        b_AllCorrosionTelloJackal.onClick.AddListener(AllCorrosionTelloJackal);
        //b_SendHLTest.onClick.AddListener(SendHLTest);
        //path = Application.streamingAssetsPath + "/ports.txt";

        //Write some text to the test.txt file

        //StreamReader reader = new StreamReader(path, false);

        //string ports = reader.ReadLine();

        //reader.Close();

        //string[] portlist = ports.Split(',');

        //SpotTelemetryPort = Int32.Parse(portlist[0]);
        //SpotCommandPort = Int32.Parse(portlist[1]);
        //HuskyTelemetryPort = Int32.Parse(portlist[2]);
    }

    void SpotCmd()
    {
        SendSpotCmd("a");
    }

    void SpotAM()
    {
        SendSpotCmd("b");
    }

    void SpotBatt()
    {
        SendSpotCmd("c");
        GenericMessages.text = "Sending Spot on mission, confirming clear path with Tello.";
        controller.StartClearPath();
    }

    void SpotCorrosion()
    {
        SendSpotCmd("d");
    }

    void HuskyCmd()
    {
        SendHuskyCmd("a");
    }

    void JackalCmd()
    {
        SendJackalCmd("a");
    }

    void HuskyArm()
    {
        SendHuskyCmd("b");
    }

    void SpotHome()
    {
        SendSpotCmd("e");
    }

    void SendHLTest()
    {
        SendHLTestData();
    }

    void AllCorrosion_MoveSpot()
    {
        //boo_AllMissionStart = true;
        //SendSpotCmd("d");
        //Debug.Log("Spot move Started");
    }

    void AllCorrosion()
    {
        SendSpotCmd("d");
        controller.StartInspection();
        SendHuskyCmd("a");
    }

    void AllCorrosionTelloJackal()
    {
        controller.StartInspection();
        SendJackalCmd("a");
    }

    void SendSpotCmd(string str)
    {
        if (spotCommandClient == null)
        {
            Debug.Log("Socket Connection Null");
            return;
        }
        try
        {
            //encode the string to bytes
            byte[] _str = Encoding.ASCII.GetBytes(str);
            //get the outgoing network stream to write to
            NetworkStream stream = spotCommandClient.GetStream();
            //write the message to the stream
            stream.Write(_str, 0, _str.Length);
            Debug.Log("Message Sent");
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void SendHuskyCmd(string str)
    {
        if (huskyClient == null)
        {
            Debug.Log("Socket Connection Null");
            return;
        }
        try
        {
            //encode the string to bytes
            byte[] _str = Encoding.ASCII.GetBytes(str);
            //get the outgoing network stream to write to
            NetworkStream stream = huskyClient.GetStream();
            //write the message to the stream
            stream.Write(_str, 0, _str.Length);
            Debug.Log("Message Sent");
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void SendJackalCmd(string str)
    {
        if (jackalClient == null)
        {
            Debug.Log("Socket Connection Null");
            return;
        }
        try
        {
            //encode the string to bytes
            byte[] _str = Encoding.ASCII.GetBytes(str);
            //get the outgoing network stream to write to
            NetworkStream stream = jackalClient.GetStream();
            //write the message to the stream
            stream.Write(_str, 0, _str.Length);
            Debug.Log("Message Sent");
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void SendHLTestData()
    {
        try
        {
            //encode the string to bytes
            byte[] _strS = Encoding.ASCII.GetBytes("spot");
            //get the outgoing network stream to write to
            //NetworkStream stream = HLSpotSocket.GetStream();
            ////write the message to the stream
            //stream.Write(_str, 0, _str.Length);
            //stream.Close();
            //Debug.Log("Message Sent");
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    #region SpotTelemetry
    void SpotListenForIncomingTelemetry()
    {
        try
        {
            spotTelemetrySocket = new TcpListener(IPAddress.Any, SpotTelemetryPort);
            spotTelemetrySocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            spotTelemetrySocket.Start();
            Debug.Log("Spot Telemetry Server listening...");
            //Set aside 1KB for the message
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (spotTelemetryClient = spotTelemetrySocket.AcceptTcpClient())
                {
                    using (NetworkStream stream = spotTelemetryClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            spotTelemetryMessage = Encoding.ASCII.GetString(incomingData);
                            //Debug.Log("Spot Message received" + spotTelemetryMessage);
                            string[] datamsgs = spotTelemetryMessage.Split('\n');
                            s_runtime = datamsgs[0].TrimStart(' ');
                            s_battperc = datamsgs[1].TrimStart(' ');
                            s_powerstate = datamsgs[2].TrimStart(' ');
                            s_chargestatus = datamsgs[3].TrimStart(' ');
                            /*data order: 
                            1. estimated run time, 
                            2. battery percentage, 
                            3. motor power state (line 1), 
                            4. charge status (line 39), 
                            -------------------------------
                            5. stow state of arm (line 677), 
                            6. foot fl contact (line 574)
                            7. foot fr contact (line 601)
                            8. foot hl contact (line 628)
                            9. foot hr contact (line 655)
                            10. body in vision linear vel x (line 414)
                            11. body in vision linear vel y (line 415)
                            12. body in vision linear vel z (line 416)
                            13. body in odometry linear vel x (line 426)
                            14. body in odometry linear vel y (line 427)
                            15. body in odometry linear vel z (line 428)
                            16. hand in vision linear vel x (line 672)
                            17. hand in vision linear vel y (line 673)
                            18. hand in vision linear vel z (line 674)
                            19. hand in odometry linear vel x (line 698)
                            20. hand in odometry linear vel y (line 699)
                            21. hand in odometry linear vel z (line 700)*/
                            //Read and instantly act upon the message received (these are from an Android client)
                            if (spotTelemetryMessage == "take off" || spotTelemetryMessage == "Take off" || spotTelemetryMessage == "Take Off" || spotTelemetryMessage == "take Off")
                            {
                                controller.takeoffbut();
                            }
                            if (spotTelemetryMessage == "land" || spotTelemetryMessage == "Land")
                            {
                                controller.landbut();
                            }
                            if (spotTelemetryMessage == "flip" || spotTelemetryMessage == "Flip")
                            {
                                controller.flipbut();
                            }
                        }
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }
    #endregion

    #region HuskyTelemetry
    void HuskyListenForIncomingTelemetry()
    {
        try
        {
            huskyTelemetrySocket = new TcpListener(IPAddress.Any, HuskyTelemetryPort);
            huskyTelemetrySocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            huskyTelemetrySocket.Start();
            Debug.Log("Husky Telemetry Server listening...");
            //Set aside 1KB for the message
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (huskyClient = huskyTelemetrySocket.AcceptTcpClient())
                {
                    using (NetworkStream stream = huskyClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            huskyTelemetryMessage = Encoding.ASCII.GetString(incomingData);
                            //Debug.Log("Husky Message received" + huskyTelemetryMessage);
                            if (huskyTelemetryMessage == "1" || huskyTelemetryMessage == "2" || huskyTelemetryMessage == "3")
                            {
                                huskyCommandMessage = huskyTelemetryMessage;
                            }
                            else
                            {
                                string[] datamsgs = huskyTelemetryMessage.Split(' ');
                                s_huskyuptime = datamsgs[0];
                                s_ros_cont_loop_freq = datamsgs[1];
                                s_huskymcucurr = datamsgs[2];
                                s_huskydcl = datamsgs[3];
                                s_huskydcr = datamsgs[4];
                                s_huskybattv = datamsgs[5];
                                s_huskylv = datamsgs[6];
                                s_huskyrv = datamsgs[7];
                                s_huskyldt = datamsgs[8];
                                s_huskyrdt = datamsgs[9];
                                s_huskylmt = datamsgs[10];
                                s_huskyrmt = datamsgs[11];
                                s_huskycapest = datamsgs[12];
                                //float chrgest = int.Parse(datamsgs[13]) * 100;
                                s_huskychrgest = datamsgs[13];
                            }

                        }
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }
    #endregion

    #region JackalTelemetry
    void JackalListenForIncomingTelemetry()
    {
        try
        {
            jackalTelemetrySocket = new TcpListener(IPAddress.Any, JackalTelemetryPort);
            jackalTelemetrySocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            jackalTelemetrySocket.Start();
            Debug.Log("Husky Telemetry Server listening...");
            //Set aside 1KB for the message
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (jackalClient = jackalTelemetrySocket.AcceptTcpClient())
                {
                    using (NetworkStream stream = jackalClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            jackalTelemetryMessage = Encoding.ASCII.GetString(incomingData);
                            //Debug.Log("Husky Message received" + huskyTelemetryMessage);
                            if (jackalTelemetryMessage == "1" || jackalTelemetryMessage == "2" || jackalTelemetryMessage == "3")
                            {
                                jackalCommandMessage = jackalTelemetryMessage;
                            }
                            else
                            {
                                string[] datamsgs = jackalTelemetryMessage.Split(' ');
                                s_jackaluptime = datamsgs[0];
                                s_jackal_ros_cont_loop_freq = datamsgs[1];
                                s_jackal_mcucurr = datamsgs[2];
                                s_jackaldcl = datamsgs[3];
                                s_jackaldcr = datamsgs[4];
                                s_jackalbattv = datamsgs[5];
                                s_jackallv = datamsgs[6];
                                s_jackalrv = datamsgs[7];
                                s_jackalldt = datamsgs[8];
                                s_jackalrdt = datamsgs[9];
                                s_jackallmt = datamsgs[10];
                                s_jackalrmt = datamsgs[11];
                                s_jackalcapest = datamsgs[12];
                                //float chrgest = int.Parse(datamsgs[13]) * 100;
                                s_jackalchrgest = datamsgs[13];
                            }

                        }
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }
    #endregion
    #region HL
    /*
    void HLSpotSocketStart()
    {
        try
        {
            HLSpotSocket = new TcpListener(IPAddress.Any, HLSpotPort);
            HLSpotSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            HLSpotSocket.Start();
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void HLTelloSocketStart()
    {
        try
        {
            HLTelloSocket = new TcpListener(IPAddress.Any, HLTelloPort);
            HLTelloSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            HLTelloSocket.Start();
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void HLHuskySocketStart()
    {
        try
        {
            HLHuskySocket = new TcpListener(IPAddress.Any, HLHuskyPort);
            HLHuskySocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            HLHuskySocket.Start();
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void HLJackalSocketStart()
    {
        try
        {
            HLJackalSocket = new TcpListener(IPAddress.Any, HLJackalPort);
            HLJackalSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            HLJackalSocket.Start();
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    void HLGeneralSocketStart()
    {
        try
        {
            HLGeneralSocket = new TcpListener(IPAddress.Any, HLGeneralPort);
            HLGeneralSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            HLGeneralSocket.Start();
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }
    */
    #endregion
    #region SpotCommands
    void SpotListenForIncomingCommands()
    {
        try
        {
            spotCommandSocket = new TcpListener(IPAddress.Any, SpotCommandPort);
            spotCommandSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            spotCommandSocket.Start();
            //Debug.Log("Spot Command Server listening...");
            //Set aside 1KB for the message
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (spotCommandClient = spotCommandSocket.AcceptTcpClient())
                {
                    using (NetworkStream stream = spotCommandClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            spotCommandMessage = Encoding.ASCII.GetString(incomingData);
                            Debug.Log("Message received" + spotCommandMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }
    #endregion

    #region HuskyCommands
    /*void HuskyListenForIncomingCommands()
    {
        try
        {
            huskyCommandSocket = new TcpListener(IPAddress.Any, HuskyCommandPort);
            huskyCommandSocket.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            huskyCommandSocket.Start();
            Debug.Log("Husky Command Server listening...");
            //Set aside 1KB for the message
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (huskyCommandClient = huskyCommandSocket.AcceptTcpClient())
                {
                    using (NetworkStream stream = huskyCommandClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            huskyCommandMessage = Encoding.ASCII.GetString(incomingData);
                            Debug.Log("Message received" + huskyCommandMessage);
                        }
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }*/
#endregion

    void targetSet()
    {
        //currentTime = Time.time;
        //targetTime = currentTime + 30f;
        //boo_HasTargetBeenSet = true;
        //Debug.Log("Target time set for " + targetTime + ", current time is " + currentTime);
        //txt.text = "Spot moving to safe layby. All platforms will perform inspection in 30 seconds";
    }

    void RunAllMission()
    {
        //HuskyCmd();
        //SpotCorrosion();
        //controller.StartInspection();
        boo_MissionRun = true;
        boo_AllMissionStart = false;
        Debug.Log("Mission running");
        GenericMessages.text = "All platforms performing corrosion inspection";
    }

    // Update is called once per frame
    void Update()
    {
        
        /*if (boo_AllMissionStart == true)
        {
            //if (boo_HasTargetBeenSet == false)
            //{
                //targetSet();
            //}
            
            Debug.Log("Mission started: " + boo_AllMissionStart + "Taget set: " + boo_HasTargetBeenSet + "Mission run: " + boo_MissionRun);
        }
        if (boo_MissionRun == false && boo_HasTargetBeenSet == true && currentTime >= targetTime)
        {
            if (boo_MissionRun == false)
            {
                RunAllMission();
            }
        }*/
        //Show the received message on a predetermined text field for sanity
        if (spotTelemetryMessage != null) //&& clientMessage != txt.text)
        {
            t_runtime.text = "Estimated runtime: " + s_runtime.Split(':')[1] + "s";
            t_runtime2.text = "Estimated runtime: " + s_runtime.Split(':')[1] + "s";
            t_battperc.text = "Battery Status: " + s_battperc.Split(':')[1] + "%";
            t_battperc2.text = "Battery Status: " + s_battperc.Split(':')[1] + "%";
            t_powerstate.text = "Motor power state: " + s_powerstate.Split(':')[1];
            t_powerstate2.text = "Motor power state: " + s_powerstate.Split(':')[1];
            t_chargestatus.text = "Battery charge status: " + s_chargestatus.Split(':')[1];
            t_chargestatus2.text = "Battery charge status: " + s_chargestatus.Split(':')[1];
            //f_SpotSymbiosisLevel += 1f;
        }
        if (spotCommandMessage != null)
        {
            //f_SpotSymbiosisLevel += 1f;

            if (spotCommandMessage == "1")
            {
                GenericMessages.text = "Systems Check Started";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "2")
            {
                GenericMessages.text = "Systems Check Complete!";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "3")
            {
                GenericMessages.text = "Autonomous Mission Started";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "4")
            {
                GenericMessages.text = "Autonomous Mission Complete!";
                spotCommandMessage = "";
                //start another mission
            }
            if (spotCommandMessage == "5")
            {
                GenericMessages.text = "Fetch Mission Started";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "6")
            {
                GenericMessages.text = "Fetch Mission Complete!";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "7")
            {
                GenericMessages.text = "All robot corrosion Mission Started";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "8")
            {
                GenericMessages.text = "All robot corrosion Mission Complete!";
                spotCommandMessage = "";
            }
            if(spotCommandMessage == "9")
            {
                GenericMessages.text = "Spot going home.";
                spotCommandMessage = "";
            }
            if (spotCommandMessage == "10")
            {
                GenericMessages.text = "Spot in layby.";
                spotCommandMessage = "";
                //AllCorrosion();
            }
        }
        if (huskyCommandMessage != null)
        {
            if (huskyCommandMessage == "1")
            {
                GenericMessages.text = "Husky autonomous mission started";
                huskyCommandMessage = "";
            }
            if (huskyCommandMessage == "2")
            {
                GenericMessages.text = "Husky autonomous mission running";
                huskyCommandMessage = "";
            }
            if (huskyCommandMessage == "3")
            {
                //if (!boo_batterymissionstart) { 
                    HuskyErrors.text = "Error: Husky arms not available due to potential power failure. Sending Spot to overview situation. Tello will confirm the path between robots is clear.";
                Debug.Log(HuskyErrors.text + "Husky error");
                    SpotBatt();
                    //boo_batterymissionstart = true;
                    huskyCommandMessage = "";
               // }
                //else { }
            }
            //f_HuskySymbiosisLevel += 0.1f;

        }
        else
        {
            if (f_SpotSymbiosisLevel > 0.0f)
            {
                //f_SpotSymbiosisLevel -= 0.1f;
            }
            if (f_SpotSymbiosisLevel < 0.1f)
            {
                //f_SpotSymbiosisLevel = 0.0f;
            }
        }
        if (huskyTelemetryMessage != null)
        {
            t_huskyuptime.text = "MCU Uptime: " + s_huskyuptime + "ms";
            t_huskyuptime2.text = "MCU Uptime: " + s_huskyuptime + "ms";
            //t_ros_cont_loop_freq.text = "ROS control loop freq = " + s_ros_cont_loop_freq;
            t_huskycurr.text = "MCU Current: " + s_huskymcucurr + "A\n"
                + "L Motor Current: = " + s_huskydcl + "A\n"
                + "R Motor Current: " + s_huskydcr + "A";
            t_huskycurr2.text = "MCU Current: " + s_huskymcucurr + "A\n"
                + "L Motor Current: = " + s_huskydcl + "A\n"
                + "R Motor Current: " + s_huskydcr + "A";
            t_huskyv.text = "Battery Voltage: " + s_huskybattv + "V\n"
                + "Left Driver Voltage: " + s_huskylv + "V\n"
                + "Right Driver Voltage: " + s_huskyrv + "V";
            t_huskyv2.text = "Battery Voltage: " + s_huskybattv + "V\n"
                + "Left Driver Voltage: " + s_huskylv + "V\n"
                + "Right Driver Voltage: " + s_huskyrv + "V";
            t_huskycomponentt.text = "Left Driver Temp: " + s_huskyldt + "C\n"
                + "Right Driver Temp: " + s_huskyrdt + "C\n"
                + "Left Motor Temp: " + s_huskylmt + "C\n"
                + "Right Motor Temp: " + s_huskyrmt + "C";
            t_huskycomponentt2.text = "Left Driver Temp: " + s_huskyldt + "C\n"
                + "Right Driver Temp: " + s_huskyrdt + "C\n"
                + "Left Motor Temp: " + s_huskylmt + "C\n"
                + "Right Motor Temp: " + s_huskyrmt + "C";
            float f_huskychrgest = float.Parse(s_huskychrgest) * 100;
            t_huskybattcap.text = "Battery Capacity = " + s_huskycapest + "Wh\n"
                + "Charge Estimate = " + f_huskychrgest + "%";
            t_huskybattcap2.text = "Battery Capacity = " + s_huskycapest + "Wh\n"
                + "Charge Estimate = " + f_huskychrgest + "%";
            //f_HuskySymbiosisLevel += 0.01f;
        }
        else
        {
            if (f_HuskySymbiosisLevel > 0.0f)
            {
                //f_HuskySymbiosisLevel -= 0.1f;
            }
            if (f_HuskySymbiosisLevel < 0.1f)
            {
                //f_HuskySymbiosisLevel = 0.0f;
            }
        }
       /* if (f_HuskySymbiosisLevel > 180.0f && huskyCommandMessage != null)
        {
            f_HuskySymbiosisLevel = 180.0f;
        }
        else if(f_HuskySymbiosisLevel > 90.0f && huskyCommandMessage == null)
        {
            f_HuskySymbiosisLevel = 90.0f;
        }
        if (f_SpotSymbiosisLevel > 180.0f)
        {
            f_SpotSymbiosisLevel = 180.0f;
        }
       */
        if (jackalCommandMessage != null)
        {
            if (jackalCommandMessage == "1")
            {
                GenericMessages.text = "Jackal autonomous mission started";
                jackalCommandMessage = "";
            }
            if (huskyCommandMessage == "2")
            {
                GenericMessages.text = "Jackal autonomous mission running";
                jackalCommandMessage = "";
            }

        }
        if (jackalTelemetryMessage != null)
        {
            t_jackaluptime.text = "MCU Uptime: " + s_jackaluptime + "ms";
            t_jackaluptime2.text = "MCU Uptime: " + s_jackaluptime + "ms";
            //t_ros_cont_loop_freq.text = "ROS control loop freq = " + s_ros_cont_loop_freq;
            t_jackalcurr.text = "MCU Current: " + s_jackal_mcucurr + "A\n"
                + "L Motor Current: = " + s_jackaldcl + "A\n"
                + "R Motor Current: " + s_jackaldcr + "A";
            t_jackalcurr2.text = "MCU Current: " + s_jackal_mcucurr + "A\n"
                + "L Motor Current: = " + s_jackaldcl + "A\n"
                + "R Motor Current: " + s_jackaldcr + "A";
            t_jackalv.text = "Battery Voltage: " + s_jackalbattv + "V\n"
                + "Left Driver Voltage: " + s_jackallv + "V\n"
                + "Right Driver Voltage: " + s_jackalrv + "V";
            t_jackalv2.text = "Battery Voltage: " + s_jackalbattv + "V\n"
                + "Left Driver Voltage: " + s_jackallv + "V\n"
                + "Right Driver Voltage: " + s_jackalrv + "V";
            t_jackalcomponentt.text = "Left Driver Temp: " + s_jackalldt + "C\n"
                + "Right Driver Temp: " + s_jackalrdt + "C\n"
                + "Left Motor Temp: " + s_jackallmt + "C\n"
                + "Right Motor Temp: " + s_jackalrmt + "C";
            t_jackalcomponentt2.text = "Left Driver Temp: " + s_huskyldt + "C\n"
                + "Right Driver Temp: " + s_jackalrdt + "C\n"
                + "Left Motor Temp: " + s_jackallmt + "C\n"
                + "Right Motor Temp: " + s_jackalrmt + "C";
            float f_jackalchrgest = float.Parse(s_jackalchrgest) * 100;
            t_jackalbattcap.text = "Battery Capacity = " + s_jackalcapest + "Wh\n"
                + "Charge Estimate = " + f_jackalchrgest + "%";
            t_jackalbattcap2.text = "Battery Capacity = " + s_jackalcapest + "Wh\n"
                + "Charge Estimate = " + f_jackalchrgest + "%";
        }

        go_HuskySymbiosisNeedle.transform.localEulerAngles = new Vector3(0, 0, -f_HuskySymbiosisLevel) + go_TelloSymbiosisNeedle.transform.localEulerAngles;
        go_SpotSymbiosisNeedle.transform.localEulerAngles = new Vector3(0, 0, -f_SpotSymbiosisLevel);
        go_TelloSymbiosisNeedle.transform.localEulerAngles = new Vector3(0, 0, -f_TelloSymbiosisLevel);

    }

    //Clean up the thread
    private void OnApplicationQuit()
    {
        if (spotTelemetrySocket != null)
        {
            spotTelemetrySocket.Stop();
            spotTelemetryThread.Abort();
        }
        if (spotCommandSocket != null)
        { 
            spotCommandSocket.Stop();
            spotCommandThread.Abort();
        }
        if (huskyTelemetrySocket != null)
        {
            huskyTelemetrySocket.Stop();
            huskyTelemetryThread.Abort();
        }
        if (huskyCommandSocket != null)
        {
            huskyCommandSocket.Stop();
            huskyCommandThread.Abort();
        }
        if (jackalTelemetrySocket != null)
        {
            jackalTelemetrySocket.Stop();
            jackalTelemetryThread.Abort();
        }
        if (jackalCommandSocket != null)
        {
            jackalCommandSocket.Stop();
            jackalCommandThread.Abort();
        }
    }
}