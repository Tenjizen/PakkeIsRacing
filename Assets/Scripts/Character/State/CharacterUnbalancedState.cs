﻿using Character.Camera;
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
        private float _timerReturnNavigationState;

        private float _timerDebug = 0;

        private int _signeBalance;
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
            _timerDebug = 0;
            CharacterManagerRef.BalanceGaugeManagerRef.ResetGauge();
            CharacterManagerRef.LerpBalanceTo0 = false;

            _signeBalance = CharacterManagerRef.Balance > 0 ? 1 : -1;

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

            #region Debug timer
            _timerDebug += Time.deltaTime;
            if (_timerDebug > Mathf.Abs(CharacterManagerRef.TimerUnbalanced) + 10)
            {
                if (CharacterManagerRef.NumberButtonIsPressed >= CharacterManagerRef.Data.NumberPressButton && _timerUnbalanced <= Mathf.Abs(CharacterManagerRef.TimerUnbalanced))
                {
                    _kayakController.CanReduceDrag = true;
                    CameraManagerRef.CanMoveCameraManually = true;
                    ResetRotationBoat(_kayakController.transform, 2);
                    //CharacterManagerRef.SetBalanceValueToCurrentSide(0);
                    CanCharacterMakeActions = true;

                    CharacterNavigationState characterNavigationState = new CharacterNavigationState();
                    CharacterManagerRef.SwitchState(characterNavigationState);

                    CameraNavigationState cameraNavigationState = new CameraNavigationState();
                    CameraManagerRef.SwitchState(cameraNavigationState);
                }
                else
                {
                    CharacterDeathState characterDeathState = new CharacterDeathState();
                    CharacterManagerRef.SwitchState(characterDeathState);
                }
            }
            #endregion

            if (CharacterManagerRef.NumberButtonIsPressed < CharacterManagerRef.Data.NumberPressButton)
            {
                ClickSpam();
                CharacterManagerRef.BalanceGaugeManagerRef.ReduceGauge(Mathf.Abs((_timerUnbalanced / DIVIDE_TIMER_PERCENT) / CharacterManagerRef.TimerUnbalanced));

                float percentGauge = CharacterManagerRef.BalanceGaugeManagerRef.PercentGauge();
                float angle = 0;

                if (CharacterManagerRef.BalanceGaugeManagerRef.Cursor.rotation.eulerAngles.z > 180)
                    angle = CharacterManagerRef.BalanceGaugeManagerRef.Cursor.rotation.eulerAngles.z - 360;
                else
                    angle = CharacterManagerRef.BalanceGaugeManagerRef.Cursor.rotation.eulerAngles.z;



                MakeBoatRotationWithBalance(_kayakController.transform, 1);

                if ((percentGauge * 100) + 2.5f < Mathf.Abs((angle / 90) * 100))
                {
                    CharacterDeathState characterDeathState = new CharacterDeathState();
                    CharacterManagerRef.SwitchState(characterDeathState);
                }
            }

            CharacterManagerRef.Balance = 10 * _signeBalance;

        }

        public override void FixedUpdate(CharacterManager character)
        {

        }

        public override void SwitchState(CharacterManager character)
        {

        }

        public override void ExitState(CharacterManager character)
        {
            CharacterManagerRef.InvincibilityTime = CharacterManagerRef.Data.InvincibleTimeAfterUnbalance;
            
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
                ResetRotationBoat(_kayakController.transform, 2);

                _timerReturnNavigationState += Time.deltaTime;

                if (_timerReturnNavigationState > 0.5f && _kayakController.transform.eulerAngles.z < 0.5f)
                {
                    _kayakController.CanReduceDrag = true;
                    CameraManagerRef.CanMoveCameraManually = true;
                    //CharacterManagerRef.SetBalanceValueToCurrentSide(0);
                    CanCharacterMakeActions = true;

                    CharacterNavigationState characterNavigationState = new CharacterNavigationState();
                    CharacterManagerRef.SwitchState(characterNavigationState);

                    CameraNavigationState cameraNavigationState = new CameraNavigationState();
                    CameraManagerRef.SwitchState(cameraNavigationState);
                }
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

        private void ResetRotationBoat(Transform kayakTransform, float multiplier)
        {
            CharacterManagerRef.Balance = 0;
            Quaternion localRotation = kayakTransform.localRotation;
            Vector3 boatRotation = localRotation.eulerAngles;
            Quaternion targetBoatRotation = Quaternion.Euler(0, boatRotation.y, CharacterManagerRef.Balance * 3 * multiplier);
            localRotation = Quaternion.Lerp(localRotation, targetBoatRotation, 0.025f);
            kayakTransform.localRotation = localRotation;
        }
        #endregion
    }
}