///This reads messages sent by SerialController.cs
///Sam Harper, 2021

using UnityEngine;
using TMPro;

public class Limpet : MonoBehaviour
{
    public TextMeshProUGUI LimpetOutputField;
    bool AccelWarn = false;

    public void OnMessageArrived(string msg)
    {
        if (msg != null)
        {
            //Displays RFID info if the card is not recognised
            string[] parts = msg.Split(',');
            string AccelX = float.Parse(parts[0]).ToString("n2");
            string AccelY = float.Parse(parts[1]).ToString("n2");
            string AccelZ = float.Parse(parts[2]).ToString("n2");
            if(float.Parse(AccelZ) > 1 || float.Parse(AccelZ) < -1)
            {
                AccelWarn = true;
            }
            string GyroX = float.Parse(parts[3]).ToString("n2");
            string GyroY = float.Parse(parts[4]).ToString("n2");
            string GyroZ = float.Parse(parts[5]).ToString("n2");
            string Temp = float.Parse(parts[6]).ToString("n2");
            if (AccelWarn == false)
            {
                LimpetOutputField.text = "Limpet\n" +
                    "Accel X: " + AccelX + "G\n" +
                    "Accel Y: " + AccelY + "G\n" +
                    "Accel Z: " + AccelZ + "G\n" +
                    "Gyro X: " + GyroX + "deg/s\n" +
                    "Gyro X: " + GyroY + "deg/s\n" +
                    "Gyro X: " + GyroZ + "deg/s\n" +
                    "Temp: " + Temp + "C\n";
            }
            else if (AccelWarn == true)
            {
                LimpetOutputField.text = "Limpet\n" +
                    "WARNING, ACCELERATION EVENT DETECTED!\n" +
                    "Accel X: " + AccelX + "G\n" +
                    "Accel Y: " + AccelY + "G\n" +
                    "Accel Z: " + AccelZ + "G\n" +
                    "Gyro X: " + GyroX + "deg/s\n" +
                    "Gyro X: " + GyroY + "deg/s\n" +
                    "Gyro X: " + GyroZ + "deg/s\n" +
                    "Temp: " + Temp + "C\n";
                LimpetOutputField.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            }
        }
        else
        {
            LimpetOutputField.text = "No Data from Limpet";
        }
    }

    void OnConnectionEvent (bool success)
    {
        //Sanity check to make sure the RFID reader is working properly
        LimpetOutputField.text = "Limpet Connected: " + success;
    }
}
