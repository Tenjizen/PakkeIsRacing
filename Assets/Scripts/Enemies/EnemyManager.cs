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
    public float CircleRadius = 1;
    public float ElevationOffset = 0;

    public float TimerToJump = 2;

    public float TimerCircleFollow = 3;

    public AnimationCurve jumpCurve;

    public bool Local = true;
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


    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(transform.position, _zoneRadius);
    //}

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
