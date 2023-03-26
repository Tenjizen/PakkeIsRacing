using System;
using Character.Camera;
using Character.State;
using Fight;
using Kayak;
using SceneTransition;
using UI;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Character
{
    public class CharacterManager : MonoBehaviour
    {

        [Header("References")]
        public CharacterStateBase CurrentStateBase;
        public CameraManager CameraManagerRef;
        [Tooltip("Reference of the KayakController script")]
        public KayakController KayakController;
        [Tooltip("Reference of the InputManagement script")]
        public InputManagement InputManagement;
        [Tooltip("Reference of the paddle Animator")]
        public Animator PaddleAnimator;
        [Tooltip("Reference of the TransitionManager script")]
        public TransitionManager TransitionManager;
        [Tooltip("Reference of the WeaponMenuManager script")]
        public WeaponUIManager weaponUIManagerRef;
        [Tooltip("Reference of the BalanceGaugeManager script")]
        public BalanceGaugeManager BalanceGaugeManagerRef;

        [Header("Balance")]
        [SerializeField, Range(0, 1), Tooltip("The lerp value that reset the balance to 0 over time")]
        private float balanceLerpTo0Value = 0.01f;
        [ReadOnly, Tooltip("Can the balance lerp itself to 0 ?")]
        public bool LerpBalanceTo0 = true;
        [ReadOnly, Tooltip("The current balance value")]
        public float Balance = 0f;
        [Range(0, 40), Tooltip("The limit over which the player will go in unbalance state")]
        public float BalanceLimit = 10f;
        [Range(0, 40), Tooltip("The limit over which the player will die")]
        public float BalanceDeathLimit = 15f;
        [Range(0, 40), Tooltip("The angle the player has to reach when unbalanced to get back balanced")]
        public float RebalanceAngle = 8f;
        [Range(0, 10), Tooltip("Minimum Time/Balance the player has to react when unbalanced")]
        public float MinimumTimeUnbalanced = 2f;
        [Range(0, 20), Tooltip("The multiplier applied to the paddle force to the balance")]
        public float RotationToBalanceMultiplier = 10f;
        [Range(0, 10), Tooltip("The multiplier to the floaters' level difference added to the balance value")]
        public float FloatersLevelDifferenceToBalanceMultiplier = 1f;

        [Header("UnBalanced")]
        [Tooltip("The number of times to press the button")]
        public int NumberPressButton;
        [Tooltip("Conversion of a 'balance' unit to time")]
        public float UnitBlanceToTimer;
        [Tooltip("The more balance you have the less time you have (formula: (unitbalance * balance) - ((balance - balanceLimit) * ReductionForce))")]
        public float ReductionForce;
        [Tooltip("The timer")]
        [ReadOnly]public float TimerUnbalanced = 0;
        [Tooltip("The number of times the button has been pressed")]
        [ReadOnly]public int NumberButtonIsPressed = 0;


        [Header("VFX")]
        [SerializeField] private ParticleSystem _paddleLeftParticle;
        [SerializeField] private ParticleSystem _paddleRightParticle;

        [Header("Weapon Mode"), ReadOnly]
        public Weapon CurrentWeapon;
        [Range(0, 0.1f), Tooltip("The lerp applied to the boat following camera direction when aiming")]
        public float BoatFollowAimLerp = 0.05f;
        [SerializeField]
        public Projectile HarpoonPrefab;
        public Projectile NetPrefab;

        private void Awake()
        {
            CharacterNavigationState navigationState =
                new CharacterNavigationState(KayakController, InputManagement, this, this, CameraManagerRef);
            CurrentStateBase = navigationState;
        }

        private void Start()
        {
            CurrentStateBase.EnterState(this);
            CurrentStateBase.OnPaddleRight.AddListener(PlayPaddleRightParticle);
            CurrentStateBase.OnPaddleLeft.AddListener(PlayPaddleLeftParticle);
            
            BalanceGaugeManagerRef.SetBalanceGaugeActive(false);
            BalanceGaugeManagerRef.ShowTrigger(false, false);
        }
        private void Update()
        {
            CurrentStateBase.UpdateState(this);

            if (CurrentStateBase.IsDead == false)
            {
                BalanceManagement();
            }
        }
        private void FixedUpdate()
        {
            CurrentStateBase.FixedUpdate(this);
        }
        public void SwitchState(CharacterStateBase stateBaseCharacter)
        {
            CurrentStateBase.ExitState(this);
            CurrentStateBase = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }

        /// <summary>
        /// Lerp the Balance value to 0 
        /// </summary>
        private void BalanceManagement()
        {
            if (LerpBalanceTo0)
            {
                Balance = Mathf.Lerp(Balance, 0, balanceLerpTo0Value);
            }
            BalanceGaugeManagerRef.SetBalanceCursor(Balance);
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

        #region VFX

        private void PlayPaddleLeftParticle()
        {
            if (_paddleLeftParticle == null)
            {
                return;
            }
            _paddleLeftParticle.Play();
        }

        private void PlayPaddleRightParticle()
        {
            if (_paddleRightParticle == null)
            {
                return;
            }
            _paddleRightParticle.Play();
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            GUI.skin.label.fontSize = 50;

            GUI.color = Color.white;
            GUI.Label(new Rect(10, 10, 500, 100), "Balance : " + Math.Round(Balance, 1));
        }

        #endregion

        public void SwitchToDeathState()
        {
            CharacterDeathState characterDeathState = new CharacterDeathState(this, KayakController, InputManagement, this, CameraManagerRef);
            SwitchState(characterDeathState);
        }
    }
}
