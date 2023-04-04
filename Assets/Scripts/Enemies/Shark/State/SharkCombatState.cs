using Character;
using Character.State;
using Enemies.Shark;
using Kayak;
using UnityEngine;

public class SharkCombatState : SharkBaseState
{

    private float _timerToPassUnderPlayer;
    float _timeAnimationCurve = 0;
    private Keyframe _lastKey;
    float _timerTargetFollow = 0;
    float _elevationOffsetBase;

    float _timerHittable;

    float _radius;

    public enum CombatState { RotateBeforeJump = 0, Wait = 1, Jump = 2, RotateAfterJump = 3, MoveToTarget = 4, RotatePoint = 5, GoToRotate = 6, RotateToMoveTarget = 7 };
    private CombatState _state;

    private bool _waveStartJump = false;
    private bool _waveEndJump = false;

    public override void EnterState(SharkManager sharkManager)
    {
        Debug.Log("combat state");
        _elevationOffsetBase = sharkManager.ElevationOffset;
        _radius = Random.Range(sharkManager.RadiusRotateAroundTargetMin, sharkManager.RadiusRotateAroundTargetMax);
        _state = CombatState.MoveToTarget;

        if (sharkManager.TargetTransform != null)
            sharkManager.TargetLerp.transform.position = sharkManager.TargetTransform.position;//modif

    }

    public override void UpdateState(SharkManager sharkManager)
    {
        if (sharkManager.TargetTransform != null)
        {
            _dist = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.TargetLerp.transform.position);
            _distSharkTarget = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.transform.position);
            sharkManager.TargetLerp.transform.position = Vector3.Lerp(sharkManager.TargetLerp.transform.position, sharkManager.TargetTransform.position, 0.01f);

            //Debug.Log(_radius + "rad");
            //Debug.Log(_distSharkPlayer + "dist shark");




            switch (_state)
            {
                #region BeforeJump
                case CombatState.RotateBeforeJump:


                    //_timerToPassUnderPlayer += Time.deltaTime;

                    //RotateAround(sharkManager);

                    //if (_timerToPassUnderPlayer > sharkManager.TimerToPassUnderPlayer /*&& _state == CombatState.RotateBeforeJump*/)
                    //{
                    //    if (sharkManager.ElevationOffset > -sharkManager.DiveDistance)
                    //    {
                    //        sharkManager.ElevationOffset -= sharkManager.DiveSpeed;
                    //    }
                    //    else
                    //        _state = CombatState.Wait;
                    //}

                    break;
                #endregion
                case CombatState.MoveToTarget:
                    if (_distSharkTarget >= _radius + 5)
                    {
                        goToTarget(sharkManager);
                    }
                    else
                    {
                        Vector2 thisTransform = new Vector2(sharkManager.transform.position.x, sharkManager.transform.position.z);
                        Vector2 pointForward = new Vector2(sharkManager.PointTarget.transform.forward.x, sharkManager.PointTarget.transform.forward.z);

                        sharkManager.Angle = Vector2.SignedAngle(pointForward, thisTransform) * (Mathf.PI / 180);

                        _delay = Random.Range(2, 4);
                        _timerDelay = 0;
                        _timerGoOnPlayer = 0;

                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, 5);
                        _state = CombatState.GoToRotate;
                    }
                    break;
                case CombatState.RotateToMoveTarget:

