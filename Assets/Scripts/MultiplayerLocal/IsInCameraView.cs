using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerLocal;
using UnityEngine;

public class IsInCameraView : MonoBehaviour
{
    public float TimerOutOfView;
    public Camera CameraMain;
    public bool IsInCameraViewValue;
    public float Timer;
    public MultipleTargetCamera MultipleTargetCamera;
    public Vector3 TargetRespawn;

    private Plane[] _cameraFrustrum;
    private Bounds _bounds;
    private Collider _collider;

    private void Start()
    {
        CameraMain = Camera.main;
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        _bounds = _collider.bounds;
        _cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(CameraMain);
        IsInCameraViewValue = GeometryUtility.TestPlanesAABB(_cameraFrustrum, _bounds);

        if (IsInCameraViewValue == false)
        {
            MultipleTargetCamera.RemoveTarget(transform);
            if (Timer < TimerOutOfView)
            {
                Timer += Time.deltaTime;
            }
            TargetRespawn = new Vector3(CameraMain.transform.localPosition.x, transform.localPosition.y , CameraMain.transform.localPosition.z);

        }
        else
        {
            Timer = 0;
        }
    }
}