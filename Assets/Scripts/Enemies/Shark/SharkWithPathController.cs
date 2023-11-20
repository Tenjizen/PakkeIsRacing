using UnityEngine;
using WaterFlowGPE.Bezier;
using SharkWithPath.Data;

public class SharkWithPathController : MonoBehaviour
{
    [Header("Path"), SerializeField] private BezierSpline _splinePath;
    [Header("Data"), SerializeField] private SharkWithPathData _data;

    private float _currentSplinePosition;

    [Header("Debug"), SerializeField] private bool _slow;


    void Start()
    {
        Initialize();
    }

    void Update()
    {
        ManageMovement();
    }

    public void Initialize()
    {

        _currentSplinePosition = 0;

        if (_splinePath == null)
        {
            return;
        }

        Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
        transform.position = new Vector3(splinePosition.x, transform.position.y, splinePosition.z);
    }


    private void ManageMovement()
    {
        if (_slow == false)
        {
            _currentSplinePosition += _data.MovingValue;
        }
        else
        {
            _currentSplinePosition += _data.SlowMovingValue;
        }
        _currentSplinePosition = Mathf.Clamp01(_currentSplinePosition);

        if (_currentSplinePosition >= 1) _currentSplinePosition = 0;

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
