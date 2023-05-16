using Character.Camera;
using Character.Camera.State;
using Kayak;
using Kayak.Data;
using Sound;
using UnityEngine;

namespace Character.State
{
    public class CharacterUnbalancedState : CharacterStateBase
    {
        #region Variables
        private const float DIVIDE_TIMER_PERCENT = 4;

        private KayakController _kayakController;
        private InputManagement _inputs;

        private float _timerUnbalanced = 0;


        #endregion

        #region Constructor

        public CharacterUnbalancedState() : base()
        {
            _kayakController = CharacterManagerRef.KayakControllerProperty;
            _inputs = CharacterManagerRef.InputManagementProperty;
            CanBeMoved = false;
            CanCharacterOpenWeapons = false;
        }

        #endregion

        #region Override Functions

        public override void EnterState(CharacterManager character)
        {
            Debug.Log("unbalanced");
            CharacterManagerRef.BalanceGaugeManagerRef.ResetGauge();
            CharacterManagerRef.LerpBalanceTo0 = false;

            //values
            CanBeMoved = false;
            CanCharacterOpenWeapons = false;

            CharacterManagerRef.NumberButtonIsPressed = 0;
            _timerUnbalanced = 0;


            var balanceTimer = (CharacterManagerRef.Data.UnitBalanceToTimer * Mathf.Abs(CharacterManagerRef.Balance));
            var collisionForce = (Mathf.Abs(CharacterManagerRef.Balance) - CharacterManagerRef.Data.BalanceLimit) * CharacterManagerRef.Data.ReductionForce;

            if (balanceTimer - collisionForce < CharacterManagerRef.Data.MinimumTimeUnbalanced)
                CharacterManagerRef.TimerUnbalanced = CharacterManagerRef.Data.MinimumTimeUnbalanced;
            else
                CharacterManagerRef.TimerUnbalanced = balanceTimer - collisionForce;

            Debug.Log(CharacterManagerRef.TimerUnbalanced + " timer");

            //balance
            if (Mathf.Abs(CharacterManagerRef.Balance) >
                CharacterManagerRef.Data.BalanceDeathLimit - CharacterManagerRef.Data.MinimumTimeUnbalanced)
            {
                CharacterManagerRef.SetBalanceValueToCurrentSide(CharacterManagerRef.Data.BalanceDeathLimit - CharacterManagerRef.Data.MinimumTimeUnbalanced);
            }

            //gauge
            CharacterManagerRef.BalanceGaugeManagerRef.SetBalanceGaugeActive(true);
            float balance = CharacterManagerRef.Balance;
            CharacterManagerRef.BalanceGaugeManagerRef.ShowTrigger(balance < 0, balance > 0);
            
            //anim
            CharacterManagerRef.PaddleAnimatorProperty.SetBool("Unbalanced", true);
            CharacterManagerRef.CharacterAnimatorProperty.SetBool("Unbalanced", true);
        }

        public override void UpdateState(CharacterManager character)
        {
            Timer();
            ClickSpam();

            CharacterManagerRef.BalanceGaugeManagerRef.ReduceGauge(Mathf.Abs((_timerUnbalanced / DIVIDE_TIMER_PERCENT) / CharacterManagerRef.TimerUnbalanced));
            float percentGauge = CharacterManagerRef.BalanceGaugeManagerRef.PercentGauge();
            float angle = 0;

            if (CharacterManagerRef.BalanceGaugeManagerRef.Cursor.rotation.eulerAngles.z > 180)
                angle = CharacterManagerRef.BalanceGaugeManagerRef.Cursor.rotation.eulerAngles.z - 360;
            else
                angle = CharacterManagerRef.BalanceGaugeManagerRef.Cursor.rotation.eulerAngles.z;



            MakeBoatRotationWithBalance(_kayakController.transform, 2);

            if (Mathf.Abs(CharacterManagerRef.Balance) < CharacterManagerRef.Data.RebalanceAngle)
            {
                this.SwitchState(character);
            }

            if ((percentGauge * 100) + 2.5f < Mathf.Abs((angle / 90) * 100))
            {
                CharacterDeathState characterDeathState = new CharacterDeathState();
                CharacterManagerRef.SwitchState(characterDeathState);
            }
        }

        public override void FixedUpdate(CharacterManager character)
        {

        }

        public override void SwitchState(CharacterManager character)
        {

        }

        public override void ExitState(CharacterManager character)
        {
            CharacterManagerRef.BalanceGaugeManagerRef.SetBalanceGaugeActive(false);
            CharacterManagerRef.BalanceGaugeManagerRef.ShowTrigger(false, false);

            CanBeMoved = true;
            CanCharacterOpenWeapons = true;
        }

        #endregion

        #region Methods
        private void Timer()
        {
            _timerUnbalanced += Time.deltaTime;

            if (CharacterManagerRef.NumberButtonIsPressed >= CharacterManagerRef.Data.NumberPressButton && _timerUnbalanced <= Mathf.Abs(CharacterManagerRef.TimerUnbalanced))
            {
                _kayakController.CanReduceDrag = true;
                CameraManagerRef.CanMoveCameraManually = true;
                CharacterManagerRef.SetBalanceValueToCurrentSide(0);
                CanCharacterMakeActions = true;

                CharacterNavigationState characterNavigationState = new CharacterNavigationState();
                CharacterManagerRef.SwitchState(characterNavigationState);

                CameraNavigationState cameraNavigationState = new CameraNavigationState();
                CameraManagerRef.SwitchState(cameraNavigationState);
            }
        }

        private void ClickSpam()
        {
            if (_inputs.GameplayInputs.Boat.UnbalancedLeft.WasPerformedThisFrame() && CharacterManagerRef.Balance < 0)
            {
                CharacterManagerRef.NumberButtonIsPressed++;
                CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
            }
            if (_inputs.GameplayInputs.Boat.UnbalancedRight.WasPerformedThisFrame() && CharacterManagerRef.Balance > 0)
            {
                CharacterManagerRef.NumberButtonIsPressed++;
                CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
            }
        }
        #endregion
    }
}