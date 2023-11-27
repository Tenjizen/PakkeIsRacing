using GPEs.WaterFlowGPE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterFlowGPE.Bezier;

public class Penguin : MonoBehaviour
{

    public enum State { Idle, Walk, Run };
    private State _stateEnum;


    [Header("Path"), SerializeField] private BezierSpline _splinePath;

    [SerializeField, Header("Speed"), Range(0, 0.001f)] float _movingValueWalk = 0.0001f;
    [SerializeField, Range(0, 0.001f)] float _movingValueRun = 0.0001f;
    [SerializeField, Range(0, 1f)] float _speedLerpToMovingValue = 0.1f;

    [SerializeField, Header("Timer"), Tooltip("Min value ")] float _valueTimerMinWalk = 0.5f;
    [SerializeField] float _valueTimerMaxWalk = 3f;
    [SerializeField] float _valueTimerMinIdle = 1f;
    [SerializeField] float _valueTimerMaxIdle = 6f;
    [SerializeField] float _timerStopRunning = 1.5f;
    [SerializeField] float _percentChanceRunning = 15f;
    [SerializeField] Animator _animator;

    private int _randIdle;

    private float _currentSplinePosition;
    private float _timerIdle;
    private float _timerWalk;
    private float _timerRun;
    private float _randTimerForIdle;
    private float _randTimerForWalk;
    private RaycastHit[] _hitBuffer = new RaycastHit[3];

    void Start()
    {
        _currentSplinePosition = 0;
        _randTimerForWalk = Random.Range(_valueTimerMinWalk, _valueTimerMaxWalk);
        _randTimerForIdle = Random.Range(_valueTimerMinIdle, _valueTimerMaxIdle);

        if (_splinePath == null)
        {
            return;
        }
        Vector3 splinePosition = _splinePath.GetPoint(_currentSplinePosition);
        transform.position = new Vector3(splinePosition.x, splinePosition.y + 1, splinePosition.z);
    }

    void Update()
    {
        switch (_stateEnum)
        {
            case State.Idle:
                if (_timerIdle < _randTimerForWalk)
                {
                    _timerIdle += Time.deltaTime;
                }
                else
                {
                    _timerWalk = 0;
                    _randTimerForIdle = Random.Range(_valueTimerMinIdle, _valueTimerMaxIdle);

                    _animator.SetBool("Idle" + _randIdle, false);
                    TryRunning(State.Walk);
                }
                break;

            case State.Walk:
                if (_timerWalk < _randTimerForIdle)
                {
                    _animator.SetBool("Walk", true);
                    _timerWalk += Time.deltaTime;
                    ManageMovement(_movingValueWalk);
                }
                else
                {
                    _timerIdle = 0;
                    _randTimerForWalk = Random.Range(_valueTimerMinWalk, _valueTimerMaxWalk);

                    _animator.SetBool("Walk", false);
                    TryRunning(State.Idle);
                }
                break;

            case State.Run:
                if (_timerRun < _timerStopRunning)
                {
                    _timerRun += Time.deltaTime;
                    ManageMovement(_movingValueRun);
                }
                else
                {
                    _timerIdle = 0;
                    _randTimerForWalk = Random.Range(_valueTimerMinWalk, _valueTimerMaxWalk);

                    _animator.SetBool("Walk", false);
                    _randIdle = Random.Range(0, 2) + 1;
                    _animator.SetBool("Idle" + _randIdle, true);

                    _stateEnum = State.Idle;
                }
                break;
        }
    }
    private void TryRunning(State state)
    {
        if (Random.Range(0, 100) < _percentChanceRunning)
        {
            _timerRun = 0;
            _animator.SetBool("Walk", true);
            _stateEnum = State.Run;
        }
        else
        {
            if (state == State.Idle)
            {
                _randIdle = Random.Range(0, 2) + 1;
                _animator.SetBool("Idle" + _randIdle, true);
            }
            else
            {
                _animator.SetBool("Walk", true);
            }

            _stateEnum = state;
        }
    }

    private void ManageMovement(float speed)
    {
        _currentSplinePosition += speed;
        _currentSplinePosition = Mathf.Clamp01(_currentSplinePosition);

        if (_currentSplinePosition >= 1)
        {
            _currentSplinePosition = 0;
        }

        Transform t = transform;
        Vector3 splinePos = _splinePath.GetPoint(_currentSplinePosition);
        Vector3 position = Vector3.Lerp(new Vector3(t.position.x, 0, t.position.z), new Vector3(splinePos.x, 0, splinePos.z), _speedLerpToMovingValue);
        Vector3 origin = transform.position;
        origin.y = transform.position.y + 5f;
        Vector3 direction = -Vector3.up;
        float distance = 20f;
        int hitCount = Physics.RaycastNonAlloc(origin, direction, _hitBuffer, distance);
        if (hitCount > 0)
        {
            //int temp = 0;
            //while (_hitBuffer[temp].collider.GetComponent<WaterFlowBlock>() == true)
            //{
            //    if (temp >= _hitBuffer.Length)
            //        return;
            //    temp++;
            //}
            //for (int i = 0; i < hitCount; i++)
            //{
            //    if (_hitBuffer[temp].distance > _hitBuffer[i].distance && _hitBuffer[i].collider.GetComponent<WaterFlowBlock>() == false)
            //    {
            //        temp = i;
            //    }
            //}
            //position.y = Mathf.Lerp(t.position.y, _hitBuffer[temp].point.y, 0.2f);

        }
        t.position = position;
        //rotation
        Vector3 pointB = _splinePath.GetPoint(Mathf.Clamp01(_currentSplinePosition + 0.01f));
        t.LookAt(pointB);
        Vector3 rotation = t.transform.rotation.eulerAngles;
        t.rotation = Quaternion.Euler(0, Mathf.Lerp(rotation.y, t.rotation.eulerAngles.y, 0.1f), 0);
    }
}