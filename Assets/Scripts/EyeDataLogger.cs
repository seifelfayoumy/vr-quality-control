using System.Runtime.InteropServices;
using UnityEngine;
using ViveSR.anipal.Eye;

public class EyeDataLogger : MonoBehaviour {

    public delegate void eyeCallbackDelegate(EyeData_v2 eyeData);

    public void eyeCallback(EyeData_v2 eyeData) {
        Debug.Log("GETTING EYE DATA");
    }

    // Callback function to receive eye data from SRanipal
    private void OnGazeRay(GazeIndex index, Vector3 origin, Vector3 direction, ref RaycastHit hitInfo, float distance, ref bool focusInfo) {
        if (index == GazeIndex.COMBINE) // Focus on combined eye data
        {
            Debug.Log("Combined Gaze Origin: " + origin);
            Debug.Log("Combined Gaze Direction: " + direction);

            // Optional: Get the object being looked at (if any)
            if (hitInfo.collider != null) {
                Debug.Log("Hit Object: " + hitInfo.collider.gameObject.name);
            }
        }
    }

    void Start() {

        eyeCallbackDelegate eyeCallBackFuntion = eyeCallback;
        System.IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(eyeCallBackFuntion);



        // Make sure eye tracking is initialized
        if (!SRanipal_Eye_Framework.Instance.EnableEye) {
            Debug.LogError("Failed to enable SRanipal Eye Framework!");
            return;
        }

        // Initialize callback if available
        if (!SRanipal_Eye_Framework.Instance.EnableEyeDataCallback) {
            Debug.LogError("SRanipal Eye Framework eye callback not supported!");
            return;
        }

        SRanipal_Eye_Framework.Instance.StartFramework();
        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(callbackPtr);
    }

    void OnEnable() {
       // SRanipal_Eye_Framework.Instance.StartFramework();
        

        // Correct Method:
        //if (SRanipal_Eye.RegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate(OnGazeRay)) != Vive.SR.anipal.Eye.SRanipal_Error.WORKING) {
        //    Debug.LogError("Error registering eye data callback!");
       // }
    }

    // Unsubscribe from the callback 
    void OnDisable() {
     //   SRanipal_Eye.UnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate(OnGazeRay));
    }
}
