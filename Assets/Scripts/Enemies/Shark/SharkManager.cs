using Character;
using Fight;
using GPEs;
using Kayak;
using Shark.Data;
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
        public GameObject VisualPossessed;
        [ReadOnly] public bool IsPossessed = true;
        [ReadOnly] public float CurrentLife;
        [ReadOnly] public float CurrentSpeed;
        [ReadOnly] public bool IsCollided;

        [field: SerializeField, Header("VFX")] public ParticleSystem HitParticles { get; private set; }
        [field: SerializeField, Header("Sound")] public AudioClip HitSound { get; private set; }
        public KayakController KayakControllerProperty { get; set; }
        [Header("Hit")] public Collider SharkCollider;

        [Header("Waves")]
        public Waves WavesData;

        [Space(5), Header("Shark Data")] public SharkData Data;

        
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
            if (TargetTransform == null && IsPossessed == true)
            {
                TargetTransform = GetComponentInParent<PlayerTriggerManager>().PropKayakController.gameObject.GetComponent<Transform>();
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
            {
                VisualPossessed.SetActive(false);
                OnExit();
                //Destroy(ParentGameObject.gameObject);
            }
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
