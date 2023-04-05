using Enemies.Shark;
using Kayak;
using UnityEngine;

public class SharkFreeRoamState : SharkBaseState
{

    public enum CombatState { RotateBeforeJump = 0, Wait = 1, Jump = 2, RotateAfterJump = 3, MoveToTarget = 4, RotatePoint = 5, GoToRotate = 6, RotateToMoveTarget = 7 };
    private CombatState _state;


    private float _timerDelay;
    private float _delay;


    float _distSharkPointTarget;

    float _randTimer;
    float _timer;
    public override void EnterState(SharkManager sharkManager)
    {
        Debug.Log("free");
        _state = CombatState.RotateToMoveTarget;
    }

    public override void UpdateState(SharkManager sharkManager)
    {

        _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, sharkManager.transform.position);

        switch (_state)
        {

            case CombatState.RotateToMoveTarget:

                if (_distSharkPointTarget <= 5)
                {
                    _delay = Random.Range(1, 3);
                    _timerDelay = 0;
                    _randTimer = Random.Range(7, 10);
                    _state = CombatState.GoToRotate;
                }
                else
                {
                    sharkManager.MoveToTarget(sharkManager);
                }

                break;
            case CombatState.GoToRotate:
                    RotateFreeRoam(sharkManager);
                break;

        }
    }

    public override void FixedUpdate(SharkManager sharkManager)
    {

    }

    public override void SwitchState(SharkManager sharkManager)
    {

    }


    private void RotateFreeRoam(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 sharkForward = new Vector2(sharkManager.transform.forward.x, sharkManager.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.transform.position.x, sharkManager.transform.position.z);

        float angle = Vector2.SignedAngle(sharkForward, targetPos - sharkPos);
        if (angle < 0)
            angle += 360;

        _timerDelay += Time.deltaTime;

        Vector3 rotation = sharkManager.transform.localEulerAngles;

        if (_timerDelay >= _delay)
        {
            if (angle > 90 && angle < 180)
            {
                rotation.y -= Time.deltaTime * sharkManager.RotationStaticSpeedFreeRoam;
            }
            else if (angle > 270 && angle < 360)
            {
                rotation.y -= Time.deltaTime * sharkManager.RotationStaticSpeedFreeRoam;
            }
            else
            {
                rotation.y += Time.deltaTime * sharkManager.RotationStaticSpeedFreeRoam;
            }
        }

        sharkManager.transform.localEulerAngles = rotation;

        sharkManager.transform.Translate(Vector3.forward * sharkManager.SpeedRotationFreeRoam * Time.deltaTime, Space.Self);

        
        _timer += Time.deltaTime;
        
        if (_timer >= _randTimer)
        {
            _state = CombatState.RotateToMoveTarget;
            _timer = 0;
        }
    }


}
