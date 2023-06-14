using Character;
using Fight;
using GPEs;
using Kayak;
using Shark.Data;
using Sound;
using UnityEngine;
using WaterAndFloating;
using Character.Camera;
using Enemies.Shark.State;
using UI;
using UnityEngine.Events;

namespace Enemies.Shark
{
    public class SharkManager : Enemy
    {
        [field: SerializeField] public SharkBaseState CurrentStateBase { get; private set; }
        [field: SerializeField, ReadOnly] public Transform TargetTransform { get; private set; }
        [field:SerializeField ,ReadOnly] public KayakController KayakControllerRef { get; set; }

        [field:SerializeField ] public Animator AnimatorSharkPossessed{ get; set; }
        [field:SerializeField ] public Animator AnimatorShark{ get; set; }


        [field: SerializeField] public PlayerTriggerManager PlayerTriggerManager { get; private set; }
        [field: SerializeField] public GameObject ParentGameObject { get; private set; }
        [field: SerializeField] public GameObject Forward { get; private set; }
        [field: SerializeField] public GameObject Circle { get; private set; }
        [field: SerializeField, Header("VFX")] public ParticleSystem HitParticles { get; private set; }
        [field: SerializeField] public GameObject PosParticles { get; private set; }

        public GameObject PointTarget;
        [ReadOnly] public float CurrentSpeed;
        [ReadOnly] public bool IsCollided;
        
        [Header("Hit")]
        public Collider SharkCollider;
        [Header("Waves")] 
        public Waves WavesData;
        [Header("feedback tempo"), Tooltip("depth at which the feedback circle pop")] 
        public float ShowCircleDepth = -14f;
        [Tooltip("distance at which the circle is from the shark in phase one")]
        public float DistanceInFrontOfShark = 5;
        [Tooltip("distance at which the circle is from the shark in side in phase one")] 
        public float DistanceSideOfShark = 1;
        [Tooltip("distance at which the circle is from the shark in phase three")] 
        public float DistanceInFrontOfSharkPhaseThree = 5;
        
        [Space(5), Header("Shark Data")] 
        public SharkData Data;
        
        [Header("Events")] 
        public UnityEvent StartJump; 
        public UnityEvent EndJump;
        
        private Vector3 _collision = Vector3.zero;
        private Transform _player;

        private void Awake()
        {
            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            CurrentStateBase = sharkFreeRoamState;
            CurrentLife = Data.Life;
            MaxLife = Data.Life;
        }

        private void Start()
        {
            IsPossessed = true;
            CurrentStateBase.EnterState(this);
        }
        
        private void Update()
        {
            CurrentStateBase.UpdateState(this);

            if (PosParticles != null)
            {
                Vector3 particlePosition = transform.position;
                particlePosition.y = WavesData.GetHeight(transform.position);
                PosParticles.transform.position = particlePosition;
            }

            if (_player != null)
            {
                HandlePlayerDistanceToSetUI(_player, Data.DistanceAtWhichPlayerDisableUI);
            }

            AvoidObstacle();
            ManagerCircleUI();

        }
        private void FixedUpdate()
        {
            CurrentStateBase.FixedUpdate(this);
        }

        private void SwitchState(SharkBaseState stateBaseCharacter)
        {
            CurrentStateBase = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }
        
        public void OnEnter()
        {
            if (TargetTransform != null || IsPossessed == false)
            {
                return;
            }

            _player = CharacterManager.Instance.transform;
            TargetTransform = GetComponentInParent<PlayerTriggerManager>().PropKayakController.gameObject.GetComponent<Transform>();
            SharkCombatState sharkCombatState = new SharkCombatState();
            SwitchState(sharkCombatState);
        }

        public void FreeRoamState()
        {
            PointTarget.transform.localPosition = new Vector3(0, 0, 0);
            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            SwitchState(sharkFreeRoamState);
            TargetTransform = null;
        }
        
