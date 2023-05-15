using Character;
using Fight;
using GPEs;
using Kayak;
using Shark.Data;
using Sound;
using UnityEngine;
using WaterAndFloating;
using Character.Camera;
using UnityEngine.Events;

namespace Enemies.Shark
{
    public class SharkManager : Enemy
    {
        [field:SerializeField] public UIEnemy UIEnemyManager { get; private set; }
        [field: SerializeField] public SharkBaseState CurrentStateBase { get; private set; }
        [field: SerializeField, ReadOnly] public Transform TargetTransform { get; private set; }
        [field: SerializeField] public PlayerTriggerManager PlayerTriggerManager { get; private set; }
        [field: SerializeField] public GameObject ParentGameObject { get; private set; }
        [field: SerializeField] public GameObject Forward { get; private set; }
        [field: SerializeField] public GameObject Circle { get; private set; }

        public GameObject PointTarget;
        public GameObject PointTargetAttack;
        public GameObject VisualPossessed;
        [ReadOnly] public bool IsPossessed = true;
        [ReadOnly] public float CurrentSpeed;
        [ReadOnly] public bool IsCollided;

        [field: SerializeField, Header("VFX")] public ParticleSystem HitParticles { get; private set; }
        [field: SerializeField] public GameObject PosParticles { get; private set; }

        [field: SerializeField, Header("Sound")] public AudioClip HitSound { get; private set; }
        public KayakController KayakControllerProperty { get; set; }

        [Header("Hit")] public Collider SharkCollider;

        [Header("Waves")] public Waves WavesData;

        [Header("feedback tempo"), Tooltip("depth at which the feedback circle pop")] //TODO modify
        public float ShowCircleDepth = -14f;
        [Tooltip("distance at which the circle is from the shark in phase one")] //TODO modify
        public float DistanceInFrontOfShark = 5;
        [Tooltip("distance at which the circle is from the shark in side in phase one")] //TODO modify
        public float DistanceSideOfShark = 1;
        [Tooltip("distance at which the circle is from the shark in phase three")] //TODO modify
        public float DistanceInFrontOfSharkPhaseThree = 5;


        [Space(5), Header("Shark Data")] public SharkData Data;

        
        [Header("Events")] public UnityEvent StartJump;
        public UnityEvent EndJump;

        private void Awake()
        {
            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            CurrentStateBase = sharkFreeRoamState;
            CurrentLife = Data.Life;
        }

        private void Start()
        {
            KayakControllerProperty = CharacterManager.Instance.KayakControllerProperty;
            CurrentStateBase.EnterState(this);
            //UIEnemyManager.SetGauge(CurrentLife, Data.Life);
        }
        Vector3 collision = Vector3.zero;
        public LayerMask LayerMask;
        private void Update()
        {
            CurrentStateBase.UpdateState(this);

            if (PosParticles != null)
            {
                //if (PosParticles.isPlaying == false)
                //{
                //PosParticles.transform.parent = null;
                var posPartic = transform.position;
                posPartic.y = WavesData.GetHeight(transform.position);
                PosParticles.transform.position = posPartic;
                //PosParticles.Play();
                //}
            }

            AvoidObstacle();
            CircleUI();

        }
        private void FixedUpdate()
        {
            CurrentStateBase.FixedUpdate(this);

        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(collision, 2f);
        }

        public void SwitchState(SharkBaseState stateBaseCharacter)
        {
            CurrentStateBase = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }
        public void OnEnter()
        {
            if (TargetTransform == null && IsPossessed == true)
            {
                TargetTransform = GetComponentInParent<PlayerTriggerManager>().PropKayakController.gameObject.GetComponent<Transform>();
                SharkCombatState sharkCombatState = new SharkCombatState();
                SwitchState(sharkCombatState);
            }
        }

        public void FreeRoamState()
        {
            PointTarget.transform.localPosition = new Vector3(0, 0, 0);
            SharkFreeRoamState sharkFreeRoamState = new SharkFreeRoamState();
            SwitchState(sharkFreeRoamState);
            TargetTransform = null;
        }


