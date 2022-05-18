///This reads messages sent by SerialController.cs
///Sam Harper, 2021

using UnityEngine;
using TMPro;

public class RFIDRead : MonoBehaviour
{
    public TextMeshProUGUI t_RFIDOutputField;
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        //Set ID to zero to ensure Tello cannot be controlled unless the correct RFID info is given
        ID = 0;
    }

    public void OnMessageArrived(string msg)
    {
        //Displays RFID info if the card is not recognised
        t_RFIDOutputField.text = msg;
        //These if statements are hardcoded with known RFID values, change these if you are using a different card set/different users
        if(msg.Contains("6400D85722C9"))
        {
            ID = 1;
            t_RFIDOutputField.text = "Hello Sam";
            
        }
        if (msg.Contains("67009457F450"))
        {
            ID = 2;
            t_RFIDOutputField.text = "Hello Daniel";

        }
        Debug.Log("ID = " + ID);
    }

    void OnConnectionEvent (bool success)
    {
        //Sanity check to make sure the RFID reader is working properly
        t_RFIDOutputField.text = "RFID Connected: " + success;
    }
}