        public override void Hit(Projectile projectile, GameObject owner, int damage)
        {
            base.Hit(projectile,owner, damage);

            if (projectile.Data.Type != WeaponThatCanHitEnemy || CurrentLife <= 0)
            {
                return;
            }
            
            if (HitParticles != null)
            {
                HitParticles.transform.parent = null;
                HitParticles.Play();
            }
        }

        protected override void Die()
        {
            base.Die();
            
            PlayerTriggerManager.enabled = false;
            PointTarget.transform.localPosition = new Vector3(0, 0, 0);
            
            SetPlayerExperience(CharacterManager.Instance.ExperienceManagerProperty.Data.ExperienceGainedAtEnemyShark);
        }

        private void ManagerCircleUI()
        {
            if (Circle.activeInHierarchy)
            {
                Circle.transform.localScale += Vector3.one * 0.2f;
            }
            else
            {
                Circle.transform.localScale = Vector3.one * 30;
            }
        }

        public void SwitchSpeed(float speed)
        {
            CurrentSpeed = Mathf.Lerp(CurrentSpeed, speed, 0.05f);
        }
        private void OnTriggerEnter(Collider col)
        {
            if (col.GetComponent<KayakController>() == null || IsCollided || IsPossessed != true)
            {
                return;
            }
            
            IsCollided = true;
            CharacterManager.Instance.AddBalanceValueToCurrentSide(8.5f);
        }
        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.GetComponent<KayakController>() == null || IsCollided || IsPossessed != true)
            {
                return;
            }
            
            IsCollided = true;
            CharacterManager.Instance.AddBalanceValueToCurrentSide(8.5f);
        }

        private void AvoidObstacle()
        {
            Ray ray = new Ray(Forward.transform.position, Forward.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 30) == false)
            {
                return;
            }

            if (hit.collider.gameObject.GetComponent<KayakController>() != null)
            {
                return;
            }
            
            _collision = hit.point;
            Vector3 rotation = Forward.transform.localEulerAngles;
                
            Vector3 hitPosition = hit.transform.position;
            Vector3 targetPosition = PointTarget.transform.position;
            Vector3 forwardForward = Forward.transform.forward;
            Vector3 forwardPosition = Forward.transform.position;
                
            Vector2 targetHitPos = new Vector2(hitPosition.x, hitPosition.z);
            Vector2 targetPos = new Vector2(targetPosition.x, targetPosition.z);
            Vector2 sharkForward = new Vector2(forwardForward.x, forwardForward.z);
            Vector2 sharkPos = new Vector2(forwardPosition.x, forwardPosition.z);
            
            float angleTarget = Vector2.SignedAngle(sharkForward, targetPos - sharkPos);
            if (angleTarget < 0)
            {
                angleTarget += 360;
            }

            float angleHit = Vector2.SignedAngle(sharkForward, targetHitPos - sharkPos);
            if (angleHit < 0)
            {
                angleHit += 360;
            }
                    
            if (angleHit <= 90 || angleHit >= 270)
            {
                switch (angleTarget)
                {
                    case >= 0 and < 180:
                        rotation.y -= Time.deltaTime * 150;
                        break;
                    case >= 180 and <= 360:
                        rotation.y += Time.deltaTime * 150;
                        break;
                }
            }
            Forward.transform.localEulerAngles = rotation;
        }

        #region CutScene
        public void CutSceneFirstWave()
        {
            Vector2 center = Data.StartFirstCircularWaveDataPhaseThree.Center;

            Vector3 position = Forward.transform.position;
            center.x = position.x;
            center.y = position.z;

            Data.StartFirstCircularWaveDataPhaseThree.Center = center;

            WavesData.LaunchCircularWave(Data.StartFirstCircularWaveDataPhaseThree);
        }
        public void CutSceneSecondWave()
        {
            Vector2 center = Data.StartSecondCircularWaveDataPhaseThree.Center;

            Vector3 position = Forward.transform.position;
            center.x = position.x;
            center.y = position.z;

            Data.StartFirstCircularWaveDataPhaseThree.Center = center;

            WavesData.LaunchCircularWave(Data.StartSecondCircularWaveDataPhaseThree);
        }
        #endregion
        
        #if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_collision, 2f);
        }
        
        #endif
    }
}
