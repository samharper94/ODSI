﻿using UnityEngine;
using System.Collections;

public class UR5ControllerL : MonoBehaviour {

    public GameObject RobotBase;
    public float[] jointValues = new float[6];
    public float[] tempJValues = new float[6];
    private GameObject[] jointList = new GameObject[6];
    private GameObject[] jointBodyList = new GameObject[6];
    private float[] upperLimit = { 180f, 180f, 180f, 180f, 180f, 180f };
    private float[] lowerLimit = { -180f, -180f, -180f, -180f, -180f, -180f };
    private Vector3 currentRotation;
    private float tempX;

    // Use this for initialization
    void Start () {


        jointValues[0] = 89.96f;
        jointValues[1] = -163.15f;
        jointValues[2] = 160.32f;
        jointValues[3] = -89.95f;
        jointValues[4] = 30.06f;
        jointValues[5] = 0f;

        initializeJoints();
	}

    /*
    0 = -45     0= -45
    -90 = 0     0= +90
    0 = 90      0= +90
    -90=58      0= +148
    0=-83       0= -83
    0=17        0= +17
         */

	// Update is called once per frame
	void LateUpdate () {
        tempJValues[0] = jointValues[0] - 45;
        tempJValues[1] = jointValues[1] + 90;
        tempJValues[2] = jointValues[2] + 90;
        tempJValues[3] = jointValues[3] + 148;
        tempJValues[4] = jointValues[4] - 83;
        tempJValues[5] = jointValues[5] + 17;
        //Debug.Log(currentRotation.x);
        for ( int i = 0; i < 6; i ++) {
            currentRotation = jointList[i].transform.localEulerAngles;
            if (i == 4)
            {
                currentRotation.x = tempJValues[i];
                currentRotation.y = 0;
                currentRotation.z = 0;
                //tempX = currentRotation.x;
                //Debug.Log(currentRotation);
            }
            else
            {
                currentRotation.y = tempJValues[i];
            }
            jointList[i].transform.localEulerAngles = currentRotation;
        }
	}

    void OnGUI() {
        int boundary = 20;

//#if UNITY_EDITOR
//        int labelHeight = 20;
//        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 20;
//#else
        int labelHeight = 40;
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = 40;
//#endif
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        for (int i = 0; i < 6; i++) {
            GUI.Label(new Rect(boundary, boundary + ( i * 2 + 1 ) * labelHeight, labelHeight * 4, labelHeight), "Joint " + i + ": ");
            jointValues[i] = GUI.HorizontalSlider(new Rect(boundary + labelHeight * 4, boundary + (i * 2 + 1) * labelHeight + labelHeight / 4, labelHeight * 5, labelHeight), jointValues[i], lowerLimit[i], upperLimit[i]);
        }
    }

    // Create the list of GameObjects that represent each joint of the robot
    void initializeJoints() {
        var RobotChildren = RobotBase.GetComponentsInChildren<Transform>();
        for (int i = 0; i < RobotChildren.Length; i++) {
            if (RobotChildren[i].name == "J1:1") {
                jointList[0] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "J2:1") {
                jointList[1] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "J3:1") {
                jointList[2] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "J4:1") {
                jointList[3] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "J5:1") {
                jointList[4] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "J6:1") {
                jointList[5] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "Body1 11")
            {
                jointBodyList[0] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "Body1 19")
            {
                jointBodyList[1] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "Body1 9")
            {
                jointBodyList[2] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "Body1 10")
            {
                jointBodyList[3] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "Body1 22")
            {
                jointBodyList[4] = RobotChildren[i].gameObject;
            }
            else if (RobotChildren[i].name == "Body1 48")
            {
                jointBodyList[5] = RobotChildren[i].gameObject;
            }
        }
    }
}
