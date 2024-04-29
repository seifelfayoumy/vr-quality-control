using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeDataLogger : MonoBehaviour
{
    private static EyeData_v2 eyeData = new EyeData_v2();
    private bool eyeCallbackRegistered = false;

    void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
         {
            enabled = false;
             return;
         }
         if (!SRanipal_Eye_Framework.Instance.EnableEyeDataCallback)
         Debug.LogError("Eye data callback not enabled!");
    }

    void Update()
    {

        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING)
            return;

        if (!eyeCallbackRegistered)
        {
            // if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback &&
            //     SRanipal_Eye.RegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye.CallbackBasic)EyeCallback)))
            // {
            //     eyeCallbackRegistered = true;
            // }
       
            //Debug.Log("not registered");
        }

        SRanipal_Eye_API.RegisterEyeDataCallback_v2(true);
  
    }

     void EyeCallback(ref EyeData_v2 eye_data)
    {
        Debug.Log("readinggggg");
        eyeData = eye_data; // Copy the eye data from the callback

        // Example: Print gaze direction
        Vector3 combinedGaze = eyeData.verbose_data.combined.eye_data.gaze_direction_normalized;
        Debug.Log("Combined Gaze Direction: " + combinedGaze); 
    }
}
