using System;
using Character.Camera;
using Character.Data;
using Character.State;
using Fight;
using GPEs.Checkpoint;
using Kayak;
using SceneTransition;
using Sound;
using Tools.SingletonClassBase;
using UI;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public class CharacterManager : Singleton<CharacterManager>
    {
        #region Properties
        
        [field:SerializeField] public CharacterStateBase CurrentStateBaseProperty { get; private set; }
        [field:SerializeField, Header("Properties")] public CameraManager CameraManagerProperty { get; private set; }
        [field:SerializeField] public KayakController KayakControllerProperty { get; private set; }
        [field:SerializeField] public InputManagement InputManagementProperty { get; private set; }
        [field:SerializeField] public Animator PaddleAnimatorProperty { get; private set; }
        [field:SerializeField] public TransitionManager TransitionManagerProperty { get; private set; } 
        [field:SerializeField] public WeaponUIManager WeaponUIManagerProperty { get; private set; } 
        [field:SerializeField] public BalanceGaugeManager BalanceGaugeManagerRef { get; private set; }
        [field:SerializeField] public SoundManager SoundManagerProperty { get; private set; }
        [field:SerializeField] public CheckpointManager CheckpointManagerProperty { get; private set; }
        [field:SerializeField] public MonoBehaviour CharacterMonoBehaviour { get; private set; }
        
        #endregion

        [Header("Character Data")]
        public CharacterData Data;
        [Header("Balance Infos"), ReadOnly, Tooltip("Can the balance lerp itself to 0 ?")]
        public bool LerpBalanceTo0 = true;
        [ReadOnly, Tooltip("The current balance value")]
        public float Balance = 0f;
        [Tooltip("The timer"), ReadOnly] 
        public float TimerUnbalanced = 0;
        [Tooltip("The number of times the button has been pressed"), ReadOnly]
        public int NumberButtonIsPressed = 0;
        [ReadOnly] 
        public Weapon CurrentWeapon;

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
            BalanceGaugeManagerRef.ShowTrigger(false, false);
        }
        private void Update()
        {
            CurrentStateBaseProperty.UpdateState(this);

            if (CurrentStateBaseProperty.IsDead == false)
            {
                BalanceManagement();
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
            if (LerpBalanceTo0)
            {
                Balance = Mathf.Lerp(Balance, 0, Data.BalanceLerpTo0Value);
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
            CharacterDeathState characterDeathState = new CharacterDeathState();
            SwitchState(characterDeathState);
        }
    }
}
