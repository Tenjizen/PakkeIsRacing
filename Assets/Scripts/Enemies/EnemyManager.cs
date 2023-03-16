using GPEs;
using Kayak;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyBaseState CurrentStateBase;

    [ReadOnly] public Transform Target;

    [Header("Rotation")]
    [Tooltip("The speed of rotation")]
    public float RotationSpeed = 1;
    [Tooltip("The minimum distance between the player and the shark")]
    public float RadiusRotateAroundTargetMin = 1;
    [Tooltip("The maximum distance between the player and the shark")]
    public float RadiusRotateAroundTargetMax = 1;
    [Tooltip("The Y position in relation to the target, the lower the value (ex : -1) the more visible the shark will be when rotating around the player")]
    public float ElevationOffset = 0;


    [Header("Timer")]
    [Tooltip("The time it will take the shark to dive")]
    public float TimerToPassUnderPlayer = 2;
    [Tooltip("The time the shark targets the player")]
    public float TimerTargetFollow = 3;


    [Header("Jump")]
    [Tooltip("The jump curve")]
    public AnimationCurve JumpCurve;
    [Tooltip("I'm not sure if it stays but it's for the shark to turn around in the areas")]
    public float ValueBeforeLastKeyFrameInCurve = 2;

    [Header("Dive")]
    [Tooltip("the distance the shark dives before the shadow appears")]
    public float DiveDistance = 20;
    [Tooltip("The speed at which the shark dives/rises")]
    public float DiveSpeed = 0.2f;

    [Header("Shadow")]
    [Tooltip("The GameObject of the shadow")]
    public GameObject Shadow;
    [Tooltip("The maximum size that the shadow takes before the shark jumps")]
    public float ShadowMaxSizeForJump = 3;

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
