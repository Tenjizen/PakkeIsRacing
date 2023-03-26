using Character.Camera;
using Character.Camera.State;
using Kayak;
using UnityEngine;

namespace Character.State
{
    public class CharacterUnbalancedState : CharacterStateBase
    {
        #region Variables

        private KayakController _kayakController;
        private InputManagement _inputs;
        private GameplayInputs _gameplayInputs;
        private KayakParameters _kayakValues;

        private float _rightPaddleCooldown, _leftPaddleCooldown;

        private float _timerUnbalanced = 0;

        #endregion

        #region Constructor

        public CharacterUnbalancedState(KayakController kayak, InputManagement inputManagement, CharacterManager characterManagerRef, MonoBehaviour monoBehaviour, CameraManager cameraManager) :
            base(characterManagerRef, monoBehaviour, cameraManager)
        {
            _kayakController = kayak;
            _inputs = inputManagement;
            _kayakValues = kayak.KayakValues;
        }

        #endregion

        #region Override Functions

        public override void EnterState(CharacterManager character)
        {
            Debug.Log("unbalanced");
            CharacterManagerRef.LerpBalanceTo0 = false;

            //values
            _rightPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;
            _leftPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;
            CanBeMoved = false;
            CanCharacterOpenWeapons = false;

            CharacterManagerRef.NumberButtonIsPressed = 0;
            _timerUnbalanced = 0;
            CharacterManagerRef.TimerUnbalanced = (CharacterManagerRef.UnitBlanceToTimer * Mathf.Abs(CharacterManagerRef.Balance)) - 
                                                    ((Mathf.Abs(CharacterManagerRef.Balance) - CharacterManagerRef.BalanceLimit)*CharacterManagerRef.ReductionForce);

            //balance
            if (Mathf.Abs(CharacterManagerRef.Balance) >
                CharacterManagerRef.BalanceDeathLimit - CharacterManagerRef.MinimumTimeUnbalanced)
            {
                CharacterManagerRef.SetBalanceValueToCurrentSide(CharacterManagerRef.BalanceDeathLimit - CharacterManagerRef.MinimumTimeUnbalanced);
            }

            //gauge
            CharacterManagerRef.BalanceGaugeManagerRef.SetBalanceGaugeActive(true);
            float balance = CharacterManagerRef.Balance;
            CharacterManagerRef.BalanceGaugeManagerRef.ShowTrigger(balance < 0, balance > 0);
        }

        public override void UpdateState(CharacterManager character)
        {
            //TimerManagement();
            NewTimer();
            PaddleCooldownManagement();

            MakeBoatRotationWithBalance(_kayakController.transform, 2);

            //Rebalance();
            ClickSpam();

            if (Mathf.Abs(CharacterManagerRef.Balance) < CharacterManagerRef.RebalanceAngle)
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

            if (CharacterManagerRef.NumberButtonIsPressed >= CharacterManagerRef.NumberPressButton && _timerUnbalanced <= CharacterManagerRef.TimerUnbalanced)
            {
                _kayakController.CanReduceDrag = true;
                CameraManagerRef.CanMoveCameraManually = true;
                CharacterManagerRef.SetBalanceValueToCurrentSide(0);
                CanCharacterMakeActions = true;

                CharacterNavigationState characterNavigationState = new CharacterNavigationState(_kayakController, _inputs, CharacterManagerRef, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterNavigationState);

                CameraNavigationState cameraNavigationState = new CameraNavigationState(CameraManagerRef, MonoBehaviourRef);
                CameraManagerRef.SwitchState(cameraNavigationState);
            }
            else if (_timerUnbalanced >= CharacterManagerRef.TimerUnbalanced)
            {
                CharacterDeathState characterDeathState = new CharacterDeathState(CharacterManagerRef, _kayakController, _inputs, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterDeathState);
            }
        }

        private void TimerManagement()
        {
            CharacterManagerRef.Balance += Time.deltaTime * Mathf.Sign(CharacterManagerRef.Balance);
            if (Mathf.Abs(CharacterManagerRef.Balance) >= CharacterManagerRef.BalanceDeathLimit)
            {
                CharacterDeathState characterDeathState = new CharacterDeathState(CharacterManagerRef, _kayakController, _inputs, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterDeathState);
            }
            else if (Mathf.Abs(CharacterManagerRef.Balance) < CharacterManagerRef.RebalanceAngle)
            {
                _kayakController.CanReduceDrag = true;
                CameraManagerRef.CanMoveCameraManually = true;
                CharacterManagerRef.SetBalanceValueToCurrentSide(0);
                CanCharacterMakeActions = true;

                CharacterNavigationState characterNavigationState = new CharacterNavigationState(_kayakController, _inputs, CharacterManagerRef, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterNavigationState);

                CameraNavigationState cameraNavigationState = new CameraNavigationState(CameraManagerRef, MonoBehaviourRef);
                CameraManagerRef.SwitchState(cameraNavigationState);
            }
        }

        private void PaddleCooldownManagement()
        {
            _leftPaddleCooldown -= Time.deltaTime;
            _rightPaddleCooldown -= Time.deltaTime;
        }

        private void ClickSpam()
        {
            if (_inputs.Inputs.PaddleLeft && _leftPaddleCooldown <= 0 && CharacterManagerRef.Balance < 0)
            {
                _leftPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;
                CharacterManagerRef.NumberButtonIsPressed++;
                CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
            }
            if (_inputs.Inputs.PaddleRight && _rightPaddleCooldown <= 0 && CharacterManagerRef.Balance > 0)
            {
                _rightPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;
                CharacterManagerRef.NumberButtonIsPressed++;
                CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
            }
        }

        private void Rebalance()
        {
            if (_inputs.Inputs.PaddleLeft && _leftPaddleCooldown <= 0 && CharacterManagerRef.Balance < 0)
            {
                //CharacterManagerRef.Balance += _kayakValues.UnbalancePaddleForce;
                _leftPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;

                    CharacterManagerRef.NumberButtonIsPressed++;

                //RebalanceEffect();
            }
            if (_inputs.Inputs.PaddleRight && _rightPaddleCooldown <= 0 && CharacterManagerRef.Balance > 0)
            {
                //CharacterManagerRef.Balance -= _kayakValues.UnbalancePaddleForce;
                _rightPaddleCooldown = _kayakValues.UnbalancePaddleCooldown;

                    CharacterManagerRef.NumberButtonIsPressed++;

                //RebalanceEffect();
            }
        }

        private void RebalanceEffect()
        {
            SoundManager.Instance.PlaySound(_kayakController.PaddlingAudioClip);
            CharacterManagerRef.BalanceGaugeManagerRef.MakeCursorFeedback();
        }

        #endregion
    }
}