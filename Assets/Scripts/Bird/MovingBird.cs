using UnityEngine;
using System.Collections;
public class MovingBird : MonoBehaviour
{
    public float HorizontalSpeed;
    public float VerticalSpeed;
    public float Amplitude;
    private Vector3 _tempPosition;
    private Vector3 _startPosition;
    private float _startTime;

    void Start()
    {
        _startTime = Random.value;
        _tempPosition = transform.position;
        _startPosition = transform.position;
    }
    void FixedUpdate()
    {
        _tempPosition += transform.forward * Time.fixedDeltaTime * HorizontalSpeed;

        _tempPosition.y = Mathf.Sin(_startTime + Time.realtimeSinceStartup * VerticalSpeed * Time.fixedDeltaTime) * Amplitude + _startPosition.y;
        transform.position = _tempPosition;

    }


    public void ResetVariable()
    {
        _tempPosition = transform.position;
        _startPosition = transform.position;
    }
}