using Character;
using Character.State;
using Enemies.Shark;
using Kayak;
using UnityEngine;

public class SharkCombatState : SharkBaseState
{

    float _timeAnimationCurve = 0;
    private Keyframe _lastKey;
    float _elevationOffsetBase;

    //float _timerTargetFollow = 0;
    //float _timerHittable;


    public enum CombatState { RushToTarget = 0, Wait = 1, JumpCrazy = 2, GoToPoint = 3, MoveToTarget = 4, RotatePoint = 5, RotateAroundPoint = 6, RotateToMoveTarget = 7, Jump = 8 };
    private CombatState _state;
    public enum AttackState { None = 0, Rush = 1, JumpInFront = 2, Crazy = 3, Wait = 4 }
    private AttackState _attackState;


    private bool _waveStartJump = false;
    private bool _waveEndJump = false;

    float _distSharkTarget;
    float _distSharkPointTarget;
    float _distSharkPointTargetAttack;

    private float _timerDelay;
    private float _delay;
    private float _timerTargetStatic = 150.0f;
    private float _timerGoOnPlayer;

    private const float DIST_POINT = 5.0f;
    private const float MIN_DIST = 8.0f;


    public override void EnterState(SharkManager sharkManager)
    {
        Debug.Log("combat state");

        _elevationOffsetBase = sharkManager.ElevationOffset;
        _currentOffSet = sharkManager.ElevationOffset;
        _state = CombatState.MoveToTarget;
        _attackState = AttackState.None;

        if (sharkManager.TargetTransform != null)
        {
            var rand = Random.Range(0, 360);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
        }

    }
    private float _attack;

