using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
using Pose = UnityEngine.XR.OpenXR.Input.Pose; // Make sure you have this using statement

public class EyeGazeNosePose : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions; // Assign your asset in the Inspector
    private InputAction eyeGazePoseAction; 

    void Start()
    {
        // Find the correct action based on its name or path
        eyeGazePoseAction = inputActions.FindAction("eye"); // Replace with your action name
        eyeGazePoseAction.Enable();
    }
    
    void Update()
    {
        Pose eyeGazePose = eyeGazePoseAction.ReadValue<Pose>();
        Debug.Log("Eye Gaze Pose: " + eyeGazePose.position + ", " + eyeGazePose.rotation);
    }

}