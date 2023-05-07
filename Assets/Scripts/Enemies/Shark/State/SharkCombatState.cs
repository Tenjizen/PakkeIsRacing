using Character;
using Character.State;
using Enemies.Shark;
using Kayak;
using UnityEngine;
using Character;

public class SharkCombatState : SharkBaseState
{
    private const float DIST_POINT = 5.0f;
    private const float MIN_DIST = 8.0f;

    public enum CombatState { RushToTarget = 0, JumpAround = 1, MoveToTarget = 2, RotateAroundPoint = 3, JumpToTarget = 4 };
    private CombatState _state;
    public enum AttackState { None = 0, Rush = 1, JumpToTarget = 2, JumpAround = 3, Wait = 4 }
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

        _elevationOffsetBase = sharkManager.Data.ElevationOffset;
        _currentOffSet = sharkManager.Data.ElevationOffset;
        _state = CombatState.MoveToTarget;
        _attackState = AttackState.None;

        if (sharkManager.TargetTransform != null)
        {
            //var rand = Random.Range(0, 360);
            //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
        }

    }
    float percentLife;
    public override void UpdateState(SharkManager sharkManager)
    {

        if (CharacterManager.Instance.CameraManagerProperty.StartDeath == true)
        {
            sharkManager.FreeRoamState();
        }
        if (sharkManager.IsPossessed == false && _attackState == AttackState.None)
        {
            sharkManager.FreeRoamState();
        }


        if (sharkManager.TargetTransform != null)
        {

            _distSharkTarget = Vector3.Distance(sharkManager.TargetTransform.position, sharkManager.transform.position);
            _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, sharkManager.transform.position);
            _distSharkPointTargetAttack = Vector3.Distance(sharkManager.PointTargetAttack.transform.position, sharkManager.transform.position);

            var rbKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody.velocity;
            rbKayak.y = 0;
            if (rbKayak.magnitude > 1f && _attackState == AttackState.None)
            {
                sharkManager.PointTarget.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position,
                    sharkManager.TargetTransform,
                    rbKayak.magnitude * sharkManager.Data.MutliplySpeed);
            }
            else if (_distSharkTarget > sharkManager.Data.DistSharkTargetPointStopMoving)
            {
                sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
            }

            if (_attackState == AttackState.None)
            {
                _attack += Time.deltaTime;
            }

            //timer Attack
            percentLife = (sharkManager.CurrentLife * 100) / sharkManager.Data.Life;

            if (_attack >= sharkManager.Data.TimerForAttack && _attackState == AttackState.None)
            {
                sharkManager.IsCollided = false;


                if (percentLife > 75)
                {
                    _attackState = AttackState.JumpAround;
                }
                else if (percentLife <= 75 && percentLife > 25)
                {
                    _attackState = AttackState.Rush;
                }
                else if (percentLife <= 25)
                {
                    //var dist = (sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseThree + sharkManager.Data.MaxDistanceBetweenTarget) / 2;
                    //sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, dist);
                    _attackState = AttackState.JumpToTarget;
                }
            }


            //RotateCombat(sharkManager);


            //rush
            if (_attackState == AttackState.Rush)
            {
                _state = CombatState.RushToTarget;
                //dist point stop moving
                if (_distSharkTarget > sharkManager.Data.DistSharkTargetPointStopMoving)
                {
                    sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                }
            }

            //attack in front target point
            if (_attackState == AttackState.JumpToTarget)
            {
                //if (_distSharkPointTargetAttack > 20)
                //{
                //    var dist = (sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseThree + sharkManager.Data.MaxDistanceBetweenTarget) / 2;
                //    sharkManager.PointTargetAttack.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position, sharkManager.TargetTransform, dist);
                //}
                ////point stop moving
                //else
                //{
                _state = CombatState.JumpToTarget;
                //}
            }


            //jump around target
            if (_attackState == AttackState.JumpAround)
            {
                _state = CombatState.JumpAround;
            }

            switch (_state)
            {
                case CombatState.MoveToTarget:
                    if (percentLife > 75)
                    {
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseOne)
                        {
                            sharkManager.SwitchSpeed(sharkManager.Data.SpeedWhenOutOfRange);
                            MoveToTarget(sharkManager);
                        }
                        else
                        {
                            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                            _state = CombatState.RotateAroundPoint;
                        }
                    }
                    else if (percentLife <= 75 && percentLife > 25)
                    {
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseTwo)
                        {
                            sharkManager.SwitchSpeed(sharkManager.Data.SpeedWhenOutOfRange);
                            MoveToTarget(sharkManager);
                        }
                        else
                        {
                            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                            _state = CombatState.RotateAroundPoint;
                        }
                    }
                    else if (percentLife <= 25)
                    {
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseThree)
                        {
                            sharkManager.SwitchSpeed(sharkManager.Data.SpeedWhenOutOfRange);
                            MoveToTarget(sharkManager);
                        }
                        else
                        {
                            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                            _state = CombatState.RotateAroundPoint;
                        }
                    }

                    break;

                case CombatState.RotateAroundPoint:
                    if (_distSharkTarget < sharkManager.Data.MaxDistanceBetweenTarget)
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
                case CombatState.JumpAround:

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
                case CombatState.JumpToTarget:

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
                    sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatJumpInFront);
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
        _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase, 0.005f);

        var pos = sharkManager.transform.position;
        pos.y = _currentOffSet;
        sharkManager.transform.position = pos;



        //rotate to angle 90 to target
        Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;
        if (percentLife > 75)
        {
            if (_distSharkTarget > sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseOne)
            {
                rotate(sharkManager, rotation);
            }
        }
        else if (percentLife <= 75 && percentLife > 25)
        {
            if (_distSharkTarget > sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseTwo)
            {
                rotate(sharkManager, rotation);
            }
        }
        else if (percentLife <= 25)
        {
            if (_distSharkTarget > sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseThree)
            {
                rotate(sharkManager, rotation);
            }
        }

        if (_attackState != AttackState.JumpToTarget)
        {
            sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatRotationAroundPoint);
        }
        else
        {
            sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatToTriggerJumpInFront);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
        }

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        if (percentLife > 75)
        {
            if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseOne)
            {
                _state = CombatState.MoveToTarget;
            }
        }
        else if (percentLife <= 75 && percentLife > 25)
        {
            if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseTwo)
            {
                _state = CombatState.MoveToTarget;
            }
        }
        else if (percentLife <= 25)
        {
            if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseThree)
            {
                _state = CombatState.MoveToTarget;
            }
        }

    }

    void rotate(SharkManager sharkManager, Vector3 rotation)
    {
        Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
        Vector2 sharkForward = new Vector2(sharkManager.Forward.transform.forward.x, sharkManager.Forward.transform.forward.z);
        Vector2 sharkPos = new Vector2(sharkManager.Forward.transform.position.x, sharkManager.Forward.transform.position.z);

        float angle = Vector2.SignedAngle(sharkForward, targetPos - sharkPos);
        if (angle < 0)
            angle += 360;
        if (angle > 90 && angle < 180)
        {
            rotation.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else if (angle > 270 && angle < 360)
        {
            rotation.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else
        {
            rotation.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        //apply rota
        sharkManager.Forward.transform.localEulerAngles = rotation;
    }


    private void animationCurve(SharkManager sharkManager)
    {
        _timeAnimationCurve += Time.deltaTime;

        _lastKey = sharkManager.Data.JumpCurve[sharkManager.Data.JumpCurve.length - 1];

        if (sharkManager.Forward.transform.position.y < sharkManager.ShowCircleProfondeur && _waveStartJump == false)
        {
            Vector3 localPosCircle = sharkManager.Forward.transform.localPosition;
            Vector3 offsetCircle = sharkManager.Forward.transform.forward * sharkManager.CircleDistanceMulptiply;
            Vector3 positionCircle = localPosCircle + offsetCircle;
            Vector3 worldPosCircle = sharkManager.ParentGameObject.transform.TransformPoint(positionCircle);
            worldPosCircle.y = 0;

            sharkManager.Circle.transform.position = worldPosCircle;
            sharkManager.Circle.SetActive(true);
        }


        if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartFirstCircularWave && _waveStartJump == false)
        {
            Vector2 center = sharkManager.Data.StartFirstCircularWaveData.Center;

            Vector3 localPos = sharkManager.Forward.transform.localPosition;
            Vector3 offset = sharkManager.Forward.transform.forward * 2;
            Vector3 position = localPos + offset;
            Vector3 worldPos = sharkManager.ParentGameObject.transform.TransformPoint(position);

            center.x = worldPos.x;
            center.y = worldPos.z;
            sharkManager.Data.StartFirstCircularWaveData.Center = center;


            sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartFirstCircularWaveData);
            _waveStartJump = true;
            sharkManager.Circle.SetActive(false);
        }

        //Vector3 pos = sharkManager.Forward.transform.position;
        //pos.y = sharkManager.Data.JumpCurve.Evaluate(_timeAnimationCurve);
        //sharkManager.Forward.transform.position = pos;
         Vector3 pos = sharkManager.transform.position;
        pos.y = sharkManager.Data.JumpCurve.Evaluate(_timeAnimationCurve);
        sharkManager.transform.position = pos;

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        Vector3 rotation = sharkManager.transform.eulerAngles;

        rotation.x = sharkManager.Data.VisualCurve.Evaluate(_timeAnimationCurve) * 100;


        if (_attackState == AttackState.JumpToTarget)
        {
            rotation.z += Time.deltaTime * sharkManager.Data.SpeedRotationOnItself;
        }


        if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartSecondCircularWaveTime && _waveEndJump == false)
        {
            Vector2 center = sharkManager.Data.StartSecondCircularWaveData.Center;

            Vector3 localPos = sharkManager.Forward.transform.localPosition;
            Vector3 offset = sharkManager.Forward.transform.forward * 2;
            Vector3 position = localPos + offset;
            Vector3 worldPos = sharkManager.ParentGameObject.transform.TransformPoint(position);

            center.x = worldPos.x;
            center.y = worldPos.z;


            sharkManager.Data.StartSecondCircularWaveData.Center = center;
            sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartSecondCircularWaveData);
            _waveEndJump = true;
        }

        if (_timeAnimationCurve >= _lastKey.time && _attackState != AttackState.None)
        {
            _timeAnimationCurve = 0;
            _waveStartJump = false;
            _waveEndJump = false;

            if (_atqNumber <= 0)
            {
                _state = CombatState.RotateAroundPoint;
            }


            //var rand = Random.Range(0, 360);
            //sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, rand, DIST_POINT);
            sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
            _startAtq = false;

            _atqNumber -= 1;
        }



        sharkManager.transform.eulerAngles = rotation;

    }



    private void animationCurveInFront(SharkManager sharkManager)
    {
        _timeAnimationCurve += Time.deltaTime;

        _lastKey = sharkManager.Data.JumpCurvePhaseThree[sharkManager.Data.JumpCurvePhaseThree.length - 1];

        if (sharkManager.Forward.transform.position.y < sharkManager.ShowCircleProfondeur && _waveStartJump == false)
        {
            Vector3 localPosCircle = sharkManager.Forward.transform.localPosition;
            Vector3 offsetCircle = sharkManager.Forward.transform.forward * sharkManager.CircleDistanceMulptiply;
            Vector3 positionCircle = localPosCircle + offsetCircle;
            Vector3 worldPosCircle = sharkManager.ParentGameObject.transform.TransformPoint(positionCircle);
            worldPosCircle.y = 0;

            sharkManager.Circle.transform.position = worldPosCircle;
            sharkManager.Circle.SetActive(true);
        }

        if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartFirstCircularWavePhaseThree && _waveStartJump == false)
        {
            Vector2 center = sharkManager.Data.StartFirstCircularWaveDataPhaseThree.Center;

            center.x = sharkManager.Forward.transform.position.x;
            center.y = sharkManager.Forward.transform.position.z;

            sharkManager.Data.StartFirstCircularWaveDataPhaseThree.Center = center;

            sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartFirstCircularWaveDataPhaseThree);
            _waveStartJump = true;
            sharkManager.Circle.SetActive(false);

        }

        //Vector3 pos = sharkManager.Forward.transform.position;
        //pos.y = sharkManager.Data.JumpCurvePhaseThree.Evaluate(_timeAnimationCurve);
        //sharkManager.Forward.transform.position = pos;
        Vector3 pos = sharkManager.transform.position;
        pos.y = sharkManager.Data.JumpCurvePhaseThree.Evaluate(_timeAnimationCurve);
        sharkManager.transform.position = pos;

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        Vector3 rotation = sharkManager.transform.eulerAngles;

        rotation.x = sharkManager.Data.VisualCurvePhaseThree.Evaluate(_timeAnimationCurve) * 100;


        rotation.z += Time.deltaTime * sharkManager.Data.SpeedRotationOnItself;


        if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartSecondCircularWaveTimePhaseThree && _waveEndJump == false)
        {
            Vector2 center = sharkManager.Data.StartSecondCircularWaveDataPhaseThree.Center;

            center.x = sharkManager.transform.position.x;
            center.y = sharkManager.transform.position.z;

            sharkManager.Data.StartSecondCircularWaveDataPhaseThree.Center = center;
            sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartSecondCircularWaveDataPhaseThree);
            _waveEndJump = true;
        }

        if (_timeAnimationCurve >= _lastKey.time && _attackState != AttackState.None)
        {
            _timeAnimationCurve = 0;
            _waveStartJump = false;
            _waveEndJump = false;


            if (_attackState == AttackState.JumpToTarget)
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
            _atqNumber = sharkManager.Data.NumberJumpWhenCrazy;
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
            rotation.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else if (angle > 270 && angle < 360)
        {
            rotation.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else
        {
            rotation.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
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
            animationCurveInFront(sharkManager);
        }
        else
        {
            Vector3 rota = sharkManager.Forward.transform.localEulerAngles;
            if (angle >= 0 && angle <= 180)
            {
                rota.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
            }
            else if (angle < 360 && angle > 180)
            {
                rota.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
            }

            sharkManager.Forward.transform.localEulerAngles = rota;

            sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);

        }

        //dist target
        if (angle < 3 || angle > 357 && _distSharkTarget < sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseThree / 2)
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
            rota.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }

        sharkManager.Forward.transform.localEulerAngles = rota;

        if (angle < 3 || angle > 357)
        {
            _currentOffSet = Mathf.Lerp(_currentOffSet, sharkManager.Data.ElevationOffsetWhenRush, 0.05f);

            sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatRush);

            var pos = sharkManager.transform.position;
            pos.y = _currentOffSet;
            sharkManager.transform.position = pos;
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
            rota.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }

        sharkManager.Forward.transform.localEulerAngles = rota;

        _currentOffSet = Mathf.Lerp(_currentOffSet, _elevationOffsetBase, 0.005f);

        var pos = sharkManager.transform.position;
        pos.y = _currentOffSet;
        sharkManager.transform.position = pos;
        //SetOffSet(sharkManager, sharkManager.ElevationOffset);

        sharkManager.SwitchSpeed(sharkManager.Data.SpeedToMoveToTarget);


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
            rota.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }
        else if (angle < 360 && angle > 180)
        {
            rota.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
        }

        //apply rota
        sharkManager.Forward.transform.localEulerAngles = rota;


        var pos = sharkManager.transform.position;
        pos.y = sharkManager.Data.ElevationOffset;
        sharkManager.transform.position = pos;

        if (_attackState != AttackState.JumpToTarget)
            sharkManager.SwitchSpeed(sharkManager.Data.SpeedToMoveToTarget);

        sharkManager.Forward.transform.Translate(Vector3.forward * sharkManager.CurrentSpeed * Time.deltaTime, Space.Self);
    }
}
