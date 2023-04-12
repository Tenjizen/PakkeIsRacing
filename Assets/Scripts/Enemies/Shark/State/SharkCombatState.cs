using Character;
using Character.State;
using Enemies.Shark;
using Kayak;
using UnityEngine;

public class SharkCombatState : SharkBaseState
{

    float _timeAnimationCurve = 0;
    private Keyframe _lastKey;
    float _timerTargetFollow = 0;
    float _elevationOffsetBase;

    float _timerHittable;


    public enum CombatState { RushToTarget = 0, Wait = 1, JumpCrazy = 2, GoToPoint = 3, MoveToTarget = 4, RotatePoint = 5, RotateAroundPoint = 6, RotateToMoveTarget = 7, Jump = 8 };
    private CombatState _state;
    public enum AttackState { None = 0, Rush = 1, JumpInFront = 2, Crazy = 3, Wait = 4 }
    private AttackState _attackState;


    private bool _waveStartJump = false;
    private bool _waveEndJump = false;

    float _distSharkTarget;
    float _distPointTarget;
    float _distSharkPointTarget;

    private float _timerDelay;
    private float _delay;
    private float _timerTargetStatic = 5.0f;
    private float _timerGoOnPlayer;

    private const float DIST_POINT = 5;


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
            _attack += Time.deltaTime;
            if (_attack >= 5 && _attackState == AttackState.None)
            {
                var randAtck = Random.Range(1, (int)AttackState.Wait);
                Debug.Log(randAtck);


                var rand = Random.Range(0, 360);
                //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, 25);
                //_attackState = AttackState.Rush;
                //_attackState = AttackState.Crazy;



                //40 debut du saut au niveau du player
                //45 passe au dessus
                //50 atteri sur le player 
                sharkManager.PointTarget.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, 45);
                _attackState = AttackState.JumpInFront;

