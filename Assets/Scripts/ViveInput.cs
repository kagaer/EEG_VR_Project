using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveInput : MonoBehaviour
{
    //public SteamVR_ActionSet testAction;
    public SteamVR_Action_Boolean leftTrigger;
    public SteamVR_Action_Boolean rightTrigger;
    //public SteamVR_Action_Boolean startTrialButton;

    public bool hasStarted = false;
    public bool inTrial = false;
    public bool objectSelected = false;
    public string objectHit = "";
    public float rt;
    public float startTrialTime;

    private void Awake()
    {
        leftTrigger = SteamVR_Actions._default.TriggerLeft;
        rightTrigger = SteamVR_Actions._default.TriggerRight;
        //startTrialButton = SteamVR_Actions._default.TouchpadSelect;
    }

    //private void Start()
    //{
    //    testAction.Activate(SteamVR_Input_Sources.Any, 0, true);
    //}
    // Update is called once per frame
    void Update()
    {
        if (hasStarted)
        {
            if (inTrial)
            {
                objectHit = "";


                if (leftTrigger[SteamVR_Input_Sources.LeftHand].stateDown)
                {
                    rt = Time.realtimeSinceStartup - startTrialTime;
                    objectHit = "left";
                    Debug.Log("Left");
                    objectSelected = true;
                    inTrial = false;
                }

                if (rightTrigger[SteamVR_Input_Sources.RightHand].stateDown)
                {
                    objectHit = "right";
                    Debug.Log("Right");
                    objectSelected = true;
                    inTrial = false;
                }
            }
        }
    }
}
