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
        [field:SerializeField] public SharkBaseState CurrentStateBase { get; private set; }
        [field:SerializeField, ReadOnly] public Transform TargetTransform { get; private set; }
        [field:SerializeField] public GameObject ParentGameObject { get; private set; }

        //TODO to scriptable object
        [Header("Life")] public float Life = 3;
        [Header("Hit")] public Collider SharkCollider;
        public float TimeStartHittable = 1;
        public float TimeEndHittable = 4;

        [field:SerializeField, Header("VFX")] public ParticleSystem HitParticles { get; private set; }
        [field:SerializeField, Header("Sound")] public AudioClip HitSound { get; private set; }

        //TODO to scriptable object
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
        [Tooltip("The speed at which the circle will grow (Ex : 1 / ShadowDivideGrowSpeed)")]
        public float ShadowDivideGrowSpeed = 2;
        [ReadOnly] public Vector3 PositionOffset;
        /*[ReadOnly]*/ public float Angle;

        [Header("Waves")]
        public Waves WavesData;
        public CircularWave StartJumpCircularWaveData;
        public CircularWave EndJumpCircularWaveData;
        [Space(5)] public float EndJumpCircularWaveTime = 1;
        [field: SerializeField] public GameObject VisualShark { get; private set; }
        public KayakController KayakControllerProperty { get; set; }

        [ReadOnly] public bool Flip = false;
        public GameObject TargetLerp;
        public GameObject PointTarget;

        public float pierre = 5;
        private void Awake()
        {
            //ColliderShadow.enabled = false;
            //SharkCombatState sharkCombatState = new SharkCombatState();
            //CurrentStateBase = sharkCombatState;

            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            CurrentStateBase = sharkFreeRoamState;
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

            Life -= 1;
            //Life -= projectile.Data.Damage

            HitParticles.transform.parent = null;
            HitParticles.Play();
            CharacterManager.Instance.SoundManagerProperty.PlaySound(HitSound);

            if (Life <= 0)
                Destroy(ParentGameObject.gameObject);
        }


    }
}
