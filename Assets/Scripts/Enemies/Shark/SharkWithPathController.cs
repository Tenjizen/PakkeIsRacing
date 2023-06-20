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
    private Quaternion _targetRotation;


    [Header("Debug"), SerializeField, ReadOnly] private bool _playerIsTrigger;
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (_playerIsTrigger == true)
        {
            ManageMovement();
        }
    }

    public void Initialize()
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


    public void SetPlayerIsTriggerAtTrue()
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
        Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition + 0.01f));
        t.LookAt(pointB);
        Vector3 rotation = t.transform.rotation.eulerAngles;
        t.rotation = Quaternion.Euler(new Vector3(rotation.x, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f), rotation.z));


    }
}
