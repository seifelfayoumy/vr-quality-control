using UnityEngine;
using UnityEngine.XR.OpenXR.Input;

public class EyeGazeNosePose : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // Eye Gaze
        TryGetActionData(OpenXRInput.EyeGazeInteraction, out var eyeGazeActionData);
        if (eyeGazeActionData.active)
        {
            Vector3 eyeGazeDirection = eyeGazeActionData.pose.forward;
            Vector3 eyeGazePosition = eyeGazeActionData.pose.position;

            // Use eyeGazeDirection and eyeGazePosition for your interactions here
            Debug.Log("Eye Gaze Direction: " + eyeGazeDirection);
            Debug.Log("Eye Gaze Position: " + eyeGazePosition);
        }

        // Nose Pose (representing head pose)
        TryGetActionData(OpenXRInput.HeadPose, out var headPoseActionData);
        if (headPoseActionData.active)
        {
            Vector3 nosePosition = headPoseActionData.pose.position;
            Quaternion noseRotation = headPoseActionData.pose.rotation;

            // Use nosePosition and noseRotation for your interactions here
            Debug.Log("Nose Position: " + nosePosition);
            Debug.Log("Nose Rotation: " + noseRotation);
        }
    }

    // Helper function to streamline getting OpenXR action data
    private bool TryGetActionData<T>(Action<T> action, out T actionData) where T : struct
    {
        if (action.enabled && action.TryGetCurrentState(out actionData))
        {
            return true;
        }

        actionData = default;
        return false;
    }
}
