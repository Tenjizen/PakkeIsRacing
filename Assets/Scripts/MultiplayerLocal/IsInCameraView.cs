using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerLocal;
using UnityEngine;

public class IsInCameraView : MonoBehaviour
{
    public float TimerOutOfView;

    private Camera _camera;
    private Plane[] _cameraFrustrum;
    public bool IsInCameraViewValue;
    private Bounds _bounds;
    public float Timer;
    private Collider _collider;

    public MultipleTargetCamera MultipleTargetCamera;

    public CharacterMultiPlayerManager Character;


    public Vector3 TargetRespawn;

    private void Start()
    {
        _camera = Camera.main;
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        //Debug.Log(_camera.transform.position + " pos");
        //Debug.Log(_camera.transform.localPosition + " local pos");

        _bounds = _collider.bounds;
        _cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(_camera);
        IsInCameraViewValue = GeometryUtility.TestPlanesAABB(_cameraFrustrum, _bounds);

        if (IsInCameraViewValue == false)
        {
            MultipleTargetCamera.RemoveTarget(transform);
            if (Timer < TimerOutOfView)
            {
                Timer += Time.deltaTime;
                Character.CharacterManager.CurrentStateBaseProperty.CanBeMoved = false;
            }
            TargetRespawn = new Vector3(_camera.transform.localPosition.x, transform.localPosition.y , _camera.transform.localPosition.z);

            //else
            //{
            //    Character.SetPosKayak(Vector3.zero);
            //    //var pos = transform.localPosition;

            //    //pos.x = Camera.main.transform.position.x;
            //    //pos.z = Camera.main.transform.position.z;

            //    //transform.localPosition = pos;

            //    Timer = 0;

            //    MultipleTargetCamera.AddTarget(transform);
            //    Character.CharacterManager.CurrentStateBaseProperty.CanBeMoved = true;
            //}
        }
        else
        {
            Timer = 0;
        }
    }
}