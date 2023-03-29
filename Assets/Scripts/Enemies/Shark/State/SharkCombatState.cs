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

    public enum CombatState { RotateBeforeJump = 0, Wait = 1, Jump = 2, RotateAfterJump = 3 };
    private CombatState _state;

    private bool _waveStartJump = false;
    private bool _waveEndJump = false;

    public override void EnterState(SharkManager sharkManager)
    {
        Debug.Log("combat state");
        _elevationOffsetBase = sharkManager.ElevationOffset;
        _radius = Random.Range(sharkManager.RadiusRotateAroundTargetMin, sharkManager.RadiusRotateAroundTargetMax);
        _state = CombatState.RotateBeforeJump;
    }

    public override void UpdateState(SharkManager sharkManager)
    {
        if (sharkManager.TargetTransform != null)
        {

            switch (_state)
            {
                #region BeforeJump
                case CombatState.RotateBeforeJump:

                    _timerToPassUnderPlayer += Time.deltaTime;

                    RotateAround(sharkManager);

                    if (_timerToPassUnderPlayer > sharkManager.TimerToPassUnderPlayer /*&& _state == CombatState.RotateBeforeJump*/)
                    {
                        if (sharkManager.ElevationOffset > -sharkManager.DiveDistance)
                        {
                            sharkManager.ElevationOffset -= sharkManager.DiveSpeed;
                        }
                        else
                            _state = CombatState.Wait;
                    }

                    break;
                #endregion

                #region Wait for jump
                case CombatState.Wait:

                    sharkManager.Shadow.SetActive(true);
                    CircleUI(sharkManager);

                    break;
                #endregion

                #region Jump
                case CombatState.Jump:

                    _timerHittable += Time.deltaTime;
                    if (_timerHittable >= sharkManager.TimeEndHittable)
                    {
                        sharkManager.SharkCollider.enabled = false;
                    }
                    else if (_timerHittable >= sharkManager.TimeStartHittable && _timerHittable < sharkManager.TimeEndHittable)
                    {
                        sharkManager.SharkCollider.enabled = true;
                    }

                    Jump(sharkManager);

                    break;
                #endregion

                #region After jump
                case CombatState.RotateAfterJump:
                    _timerHittable = 0;
                    RotateAround(sharkManager);

                    if (sharkManager.ElevationOffset < _elevationOffsetBase)
                    {
                        sharkManager.ElevationOffset += sharkManager.DiveSpeed;
                    }
                    else
                    {
                        sharkManager.ElevationOffset = _elevationOffsetBase;
                        _radius = Random.Range(sharkManager.RadiusRotateAroundTargetMin, sharkManager.RadiusRotateAroundTargetMax);
                        _state = CombatState.RotateBeforeJump;
                    }

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
    private void RotateAround(SharkManager sharkManager)
    {
        Vector3 position = sharkManager.TargetTransform != null ? sharkManager.TargetTransform.position : Vector3.zero;

        // float radius = Random.Range(enemyManager.RadiusRotateAroundTargetMin, enemyManager.RadiusRotateAroundTargetMax);

        sharkManager.PositionOffset.Set(Mathf.Cos(sharkManager.Angle) * _radius,
            sharkManager.ElevationOffset,
            Mathf.Sin(sharkManager.Angle) * _radius);

        sharkManager.transform.position = new Vector3(sharkManager.TargetTransform.position.x + sharkManager.PositionOffset.x,
            sharkManager.ElevationOffset,
            sharkManager.TargetTransform.position.z + sharkManager.PositionOffset.z);

        sharkManager.Angle += Time.deltaTime * sharkManager.RotationSpeed;

        Quaternion rotation = sharkManager.transform.rotation;
        Vector3 enemyPos = sharkManager.transform.position;

        rotation = Quaternion.LookRotation(new Vector3(position.x - enemyPos.x, 0, position.z - enemyPos.z), sharkManager.TargetTransform.up);
        rotation.x = 0;
        rotation.z = 0;
        sharkManager.transform.rotation = rotation;

        sharkManager.Angle += Time.deltaTime * sharkManager.RotationSpeed;
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
            sharkManager.StartJumpCircularWaveData.Center = sharkManager.transform.position;
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
            rotation.z = -90;
            sharkManager.transform.eulerAngles = rotation;
        }
        else
        {
            rotation.z = 90;
            sharkManager.transform.eulerAngles = rotation;
        }

        if (_timeAnimationCurve >= _lastKey.time - sharkManager.EndJumpCircularWaveTime && _waveEndJump == false)
        {
            sharkManager.StartJumpCircularWaveData.Center = sharkManager.transform.position;
            sharkManager.WavesData.LaunchCircularWave(sharkManager.StartJumpCircularWaveData);
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
        }
        #endregion
    }


    #endregion
}
