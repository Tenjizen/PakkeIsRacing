using GPEs;
using Kayak;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyBaseState CurrentStateBase;

    public Transform Target;

    public float RotationSpeed = 1;
    public float RadiusRotateAroundTarget = 1;
    public float ElevationOffset = 0;

    public float TimerToPassUnderPlayer = 2;
    public float TimerToJump = 2;

    public float TimerTargetFollow = 3;

    public AnimationCurve JumpCurve;

    public GameObject Shadow;
    public float ShadowMaxSizeForJump = 3;

    public float DiveDistance = 20;
    public float DiveSpeed = 0.2f;

    [ReadOnly] public Vector3 PositionOffset;
    [ReadOnly] public float Angle;

    private void Awake()
    {
        EnemyCombatState enemyCombatState = new EnemyCombatState();
        CurrentStateBase = enemyCombatState;

        //EnemyFreeRoamState enemyFreeRoamState = new EnemyFreeRoamState();
        //CurrentStateBase = enemyFreeRoamState;
    }

    private void Start()
    {
        CurrentStateBase.EnterState(this);
    }
    private void Update()
    {
        CurrentStateBase.UpdateState(this);
    }
    private void FixedUpdate()
    {
        CurrentStateBase.FixedUpdate(this);
    }
    public void SwitchState(EnemyBaseState stateBaseCharacter)
    {
        CurrentStateBase = stateBaseCharacter;
        stateBaseCharacter.EnterState(this);
    }

    public void OnEnter()
    {
        EnemyCombatState enemyCombatState = new EnemyCombatState();
        SwitchState(enemyCombatState);
        Target = GetComponent<PlayerTriggerManager>().PropKayakController.gameObject.GetComponent<Transform>();
    }

    public void OnExit()
    {
        EnemyFreeRoamState enemyFreeRoamState = new EnemyFreeRoamState();
        SwitchState(enemyFreeRoamState);
        Target = null;
    }
}
