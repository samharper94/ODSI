using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseMovement : MonoBehaviour
{
    float rotationSpeed = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
            // select the axis by which you want to rotate the GameObject
            transform.RotateAround(Vector3.down, XaxisRotation);
            transform.RotateAround(Vector3.right, YaxisRotation);
        }

        if (Input.GetMouseButton(1))
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
}
