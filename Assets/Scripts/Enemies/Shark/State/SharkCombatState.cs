using Character;
using Character.State;
using Enemies.Shark;
using Kayak;
using UnityEngine;

public class SharkCombatState : SharkBaseState
{
    private const float DIST_POINT = 5.0f;
    private const float MIN_DIST = 8.0f;

    public enum CombatState { RushToTarget = 0, JumpCrazy = 1, MoveToTarget = 2, RotateAroundPoint = 3, Jump = 4 };
    private CombatState _state;
    public enum AttackState { None = 0, Rush = 1, JumpInFront = 2, Crazy = 3, Wait = 4 }
    private AttackState _attackState;

    float _timeAnimationCurve = 0;
    private Keyframe _lastKey;

    private bool _waveStartJump = false;
    private bool _waveEndJump = false;

    float _elevationOffsetBase;
    private float _currentOffSet;

    float _distSharkTarget;
    float _distSharkPointTarget;
    float _distSharkPointTargetAttack;

    private float _attack;
    private int _atqNumber = 0;
    bool _startJump = false;
    bool _startAtq = false;

    public override void EnterState(SharkManager sharkManager)
    {
        Debug.Log("combat state");

        _elevationOffsetBase = sharkManager.ElevationOffset;
        _currentOffSet = sharkManager.ElevationOffset;
        _state = CombatState.MoveToTarget;
        _attackState = AttackState.None;

        if (sharkManager.TargetTransform != null)
        {
            //var rand = Random.Range(0, 360);
            //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
        }

    }

    public override void UpdateState(SharkManager sharkManager)
    {


        if (sharkManager.TargetTransform != null)
        {

            _distSharkTarget = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.transform.position);
            _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, sharkManager.transform.position);
            _distSharkPointTargetAttack = Vector3.Distance(sharkManager.PointTargetAttack.transform.position, sharkManager.transform.position);

            Debug.Log(_state);
            Debug.Log(_attackState);