        public override void Hit(Projectile projectile, GameObject owner)
        {
            base.Hit(projectile,owner);
            //Life -= projectile.Data.Damage

            if (HitParticles != null)
            {
                HitParticles.transform.parent = null;
                HitParticles.Play();

            }

            UIEnemyManager.SetGauge(CurrentLife, Data.Life);

            //CharacterManager.Instance.SoundManagerProperty.PlaySound(HitSound);

            if (CurrentLife <= 0)
            {
                VisualPossessed.SetActive(false);
                PlayerTriggerManager.enabled = false;
                IsPossessed = false;
                PointTarget.transform.localPosition = new Vector3(0, 0, 0);
            }
        }
        
        public void CircleUI()
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
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<KayakController>() == true && IsCollided == false && IsPossessed == true)
            {
                IsCollided = true;
                CharacterManager.Instance.AddBalanceValueToCurrentSide(8.5f);
                //SharkCollider.enabled = false;
                Debug.Log("collision");

            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.GetComponent<KayakController>() == true && IsCollided == false && IsPossessed == true)
            {
                IsCollided = true;
                CharacterManager.Instance.AddBalanceValueToCurrentSide(8.5f);
                //SharkCollider.enabled = false;
                Debug.Log("collision");

            }
        }

        private void AvoidObstacle()
        {
            //avoid iceberg
            Ray ray = new Ray(Forward.transform.position, Forward.transform.forward);
            //Ray ray = new Ray(this.transform.position, transform.forward);
            RaycastHit hit;
            //if(Physics.Raycast(ray, out hit, 200))
            if (Physics.Raycast(ray, out hit, 15))
            {
                if (hit.collider.gameObject.GetComponent<KayakController>() == false)
                {
                    collision = hit.point;

                    Vector2 targetHitPos = new Vector2(hit.transform.position.x, hit.transform.position.z);

                    Vector2 targetPos = new Vector2(PointTarget.transform.position.x, PointTarget.transform.position.z);

                    Vector2 sharkForward = new Vector2(Forward.transform.forward.x, Forward.transform.forward.z);
                    Vector2 sharkPos = new Vector2(Forward.transform.position.x, Forward.transform.position.z);

                    float angleTarget = Vector2.SignedAngle(sharkForward, targetPos - sharkPos);
                    if (angleTarget < 0)
                        angleTarget += 360;

                    Vector3 rotation = Forward.transform.localEulerAngles;


                    float angleHit = Vector2.SignedAngle(sharkForward, targetHitPos - sharkPos);
                    if (angleHit < 0)
                        angleHit += 360;

                    ////var lastRota = rotation.y 

                    if (angleHit <= 90 || angleHit >= 360 - 90)
                    {
                        if (angleTarget >= 0 && angleTarget < 180)
                        {
                            //rotation.y -= Time.deltaTime * Data.SpeedCombatRotationStatic;
                            rotation.y -= Time.deltaTime * 150;
                        }
                        else if (angleTarget >= 180 && angleTarget <= 360)
                        {
                            //rotation.y -= Time.deltaTime * Data.SpeedCombatRotationStatic;
                            rotation.y += Time.deltaTime * 150;
                        }
                    }
                    //apply rota
                    Forward.transform.localEulerAngles = rotation;
                }
            }
        }


        #region CutScene
        public void CutSceneFirstWave()
        {
            Vector2 center = Data.StartFirstCircularWaveDataPhaseThree.Center;

            center.x = Forward.transform.position.x;
            center.y = Forward.transform.position.z;

            Data.StartFirstCircularWaveDataPhaseThree.Center = center;

            WavesData.LaunchCircularWave(Data.StartFirstCircularWaveDataPhaseThree);
        }
        public void CutSceneSecondWave()
        {
            Vector2 center = Data.StartSecondCircularWaveDataPhaseThree.Center;

            center.x = Forward.transform.position.x;
            center.y = Forward.transform.position.z;

            Data.StartFirstCircularWaveDataPhaseThree.Center = center;

            WavesData.LaunchCircularWave(Data.StartSecondCircularWaveDataPhaseThree);
        }
        #endregion
    }
}