                    if (_distSharkTarget <= 5)
                    {
                        _delay = Random.Range(1, 3);
                        _timerDelay = 0;
                        _timerGoOnPlayer = 0;
                        _timerTargetStatic = Random.Range(5, 10);

                        var rand = Random.Range(0, 360);
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, 5);
                        _state = CombatState.GoToRotate;
                    }
                    else
                    {
                        goToTarget(sharkManager);
                    }

                    break;
                //case CombatState.RotatePoint:
                //    if (_distSharkPlayer <= _radius + 30)
                //    {
                //        RotateAroundPoint(sharkManager);
                //    }
                //    else
                //    {
                //        _state = CombatState.MoveToTarget;
                //    }
                //    break;
                case CombatState.GoToRotate:
                    Rotate(sharkManager);
                    break;

                //#region Wait for jump
                //case CombatState.Wait:

                //    sharkManager.Shadow.SetActive(true);
                //    CircleUI(sharkManager);

                //    break;
                //#endregion

                //#region Jump
                //case CombatState.Jump:

                //    _timerHittable += Time.deltaTime;
                //    if (_timerHittable >= sharkManager.TimeEndHittable)
                //    {
                //        sharkManager.SharkCollider.enabled = false;
                //    }
                //    else if (_timerHittable >= sharkManager.TimeStartHittable && _timerHittable < sharkManager.TimeEndHittable)
                //    {
                //        sharkManager.SharkCollider.enabled = true;
                //    }

                //    Jump(sharkManager);

                //    break;
                //#endregion

                //#region After jump
                //case CombatState.RotateAfterJump:
                //    _timerHittable = 0;
                //    RotateAround(sharkManager);

                //    if (sharkManager.ElevationOffset < _elevationOffsetBase)
                //    {
                //        sharkManager.ElevationOffset += sharkManager.DiveSpeed;
                //    }
                //    else
                //    {
                //        sharkManager.ElevationOffset = _elevationOffsetBase;
                //        _radius = Random.Range(sharkManager.RadiusRotateAroundTargetMin, sharkManager.RadiusRotateAroundTargetMax);
                //        _state = CombatState.RotateBeforeJump;
                //    }

                //    break;
                //#endregion

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
    float _dist;
    float _distSharkTarget;
    private void goToTarget(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        //Vector2 targetPos = new Vector2(sharkManager.TargetTransform.position.x, sharkManager.TargetTransform.position.z);
        Vector2 forward = new Vector2(sharkManager.transform.forward.x, sharkManager.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.transform.position.x, sharkManager.transform.position.z);

        float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
        if (angle < 0)
            angle += 360;

        Vector3 rota = sharkManager.transform.localEulerAngles;
        if (angle >= 0 && angle <= 180)
        {
            rota.y += Time.deltaTime * 100;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * 100;
        }

        sharkManager.transform.localEulerAngles = rota;

        var pos = sharkManager.transform.position;
        pos.y = sharkManager.ElevationOffset;
        sharkManager.transform.position = pos;


        sharkManager.transform.Translate(Vector3.forward * 10 * Time.deltaTime, Space.Self);
    }
    private float _timerDelay;
    private float _timerGoOnPlayer;
    private float _delay;
    private float _timerTargetStatic = 5.0f;

    private void Rotate(SharkManager sharkManager)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        //Vector2 targetPos = new Vector2(sharkManager.TargetTransform.position.x, sharkManager.TargetTransform.position.z);
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
                rotation.y -= Time.deltaTime * 70f;
            }
            else if (angle > 270 && angle < 360)
            {
                rotation.y -= Time.deltaTime * 70f;
            }
            else
            {
                rotation.y += Time.deltaTime * 70f;
            }
            _timerGoOnPlayer += Time.deltaTime;
        }

        sharkManager.transform.localEulerAngles = rotation;

        sharkManager.transform.Translate(Vector3.forward * 10 * Time.deltaTime, Space.Self);

        if (_distSharkTarget >= _radius + 15)
        {
            _state = CombatState.MoveToTarget;
        }

        if (_timerGoOnPlayer >= _timerTargetStatic)
        {
            _state = CombatState.RotateToMoveTarget;

        }
    }

    private void RotateAroundPoint(SharkManager sharkManager)
    {
        sharkManager.PositionOffset.Set(Mathf.Cos(sharkManager.Angle) * _radius,
           sharkManager.ElevationOffset,
           Mathf.Sin(sharkManager.Angle) * _radius);

        var pos = sharkManager.transform.position;

        pos.x = sharkManager.PointTarget.transform.position.x + sharkManager.PositionOffset.x;
        pos.y = sharkManager.ElevationOffset;
        pos.z = sharkManager.PointTarget.transform.position.z + sharkManager.PositionOffset.z;

        sharkManager.transform.position = pos;

        sharkManager.Angle += Time.deltaTime * sharkManager.RotationSpeed;

        Quaternion rotation = sharkManager.transform.rotation;
        Vector3 position = sharkManager.transform.position;
        Vector3 target = sharkManager.PointTarget.transform.position;

        rotation = Quaternion.LookRotation(new Vector3(target.x - position.x, 0, target.z - position.z), sharkManager.TargetLerp.transform.up);
        rotation.x = 0;
        rotation.z = 0;

        sharkManager.transform.rotation = rotation;
    }


    private void RotateAround(SharkManager sharkManager)
    {
        Vector3 position = sharkManager.TargetTransform != null ? sharkManager.PointTarget.transform.position : Vector3.zero;

        sharkManager.PositionOffset.Set(Mathf.Cos(sharkManager.Angle) * _radius,
            sharkManager.ElevationOffset,
            Mathf.Sin(sharkManager.Angle) * _radius);

        //sharkManager.TargetLerp.transform.position = Vector3.Lerp(sharkManager.TargetLerp.transform.position, sharkManager.TargetTransform.position, 0.005f);

        sharkManager.transform.position = new Vector3(sharkManager.PointTarget.transform.position.x + sharkManager.PositionOffset.x,
            sharkManager.ElevationOffset,
            sharkManager.PointTarget.transform.position.z + sharkManager.PositionOffset.z);

        //sharkManager.transform.position = new Vector3(sharkManager.TargetLerp.transform.position.x + sharkManager.PositionOffset.x,
        //   sharkManager.ElevationOffset,
        //   sharkManager.TargetLerp.transform.position.z + sharkManager.PositionOffset.z);

        Quaternion rotation = sharkManager.transform.rotation;
        Vector3 enemyPos = sharkManager.transform.position;

        rotation = Quaternion.LookRotation(new Vector3(position.x - enemyPos.x, 0, position.z - enemyPos.z), sharkManager.TargetLerp.transform.up);
        rotation.x = 0;
        rotation.z = 0;

        //Debug.Log(rotation);

        sharkManager.transform.rotation = rotation;

        Vector3 visualRotation = sharkManager.VisualShark.transform.localEulerAngles;

        if (sharkManager.Flip == false)
        {
            visualRotation.y = 90;
            sharkManager.VisualShark.transform.localEulerAngles = visualRotation;

            sharkManager.Angle += Time.deltaTime * sharkManager.RotationSpeed;
        }
        else if (sharkManager.Flip == true)
        {
            visualRotation.y = -90;
            sharkManager.VisualShark.transform.localEulerAngles = visualRotation;

            sharkManager.Angle -= Time.deltaTime * sharkManager.RotationSpeed;
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
            //sharkManager.ColliderShadow.enabled = false;

            sharkManager.Shadow.transform.localScale += (Vector3.one / sharkManager.ShadowDivideGrowSpeed) * Time.deltaTime;
        }
        else
        {
            //sharkManager.ColliderShadow.enabled = true;

            _state = CombatState.Jump;

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

    private void Jump(SharkManager sharkManager)
    {

        _timeAnimationCurve += Time.deltaTime;

        sharkManager.transform.position = sharkManager.Shadow.transform.position;

        if (_waveStartJump == false)
        {
            Vector2 center = sharkManager.StartJumpCircularWaveData.Center;

            center.x = sharkManager.transform.position.x;
            center.y = sharkManager.transform.position.z;

            sharkManager.StartJumpCircularWaveData.Center = center;

            sharkManager.WavesData.LaunchCircularWave(sharkManager.StartJumpCircularWaveData);
            _waveStartJump = true;
        }


        Vector3 pos = sharkManager.transform.position;
        pos.y = sharkManager.JumpCurve.Evaluate(_timeAnimationCurve);
        sharkManager.transform.position = pos;

        #region animation curve
        _lastKey = sharkManager.JumpCurve[sharkManager.JumpCurve.length - 1];

        // rotation y
        Vector3 rotation = sharkManager.transform.eulerAngles;



        if (_timeAnimationCurve > _lastKey.time / sharkManager.ValueBeforeLastKeyFrameInCurve)
        {
            if (sharkManager.Flip == false)
            {
                rotation.z = -90;
            }
            else
            {
                rotation.z = 90;
            }
            sharkManager.transform.eulerAngles = rotation;
        }
        else
        {
            if (sharkManager.Flip == false)
            {
                rotation.z = 90;
            }
            else
            {
                rotation.z = -90;
            }
            sharkManager.transform.eulerAngles = rotation;
        }

        if (_timeAnimationCurve >= _lastKey.time - sharkManager.EndJumpCircularWaveTime && _waveEndJump == false)
        {
            Vector2 center = sharkManager.EndJumpCircularWaveData.Center;

            center.x = sharkManager.transform.position.x;
            center.y = sharkManager.transform.position.z;

            sharkManager.EndJumpCircularWaveData.Center = center;
            sharkManager.WavesData.LaunchCircularWave(sharkManager.EndJumpCircularWaveData);
            _waveEndJump = true;
        }
        // end of the jump
        if (_timeAnimationCurve >= _lastKey.time)
        {
            _state = CombatState.RotateAfterJump;
            _timerToPassUnderPlayer = 0;
            _timeAnimationCurve = 0;
            _waveStartJump = false;
            _waveEndJump = false;

            sharkManager.Flip = RandomBoolean();
            sharkManager.TargetLerp.transform.position = sharkManager.TargetTransform.position;//modif

        }
        #endregion
    }
    bool RandomBoolean()
    {
        if (Random.value >= 0.5)
        {
            return true;
        }
        return false;
    }
    #endregion

}
