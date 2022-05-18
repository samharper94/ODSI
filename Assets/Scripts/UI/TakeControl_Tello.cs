///This class allows the user to take control of the Tello and toggles all appropriate buttons and controls
///Sam Harper, 2021

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class TakeControl_Tello : MonoBehaviour
{
    public Button b_TakeControlTello, b_TakeOff, b_Land, b_Flip;
    public TextMeshProUGUI t_TakeControlButtonText, t_ButtonInstructions, t_LoginWarning;
    public GameObject go_TelloVideo;
    bool isTelloToggled;
    public RFIDRead rfidRead;
    public InputAction IA_SkipRFID;

    private void OnEnable()
    {
        IA_SkipRFID.Enable();
    }

    private void OnDisable()
    {
        IA_SkipRFID.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        isTelloToggled = false;
        b_TakeControlTello.onClick.AddListener(ToggleTelloControls);
    }

    void ToggleTelloControls()
    {
        if(!isTelloToggled)
        {
//            if(rfidRead.ID == 1 || rfidRead.ID == 2)
  //          {
                b_TakeOff.gameObject.SetActive(true);
                b_Land.gameObject.SetActive(true);
                b_Flip.gameObject.SetActive(true);
                isTelloToggled = true;
                t_ButtonInstructions.gameObject.SetActive(true);
                t_LoginWarning.gameObject.SetActive(false);
    //        }
     //       else
       //     {
         //       t_LoginWarning.gameObject.SetActive(true);
           // }
        }
        else
        {
            b_TakeOff.gameObject.SetActive(false);
            b_Land.gameObject.SetActive(false);
            b_Flip.gameObject.SetActive(false);
            isTelloToggled = false;
            t_ButtonInstructions.gameObject.SetActive(false);
        }
    }

    /*private void Update()
    {
        if (IA_SkipRFID.triggered)
        {
            rfidRead.ID = 1;
        }
    }*/
}