    public override void UpdateState(SharkManager sharkManager)
    {
        //sharkManager.RandTimer += Time.deltaTime;

        if (sharkManager.TargetTransform != null)
        {
            if (_attackState == AttackState.None)
            {
                _attack += Time.deltaTime;
            }
            //timer Attack
            if (_attack >= sharkManager.TimerForAttack && _attackState == AttackState.None)
            {
                sharkManager.IsCollided = false;
                var randAtck = Random.Range(1, (int)AttackState.Wait);
                Debug.Log(randAtck);



                if (randAtck == 1)
                {

                    //Rush
                    //var rand = Random.Range(0, 360);
                    //sharkManager.PointTargetAttack.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, 55);
                    _attackState = AttackState.Rush;
                }



                if (randAtck == 2)
                {

                    //Crazy jump
                    _attackState = AttackState.Crazy;

                }



                if (randAtck == 3)
                {

                    //Front jump
                    //40 debut du saut au niveau du player
                    //45 passe au dessus
                    //50 atteri sur le player 
                    //entre dist max et min
                    //sharkManager.MaxDistanceBetweenTargetWhenRotate
                    //sharkManager.MinDistanceBetweenTargetWhenRotate
                    var dist = (sharkManager.MinDistanceBetweenTargetWhenRotate + sharkManager.MaxDistanceBetweenTarget) / 2;

                    sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, dist);
                    _attackState = AttackState.JumpInFront;
                }

                //_state = CombatState.GoToPoint;
            }

            _distSharkTarget = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.transform.position);
            _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, sharkManager.transform.position);
            _distSharkPointTargetAttack = Vector3.Distance(sharkManager.PointTargetAttack.transform.position, sharkManager.transform.position);


            //if (_distSharkTarget > sharkManager.MaxDistanceBetweenTarget && _attackState == AttackState.None)
            //{
            //    _state = CombatState.MoveToTarget;
            //}


            //if (_attackState == AttackState.Rush && _distSharkTarget > 25)
            //{
            //    sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);

            //    Debug.Log("remove comms after");
            //    //sharkManager.SharkCollider.enabled = true;
            //    sharkManager.IsCollided = false;

            //    _state = CombatState.RushToTarget;
            //}

            //if (_attackState == AttackState.JumpInFront && _state == CombatState.GoToPoint && _distSharkPointTarget < 15)
            //{
            //    //if(_distSharkPointTarget < 15)
            //    if (_distSharkTarget < 30 && _startAtq == false)
            //    {
            //        sharkManager.SwitchSpeed(sharkManager.SpeedRush + 10);
            //        sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, 45);
            //    }
            //}

            //rush
            if (_attackState == AttackState.Rush)
            {
                _state = CombatState.RushToTarget;
                //dist point stop moving
                if (_distSharkTarget > sharkManager.DistSharkTargetPointStopMoving)
                    sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
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

            //if (_currentOffSet >= _elevationOffsetBase - 0.05f)
            //{
            //    Debug.Log("remove comms after");
            //    //sharkManager.SharkCollider.enabled = false;
            //}


            Debug.Log(_state);

            switch (_state)
            {
                case CombatState.MoveToTarget:
                    if (_distSharkPointTarget >= sharkManager.MaxDistanceTriggerBetweenPointAndShark)
                    {
                        MoveToTarget(sharkManager);
                    }
                    else
                    {
                        //delay min & max
                        //_delay = Random.Range(2, 3);
                        //_timerDelay = 0;
                        _timerGoOnPlayer = 0;

                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
                        _state = CombatState.RotateAroundPoint;
                    }
                    break;

                case CombatState.RotateToMoveTarget:

                    if (_distSharkPointTarget <= MIN_DIST)
                    {
                        //delay min & max
                        //_delay = Random.Range(2, 3);
                        //_timerDelay = 0;
                        _timerGoOnPlayer = 0;
                        //_timerTargetStatic = Random.Range(5, 10);

                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
                        _state = CombatState.RotateAroundPoint;
                    }
                    else
                    {
                        MoveToTarget(sharkManager);
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
                        //delay min & max
                        //_delay = Random.Range(2, 3);
                        //_timerDelay = 0;
                        _timerGoOnPlayer = 0;
                        _attack = 0;

                        _state = CombatState.RotateAroundPoint;
                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);

                        _attackState = AttackState.None;
                    }
                    else
                    {
                        Debug.Log("remove comms after");
                        //sharkManager.SharkCollider.enabled = true;
                        RushToTarget(sharkManager);
                    }

                    break;

                case CombatState.GoToPoint:
                    //je sais plus
                    if (_distSharkPointTarget <= 5)
                    {
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);

                        //sharkManager.SharkCollider.enabled = true;
                        sharkManager.IsCollided = false;
                    }
                    else
                    {
                        MoveToTargetPoint(sharkManager);
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
    private float _currentOffSet;

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

        _timerDelay += Time.deltaTime;


        //rotate to angle 90 to target
        Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;
        if (/*_timerDelay >= _delay && */_distSharkTarget > sharkManager.MinDistanceBetweenTargetWhenRotate)
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
            _timerGoOnPlayer += Time.deltaTime;
        }
        //apply rota
        sharkManager.Forward.transform.localEulerAngles = rotation;


        sharkManager.SwitchSpeed(sharkManager.SpeedCombatRotationAroundPoint);


        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);


        if (_distSharkPointTarget >= sharkManager.MaxDistanceBetweenPointInCombat)
        {
            _state = CombatState.MoveToTarget;
        }

        if (_timerGoOnPlayer >= _timerTargetStatic)
        {
            _state = CombatState.RotateToMoveTarget;

        }
    }

    private int _atqNumber = 0;
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

        //torpille
        if (_attackState == AttackState.JumpInFront)
            rotation.z += Time.deltaTime * sharkManager.SpeedRotationOnItself; //jump in front

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
                rotation.z = 0; //jump in front


            _state = CombatState.RotateAroundPoint;
            var rand = Random.Range(0, 360);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
            _startAtq = false;

            _atqNumber -= 1;
            //_attackState = AttackState.None;
        }



        sharkManager.transform.eulerAngles = rotation;



    }
    bool _startJump = false;
    private void JumpCrazy(SharkManager sharkManager)
    {

        if (_startJump == false)
        {
            //nombre d'atq
            _atqNumber = sharkManager.NumberJumpWhenCrazy;
            _startJump = true;
        }
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        //float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
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

        //sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);


        animationCurve(sharkManager);
        //animationCurve(sharkManager);

        if (_atqNumber <= 0)
        {
            _state = CombatState.RotateAroundPoint;
            _attackState = AttackState.None;
            _attack = 0;
            _startJump = false;
        }
    }


    bool _startAtq = false;
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
            sharkManager.SwitchSpeed(sharkManager.SpeedFreeRoamRotationAroundPoint);

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
        if (angle < 3 || angle > 357 && _distSharkTarget < sharkManager.MinDistanceBetweenTargetWhenRotate /2)
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
            _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase + 2, 0.05f);

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

        sharkManager.SwitchSpeed(sharkManager.SpeedToMoveToTarget);

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);
    }
}
