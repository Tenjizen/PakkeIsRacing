using System;
using System.Collections;
using System.Collections.Generic;
using Art.Script;
using Art.Test.Dissolve;
using Character.Data.Character;
using Character.State;
using Fight;
using Kayak;
using SceneTransition;
using UI;
using UI.Menu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Character
{
    [Serializable]
    public class PlayerStatsMultipliers
    {
        public float BreakingDistanceMultiplier = 1;
        public float MaximumSpeedMultiplier = 1;
        public float RotationSpeedMultiplier = 1;
        public float UnbalancedThresholdMultiplier = 1;
    }

    public class CharacterManager : MonoBehaviour
    {
        #region Properties

        public CharacterMultiPlayerManager Character;

        [field: SerializeField] public CharacterStateBase CurrentStateBaseProperty { get; private set; }
        [field: SerializeField] public KayakController KayakControllerProperty { get; private set; }
        [field: SerializeField] public InputManagement InputManagementProperty { get; private set; }
        [field: SerializeField] public Animator PaddleAnimatorProperty { get; private set; }

        [field: SerializeField] public Animator CharacterAnimatorProperty { get; private set; }

        //[field: SerializeField] public TransitionManager TransitionManagerProperty { get; private set; }
        //[field: SerializeField] public NotificationsController NotificationsUIController { get; private set; }
        [field: SerializeField] public BalanceGaugeManager BalanceGaugeManagerRef { get; private set; }
        [field: SerializeField] public ExperienceManager ExperienceManagerProperty { get; private set; }
        [field: SerializeField] public IKControl IKPlayerControl { get; private set; }
        [field: SerializeField] public PlayerParameters Parameters { get; set; }
        [field: SerializeField] public PlayerAbilities Abilities { get; set; }

        [field: SerializeField, Header("Sprint")]
        public UISprintManager SprintUIManager { get; private set; }

        public IsInCameraView InCam;

        #endregion

        [Header("Bump")]
        [SerializeField] private Bump _bumpPrefab;
        [SerializeField] private float _bumpTimer;
        [SerializeField] private Collider _collider;
        
        [Header("Character Data")] public CharacterData Data;
        [Range(0, 360)] public float BaseOrientation;

        [Header("Balance Infos"), ReadOnly, Tooltip("Can the balance lerp itself to 0 ?")]
        public bool LerpBalanceTo0 = true;

        [ReadOnly, Tooltip("The current balance value")]
        public float Balance = 0f;

        [ReadOnly, Tooltip("Reset at last checkpoint in menu")]
        public bool RespawnLastCheckpoint = false;

        [Tooltip("The timer"), ReadOnly] public float TimerUnbalanced = 0;

        [Tooltip("The number of times the button has been pressed"), ReadOnly]
        public int NumberButtonIsPressed = 0;

        [Header("VFX")] public ParticleSystem SplashLeft;
        public ParticleSystem SplashRight;

        [Header("Events")] public UnityEvent StartGame;
        public UnityEvent OnPaddle;
        public UnityEvent OnEnterSprint;
        public UnityEvent OnStopSprint;

        [HideInInspector] public float InvincibilityTime;
        [HideInInspector] public bool IsGameLaunched;

        [ReadOnly] public bool SprintInProgress = false;
        [ReadOnly] public bool InWaterFlow = false;

        public PlayerStatsMultipliers PlayerStats;

        protected void Awake()
        {
            PlayerStats = new PlayerStatsMultipliers();

            Cursor.visible = false;
        }

        private void Start()
        {
            CharacterNavigationState navigationState = new CharacterNavigationState(Character);
            CurrentStateBaseProperty = navigationState;
            CurrentStateBaseProperty.Initialize();

            CurrentStateBaseProperty.EnterState(this);
            //BalanceGaugeManagerRef.SetBalanceGaugeActive(false);
            //ExperienceManagerProperty.ExperienceUIManagerProperty.SetActive(false);
            //BalanceGaugeManagerRef.ShowTrigger(false, false);

            //rotate kayak
            Transform kayakTransform = KayakControllerProperty.transform;
            kayakTransform.eulerAngles = new Vector3(0, BaseOrientation, 0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) SetCanMove(!CurrentStateBaseProperty.CanBeMoved);


            CurrentStateBaseProperty.UpdateState(this);

            if (CurrentStateBaseProperty.IsDead == false)
            {
                BalanceManagement();
            }

            ManageInvincibilityBalance();

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

        public void SetCanMove(bool value)
        {
            CurrentStateBaseProperty.CanBeMoved = value;
        }
        private bool _canBump = true;

        public void CreateBump()
        {
            if (_canBump)
            {
                Bump bump = Instantiate(_bumpPrefab, transform.position, Quaternion.identity);
                bump.Explode(_collider);
                _canBump = false;
                StartCoroutine(WaitToBumpAgain());
            }
        }

        private IEnumerator WaitToBumpAgain()
        {
            yield return new WaitForSeconds(_bumpTimer);
            _canBump = true;
        }

        /// <summary>
        /// Lerp the Balance value to 0 
        /// </summary>
        private void BalanceManagement()
        {
            //if (CurrentStateBaseProperty.IsDead)
            //{
            //    return;
            //}

            //InvincibilityTime -= Time.deltaTime;

            //if (LerpBalanceTo0)
            //{
            //    Balance = Mathf.Lerp(Balance, 0, Data.BalanceLerpTo0Value);
            //}

            //if (Balance >= 0)
            //{
            //    float function = Mathf.Pow(Balance, 2) - (((float)NumberButtonIsPressed / (float)Data.NumberPressButton) * 10) * (Mathf.Pow(Balance, 2) / 10);
            //    BalanceGaugeManagerRef.SetBalanceCursor(function);
            //}
            //else if (Balance < 0)
            //{
            //    //Change of sign
            //    float function = -Mathf.Pow(Balance, 2) + (((float)NumberButtonIsPressed / (float)Data.NumberPressButton) * 10) * (Mathf.Pow(Balance, 2) / 10);
            //    BalanceGaugeManagerRef.SetBalanceCursor(function);
            //}
        }

        /// <summary>
        /// Set the current balance value multiplied by the sign of it
        /// </summary>
        /// <param name="value">the (float)value to add</param>
        public void SetBalanceValueToCurrentSide(float value)
        {
            //float sign = Mathf.Sign(Balance);
            //Balance = value * sign;
        }

        /// <summary>
        /// Add to the current balance value
        /// </summary>
        /// <param name="value">the (float)value to add</param>
        public void AddBalanceValueToCurrentSide(float value)
        {
            //float sign = Mathf.Sign(Balance);
            //Balance += value * sign;
        }

        public void AddBalanceValueToCurrentSide(double value)
        {
            AddBalanceValueToCurrentSide((float)value);
        }

        private void ManageInvincibilityBalance()
        {
            if (InvincibilityTime < 0 && KayakControllerProperty.Rigidbody.freezeRotation == false)
                return;

            //if (InvincibilityTime >= 0)
            //{
            //    Balance = 0;
            //    KayakControllerProperty.Rigidbody.freezeRotation = true;
            //}
            //else if (KayakControllerProperty.Rigidbody.freezeRotation == true)
            //{
            //    KayakControllerProperty.Rigidbody.freezeRotation = false;
            //}
        }

        public void SendDebugMessage(string message)
        {
            Debug.Log(message);
        }
    }

    [Serializable]
    public struct PlayerParameters
    {
        public bool InversedControls;
        public bool Language;
    }

    [Serializable]
    public struct PlayerAbilities
    {
        public bool SprintUnlock;
    }
}