using Character;
using UI;
using UnityEngine;

namespace Enemies.Shark.State
{
    public class SharkCombatState : SharkBaseState
    {
        #region Variable
        private enum CombatState { RushToTarget = 0, JumpAround = 1, MoveToTarget = 2, RotateAroundPoint = 3, JumpToTarget = 4 };
        private enum AttackState { None = 0, Rush = 1, JumpToTarget = 2, JumpAround = 3 }

        private const float MinDistance = 8.0f;
        private CombatState _state;
        private AttackState _attackState;
        private float _timeAnimationCurve = 0;
        private Keyframe _lastKey;

        private bool _waveStartJump = false;
        private bool _waveEndJump = false;

        private float _elevationOffsetBase;
        private float _currentOffSet;

        private float _distSharkTarget;
        private float _distSharkPointTarget;

        private int _attackNumber;
        private float _attack;
        private float _percentLife;
        private bool _startJump;
        private bool _startAttack;
        private bool _left;
        private bool _movePointTarget;
        #endregion
        public override void EnterState(SharkManager sharkManager)
        {
            sharkManager.SetUpStartEnemyUI();

            sharkManager.SharkCollider.enabled = true;

            _elevationOffsetBase = sharkManager.Data.ElevationOffset;

            _currentOffSet = sharkManager.Data.ElevationOffset;
            _state = CombatState.MoveToTarget;
            _attackState = AttackState.None;

            if (sharkManager.TargetTransform != null)
            {
                sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
            }
        }

        public override void UpdateState(SharkManager sharkManager)
        {
            if (CharacterManager.Instance.CameraManagerProperty.StartDeath == true || sharkManager.IsPossessed == false && _attackState == AttackState.None)
            {
                sharkManager.FreeRoamState();
            }

            if (sharkManager.TargetTransform == null) return;

            GetDistances(sharkManager);

            Vector3 kayakVelocity = CharacterManager.Instance.KayakControllerProperty.Rigidbody.velocity;
            kayakVelocity.y = 0;
            if (kayakVelocity.magnitude > 1f && _attackState == AttackState.None)
            {
                sharkManager.PointTarget.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position,
                    sharkManager.TargetTransform,
                    kayakVelocity.magnitude * sharkManager.Data.MutliplySpeed);
            }
            else if (_attackState == AttackState.JumpToTarget && _distSharkTarget > sharkManager.Data.DistSharkTargetPointStopMoving)
            {
                sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
            }

            if (_attackState == AttackState.None)
            {
                _attack += Time.deltaTime;
            }

            SetAttack(sharkManager);

            StateCombat(sharkManager);
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

            Vector3 sharkPosition = sharkManager.transform.position;
            sharkPosition.y = _currentOffSet;
            sharkManager.transform.position = sharkPosition;

