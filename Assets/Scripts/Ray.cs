//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class RayEye_Single : MonoBehaviour
            {

                [SerializeField] private Color raycolor = Color.white;
                [SerializeField] private float gazeRayVerticalOffset = 0.05f;
                [SerializeField] private float lengthOfRay = 10.0f;
                [SerializeField] private float widthOfRay = 0.005f;
                [SerializeField] private LineRenderer gazeRayRenderer;
                private static EyeData_v2 eyeData = new EyeData_v2();
                private bool eye_callback_registered = false;
                private float defaultRayLength = 25;
                private Vector3 gazeOrigin, gazeDirection, iPDOffset, gazeDirectionCombined, startPos = Vector3.zero;
                private readonly float IPD = 0.065f;
                private enum GazeType
                {
                    Left,
                    Right,
                    Cyclopian
                }
                [SerializeField] private GazeType gazeType = GazeType.Cyclopian;

                private void Start()
                {
                    if (!SRanipal_Eye_Framework.Instance.EnableEye)
                    {
                        enabled = false;
                        return;
                    }
                    defaultRayLength = lengthOfRay;
                    gazeRayRenderer = GetComponent<LineRenderer>();
                    SetUpLineRenderer(gazeRayRenderer);
                }

                private void Update()
                {

                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING &&
                        SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.NOT_SUPPORT)
                    {
                        DoDeviceSimulator();
                    }
                    else
                    {
                        DoSRAnipal();
                    }
                }


                private void SetUpLineRenderer(LineRenderer _lineRenderer)
                {
                    _lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));

                    _lineRenderer.startColor = raycolor;
                    _lineRenderer.endColor = raycolor;

                    _lineRenderer.startWidth = widthOfRay;
                    _lineRenderer.endWidth = widthOfRay;
                }

                private void DoDeviceSimulator()
                {

                    gazeDirection = Camera.main.transform.forward;

                    switch (gazeType)
                    {
                        case GazeType.Left:
                            iPDOffset = Camera.main.transform.right * (IPD / 2);
                            break;
                        case GazeType.Right:
                            iPDOffset = Camera.main.transform.right * (-IPD / 2);
                            break;
                        case GazeType.Cyclopian:
                            iPDOffset = Vector3.zero;
                            break;
                    }
                    VisualizeGaze(false, gazeRayRenderer);
                }

                private void DoSRAnipal()
                {
                    // TODO check if we can to this in start
                    if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == true && eye_callback_registered == false)
                    {
                        SRanipal_Eye_v2.WrapperRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = true;
                        Debug.Log("EyeCallback registered");
                    }
                    else if (SRanipal_Eye_Framework.Instance.EnableEyeDataCallback == false && eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback));
                        eye_callback_registered = false;
                        Debug.Log("EyeCallback not registered");
                    }

					//which eye
                    if (eye_callback_registered)
                    {
                        switch (gazeType)
                        {
                            case GazeType.Left:
                                DoLeft();
                                break;
                            case GazeType.Right:
                                DoRight();
                                break;
                            case GazeType.Cyclopian:
                                DoCyclopian();
                                break;
                        }
                    }
                    else
                    {
                        SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOrigin, out gazeDirection, eyeData);
                    }

					//do something with the ray
                    DoRaycast();
                }

                private void SetLineRendererStatus(LineRenderer _lr, bool _isActive)
                {
                    _lr.enabled = _isActive;
                }

                private void DoLeft()
                {
                    SRanipal_Eye_v2.GetGazeRay(GazeIndex.LEFT, out gazeOrigin, out gazeDirection, eyeData);
                    iPDOffset = Camera.main.transform.right * (IPD / 2);
                    VisualizeGaze(true, gazeRayRenderer);
                }

                private void DoRight()
                {
                    SRanipal_Eye_v2.GetGazeRay(GazeIndex.RIGHT, out gazeOrigin, out gazeDirection, eyeData);
                    iPDOffset = Camera.main.transform.right * (-IPD / 2);
                    VisualizeGaze(true, gazeRayRenderer);
                }

                private void DoCyclopian()
                {
                    SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOrigin, out gazeDirection, eyeData);
                    iPDOffset = Vector3.zero;
                    VisualizeGaze(true, gazeRayRenderer);
                }

                private void VisualizeGaze(bool _isTrueEyeTracker, LineRenderer _lineRenderer)
                {
					VisualizeRay(_isTrueEyeTracker, _lineRenderer);
                }

                private void VisualizeRay(bool _isTrueEyeTracker, LineRenderer _lineRenderer)
                {
                    SetLineRendererStatus(_lineRenderer, true);

                    startPos = Camera.main.transform.position - Camera.main.transform.up * gazeRayVerticalOffset - iPDOffset;

                    if (_isTrueEyeTracker)//this is needed for the device simulator
                    {
                        gazeDirectionCombined = Camera.main.transform.TransformDirection(gazeDirection);
                    }
                    else
                    {
                        gazeDirectionCombined = Camera.main.transform.forward;
                    }
                    _lineRenderer.SetPosition(0, startPos);
                    _lineRenderer.SetPosition(1, startPos + gazeDirectionCombined * lengthOfRay);
                }

                private void Release()
                {
                    if (eye_callback_registered == true)
                    {
                        SRanipal_Eye_v2.WrapperUnRegisterEyeDataCallback(
                            Marshal.GetFunctionPointerForDelegate((SRanipal_Eye_v2.CallbackBasic)EyeCallback)
                            );
                        eye_callback_registered = false;
                        Debug.Log("EyeCallback de/registered");
                    }
                }

                private static void EyeCallback(ref EyeData_v2 eye_data)
                {
                    eyeData = eye_data;
                }

                private void DoRaycast()
                {
					DoRaycastSingle();
                }


                private void DoRaycastSingle()
                {
                    RaycastHit hit;
                    bool isHit = Physics.Raycast(startPos, gazeDirectionCombined, out hit, Mathf.Infinity);

                    if (isHit)
                    {
                        Debug.Log(hit.collider.gameObject.name + "; " + this.transform.gameObject.name);
						lengthOfRay = hit.distance;
                    }
                    else
                    {
                        resetRayLength();
                    }
                }

                private void resetRayLength()
                {
                    lengthOfRay = defaultRayLength;
                }
            }
        }
    }
}
