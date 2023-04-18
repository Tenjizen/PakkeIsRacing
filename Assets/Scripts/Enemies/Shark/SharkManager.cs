using Character;
using Fight;
using GPEs;
using Kayak;
using Sound;
using UnityEngine;
using WaterAndFloating;

namespace Enemies.Shark
{
    public class SharkManager : MonoBehaviour, IHittable
    {
        [field: SerializeField] public SharkBaseState CurrentStateBase { get; private set; }
        [field: SerializeField, ReadOnly] public Transform TargetTransform { get; private set; }
        [field: SerializeField] public GameObject ParentGameObject { get; private set; }
        [field: SerializeField] public GameObject Forward { get; private set; }

        public GameObject PointTarget;
        public GameObject PointTargetAttack;

        //TODO to scriptable object
        [Header("Life")] public float Life = 3;
        [ReadOnly] public float CurrentLife;
        [Header("Hit")] public Collider SharkCollider;
        //public float TimeStartHittable = 1;
        //public float TimeEndHittable = 4;

        [field: SerializeField, Header("VFX")] public ParticleSystem HitParticles { get; private set; }
        [field: SerializeField, Header("Sound")] public AudioClip HitSound { get; private set; }
        public KayakController KayakControllerProperty { get; set; }


        //TODO to scriptable object
        [Tooltip("The Y position in relation to the target, the lower the value (ex : -1) the more visible the shark will be when rotating around the player")]
        public float ElevationOffset = 0;
        [Tooltip("The Y position in relation to the target when rushing to the target")]
        public float ElevationOffsetWhenRush = 0;

        [Header("Jump")]
        [Tooltip("The jump curve")]
        public AnimationCurve JumpCurve;
        [Tooltip("The visual curve to do according to the jump curve")]
        public AnimationCurve VisualCurve;

        [Header("Waves")]
        public Waves WavesData;
        [Tooltip("The time before the last key fram before launching the first wave")]
        public float StartFirstCircularWave = 1;
        [Tooltip("Information for the first wave")]
        public CircularWave StartFirstCircularWaveData;
        [Tooltip("The time before the last key fram before launching the second wave")]
        [Space(15)] public float StartSecondCircularWaveTime = 1;
        [Tooltip("Information for the second wave")]
        public CircularWave StartSecondCircularWaveData;

        [Header("Speed")]
        //public float RotationStaticSpeed = 100.0f;
        [Tooltip("the speed of static rotation when free roam")]
        public float SpeedFreeRoamRotationStatic = 70.0f;
        [Tooltip("the speed of static rotation when fighting")]
        public float SpeedCombatRotationStatic = 100.0f;

        [Tooltip("the speed of rotation when moving around target in free roam")]
        public float SpeedFreeRoamRotationAroundPoint = 10.0f;
        [Tooltip("the speed of rotation when moving around target in fighting")]
        public float SpeedCombatRotationAroundPoint = 20.0f;
        [Tooltip("the speed when moving to the target")]
        public float SpeedToMoveToTarget = 20.0f;
        [Tooltip("the speed when moving to the target")]
        public float SpeedWhenOutOfRange = 30.0f;


        [Header("Distance")]
        [Tooltip("the distance maximun between the shark and the target")]
        public float MaxDistanceBetweenTarget = 80.0f;
        [Tooltip("the maximum distance between the shark and the target when rotate around before the shark returns to the target")]
        public float MaxDistanceBetweenPointInCombat = 80.0f;
        [Tooltip("the minimum distance between the shark and the target when rotate around")]
        public float MinDistanceBetweenTargetWhenRotate = 50.0f;
        [Tooltip("the distance has to trigger the atq of the jump in front of the player")]
        public float MaxDistanceTriggerBetweenPointAndShark = 15.0f;
        [Tooltip("the distance between the shark and the target before the shark stops focusing precisely on the target ")]
        public float DistSharkTargetPointStopMoving = 20.0f;

        [Tooltip("When moving the distance between the player and target")]
        public float MutliplySpeed = 1.0f;


        [Header("Attack")]
        [Tooltip("Timer between end attack and start attacks")]
        public float TimerForAttack = 10.0f;
        [Tooltip("When jump attack rotate speed on itself")]
        public float SpeedRotationOnItself = 500.0f;
        [Tooltip("Number of jump before returning rotate around target")]
        public int NumberJumpWhenCrazy = 3;
        [Tooltip("Speed when rush the target")]
        public float SpeedCombatRush = 20.0f;
        [Tooltip("Speed when jump in front of the target")]
        public float SpeedCombatJumpInFront = 15.0f;
        [Tooltip("Speed when rush for trigger the jump in front")]
        public float SpeedCombatToTriggerJumpInFront = 35.0f;


        [ReadOnly] public float CurrentSpeed;
        [ReadOnly] public bool IsCollided;

        private void Awake()
        {
            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            CurrentStateBase = sharkFreeRoamState;

            CurrentLife = Life;
        }

        private void Start()
        {
            KayakControllerProperty = CharacterManager.Instance.KayakControllerProperty;
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


        public void SwitchState(SharkBaseState stateBaseCharacter)
        {
            CurrentStateBase = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }
        public void OnEnter()
        {
            if (TargetTransform == null)
            {
                TargetTransform = GetComponent<PlayerTriggerManager>().PropKayakController.gameObject.GetComponent<Transform>();
                SharkCombatState sharkCombatState = new SharkCombatState();
                SwitchState(sharkCombatState);
            }
        }

        public void OnExit()
        {
            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            SwitchState(sharkFreeRoamState);
            TargetTransform = null;
        }


        public void Hit(Projectile projectile, GameObject owner)
        {

            CurrentLife -= 1;
            //Life -= projectile.Data.Damage

            if (HitParticles != null)
            {
                HitParticles.transform.parent = null;
                HitParticles.Play();
            }
            //CharacterManager.Instance.SoundManagerProperty.PlaySound(HitSound);

            if (CurrentLife <= 0)
                Destroy(ParentGameObject.gameObject);
        }





        public void SwitchSpeed(float speed)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, speed, 0.05f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<KayakController>() == true && IsCollided == false)
            {
                IsCollided = true;
                CharacterManager.Instance.AddBalanceValueToCurrentSide(8.5f);
                //SharkCollider.enabled = false;
                Debug.Log("collision");
            }
        }
    }
}
