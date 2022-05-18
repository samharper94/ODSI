using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading;
using System.Net.Sockets;
using System.Text;

public class CustomTelloController : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread udpClientThread;

    // Start is called before the first frame update
    void Start()
    {
        udpClientThread = new Thread(new ThreadStart(connectToTello));
        udpClientThread.IsBackground = true;
        udpClientThread.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void connectToTello()
    {
        try
        {
            udpClient.Connect("192.168.10.1", 8889);
            byte[] sendBytes = Encoding.ASCII.GetBytes("Command");

            udpClient.Send(sendBytes, sendBytes.Length);

            sendBytes = Encoding.ASCII.GetBytes("takeoff");
        }
        catch (System.Exception e)
        {
            Debug.Log("UDP Exception: " + e);
        }

    }
}
