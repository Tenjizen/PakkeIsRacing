using GPEs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterFlowGPE.Bezier;
using SharkWithPath.Data;
public class SharkWithPathController : MonoBehaviour
{
    [Header("Player detection"), SerializeField] private PlayerTriggerManager _playerTrigger;
    [Header("Path"), SerializeField] private BezierSpline _splinePath;
    [Header("Data"), SerializeField] private SharkWithPathData _data;
    [Header("Referance"), SerializeField] private GameObject _parent;

    private float _currentSplinePosition;

    [Header("Debug"), SerializeField, ReadOnly] private bool _playerIsTrigger;
    void Start()
    {
        _playerTrigger.OnPlayerEntered.AddListener(SetPlayerIsTriggerAtTrue);

        _currentSplinePosition = 0;

        if (_playerTrigger == null)
        {
            return;
        }
        if (_splinePath == null)
        {
            return;
        }

        Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
        transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
    }

    void Update()
    {
        if (_playerIsTrigger == true)
        {
            ManageMovement();
        }
    }


    private void SetPlayerIsTriggerAtTrue()
    {
        _playerIsTrigger = true;
    }

    private void ManageMovement()
    {
        _currentSplinePosition += _data.MovingValue;
        _currentSplinePosition = Mathf.Clamp01(_currentSplinePosition);

        if (_currentSplinePosition >= 1)
            _parent.SetActive(false);

        Transform t = transform;

        Vector3 position = Vector3.Lerp(t.position, _splinePath.GetPoint(_currentSplinePosition), _data.SpeedLerpToMovingValue);
        t.position = position;

        //rotation
        Vector3 rotation = t.localEulerAngles;
        Vector3 direction = _splinePath.GetDirection(_currentSplinePosition);

        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        t.localRotation = Quaternion.Euler(rotation.x, angle, rotation.z);
    }
}
