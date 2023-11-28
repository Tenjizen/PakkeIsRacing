using System;
using System.Collections;
using System.Collections.Generic;
using Kayak;
using MultiplayerLocal;
using UnityEngine;
using UnityEngine.Events;

public class IsInCameraView : MonoBehaviour
{
    public float TimerOutOfView;
    public Camera CameraMain;
    public bool IsInCameraViewValue;
    public float Timer;
    public MultipleTargetCamera MultipleTargetCamera;
    public Vector3 TargetRespawn;
    public UnityEvent OnRespawn;

    private Plane[] _cameraFrustrum;
    private Bounds _bounds;
    private Collider _collider;
    private SetParticles _setParticles;
    private KayakController _kayakController;

    private void Start()
    {
        CameraMain = Camera.main;
        _setParticles = CameraMain.gameObject.GetComponentInChildren<SetParticles>();
        _collider = GetComponent<Collider>();
        _kayakController = GetComponent<KayakController>();
    }

    private bool _hadPlayFX;

    private void Update()
    {
        _bounds = _collider.bounds;
        _cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(CameraMain);
        IsInCameraViewValue = GeometryUtility.TestPlanesAABB(_cameraFrustrum, _bounds);

        if (IsInCameraViewValue == false)
        {
            if (_hadPlayFX == false)
            {
                _setParticles.StartParticle("Death");
                _hadPlayFX = true;
            }

            MultipleTargetCamera.RemoveTarget(transform);

            if (Timer < TimerOutOfView)
            {
                Timer += Time.deltaTime;
            }
            else
            {
                Respawn(TargetRespawn);
                IsInCameraViewValue = true;
            }

            TargetRespawn = GameManager.Instance.SharkPossessed.transform.position + new Vector3(0, 0, -5);
        }
        else
        {
            Timer = 0;
            _hadPlayFX = false;
        }
    }

    private void Respawn(Vector3 vector3)
    {
        //put kayak in checkpoint position & rotation
        vector3.y = GameManager.Instance.WavesRef.GetHeight(vector3);
        _kayakController.transform.position = vector3;

        _kayakController.transform.eulerAngles = new Vector3(_kayakController.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, _kayakController.transform.eulerAngles.z);
        _kayakController.Rigidbody.velocity = Vector3.zero;

        MultipleTargetCamera.AddTarget(_kayakController.transform);

        OnRespawn.Invoke();
        //CharacterManagerRef.InCam.IsInCameraViewValue = true;
        //CharacterManagerRef.InCam.Timer = 0;

        //SwitchState(character);
    }
}