            var rbKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody.velocity;
            rbKayak.y = 0;
            if (rbKayak.magnitude > 1f && _attackState == AttackState.None)
            {
                sharkManager.PointTarget.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position,
                    sharkManager.TargetTransform,
                    rbKayak.magnitude * sharkManager.MutliplySpeed);
            }
            else if (_distSharkTarget > sharkManager.DistSharkTargetPointStopMoving)
            {
                sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
            }

            if (_attackState == AttackState.None)
            {
                _attack += Time.deltaTime;
            }

            //timer Attack
            if (_attack >= sharkManager.TimerForAttack && _attackState == AttackState.None)
            {
                sharkManager.IsCollided = false;


                float percentLife = (sharkManager.CurrentLife * 100) / sharkManager.Life;
                Debug.Log(percentLife);

                if (percentLife > 75)
                {
                    _attackState = AttackState.Crazy;
                }
                else if (percentLife <= 75 && percentLife > 25)
                {
                    _attackState = AttackState.Rush;
                }
                else if(percentLife <= 25)
                {
                    var dist = (sharkManager.MinDistanceBetweenTargetWhenRotate + sharkManager.MaxDistanceBetweenTarget) / 2;
                    sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, dist);
                    _attackState = AttackState.JumpInFront;
                }

                //var randAtck = Random.Range(1, (int)AttackState.Wait);
                //Debug.Log(randAtck);
                //if (randAtck == 2)
                //{
                //    //100% de vie
                //    //Crazy jump
                //    _attackState = AttackState.Crazy;
                //}

                //if (randAtck == 1)
                //{
                //    //75% de vie
                //    //Rush
                //    _attackState = AttackState.Rush;
                //}

                //if (randAtck == 3)
                //{
                //    //50% ?
                //    var dist = (sharkManager.MinDistanceBetweenTargetWhenRotate + sharkManager.MaxDistanceBetweenTarget) / 2;
                //    sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, dist);
                //    _attackState = AttackState.JumpInFront;
                //}

            }



            //rush
            if (_attackState == AttackState.Rush)
            {
                _state = CombatState.RushToTarget;
                //dist point stop moving
                if (_distSharkTarget > sharkManager.DistSharkTargetPointStopMoving)
                {
                    sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                }
            }

            //attack in front target point
            if (_attackState == AttackState.JumpInFront)
            {
                //rework
                if (_distSharkPointTargetAttack > 20)
                {
                    var dist = (sharkManager.MinDistanceBetweenTargetWhenRotate + sharkManager.MaxDistanceBetweenTarget) / 2;
                    sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, dist);
                }
                //point stop moving
                else
                {
                    _state = CombatState.Jump;
                }
            }


            //jump around target
            if (_attackState == AttackState.Crazy)
            {
                _state = CombatState.JumpCrazy;
            }

            switch (_state)
            {
                case CombatState.MoveToTarget:
                    if (_distSharkPointTarget >= sharkManager.MaxDistanceBetweenPointInCombat)
                    {
                        sharkManager.SwitchSpeed(sharkManager.SpeedWhenOutOfRange);
                        MoveToTarget(sharkManager);
                    }
                    else
                    {
                        //var rand = Random.Range(0, 360);
                        //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                        _state = CombatState.RotateAroundPoint;
                    }
                    break;

                case CombatState.RotateAroundPoint:
                    if (_distSharkTarget < sharkManager.MaxDistanceBetweenTarget)
                    {
                        RotateCombat(sharkManager);
                    }
                    else
                    {
                        _state = CombatState.MoveToTarget;
                    }
                    break;


                #region attack
                case CombatState.RushToTarget:
                    if (_distSharkPointTarget <= MIN_DIST)
                    {
                        _attack = 0;

                        _state = CombatState.RotateAroundPoint;
                        //var rand = Random.Range(0, 360);
                        //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);

                        _attackState = AttackState.None;
                    }
                    else
                    {
                        Debug.Log("remove comms after");
                        //sharkManager.SharkCollider.enabled = true;
                        RushToTarget(sharkManager);
                    }

                    break;

                #region Jump
                case CombatState.JumpCrazy:

                    #region standby
                    //_timerHittable += Time.deltaTime;
                    //if (_timerHittable >= sharkManager.TimeEndHittable)
                    //{
                    //    sharkManager.SharkCollider.enabled = false;
                    //}
                    //else if (_timerHittable >= sharkManager.TimeStartHittable && _timerHittable < sharkManager.TimeEndHittable)
                    //{
                    //    sharkManager.SharkCollider.enabled = true;
                    //}
                    #endregion

                    JumpCrazy(sharkManager);

                    break;
                case CombatState.Jump:

                    #region standby
                    //_timerHittable += Time.deltaTime;
                    //if (_timerHittable >= sharkManager.TimeEndHittable)
                    //{
                    //    sharkManager.SharkCollider.enabled = false;
                    //}
                    //else if (_timerHittable >= sharkManager.TimeStartHittable && _timerHittable < sharkManager.TimeEndHittable)
                    //{
                    //    sharkManager.SharkCollider.enabled = true;
                    //}
                    #endregion
                    sharkManager.SwitchSpeed(sharkManager.SpeedCombatJumpInFront);
                    JumpInFront(sharkManager);

                    break;

                #endregion

                #endregion

                default:

                    break;
            }


        }
    }
    public override void FixedUpdate(SharkManager sharkManager)
    {

    }

    public override void SwitchState(SharkManager sharkManager)
    {

    }

    #region Fonction

    private void RotateCombat(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 sharkForward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase, 0.005f);

        var pos = sharkManager.Forward.transform.position;
        pos.y = _currentOffSet;
        sharkManager.Forward.transform.position = pos;

        float angle = Vector2.SignedAngle(sharkForward, targetPos - sharkPos);
        if (angle < 0)
            angle += 360;

        //rotate to angle 90 to target
        Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;
        if (_distSharkTarget > sharkManager.MinDistanceBetweenTargetWhenRotate)
        {
            if (angle > 90 && angle < 180)
            {
                rotation.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
            }
            else if (angle > 270 && angle < 360)
            {
                rotation.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
            }
            else
            {
                rotation.y += Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
            }
        }
        //apply rota
        sharkManager.Forward.transform.localEulerAngles = rotation;


        if (_attackState != AttackState.JumpInFront)
        {
            sharkManager.SwitchSpeed(sharkManager.SpeedCombatRotationAroundPoint);
        }
        else
        {
            sharkManager.SwitchSpeed(sharkManager.SpeedCombatToTriggerJumpInFront);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
        }


        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);


        if (_distSharkPointTarget >= sharkManager.MaxDistanceBetweenPointInCombat)
        {
            _state = CombatState.MoveToTarget;
        }
    }

    private void animationCurve(SharkManager sharkManager)
    {
        _timeAnimationCurve += Time.deltaTime;

        _lastKey = sharkManager.JumpCurve[sharkManager.JumpCurve.length - 1];

        if (_timeAnimationCurve >= _lastKey.time - sharkManager.StartFirstCircularWave && _waveStartJump == false)
        {
            Vector2 center = sharkManager.StartFirstCircularWaveData.Center;

            center.x = sharkManager.Forward.transform.position.x;
            center.y = sharkManager.Forward.transform.position.z;

            sharkManager.StartFirstCircularWaveData.Center = center;

            sharkManager.WavesData.LaunchCircularWave(sharkManager.StartFirstCircularWaveData);
            _waveStartJump = true;
        }

        Vector3 pos = sharkManager.Forward.transform.position;
        pos.y = sharkManager.JumpCurve.Evaluate(_timeAnimationCurve);
        sharkManager.Forward.transform.position = pos;

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        Vector3 rotation = sharkManager.transform.eulerAngles;

        rotation.x = sharkManager.VisualCurve.Evaluate(_timeAnimationCurve) * 100;


        if (_attackState == AttackState.JumpInFront)
        {
            rotation.z += Time.deltaTime * sharkManager.SpeedRotationOnItself;
        }


        if (_timeAnimationCurve >= _lastKey.time - sharkManager.StartSecondCircularWaveTime && _waveEndJump == false)
        {
            Vector2 center = sharkManager.StartSecondCircularWaveData.Center;

            center.x = sharkManager.transform.position.x;
            center.y = sharkManager.transform.position.z;

            sharkManager.StartSecondCircularWaveData.Center = center;
            sharkManager.WavesData.LaunchCircularWave(sharkManager.StartSecondCircularWaveData);
            _waveEndJump = true;
        }

        if (_timeAnimationCurve >= _lastKey.time && _attackState != AttackState.None)
        {
            _timeAnimationCurve = 0;
            _waveStartJump = false;
            _waveEndJump = false;


            if (_attackState == AttackState.JumpInFront)
            {
                rotation.z = 0;
            }

            _state = CombatState.RotateAroundPoint;
            //var rand = Random.Range(0, 360);
            //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
            _startAtq = false;

            _atqNumber -= 1;
        }



        sharkManager.transform.eulerAngles = rotation;



    }
    private void JumpCrazy(SharkManager sharkManager)
    {

        if (_startJump == false)
        {
            //number of attack
            _atqNumber = sharkManager.NumberJumpWhenCrazy;
            _startJump = true;
        }

        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        float angle = Vector2.SignedAngle(forward, targetPos - sharkPos);
        if (angle < 0)
            angle += 360;

        Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;

        if (angle > 90 && angle < 180)
        {
            rotation.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }
        else if (angle > 270 && angle < 360)
        {
            rotation.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }
        else
        {
            rotation.y += Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }

        sharkManager.Forward.transform.localEulerAngles = rotation;

        animationCurve(sharkManager);

        if (_atqNumber <= 0)
        {
            _state = CombatState.RotateAroundPoint;
            _attackState = AttackState.None;
            _attack = 0;
            _startJump = false;
        }
    }


    void JumpInFront(SharkManager sharkManager)
    {
        if (_startJump == false)
        {
            _atqNumber = 1;
            _startJump = true;
        }

        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);
        float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
        if (angle < 0)
            angle += 360;

        if (_startAtq == true)
        {
            animationCurve(sharkManager);
        }
        else
        {
            Vector3 rota = sharkManager.Forward.transform.localEulerAngles;
            if (angle >= 0 && angle <= 180)
            {
                rota.y += Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
            }
            else if (angle < 360 && angle > 180)
            {
                rota.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
            }

            sharkManager.Forward.transform.localEulerAngles = rota;

            sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        }

        //dist target
        if (angle < 3 || angle > 357 && _distSharkTarget < sharkManager.MinDistanceBetweenTargetWhenRotate / 2)
        {
            _startAtq = true;
        }

        if (_atqNumber <= 0)
        {
            _state = CombatState.RotateAroundPoint;
            _attackState = AttackState.None;
            _attack = 0;
            _startJump = false;
        }
    }


    public void RushToTarget(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
        if (angle < 0)
            angle += 360;


        Vector3 rota = sharkManager.Forward.transform.localEulerAngles;
        if (angle >= 0 && angle <= 180)
        {
            rota.y += Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }

        sharkManager.Forward.transform.localEulerAngles = rota;

        if (angle < 3 || angle > 357)
        {
            _currentOffSet = Mathf.Lerp(_currentOffSet,sharkManager.ElevationOffsetWhenRush, 0.05f);

            sharkManager.SwitchSpeed(sharkManager.SpeedCombatRush);

            var pos = sharkManager.Forward.transform.position;
            pos.y = _currentOffSet;
            sharkManager.Forward.transform.position = pos;
        }

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

    }

    #endregion

    public void MoveToTargetPoint(SharkManager sharkManager)
    {

        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
        if (angle < 0)
            angle += 360;

        Vector3 rota = sharkManager.Forward.transform.localEulerAngles;
        if (angle >= 0 && angle <= 180)
        {
            rota.y += Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }

        sharkManager.Forward.transform.localEulerAngles = rota;

        _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase, 0.005f);

        var pos = sharkManager.Forward.transform.position;
        pos.y = _currentOffSet;
        sharkManager.Forward.transform.position = pos;
        //SetOffSet(sharkManager, sharkManager.ElevationOffset);

        sharkManager.SwitchSpeed(sharkManager.SpeedToMoveToTarget);


        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);
    }


    public static Vector3 GetPointInFrontAndDistance(Vector3 startingPoint, Transform transform, float distance)
    {
        // Create a new Vector3 in front of target
        Vector3 newPoint = transform.forward;

        newPoint.x = (newPoint.x * distance) + startingPoint.x;
        newPoint.y = 0;
        newPoint.z = (newPoint.z * distance) + startingPoint.z;

        return newPoint;
    }


    public void MoveToTarget(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
        if (angle < 0)
            angle += 360;

        //rotate to angle 0 to target
        Vector3 rota = sharkManager.Forward.transform.localEulerAngles;
        if (angle >= 0 && angle <= 180)
        {
            rota.y += Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.SpeedCombatRotationStatic;
        }

        //apply rota
        sharkManager.Forward.transform.localEulerAngles = rota;


        var pos = sharkManager.Forward.transform.position;
        pos.y = sharkManager.ElevationOffset;
        sharkManager.Forward.transform.position = pos;

        if (_attackState != AttackState.JumpInFront)
            sharkManager.SwitchSpeed(sharkManager.SpeedToMoveToTarget);

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);
    }
}
