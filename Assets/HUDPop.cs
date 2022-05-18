using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDPop : MonoBehaviour
{
    public Canvas LInfo, RInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LInfo.enabled = false;
        RInfo.enabled = false;
        RaycastHit hitInfo;
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out hitInfo,
                20.0f,
                Physics.DefaultRaycastLayers))
        {

            if (hitInfo.collider.gameObject.tag == "RWheel")
            {
                RInfo.enabled = true;
            }
            if (hitInfo.collider.gameObject.name == "LWheel")
            {
                Debug.Log("Hit");
                LInfo.enabled = true;
            }
        }
    }
}
