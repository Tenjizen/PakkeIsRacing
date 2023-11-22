using System;
using System.Collections;
using System.Collections.Generic;
using MultiplayerLocal;
using UnityEngine;

public class IsInCameraView : MonoBehaviour
{
    [SerializeField] private float _timerOutOfView;

    private Camera _camera;
    private Plane[] _cameraFrustrum;
    public bool _isInCameraView;
    private Bounds _bounds;
    private float _timer;
    private Collider _collider;

    public MultipleTargetCamera MultipleTargetCamera;

    private void Start()
    {
        _camera = Camera.main;
        _collider = GetComponent<Collider>();
    }

    private void Update()
    {
        _bounds = _collider.bounds;
        _cameraFrustrum = GeometryUtility.CalculateFrustumPlanes(_camera);
        _isInCameraView = GeometryUtility.TestPlanesAABB(_cameraFrustrum, _bounds);

        if (_isInCameraView == false)
        {
            MultipleTargetCamera.RemoveTarget(transform);
            if (_timer < _timerOutOfView)
            {
                _timer += Time.deltaTime;
            }
            else
            {
            }
        }
        else
        {
            _timer = 0;
        }
    }
}