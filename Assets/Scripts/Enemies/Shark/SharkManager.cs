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

        [Header("Waves")]
        public Waves WavesData;
        public CircularWave StartJumpCircularWaveData;
        public CircularWave EndJumpCircularWaveData;
        [Space(5)] public float EndJumpCircularWaveTime = 1;
        public KayakController KayakControllerProperty { get; set; }

        public GameObject PointTarget;

        public float RotationStaticSpeed = 100.0f;
        public float RotationStaticSpeedFreeRoam = 70.0f;
        public float RotationStaticSpeedCombat = 70.0f;

        public float SpeedToMoveToTarget = 10.0f;

        public float SpeedRotationFreeRoam = 10.0f;
        public float SpeedRotationCombat = 20.0f;

        public float MaxDistanceBetweenTarget = 30.0f;

        public float MinDistanceBetweenPoint = 5.0f;
        public float MaxDistanceBetweenPoint = 15.0f;
        public float MaxDistanceBetweenPointInCombat = 30.0f;



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

        public void MoveToTarget(SharkManager sharkManager)
        {
            Vector2 targetPos = new Vector2(sharkManager.PointTarget.transform.position.x, sharkManager.PointTarget.transform.position.z);
            Vector2 forward = new Vector2(sharkManager.transform.forward.x, sharkManager.transform.forward.z);
            Vector2 sharkPos = new Vector2(sharkManager.transform.position.x, sharkManager.transform.position.z);

            float angle = Vector2.SignedAngle(targetPos - sharkPos, forward);
            if (angle < 0)
                angle += 360;

            Vector3 rota = sharkManager.transform.localEulerAngles;
            if (angle >= 0 && angle <= 180)
            {
                rota.y += Time.deltaTime * RotationStaticSpeed;
            }
            else if (angle < 360 && angle > 180)
            {
                rota.y -= Time.deltaTime * RotationStaticSpeed;
            }

            sharkManager.transform.localEulerAngles = rota;

            var pos = sharkManager.transform.position;
            pos.y = sharkManager.ElevationOffset;
            sharkManager.transform.position = pos;


            sharkManager.transform.Translate(Vector3.forward * SpeedToMoveToTarget * Time.deltaTime, Space.Self);
        }
    }
}
