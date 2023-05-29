using System;
using Art.Script;
using Character.Camera;
using Character.Data.Character;
using Character.State;
using Fight;
using GPEs.Checkpoint;
using Kayak;
using SceneTransition;
using Sedna;
using Tools.SingletonClassBase;
using UI;
using UI.Menu;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        #region Properties

        [field: SerializeField] public CharacterStateBase CurrentStateBaseProperty { get; private set; }
        [field: SerializeField, Header("Properties")] public CameraManager CameraManagerProperty { get; private set; }
        [field: SerializeField] public KayakController KayakControllerProperty { get; private set; }
        [field: SerializeField] public InputManagement InputManagementProperty { get; private set; }
        [field: SerializeField] public SednaManager SednaManagerProperty { get; private set; }
        [field: SerializeField] public Animator PaddleAnimatorProperty { get; private set; }
        [field: SerializeField] public Animator CharacterAnimatorProperty { get; private set; }
        [field: SerializeField] public TransitionManager TransitionManagerProperty { get; private set; }
        [field: SerializeField] public WeaponUIManager WeaponUIManagerProperty { get; private set; }
        [field: SerializeField] public UIEnemyManager EnemyUIManager { get; private set; }
        [field: SerializeField] public BalanceGaugeManager BalanceGaugeManagerRef { get; private set; }
        [field: SerializeField] public CheckpointManager CheckpointManagerProperty { get; private set; }
        [field: SerializeField] public MonoBehaviour CharacterMonoBehaviour { get; private set; }
        [field: SerializeField] public ExperienceManager ExperienceManagerProperty { get; private set; }
        [field: SerializeField] public Transform WeaponSpawnPosition { get; private set; }
        [field: SerializeField] public IKControl IKPlayerControl { get; private set; }
        [field: SerializeField] public PlayerParameters Parameters { get; set; }
        [field: SerializeField] public OptionMenuManager OptionMenuManager { get; private set; }
        [field: SerializeField] public ScriptForDebug ScriptDebug { get; private set; }
        [field: SerializeField, Header("Animation")] public Animator HarpoonAnimator { get; private set; }
        [field: SerializeField] public Animator NetAnimator { get; private set; }

        #endregion

        [Header("Character Data")]
        public CharacterData Data;
        [Header("Balance Infos"), ReadOnly, Tooltip("Can the balance lerp itself to 0 ?")]
        public bool LerpBalanceTo0 = true;
        [ReadOnly, Tooltip("The current balance value")]
        public float Balance = 0f;
        [ReadOnly, Tooltip("Reset at last checkpoint in menu")]
        public bool RespawnLastCheckpoint = false;
        [Tooltip("The timer"), ReadOnly]
        public float TimerUnbalanced = 0;
        [Tooltip("The number of times the button has been pressed"), ReadOnly]
        public int NumberButtonIsPressed = 0;
        [ReadOnly]
        public Projectile CurrentProjectile;
        [Header("VFX")]
        public ParticleSystem WeaponChargedParticleSystem;

        [Header("Events")] public UnityEvent StartGame;
        public UnityEvent OnPaddle;
        
        [HideInInspector] public float WeaponCooldown;
        [HideInInspector] public float WeaponCooldownBase;
        [HideInInspector] public float InvincibilityTime;
        [HideInInspector] public bool ProjectileIsInAir;

        protected override void Awake()
        {
            base.Awake();
            CharacterMonoBehaviour = this;
        }

        private void Start()
        {
            CharacterNavigationState navigationState = new CharacterNavigationState();
            CurrentStateBaseProperty = navigationState;
            CurrentStateBaseProperty.Initialize();

            CurrentStateBaseProperty.EnterState(this);

            BalanceGaugeManagerRef.SetBalanceGaugeActive(false);
            ExperienceManagerProperty.ExperienceUIManagerProperty.SetActive(false);
            BalanceGaugeManagerRef.ShowTrigger(false, false);
        }
        private void Update()
        {
            CurrentStateBaseProperty.UpdateState(this);

            if (CurrentStateBaseProperty.IsDead == false)
            {
                BalanceManagement();
                ManageWeaponCooldown();
            }

            //anim
            if (IKPlayerControl.CurrentType != IKType.Paddle || IKPlayerControl.Type == IKType.Paddle)
            {
                return;
            }
            CurrentStateBaseProperty.TimeBeforeSettingPaddleAnimator -= Time.deltaTime;
            if (CurrentStateBaseProperty.TimeBeforeSettingPaddleAnimator <= 0)
            {
                IKPlayerControl.SetPaddle();
            }
        }
        private void FixedUpdate()
        {
            CurrentStateBaseProperty.FixedUpdate(this);
        }
        public void SwitchState(CharacterStateBase stateBaseCharacter)
        {
            CurrentStateBaseProperty.ExitState(this);
            CurrentStateBaseProperty = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }

        /// <summary>
        /// Lerp the Balance value to 0 
        /// </summary>
        private void BalanceManagement()
        {
            if (CurrentStateBaseProperty.IsDead)
            {
                return;
            }

            InvincibilityTime -= Time.deltaTime;
            
            if (LerpBalanceTo0)
            {
                Balance = Mathf.Lerp(Balance, 0, Data.BalanceLerpTo0Value);
            }

            if (Balance >= 0)
            {
                float function = Mathf.Pow(Balance, 2) - (((float)NumberButtonIsPressed / (float)Data.NumberPressButton) * 10) * (Mathf.Pow(Balance, 2) / 10);
                BalanceGaugeManagerRef.SetBalanceCursor(function);
            }
            else if (Balance < 0)
            {
                //Change of sign
                float function = -Mathf.Pow(Balance, 2) + (((float)NumberButtonIsPressed / (float)Data.NumberPressButton) * 10) * (Mathf.Pow(Balance, 2) / 10);
                BalanceGaugeManagerRef.SetBalanceCursor(function);
            }
        }

        /// <summary>
        /// Set the current balance value multiplied by the sign of it
        /// </summary>
        /// <param name="value">the (float)value to add</param>
        public void SetBalanceValueToCurrentSide(float value)
        {
            float sign = Mathf.Sign(Balance);
            Balance = value * sign;
        }

        /// <summary>
        /// Add to the current balance value
        /// </summary>
        /// <param name="value">the (float)value to add</param>
        public void AddBalanceValueToCurrentSide(float value)
        {
            float sign = Mathf.Sign(Balance);
            Balance += value * sign;
        }
        public void AddBalanceValueToCurrentSide(double value)
        {
            AddBalanceValueToCurrentSide((float)value);
        }

        private void ManageWeaponCooldown()
        {
            if (ProjectileIsInAir == false && WeaponCooldown > 0)
            {
                WeaponCooldown -= Time.deltaTime;
                float value = WeaponCooldown / WeaponCooldownBase;
                WeaponUIManagerProperty.SetCooldownUI(value);
            }
        }

        public void SendDebugMessage(string message)
        {
            Debug.Log(message);
        }

        #region GUI

        private void OnGUI()
        {
#if UNITY_EDITOR
            GUI.skin.label.fontSize = 30;

            GUI.color = Color.white;
            GUI.Label(new Rect(10, 10, 500, 100), "Balance : " + Math.Round(Balance, 1));
#endif
        }

        #endregion

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            UnityEngine.Camera mainCamera = UnityEngine.Camera.main;

            Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
            Vector3 rayDirection = mainCamera.ViewportPointToRay(screenCenter).direction;
            Ray ray = new Ray(mainCamera.transform.position, rayDirection);

            Color gizmoColor = new Color(1f, 0.36f, 0.24f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Gizmos.color = gizmoColor;

                Gizmos.DrawLine(CameraManagerProperty.VirtualCameraCombat.transform.position, hit.point);
                Gizmos.DrawSphere(hit.point, 1f);
            }

            Transform camera = UnityEngine.Camera.main.transform;
            for (int i = 0; i < Data.AutoAimNumberOfCastStep; i++)
            {
                break;
                float positionMultiplier = Mathf.Clamp((Data.AutoAimDistanceBetweenEachStep * i), 1, 10000);
                Vector3 newPosition = camera.position + camera.forward * positionMultiplier;
                float radiusMultiplier = Mathf.Clamp(Vector3.Distance(camera.position, newPosition) / 5, 1, 10000);
                float radius = Data.AutoAimSize * radiusMultiplier;
                Gizmos.DrawWireSphere(newPosition, radius);
            }
        }

#endif
    }

    [Serializable]
    public struct PlayerParameters
    {
        public bool AutoAim;
        public bool InversedControls;
        public bool Language;
    }
}