                _state = CombatState.GoToPoint;
            }

            _distSharkTarget = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.transform.position);
            _distPointTarget = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.PointTarget.transform.position);
            _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, sharkManager.transform.position);

            if (_distSharkTarget > sharkManager.MaxDistanceBetweenTarget && _attackState == AttackState.None)
            {
                _state = CombatState.MoveToTarget;
            }


            if (_attackState == AttackState.Rush && _distSharkTarget > 25)
            {
                sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);

                Debug.Log("remove comms after");
                //sharkManager.SharkCollider.enabled = true;
                sharkManager.IsCollided = false;

                _state = CombatState.RushToTarget;
            }

            if (_attackState == AttackState.JumpInFront && _state == CombatState.GoToPoint && _distSharkPointTarget < 15)
            {
                //if(_distSharkPointTarget < 15)
                if(_distSharkTarget < 15 && _startAtq == false)
                {
                    sharkManager.SwitchSpeed(sharkManager.SpeedRush +10);
                    sharkManager.PointTarget.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, 45);
                }

                
            }


            if (_distPointTarget > 10 && _attackState == AttackState.None)
            {
                var rand = Random.Range(0, 360);
                sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
            }

            if (_currentOffSet >= _elevationOffsetBase - 0.05f)
            {
                Debug.Log("remove comms after");
                //sharkManager.SharkCollider.enabled = false;
            }

            switch (_state)
            {
                case CombatState.MoveToTarget:
                    if (_distSharkPointTarget >= sharkManager.MaxDistanceBetweenPoint)
                    {
                        sharkManager.MoveToTarget(sharkManager);
                    }
                    else
                    {
                        _delay = Random.Range(2, 3);
                        _timerDelay = 0;
                        _timerGoOnPlayer = 0;

                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
                        _state = CombatState.RotateAroundPoint;
                    }
                    break;

                case CombatState.RotateToMoveTarget:

                    if (_distSharkPointTarget <= sharkManager.MinDistanceBetweenPoint)
                    {
                        _delay = Random.Range(2, 3);
                        _timerDelay = 0;
                        _timerGoOnPlayer = 0;
                        _timerTargetStatic = Random.Range(5, 10);

                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
                        _state = CombatState.RotateAroundPoint;
                    }
                    else
                    {
                        sharkManager.MoveToTarget(sharkManager);
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





                case CombatState.RushToTarget:
                    if (_distSharkPointTarget <= sharkManager.MinDistanceBetweenPoint)
                    {
                        _delay = Random.Range(2, 3);
                        _timerDelay = 0;
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
                    if (_distSharkPointTarget <= 5)
                    {
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);

                        //sharkManager.SharkCollider.enabled = true;
                        sharkManager.IsCollided = false;
                        switch (_attackState)
                        {
                            case AttackState.None:
                                break;
                            case AttackState.Rush:
                                _state = CombatState.RushToTarget;
                                break;
                            case AttackState.JumpInFront:
                                _state = CombatState.Jump;
                                break;

                            case AttackState.Crazy:
                                _state = CombatState.JumpCrazy;
                                break;

                            case AttackState.Wait:
                                break;
                            default:
                                break;
                        }


                    }
                    else
                    {
                        MoveToTargetPoint(sharkManager);
                    }

                    break;

                #region Wait for jump
                //case CombatState.Wait:

                //    sharkManager.Shadow.SetActive(true);
                //    CircleUI(sharkManager);

                //    break;
                #endregion

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

                    Jump(sharkManager);

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

        Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;

        if (_timerDelay >= _delay)
        {
            if (angle > 90 && angle < 180)
            {
                rotation.y -= Time.deltaTime * sharkManager.RotationStaticSpeedCombat;
            }
            else if (angle > 270 && angle < 360)
            {
                rotation.y -= Time.deltaTime * sharkManager.RotationStaticSpeedCombat;
            }
            else
            {
                rotation.y += Time.deltaTime * sharkManager.RotationStaticSpeedCombat;
            }
            _timerGoOnPlayer += Time.deltaTime;
        }

        sharkManager.Forward.transform.localEulerAngles = rotation;


        sharkManager.SwitchSpeed(sharkManager.SpeedRotationCombat);

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

    private void CircleUI(SharkManager sharkManager)
    {
        if (_timerTargetFollow < sharkManager.TimerTargetFollow)
        {
            sharkManager.Shadow.transform.position = sharkManager.TargetTransform.position;
        }

        _timerTargetFollow += Time.deltaTime;

        if (sharkManager.Shadow.transform.localScale.x < sharkManager.ShadowMaxSizeForJump)
        {

            sharkManager.Shadow.transform.localScale += (Vector3.one / sharkManager.ShadowDivideGrowSpeed) * Time.deltaTime;
        }
        else
        {
            _state = CombatState.JumpCrazy;

            if (sharkManager.KayakControllerProperty != null)
            {
                Debug.Log("perso dead");
                CharacterManager.Instance.SwitchToDeathState();
            }

            sharkManager.Shadow.SetActive(false);
            sharkManager.Shadow.transform.localScale = Vector3.one;
            _timerTargetFollow = 0;


        }
    }
    private void animationCurve(SharkManager sharkManager)
    {
        _timeAnimationCurve += Time.deltaTime;

        _lastKey = sharkManager.JumpCurve[sharkManager.JumpCurve.length - 1];

        if (_timeAnimationCurve >= _lastKey.time - sharkManager.StartJumpCircularWaveTime && _waveStartJump == false)
        {
            Vector2 center = sharkManager.StartJumpCircularWaveData.Center;

            center.x = sharkManager.Forward.transform.position.x;
            center.y = sharkManager.Forward.transform.position.z;

            sharkManager.StartJumpCircularWaveData.Center = center;

            sharkManager.WavesData.LaunchCircularWave(sharkManager.StartJumpCircularWaveData);
            _waveStartJump = true;
        }

        Vector3 pos = sharkManager.Forward.transform.position;
        pos.y = sharkManager.JumpCurve.Evaluate(_timeAnimationCurve);
        sharkManager.Forward.transform.position = pos;

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        Vector3 rotation = sharkManager.transform.eulerAngles;

        rotation.x = sharkManager.VisualCurve.Evaluate(_timeAnimationCurve) * 100;

        if (_attackState == AttackState.JumpInFront)
            rotation.z += Time.deltaTime * 500; //jump in front

        if (_timeAnimationCurve >= _lastKey.time - sharkManager.EndJumpCircularWaveTime && _waveEndJump == false)
        {
            Vector2 center = sharkManager.EndJumpCircularWaveData.Center;

            center.x = sharkManager.transform.position.x;
            center.y = sharkManager.transform.position.z;

            sharkManager.EndJumpCircularWaveData.Center = center;
            sharkManager.WavesData.LaunchCircularWave(sharkManager.EndJumpCircularWaveData);
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


            _attackState = AttackState.None;
        }

        sharkManager.transform.eulerAngles = rotation;
    }
    private void Jump(SharkManager sharkManager)
    {
        animationCurve(sharkManager);
    }


    bool _startAtq = false;
    void JumpInFront(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 forward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);
        float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
        if (angle < 0)
            angle += 360;

        if (_startAtq == true)
        {
            sharkManager.SwitchSpeed(sharkManager.SpeedRotationCombat);

            animationCurve(sharkManager);
        }
        else
        {
            Vector3 rota = sharkManager.Forward.transform.localEulerAngles;
            if (angle >= 0 && angle <= 180)
            {
                rota.y += Time.deltaTime * sharkManager.RotationStaticSpeed;
            }
            else if (angle < 360 && angle > 180)
            {
                rota.y -= Time.deltaTime * sharkManager.RotationStaticSpeed;
            }

            sharkManager.Forward.transform.localEulerAngles = rota;

            sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        }


        if (angle < 3 || angle > 357)
        {
            _startAtq = true;
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
            rota.y += Time.deltaTime * sharkManager.RotationStaticSpeed;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.RotationStaticSpeed;
        }

        sharkManager.Forward.transform.localEulerAngles = rota;

        if (angle < 3 || angle > 357)
        {
            _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase + 2, 0.05f);

            sharkManager.SwitchRush(sharkManager.SpeedRush);
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
            rota.y += Time.deltaTime * sharkManager.RotationStaticSpeed;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.RotationStaticSpeed;
        }

        sharkManager.Forward.transform.localEulerAngles = rota;

        _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase, 0.005f);

        var pos = sharkManager.Forward.transform.position;
        pos.y = _currentOffSet;
        sharkManager.Forward.transform.position = pos;
        //SetOffSet(sharkManager, sharkManager.ElevationOffset);

        sharkManager.SwitchRush(sharkManager.SpeedToMoveToTarget);


        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.SpeedToMoveToTarget * Time.deltaTime, Space.Self);
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
}