            switch (_percentLife)
            {
                case > 75:
                    {
                        if (_distSharkTarget > sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseOne)
                        {
                            RotateTo90(sharkManager);
                        }
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseOne)
                        {
                            _state = CombatState.MoveToTarget;
                        }
                        break;
                    }
                case <= 75 and > 25:
                    {
                        if (_distSharkTarget > sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseTwo)
                        {
                            RotateTo90(sharkManager);
                        }
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseTwo)
                        {
                            _state = CombatState.MoveToTarget;
                        }
                        break;
                    }
                case <= 25:
                    {
                        if (_distSharkTarget > sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseThree)
                        {
                            RotateTo90(sharkManager);
                        }
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseThree)
                        {
                            _state = CombatState.MoveToTarget;
                        }
                        break;
                    }
            }

            sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatRotationAroundPoint);
            sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);
        }
        private void JumpCrazy(SharkManager sharkManager)
        {
            if (_startJump == false)
            {
                //number of attack
                _attackNumber = sharkManager.Data.NumberJumpWhenCrazy;
                _startJump = true;
            }

            RotateTo90(sharkManager);

            if (_movePointTarget)
            {
                var rbKayak = CharacterManager.Instance.KayakControllerProperty.Rigidbody.velocity;
                rbKayak.y = 0;
                if (rbKayak.magnitude > 1f)
                {
                    sharkManager.PointTarget.transform.position = GetPointInFrontAndDistance(sharkManager.TargetTransform.position,
                        sharkManager.TargetTransform,
                        rbKayak.magnitude * sharkManager.Data.MutliplySpeed);
                }
                else
                {
                    sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                }
            }

            HandleAnimationCurve(sharkManager);

            if (_attackNumber > 0)
            {
                return;
            }
            _state = CombatState.RotateAroundPoint;
            _attackState = AttackState.None;
            _attack = 0;
            _startJump = false;
        }
        private void JumpInFront(SharkManager sharkManager)
        {
            if (_startJump == false)
            {
                _attackNumber = 1;
                _startJump = true;
            }

            float angle = GetAngle(sharkManager);

            if (_startAttack)
            {
                HandleAnimationCurveInFront(sharkManager);
            }
            else
            {
                RotateTo0(sharkManager);

                sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);
            }

            //distance target
            if (angle < 3 || angle > 357 && _distSharkTarget < sharkManager.Data.MinDistanceBetweenTargetWhenRotatePhaseThree / 2)
            {
                _startAttack = true;
            }

            if (_attackNumber > 0)
            {
                return;
            }
            _state = CombatState.RotateAroundPoint;
            _attackState = AttackState.None;
            _attack = 0;
            _startJump = false;
        }
        private void RushToTarget(SharkManager sharkManager)
        {
            float angle = GetAngle(sharkManager);

            RotateTo0(sharkManager);

            if (angle < 3 || angle > 357)
            {
                _currentOffSet = Mathf.Lerp(_currentOffSet, sharkManager.Data.ElevationOffsetWhenRush, 0.05f);

                sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatRush);

                var pos = sharkManager.transform.position;
                pos.y = _currentOffSet;
                sharkManager.transform.position = pos;
            }

            sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);
        }
        private void MoveToTarget(SharkManager sharkManager)
        {
            RotateTo0(sharkManager);

            var pos = sharkManager.transform.position;
            pos.y = sharkManager.Data.ElevationOffset;
            sharkManager.transform.position = pos;

            if (_attackState != AttackState.JumpToTarget)
                sharkManager.SwitchSpeed(sharkManager.Data.SpeedToMoveToTarget);

            sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);
        }
        private void HandleAnimationCurve(SharkManager sharkManager)
        {
            _timeAnimationCurve += Time.deltaTime;

            if (_waveStartJump == false)
            {
                _movePointTarget = false;
            }

            _lastKey = sharkManager.Data.JumpCurve[sharkManager.Data.JumpCurve.length - 1];

            if (sharkManager.transform.position.y < sharkManager.ShowCircleDepth && _waveStartJump == false)
            {
                Vector3 localPosCircle = sharkManager.Forward.transform.localPosition;

                Vector3 offsetCircle = sharkManager.Forward.transform.forward * sharkManager.DistanceInFrontOfShark + sharkManager.Forward.transform.right * sharkManager.DistanceSideOfShark;
                if (_left == true)
                {
                    offsetCircle = sharkManager.Forward.transform.forward * sharkManager.DistanceInFrontOfShark - sharkManager.Forward.transform.right * sharkManager.DistanceSideOfShark;
                }
                else
                {
                    offsetCircle = sharkManager.Forward.transform.forward * sharkManager.DistanceInFrontOfShark + sharkManager.Forward.transform.right * sharkManager.DistanceSideOfShark;
                }
                Vector3 positionCircle = localPosCircle + offsetCircle;
                Vector3 worldPosCircle = sharkManager.ParentGameObject.transform.TransformPoint(positionCircle);
                worldPosCircle.y = 0;

                sharkManager.Circle.transform.position = worldPosCircle;
                sharkManager.Circle.SetActive(true);
            }

            if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartFirstCircularWave && _waveStartJump == false)
            {
                sharkManager.Data.StartFirstCircularWaveData.Center = GetCenter(sharkManager);
                sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartFirstCircularWaveData);

                sharkManager.StartJump.Invoke();
                _waveStartJump = true;
                _movePointTarget = true;
                sharkManager.Circle.SetActive(false);
                sharkManager.PosParticles.SetActive(false);
            }

            Vector3 pos = sharkManager.transform.position;
            pos.y = sharkManager.Data.JumpCurve.Evaluate(_timeAnimationCurve);
            sharkManager.transform.position = pos;

            sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);

            Vector3 rotation = sharkManager.transform.eulerAngles;

            rotation.x = sharkManager.Data.VisualCurve.Evaluate(_timeAnimationCurve) * 100;

            if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartSecondCircularWaveTime && _waveEndJump == false)
            {
                sharkManager.Data.StartSecondCircularWaveData.Center = GetCenter(sharkManager);
                sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartSecondCircularWaveData);

                sharkManager.EndJump.Invoke();
                _waveEndJump = true;
                sharkManager.PosParticles.SetActive(true);
            }

            if (_timeAnimationCurve >= _lastKey.time && _attackState != AttackState.None)
            {
                _timeAnimationCurve = 0;
                _waveStartJump = false;
                _waveEndJump = false;

                if (_attackNumber <= 0)
                {
                    _state = CombatState.RotateAroundPoint;
                }

                _startAttack = false;
                _attackNumber -= 1;
            }

            sharkManager.transform.eulerAngles = rotation;
        }
        private void HandleAnimationCurveInFront(SharkManager sharkManager)
        {
            _timeAnimationCurve += Time.deltaTime;

            _lastKey = sharkManager.Data.JumpCurvePhaseThree[sharkManager.Data.JumpCurvePhaseThree.length - 1];

            if (sharkManager.transform.position.y < sharkManager.ShowCircleDepth && _waveStartJump == false)
            {
                Vector3 localPosCircle = sharkManager.Forward.transform.localPosition;

                Vector3 offsetCircle = sharkManager.Forward.transform.forward * sharkManager.DistanceInFrontOfSharkPhaseThree;

                Vector3 positionCircle = localPosCircle + offsetCircle;
                Vector3 worldPosCircle = sharkManager.ParentGameObject.transform.TransformPoint(positionCircle);
                worldPosCircle.y = 0;

                sharkManager.Circle.transform.position = worldPosCircle;
                sharkManager.Circle.SetActive(true);
            }
            if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartFirstCircularWavePhaseThree && _waveStartJump == false)
            {
                sharkManager.Data.StartFirstCircularWaveDataPhaseThree.Center = GetCenter(sharkManager);
                sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartFirstCircularWaveDataPhaseThree);

                sharkManager.StartJump.Invoke();
                _waveStartJump = true;
                sharkManager.Circle.SetActive(false);

            }
            Vector3 sharkPosition = sharkManager.transform.position;
            sharkPosition.y = sharkManager.Data.JumpCurvePhaseThree.Evaluate(_timeAnimationCurve);
            sharkManager.transform.position = sharkPosition;

            sharkManager.Forward.transform.Translate(Vector3.forward * (sharkManager.CurrentSpeed * Time.deltaTime), Space.Self);

            Vector3 rotation = sharkManager.transform.eulerAngles;

            rotation.x = sharkManager.Data.VisualCurvePhaseThree.Evaluate(_timeAnimationCurve) * 100;
            rotation.z += Time.deltaTime * sharkManager.Data.SpeedRotationOnItself;


            if (_timeAnimationCurve >= _lastKey.time - sharkManager.Data.StartSecondCircularWaveTimePhaseThree && _waveEndJump == false)
            {
                sharkManager.Data.StartSecondCircularWaveDataPhaseThree.Center = GetCenter(sharkManager);
                sharkManager.WavesData.LaunchCircularWave(sharkManager.Data.StartSecondCircularWaveDataPhaseThree);

                sharkManager.EndJump.Invoke();
                _waveEndJump = true;
                sharkManager.PosParticles.SetActive(true);
            }

            if (_timeAnimationCurve >= _lastKey.time && _attackState != AttackState.None)
            {
                _timeAnimationCurve = 0;
                _waveStartJump = false;
                _waveEndJump = false;
                rotation.z = 0;

                _state = CombatState.RotateAroundPoint;

                _startAttack = false;
                _attackNumber -= 1;
            }

            sharkManager.transform.eulerAngles = rotation;
        }
        private void RotateTo90(SharkManager sharkManager)
        {
            Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;
            Transform forwardTransform = sharkManager.Forward.transform;
            Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
            Vector2 sharkForward = new Vector2(forwardTransform.forward.x, forwardTransform.forward.z);
            Vector2 sharkPos = new Vector2(forwardTransform.position.x, forwardTransform.position.z);
            float angle = Vector2.SignedAngle(sharkForward, targetPos - sharkPos); //Do not GetAngle() it's not the same order
            if (angle < 0) angle += 360;

            if (angle > 90 && angle < 180 || angle > 270 && angle < 360)
            {
                rotation.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
                if (angle > 90 && angle < 180) _left = true;
                else if (angle > 270 && angle < 360) _left = false;
            }
            else
            {
                rotation.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
            }
            sharkManager.Forward.transform.localEulerAngles = rotation;
        }
        private void RotateTo0(SharkManager sharkManager)
        {
            float angle = GetAngle(sharkManager);
            Vector3 rotation = sharkManager.Forward.transform.localEulerAngles;
            switch (angle)
            {
                case >= 0 and <= 180:
                    rotation.y += Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
                    break;
                case < 360 and > 180:
                    rotation.y -= Time.deltaTime * sharkManager.Data.SpeedCombatRotationStatic;
                    break;
            }
            sharkManager.Forward.transform.localEulerAngles = rotation;
        }
        private void StateCombat(SharkManager sharkManager)
        {
            switch (_state)
            {
                case CombatState.MoveToTarget:
                    if (_percentLife > 75)
                    {
                        if (_distSharkPointTarget >= sharkManager.Data.MaxDistanceBetweenPointInCombatPhaseOne)
                        {
                            sharkManager.SwitchSpeed(sharkManager.Data.SpeedWhenOutOfRange);
                            MoveToTarget(sharkManager);
                        }
                        else
                        {
                            _state = CombatState.RotateAroundPoint;
                        }
                    }
                    else if (_percentLife <= 75 && _percentLife > 25)
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
                    else if (_percentLife <= 25)
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
                case CombatState.RushToTarget:
                    if (_distSharkPointTarget <= MinDistance)
                    {
                        _attack = 0;
                        _state = CombatState.RotateAroundPoint;
                        sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                        _attackState = AttackState.None;
                    }
                    else
                    {
                        RushToTarget(sharkManager);
                    }
                    break;
                case CombatState.JumpAround:
                    JumpCrazy(sharkManager);
                    break;
                case CombatState.JumpToTarget:
                    sharkManager.SwitchSpeed(sharkManager.Data.SpeedCombatJumpInFront);
                    JumpInFront(sharkManager);
                    break;
            }
        }

        private void SetAttack(SharkManager sharkManager)
        {
            //rush
            if (_attackState == AttackState.Rush)
            {
                _state = CombatState.RushToTarget;
                //distance point stop moving
                if (_distSharkTarget > sharkManager.Data.DistSharkTargetPointStopMoving)
                {
                    sharkManager.PointTarget.transform.position = MathTools.GetPointFromAngleAndDistance(sharkManager.TargetTransform.position, 0, 0);
                }
            }
            //attack in front target point
            if (_attackState == AttackState.JumpToTarget)
            {
                _state = CombatState.JumpToTarget;
            }
            //jump around target
            if (_attackState == AttackState.JumpAround)
            {
                _state = CombatState.JumpAround;
            }
            if (_attack >= sharkManager.Data.TimerForAttack && _attackState == AttackState.None)
            {
                sharkManager.IsCollided = false;
                _percentLife = (sharkManager.CurrentLife * 100) / sharkManager.Data.Life;

                _attackState = _percentLife switch
                {
                    > 75 => AttackState.JumpAround,
                    <= 75 and > 25 => AttackState.Rush,
                    <= 25 => AttackState.JumpToTarget,
                    _ => _attackState
                };
            }
        }
        private Vector3 GetPointInFrontAndDistance(Vector3 startingPoint, Transform transform, float distance)
        {
            // Create a new Vector3 in front of target
            Vector3 newPoint = transform.forward;
            newPoint.x = (newPoint.x * distance) + startingPoint.x;
            newPoint.y = 0;
            newPoint.z = (newPoint.z * distance) + startingPoint.z;
            return newPoint;
        }
        private float GetAngle(SharkManager sharkManager)
        {
            Transform forwardTransform = sharkManager.Forward.transform;
            Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
            Vector2 sharkForward = new Vector2(forwardTransform.forward.x, forwardTransform.forward.z);
            Vector2 sharkPos = new Vector2(forwardTransform.position.x, forwardTransform.position.z);
            float angle = Vector2.SignedAngle(targetPos - sharkPos, sharkForward);

            if (angle < 0) angle += 360;
            return angle;
        }
        private Vector2 GetCenter(SharkManager sharkManager)
        {
            Vector3 localPosition = sharkManager.Forward.transform.localPosition;
            Vector3 offset = sharkManager.Forward.transform.forward * 2;
            Vector3 offsetPosition = localPosition + offset;
            Vector3 worldPos = sharkManager.ParentGameObject.transform.TransformPoint(offsetPosition);
            Vector2 center = sharkManager.Data.StartSecondCircularWaveDataPhaseThree.Center;
            center.x = worldPos.x;
            center.y = worldPos.z;

            return center;
        }
        private void GetDistances(SharkManager sharkManager)
        {
            Transform shark = sharkManager.transform;
            _distSharkTarget = Vector3.Distance(sharkManager.TargetTransform.position, shark.position);
            _distSharkPointTarget = Vector3.Distance(sharkManager.PointTarget.transform.position, shark.position);
        }
        #endregion
    }
}
