using Kayak;
using UnityEngine;

public class EnemyCombatState : EnemyBaseState
{

    private float _timerToPassUnderPlayer;
    float _timeAnimationCurve = 0;
    private Keyframe _lastKey;
    float _timerTargetFollow = 0;
    float _elevationOffsetBase;

    public enum CombatState { RotateBeforeJump = 0, Wait = 1, Jump = 2, RotateAfterJump = 3 };
    private CombatState _state;


    public override void EnterState(EnemyManager enemyManager)
    {
        Debug.Log("combat state");
        _elevationOffsetBase = enemyManager.ElevationOffset;
        _state = CombatState.RotateBeforeJump;
    }

    public override void UpdateState(EnemyManager enemyManager)
    {
        if (enemyManager.Target != null)
        {

            switch (_state)
            {
                #region BeforeJump
                case CombatState.RotateBeforeJump:

                    _timerToPassUnderPlayer += Time.deltaTime;

                    RotateAround(enemyManager);

                    if (_timerToPassUnderPlayer > enemyManager.TimerToPassUnderPlayer /*&& _state == CombatState.RotateBeforeJump*/)
                    {
                        if (enemyManager.ElevationOffset > -enemyManager.DiveDistance)
                        {
                            enemyManager.ElevationOffset -= enemyManager.DiveSpeed;
                        }
                        else
                            _state = CombatState.Wait;
                    }

                    break;
                #endregion
                #region Wait for jump
                case CombatState.Wait:

                    enemyManager.Shadow.SetActive(true);
                    CircleUI(enemyManager);

                    break;
                #endregion

                #region Jump
                case CombatState.Jump:

                    Jump(enemyManager);

                    break;
                #endregion

                #region After jump
                case CombatState.RotateAfterJump:

                    RotateAround(enemyManager);

                    if (enemyManager.ElevationOffset < _elevationOffsetBase)
                    {
                        enemyManager.ElevationOffset += enemyManager.DiveSpeed;
                    }
                    else
                    {
                        enemyManager.ElevationOffset = _elevationOffsetBase;
                        _state = CombatState.RotateBeforeJump;
                    }

                    break;
                #endregion

                default:

                    break;
            }
        }
    }
    public override void FixedUpdate(EnemyManager enemyManager)
    {

    }

    public override void SwitchState(EnemyManager enemyManager)
    {

    }

    #region Fonction
    private void RotateAround(EnemyManager enemyManager)
    {
        Vector3 position = enemyManager.Target != null ? enemyManager.Target.position : Vector3.zero;

        float radius = Random.Range(enemyManager.RadiusRotateAroundTargetMin, enemyManager.RadiusRotateAroundTargetMax);

        enemyManager.PositionOffset.Set(Mathf.Cos(enemyManager.Angle) * radius,
            enemyManager.ElevationOffset,
            Mathf.Sin(enemyManager.Angle) * radius);

        enemyManager.transform.position = new Vector3(enemyManager.Target.position.x + enemyManager.PositionOffset.x, 
            enemyManager.ElevationOffset, 
            enemyManager.Target.position.z + enemyManager.PositionOffset.z);

        enemyManager.Angle += Time.deltaTime * enemyManager.RotationSpeed;

        Quaternion rotation = enemyManager.transform.rotation;
        Vector3 enemyPos = enemyManager.transform.position;

        rotation = Quaternion.LookRotation(new Vector3(position.x - enemyPos.x, 0, position.z - enemyPos.z), enemyManager.Target.up);
        rotation.x = 0;
        rotation.z = 0;
        enemyManager.transform.rotation = rotation;

        enemyManager.Angle += Time.deltaTime * enemyManager.RotationSpeed;
    }

    private void CircleUI(EnemyManager enemyManager)
    {
        if (_timerTargetFollow < enemyManager.TimerTargetFollow)
        {
            enemyManager.Shadow.transform.position = enemyManager.Target.position;
        }

        _timerTargetFollow += Time.deltaTime;

        if (enemyManager.Shadow.transform.localScale.x < enemyManager.ShadowMaxSizeForJump)
        {
            enemyManager.Shadow.transform.localScale += (Vector3.one / 2) * Time.deltaTime;
        }
        else
        {
            _state = CombatState.Jump;

            enemyManager.Shadow.SetActive(false);
            enemyManager.Shadow.transform.localScale = Vector3.one;
            _timerTargetFollow = 0;
        }
    }

    private void Jump(EnemyManager enemyManager)
    {
        _timeAnimationCurve += Time.deltaTime;

        enemyManager.transform.position = enemyManager.Shadow.transform.position;

        Vector3 pos = enemyManager.transform.position;
        pos.y = enemyManager.JumpCurve.Evaluate(_timeAnimationCurve);
        enemyManager.transform.position = pos;

        #region animation curve
        _lastKey = enemyManager.JumpCurve[enemyManager.JumpCurve.length - 1];

        // rotation y
        Vector3 rotation = enemyManager.transform.eulerAngles;

        if (_timeAnimationCurve > _lastKey.time / enemyManager.ValueBeforeLastKeyFrameInCurve)
        {
            rotation.z = -90;
            enemyManager.transform.eulerAngles = rotation;
        }
        else
        {
            rotation.z = 90;
            enemyManager.transform.eulerAngles = rotation;
        }

        // end of the jump
        if (_timeAnimationCurve >= _lastKey.time)
        {
            _state = CombatState.RotateAfterJump;
            _timerToPassUnderPlayer = 0;
            _timeAnimationCurve = 0;
        }
        #endregion
    }


    #endregion
}
