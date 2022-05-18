using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using TelloLib;

public class TelloAutonomousMission : SingletonMonoBehaviour<TelloAutonomousMission>
{
    public Button b_AutonomousTelloMission;
    private Thread waitThread;
    float timeDiff = 0f;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        //waitThread = new Thread(new ThreadStart(Inspection));
        //waitThread.IsBackground = true;
        
        b_AutonomousTelloMission.onClick.AddListener(StartMission);
        float time = Time.time;
    }

    void StartMission()
    {
        Tello.TelloEduMission("go 160 0 200 70 m4");

        Debug.Log("Going 160cm from pad 4");





        //waitThread.Start();
    }

    void Inspection()
    {

        //while (Tello.state.height > 1)
        //{
        //    Tello.land();
        //}
        //Thread.CurrentThread.Abort();
    }
    /*
    IEnumerator TelloInspection()
    {
        Tello.TelloEduMission("mon");

        Debug.Log("Marker detection enabled");

        yield return new WaitForSeconds(0.5f);

        Tello.TelloEduMission("mdirection 2");

        Debug.Log("Cameras enabled");

        yield return new WaitForSeconds(0.5f);

        Tello.takeOff();

        Debug.Log("Takeoff");

        yield return new WaitForSeconds(10f);

        Tello.TelloEduMission("go 0 0 100 50 m4");

        Debug.Log("Going to pad 4");

        yield return new WaitForSeconds(10f);

        Tello.TelloEduMission("go 160 0 200 70 m1");

        Debug.Log("Going to pad 1");

        yield return new WaitForSeconds(8f);

        Tello.land();

        Debug.Log("Land");

        //yield return new WaitForSeconds(5);

        //waitThread.Start();
    }*/
}
