using System.Collections;
using System.Collections.Generic;
using ViveSR.anipal.Eye;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

/// <summary>
/// Example usage for eye tracking callback
/// Note: Callback runs on a separate thread to report at ~120hz.
/// Unity is not threadsafe and cannot call any UnityEngine api from within callback thread.
/// </summary>
public class EyeBase : MonoBehaviour
{
    private static EyeData_v2 eyeData = new EyeData_v2();
    private static bool eye_callback_registered = false;
    private static float lastTime, currentTime;
    private float updateSpeed = 0;

    private void Start()
    {
        if (!SRanipal_Eye_Framework.Instance.EnableEye)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
            SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT) return;

        if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
        {
            SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = true;
            Debug.Log("EyeCallback2 registered");

        }
        else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
        {
            SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
            Debug.Log("EyeCallback2 not registered");
        }

        

        updateSpeed = currentTime - lastTime;
        // Debug.Log("Update Speed: " + updateSpeed + " ms.");
    }

    private void OnDisable()
    {
        Release();
    }

    void OnApplicationQuit()
    {
        Release();
    }

    /// <summary>
    /// Release callback thread when disabled or quit
    /// </summary>
    private static void Release()
    {
        if (eye_callback_registered == true)
        {
            SRanipal_Eye.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
            eye_callback_registered = false;
            Debug.Log("EyeCallback2 de/registered");

        }
    }

    /// <summary>
    /// Eye tracking data callback thread.
    /// Reports data at ~120hz
    /// MonoPInvokeCallback attribute required for IL2CPP scripting backend
    /// </summary>
    /// <param name="eye_data">Reference to latest eye_data</param>
    private static void EyeCallback(ref EyeData_v2 eye_data)
    {
        // Note: working with the eyeData struct in Update() messes up latency!
        eyeData = eye_data;
        lastTime = currentTime;
        currentTime = eyeData.timestamp;
        // Debug.Log("EyeCallback2 executed");
    }
}
