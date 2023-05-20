using UnityEngine;
using UnityEngine.Serialization;
using WaterAndFloating;

namespace Shark.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SharkData", order = 1)]
    public class SharkData : ScriptableObject
    {
        [field: SerializeField, Header("Life")] 
        public float Life = 3;
        [field: SerializeField, Header("Elevation"), Tooltip("The Y position in relation to the target, the lower the value (ex : -1) the more visible the shark will be when rotating around the player")]
        public float ElevationOffset { get; private set; } = 0;
        [field: SerializeField, Tooltip("The Y position in relation to the target when rushing to the target")]
        public float ElevationOffsetWhenRush { get; private set; } = 0;


        [field: SerializeField, Header("FreeRoam"), Tooltip("the minimum distance between the shark and the target when rotate around in free roam")]
        public float MinDistanceBetweenPointFreeRoam { get; private set; } = 30.0f;
        [field: SerializeField, Tooltip("the speed of static rotation when free roam")]
        public float SpeedFreeRoamRotationStatic { get; private set; } = 70.0f;
        [field: SerializeField, Tooltip("the speed of rotation when moving around target in free roam")]
        public float SpeedFreeRoamRotationAroundPoint { get; private set; } = 10.0f;


        [Header("Jump")]
        [Header("Attack one")]
        [field: SerializeField, Tooltip("The jump curve")]
        public AnimationCurve JumpCurve;
        [field: SerializeField, Tooltip("The visual curve to do according to the jump curve")]
        public AnimationCurve VisualCurve;

        [Header("Attack three")]
        [field: SerializeField, Tooltip("The jump curve")]
        public AnimationCurve JumpCurvePhaseThree;
        [field: SerializeField, Tooltip("The visual curve to do according to the jump curve")]
        public AnimationCurve VisualCurvePhaseThree;


        [field: SerializeField, Header("Waves Attack one"), Tooltip("The time before the last key fram before launching the first wave")]
        public float StartFirstCircularWave { get; private set; } = 1;
        [field: SerializeField, Tooltip("Information for the first wave")]
        public CircularWave StartFirstCircularWaveData;
        [field: SerializeField, Space(5), Tooltip("The time before the last key fram before launching the second wave")]
        public float StartSecondCircularWaveTime { get; private set; } = 1;
        [field: SerializeField, Tooltip("Information for the second wave")]
        public CircularWave StartSecondCircularWaveData;


        [field: SerializeField, Header("Waves Attack three"), Tooltip("The time before the last key fram before launching the first wave")]
        public float StartFirstCircularWavePhaseThree { get; private set; } = 1;
        [field: SerializeField, Tooltip("Information for the first wave")]
        public CircularWave StartFirstCircularWaveDataPhaseThree;
        [field: SerializeField, Space(5), Tooltip("The time before the last key fram before launching the second wave")]
        public float StartSecondCircularWaveTimePhaseThree { get; private set; } = 1;
        [field: SerializeField, Tooltip("Information for the second wave")]
        public CircularWave StartSecondCircularWaveDataPhaseThree;

        [field: SerializeField, Header("Speed"), Tooltip("the speed of static rotation when fighting")]
        public float SpeedCombatRotationStatic { get; private set; } = 100.0f;

        [field: SerializeField, Tooltip("the speed of rotation when moving around target in fighting")]
        public float SpeedCombatRotationAroundPoint { get; private set; } = 20.0f;
        [field: SerializeField, Tooltip("the speed when moving to the target")]
        public float SpeedToMoveToTarget { get; private set; } = 20.0f;
        [field: SerializeField, Tooltip("the speed when moving to the target")]
        public float SpeedWhenOutOfRange { get; private set; } = 30.0f;
        [field: SerializeField, Tooltip("When moving the distance between the player and target")]
        public float MutliplySpeed { get; private set; } = 1.0f;


        [field: SerializeField, Header("Distance"), Tooltip("the distance maximun between the shark and the target")]
        public float MaxDistanceBetweenTarget { get; private set; } = 80.0f;
        [field: SerializeField, Tooltip("the distance has to trigger the atq of the jump in front of the player")]
        public float MaxDistanceTriggerBetweenPointAndShark { get; private set; } = 15.0f;
        [field: SerializeField, Tooltip("the distance between the shark and the target before the shark stops focusing precisely on the target ")]
        public float DistSharkTargetPointStopMoving { get; private set; } = 20.0f;


        [field: SerializeField, Header("Distance Attack one"), Tooltip("the maximum distance between the shark and the target when rotate around before the shark returns to the target")]
        public float MaxDistanceBetweenPointInCombatPhaseOne { get; private set; } = 80.0f;
        [field: SerializeField, Tooltip("the minimum distance between the shark and the target when rotate around")]
        public float MinDistanceBetweenTargetWhenRotatePhaseOne { get; private set; } = 50.0f;


        [field: SerializeField, Header("Distance Attack two"), Tooltip("the maximum distance between the shark and the target when rotate around before the shark returns to the target")]
        public float MaxDistanceBetweenPointInCombatPhaseTwo { get; private set; } = 80.0f;
        [field: SerializeField, Tooltip("the minimum distance between the shark and the target when rotate around")]
        public float MinDistanceBetweenTargetWhenRotatePhaseTwo { get; private set; } = 50.0f;


        [field: SerializeField, Header("Distance Attack three"), Tooltip("the maximum distance between the shark and the target when rotate around before the shark returns to the target")]
        public float MaxDistanceBetweenPointInCombatPhaseThree { get; private set; } = 80.0f;
        [field: SerializeField, Tooltip("the minimum distance between the shark and the target when rotate around")]
        public float MinDistanceBetweenTargetWhenRotatePhaseThree { get; private set; } = 50.0f;



        [field: SerializeField, Header("Attack"), Tooltip("Timer between end attack and start attacks")]
        public float TimerForAttack { get; private set; } = 10.0f;
        [field: SerializeField, Tooltip("When jump attack rotate speed on itself")]
        public float SpeedRotationOnItself { get; private set; } = 500.0f;
        [field: SerializeField, Tooltip("Number of jump before returning rotate around target")]
        public int NumberJumpWhenCrazy { get; private set; } = 3;
        [field: SerializeField, Tooltip("Speed when rush the target")]
        public float SpeedCombatRush { get; private set; } = 20.0f;
        [field: SerializeField, Tooltip("Speed when jump in front of the target")]
        public float SpeedCombatJumpInFront { get; private set; } = 15.0f;
        [field: SerializeField, Tooltip("Speed when rush for trigger the jump in front")]
        public float SpeedCombatToTriggerJumpInFront { get; private set; } = 35.0f;

        [field: SerializeField] public float DistanceAtWhichPlayerDisableUI { get; private set; } = 50;
    }
}