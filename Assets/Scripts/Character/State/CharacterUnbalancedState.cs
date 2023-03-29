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

        private KayakController _kayakController;
        private InputManagement _inputs;

        //private KayakParameters _kayakValues;
        //private float _rightPaddleCooldown, _leftPaddleCooldown;

        private float _timerUnbalanced = 0;

        #endregion

        #region Constructor

        public CharacterUnbalancedState() : base()
        {
            _kayakController = CharacterManagerRef.KayakControllerProperty;
            _inputs = CharacterManagerRef.InputManagementProperty;
            //_kayakValues = CharacterManagerRef.KayakControllerProperty.Data.KayakValues;
        }

        #endregion

        #region Override Functions

        public override void EnterState(CharacterManager character)
        {
            Debug.Log("unbalanced");
            CharacterManagerRef.LerpBalanceTo0 = false;

            //values
            //_rightPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;
            //_leftPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;
            CanBeMoved = false;
            CanCharacterOpenWeapons = false;

            CharacterManagerRef.NumberButtonIsPressed = 0;
            _timerUnbalanced = 0;
            CharacterManagerRef.TimerUnbalanced = (CharacterManagerRef.Data.UnitBalanceToTimer * Mathf.Abs(CharacterManagerRef.Balance)) -
                                                    ((Mathf.Abs(CharacterManagerRef.Balance) - CharacterManagerRef.Data.BalanceLimit) * CharacterManagerRef.Data.ReductionForce);

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
        }

        public override void UpdateState(CharacterManager character)
        {
            NewTimer();
            ClickSpam();

            //TimerManagement();
            //PaddleCooldownManagement();

            MakeBoatRotationWithBalance(_kayakController.transform, 2);

            //Rebalance();

            if (Mathf.Abs(CharacterManagerRef.Balance) < CharacterManagerRef.Data.RebalanceAngle)
            {
                this.SwitchState(character);
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
        }

        #endregion

        #region Methods
        private void NewTimer()
        {
            _timerUnbalanced += Time.deltaTime;

            if (CharacterManagerRef.NumberButtonIsPressed >= CharacterManagerRef.Data.NumberPressButton && _timerUnbalanced <= CharacterManagerRef.TimerUnbalanced)
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
            else if (_timerUnbalanced >= CharacterManagerRef.TimerUnbalanced)
            {
                CharacterDeathState characterDeathState = new CharacterDeathState();
                CharacterManagerRef.SwitchState(characterDeathState);
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

        #region old
        //private void TimerManagement()
        //{
        //    CharacterManagerRef.Balance += Time.deltaTime * Mathf.Sign(CharacterManagerRef.Balance);
        //    if (Mathf.Abs(CharacterManagerRef.Balance) >= CharacterManagerRef.Data.BalanceDeathLimit)
        //    {
        //        CharacterDeathState characterDeathState = new CharacterDeathState();
        //        CharacterManagerRef.SwitchState(characterDeathState);
        //    }
        //    else if (Mathf.Abs(CharacterManagerRef.Balance) < CharacterManagerRef.Data.RebalanceAngle)
        //    {
        //        _kayakController.CanReduceDrag = true;
        //        CameraManagerRef.CanMoveCameraManually = true;
        //        CharacterManagerRef.SetBalanceValueToCurrentSide(0);
        //        CanCharacterMakeActions = true;

        //        CharacterNavigationState characterNavigationState = new CharacterNavigationState();
        //        CharacterManagerRef.SwitchState(characterNavigationState);

        //        CameraNavigationState cameraNavigationState = new CameraNavigationState();
        //        CameraManagerRef.SwitchState(cameraNavigationState);
        //    }
        //}
        //private void PaddleCooldownManagement()
        //{
        //    _leftPaddleCooldown -= Time.deltaTime;
        //    _rightPaddleCooldown -= Time.deltaTime;
        //}
        //private void Rebalance()
        //{
        //    if (_inputs.Inputs.PaddleLeft && _leftPaddleCooldown <= 0 && CharacterManagerRef.Balance < 0)
        //    {
        //        CharacterManagerRef.Balance += _kayakValues.UnbalancePaddleForce;
        //        _leftPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;

        //        RebalanceEffect();
        //    }
        //    if (_inputs.Inputs.PaddleRight && _rightPaddleCooldown <= 0 && CharacterManagerRef.Balance > 0)
        //    {
        //        CharacterManagerRef.Balance -= _kayakValues.UnbalancePaddleForce;
        //        _rightPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;

        //        RebalanceEffect();
        //    }
        //}

        //private void RebalanceEffect()
        //{
        //    CharacterManager.Instance.SoundManagerProperty.PlaySound(_kayakController.Data.PaddlingAudioClip);
        //    CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
        //}        //private void TimerManagement()
        //{
        //    CharacterManagerRef.Balance += Time.deltaTime * Mathf.Sign(CharacterManagerRef.Balance);
        //    if (Mathf.Abs(CharacterManagerRef.Balance) >= CharacterManagerRef.Data.BalanceDeathLimit)
        //    {
        //        CharacterDeathState characterDeathState = new CharacterDeathState();
        //        CharacterManagerRef.SwitchState(characterDeathState);
        //    }
        //    else if (Mathf.Abs(CharacterManagerRef.Balance) < CharacterManagerRef.Data.RebalanceAngle)
        //    {
        //        _kayakController.CanReduceDrag = true;
        //        CameraManagerRef.CanMoveCameraManually = true;
        //        CharacterManagerRef.SetBalanceValueToCurrentSide(0);
        //        CanCharacterMakeActions = true;

        //        CharacterNavigationState characterNavigationState = new CharacterNavigationState();
        //        CharacterManagerRef.SwitchState(characterNavigationState);

        //        CameraNavigationState cameraNavigationState = new CameraNavigationState();
        //        CameraManagerRef.SwitchState(cameraNavigationState);
        //    }
        //}
        //private void PaddleCooldownManagement()
        //{
        //    _leftPaddleCooldown -= Time.deltaTime;
        //    _rightPaddleCooldown -= Time.deltaTime;
        //}
        //private void Rebalance()
        //{
        //    if (_inputs.Inputs.PaddleLeft && _leftPaddleCooldown <= 0 && CharacterManagerRef.Balance < 0)
        //    {
        //        CharacterManagerRef.Balance += _kayakValues.UnbalancePaddleForce;
        //        _leftPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;

        //        RebalanceEffect();
        //    }
        //    if (_inputs.Inputs.PaddleRight && _rightPaddleCooldown <= 0 && CharacterManagerRef.Balance > 0)
        //    {
        //        CharacterManagerRef.Balance -= _kayakValues.UnbalancePaddleForce;
        //        _rightPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;

        //        RebalanceEffect();
        //    }
        //}

        //private void RebalanceEffect()
        //{
        //    CharacterManager.Instance.SoundManagerProperty.PlaySound(_kayakController.Data.PaddlingAudioClip);
        //    CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
        //}
        #endregion

        #endregion
    }
}