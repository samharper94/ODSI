using System.Collections;
using UnityEngine;
using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class TCPServer_Image : MonoBehaviour
{
    private Thread tcpListenerThread;
    private TcpClient connectedTcpClient;
    public int port;
    int recieved;
    int fileSize;
    public Texture2D camTexture;
    public RawImage camImage;
    NetworkStream dataStream;
    MemoryStream ms;

    // Use this for initialization
    void Start()
    {
        //Start a thread for listening so application doesn't hang. This runs until the application is closed.
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncoming));
        tcpListenerThread.IsBackground = true;
        tcpListenerThread.Start();
    }
    void ListenForIncoming()
    {
        try
        {
            fileSize = 0;
            recieved = 0;
            connectedTcpClient = new TcpClient("127.0.0.1", port);
            dataStream = connectedTcpClient.GetStream();
            Debug.Log("Client listening...");
            //Set aside 1KB for the message
            byte[] bytes = new byte[1024];
            dataStream.Read(bytes, 0, bytes.Length);
            fileSize = Int32.Parse(Encoding.Default.GetString(bytes));
            byte[] results = Encoding.ASCII.GetBytes(fileSize.ToString());
            dataStream.Write(results, 0, results.Length);
            ms = new MemoryStream();
            int increment = 0;
            while (recieved < fileSize)
            {
                byte[] data = new byte[connectedTcpClient.ReceiveBufferSize];
                increment = dataStream.Read(data, 0, data.Length);
                recieved += increment;
                ms.Write(data.Take(increment).ToArray(), 0, increment);

            }
            UnityMainThreadDispatcher.Instance().Enqueue(convertBytesToTexture(ms.ToArray()));

            //while (true)
            //{
            //    using (connectedTcpClient = tcpListener.AcceptTcpClient())
            //    {
            //        using (NetworkStream stream = connectedTcpClient.GetStream())
            //        {
            //            int length;
            //            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            //            {
            //                var incomingData = new byte[length];
            //                Array.Copy(bytes, 0, incomingData, 0, length);
            //                clientMessage = Encoding.ASCII.GetString(incomingData);
            //                Debug.Log("Message received" + clientMessage);
            //            }
            //        }
            //    }
            //}
        }
        catch (SocketException e)
        {
            Debug.Log("Socket Exception " + e);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Clean up the thread
    private void OnApplicationQuit()
    {
        tcpListenerThread.Abort();
    }

    IEnumerator convertBytesToTexture(byte[] byteArray)
    {
        try
        {
            camTexture.LoadImage(byteArray); //Texture2D object
            camImage.texture = camTexture; //RawImage object
        }
        catch (Exception e)
        {
            print(e);
        }
        yield return 0;
    }
}
