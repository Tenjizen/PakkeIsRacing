using System;
using System.Collections;
using Kayak;
using Kayak.Data;
using Sound;
using UnityEngine;
using UnityEngine.Events;

namespace Character.State
{
    public class CharacterNavigationState : CharacterStateBase
    {
        //enum
        public enum Direction
        {
            Left = 0,
            Right = 1
        }

        private enum RotationType
        {
            Static = 0,
            Paddle = 1
        }

        //inputs
        private InputManagement _inputs;
        private float _staticInputTimer;

        //kayak
        private Vector2 _paddleForceValue;
        private float _leftPaddleCooldown, _rightPaddleCooldown;
        private Direction _lastPaddleSide;

        //reference
        private KayakController _kayakController;
        private KayakParameters _kayakValues;

        private Rigidbody _kayakRigidbody;
        //private Floaters _floaters;

        //priority
        private RotationType _lastInputType;
        private float _staticTime;
        private float _paddleTime;
        private bool _paddleWasReleased, _staticWasReleased;

        private float _timerLastInputTrigger = 0;
        private Direction _lastInputPaddle;

        #region Constructor

        public CharacterNavigationState(CharacterMultiPlayerManager character) : base(character)
        {
            _kayakController = CharacterManagerRef.KayakControllerProperty;
            _kayakRigidbody = CharacterManagerRef.KayakControllerProperty.Rigidbody;
            _kayakValues = CharacterManagerRef.KayakControllerProperty.Data.KayakValues;
            _inputs = CharacterManagerRef.InputManagementProperty;

            _lastInputType = RotationType.Static;
        }

        #endregion

        #region CharacterBaseState overrided function

        public override void EnterState(CharacterManager character)
        {
            //values
            _rightPaddleCooldown = _kayakValues.PaddleCooldown;
            _leftPaddleCooldown = _kayakValues.PaddleCooldown;
            _staticInputTimer = _kayakValues.StaticRotationCooldownAfterPaddle;
            CharacterManagerRef.Balance = 0;

            //booleans
            CharacterManagerRef.LerpBalanceTo0 = true;
            //CanBeMoved = true;
            CanCharacterMakeActions = true;

            //anim
            TimeBeforeSettingPaddleAnimator = 1f;
        }

        public override void UpdateState(CharacterManager character)
        {
            CheckCamView(character);
            if (CanBeMoved == false) return;

            PaddleCooldownManagement();

            MakeBoatRotationWithBalance(_kayakController.transform, 1);

            if (_timerLastInputTrigger > _kayakValues.TimerMaxForSprint)
            {
                CharacterManagerRef.SprintInProgress = false;
            }

            //if (CharacterManagerRef.InWaterFlow == false)
            //{
            //    CharacterManagerRef.SprintUIManager.SprintEnded(_timerLastInputTrigger);
            //}
            //else if (CharacterManagerRef.InWaterFlow == true)
            //{
            //    CharacterManagerRef.SprintUIManager.ParticleSpeedEmission(true);
            //    CharacterManagerRef.SprintUIManager.TrailEmitting(true);
            //}

            CheckPurify();
            CheckBump();
        }

        private void CheckCamView(CharacterManager character)
        {
            
            if (CharacterManagerRef.InCam.IsInCameraViewValue == false && CharacterManagerRef.InCam.Timer >= CharacterManagerRef.InCam.TimerOutOfView)
            {
                Respawn(GameManager.Instance.SharkPossessed.transform.position, character);
            }
        }
        bool spawn;
        private void Respawn(Vector3 vector3, CharacterManager character)
        {

            //put kayak in checkpoint position & rotation
            vector3.y = GameManager.Instance.WavesRef.GetHeight(vector3);
            _kayakController.transform.position = vector3;

            _kayakController.transform.eulerAngles = new Vector3(_kayakController.transform.eulerAngles.x, Camera.main.transform.eulerAngles.y, _kayakController.transform.eulerAngles.z);
            _kayakController.Rigidbody.velocity = Vector3.zero;

            CharacterManagerRef.InCam.MultipleTargetCamera.AddTarget(_kayakController.transform);
            //CharacterManagerRef.InCam.IsInCameraViewValue = true;
            //CharacterManagerRef.InCam.Timer = 0;

            //SwitchState(character);
        }
        private void CheckPurify()
        {
            if (_inputs.Inputs.Purify == true && Character.MaxPts == true && Character.InSharkZone == true)
            {
                Character.MaxPts = false;
                Character.RemovePoint(GameManager.Instance.MaxPointToUnlockButton);
                Debug.Log("wwwwiiiiiiiiiiiinnnnnnnnneeeeeeeeeerrrrrrrrrrrrr");
                GameManager.Instance.SharkPossessed.SetActive(false);
                GameManager.Instance.EnnemyPossessed = false;
            }
        }

        private void CheckBump()
        {
            if (_inputs.Inputs.Bump)
            {
                CharacterManagerRef.CreateBump();
            }
        }

        public override void FixedUpdate(CharacterManager character)
        {
            SetBrakeAnimationToFalse();

            if (CanCharacterMove == false)
            {
                StopCharacter();
                return;
            }

            ManageKayakMovementInputs();

            KayakRotationManager(RotationType.Paddle);
            KayakRotationManager(RotationType.Static);

            VelocityToward();

            CheckRigidbodyFloatersBalance();
        }

        public override void SwitchState(CharacterManager character)
        {
            //CanBeMoved = true;
        }

        public override void ExitState(CharacterManager character)
        {
            DisableSprint();
        }

        #endregion

        #region Methods

        /// <summary>
        /// manages the rotation of the kayak based on the input rotation type and updates the relevant rotation force values.
        /// </summary>
        private void KayakRotationManager(RotationType rotationType)
        {
            //get rotation
            float rotationForceY = rotationType == RotationType.Paddle ? RotationPaddleForceY : RotationStaticForceY;

            //calculate rotation
            if (Mathf.Abs(rotationForceY) > 0.001f)
            {
                rotationForceY = Mathf.Lerp(rotationForceY, 0,
                    rotationType == RotationType.Paddle
                        ? _kayakValues.PaddleRotationDeceleration
                        : _kayakValues.StaticRotationDeceleration);
            }
            else
            {
                rotationForceY = 0;
            }

            //apply transform
            Transform kayakTransform = _kayakController.transform;
            kayakTransform.Rotate(Vector3.up, rotationForceY);

            //changes values
            switch (rotationType)
            {
                case RotationType.Paddle:
                    RotationPaddleForceY = rotationForceY;
                    break;
                case RotationType.Static:
                    RotationStaticForceY = rotationForceY;
                    break;
            }
        }

        /// <summary>
        /// Lerp the character velocity to 0
        /// </summary>
        private void StopCharacter()
        {
            _kayakRigidbody.velocity = Vector3.Lerp(_kayakRigidbody.velocity, Vector3.zero, 0.01f);
        }

        /// <summary>
        /// Set the animators brake booleans to false
        /// </summary>
        private void SetBrakeAnimationToFalse()
        {
            CharacterManagerRef.PaddleAnimatorProperty.SetBool("BrakeLeft", false);
            CharacterManagerRef.PaddleAnimatorProperty.SetBool("BrakeRight", false);
            CharacterManagerRef.CharacterAnimatorProperty.SetBool("BrakeLeft", false);
            CharacterManagerRef.CharacterAnimatorProperty.SetBool("BrakeRight", false);
        }

        /// <summary>
        /// Manage the kayak static/paddle movement method choice
        /// </summary>
        private void ManageKayakMovementInputs()
        {
            const float timeToSetLastInput = 1.5f;
            bool staticInput = _inputs.Inputs.RotateLeft != 0 || _inputs.Inputs.RotateRight != 0;
            bool paddleInput = _inputs.Inputs.PaddleLeft || _inputs.Inputs.PaddleRight;

            if (((paddleInput == false) ||
                 (_lastInputType == RotationType.Paddle) ||
                 (_paddleWasReleased) == false && _lastInputType == RotationType.Paddle) &&
                _staticInputTimer <= 0 && staticInput)
            {
                HandleStaticRotation();

                _staticTime += Time.deltaTime;
                if (_staticTime >= timeToSetLastInput)
                {
                    _paddleTime = 0f;
                    _lastInputType = RotationType.Static;
                }
            }

            if (((staticInput == false) ||
                 (_lastInputType == RotationType.Static)) &&
                paddleInput)
            {
                HandlePaddleMovement();

                _paddleTime += Time.deltaTime;
                if (_paddleTime >= timeToSetLastInput)
                {
                    _staticTime = 0f;
                    _paddleWasReleased = false;
                    if (staticInput == false)
                    {
                        _lastInputType = RotationType.Paddle;
                    }
                }
            }

            if (paddleInput == false)
            {
                _paddleWasReleased = true;
                if (_paddleTime >= timeToSetLastInput)
                {
                    _lastInputType = RotationType.Paddle;
                }
            }
        }

        #endregion

        #region Paddle Movement

        /// <summary>
        /// Handle the paddling of the kayak, add rotation force, launch the paddleForceCurve method to move the rigidbody, play paddle sound, set animation paddle trigger
        /// </summary>
        private void Paddle(Direction direction)
        {
            //timers
            _kayakController.DragReducingTimer = 0.5f;
            _staticInputTimer = _kayakValues.StaticRotationCooldownAfterPaddle;

            //rotation
            if (direction == _lastPaddleSide)
            {
                float rotation = _kayakValues.PaddleSideRotationForce *
                                 CharacterManagerRef.PlayerStats.RotationSpeedMultiplier;
                RotationPaddleForceY += direction == Direction.Right ? -rotation : rotation;
            }

            _lastPaddleSide = direction;

            //balance
            //CharacterManagerRef.Balance += RotationPaddleForceY * CharacterManagerRef.Data.RotationToBalanceMultiplier;

            //force
            MonoBehaviourRef.StartCoroutine(PaddleForceCurve());

            //animation & particles
            CharacterManagerRef.PaddleAnimatorProperty.SetTrigger(direction == Direction.Left
                ? "PaddleLeft"
                : "PaddleRight");
            CharacterManagerRef.CharacterAnimatorProperty.SetTrigger(direction == Direction.Left
                ? "PaddleLeft"
                : "PaddleRight");
            _kayakController.PlayPaddleParticle(direction);

            //events
            switch (direction)
            {
                case Direction.Left:
                    OnPaddleLeft.Invoke();
                    break;
                case Direction.Right:
                    OnPaddleRight.Invoke();
                    break;
            }

            CharacterManagerRef.OnPaddle.Invoke();
        }

        /// <summary>
        /// Detect & verify paddle input and launch paddle method 
        /// </summary>
        private void HandlePaddleMovement()
        {
            //input -> paddleMovement
            if (_inputs.Inputs.PaddleRight && _inputs.Inputs.PaddleLeft)
            {
                HandleBothPress();
            }

            if (_inputs.Inputs.PaddleLeft && _leftPaddleCooldown <= 0 && _inputs.Inputs.PaddleRight == false)
            {
                Direction direction =
                    CharacterManagerRef.Parameters.InversedControls ? Direction.Right : Direction.Left;
                _leftPaddleCooldown = _kayakValues.PaddleCooldown;
                _rightPaddleCooldown = _kayakValues.PaddleCooldown / 2;
                CheckIfSprint(direction);
                Paddle(direction);
            }

            if (_inputs.Inputs.PaddleRight && _rightPaddleCooldown <= 0 && _inputs.Inputs.PaddleLeft == false)
            {
                Direction direction =
                    CharacterManagerRef.Parameters.InversedControls ? Direction.Left : Direction.Right;
                _rightPaddleCooldown = _kayakValues.PaddleCooldown;
                _leftPaddleCooldown = _kayakValues.PaddleCooldown / 2;
                CheckIfSprint(direction);
                Paddle(direction);
            }
        }

        private void HandleBothPress()
        {
            if (_leftPaddleCooldown < 0)
            {
                Direction direction =
                    CharacterManagerRef.Parameters.InversedControls ? Direction.Right : Direction.Left;
                _leftPaddleCooldown = _kayakValues.PaddleCooldown;
                _rightPaddleCooldown = _kayakValues.PaddleCooldown / 2;
                Paddle(direction);
            }

            if (_rightPaddleCooldown < 0)
            {
                Direction direction =
                    CharacterManagerRef.Parameters.InversedControls ? Direction.Left : Direction.Right;
                _rightPaddleCooldown = _kayakValues.PaddleCooldown;
                _leftPaddleCooldown = _kayakValues.PaddleCooldown / 2;
                Paddle(direction);
            }

            //change the rotation inertia to 0
            RotationPaddleForceY = Mathf.Lerp(RotationPaddleForceY, 0, 0.1f);
            RotationStaticForceY = Mathf.Lerp(RotationStaticForceY, 0, 0.1f);

        }

        private void CheckIfSprint(Direction direction)
        {
            //CharacterManagerRef.SprintUIManager.EnableFeedback(direction);
            //if (_lastInputPaddle == direction || CharacterManagerRef.Abilities.SprintUnlock == false)
            //{
            //    return;
            //}

            //if (_timerLastInputTrigger >= _kayakValues.TimerMinForSprint &&
            //    _timerLastInputTrigger <= _kayakValues.TimerMaxForSprint)
            //{
            //    CharacterManagerRef.SprintInProgress = true;
            //    CharacterManagerRef.OnEnterSprint.Invoke();
            //    MonoBehaviourRef.StartCoroutine(CharacterManagerRef.SprintUIManager.GoodTiming());
            //}
            //else
            //{
            //    DisableSprint();
            //}

            //_timerLastInputTrigger = 0;
            //_lastInputPaddle = direction;
        }

        public void DisableSprint()
        {
            //CharacterManagerRef.SprintInProgress = false;
            //CharacterManagerRef.OnStopSprint.Invoke();
            //CharacterManagerRef.SprintUIManager.DisableSprintUI();
        }

        /// <summary>
        /// Add paddle force to the kayak a certain number of times
        /// </summary>
        private IEnumerator PaddleForceCurve()
        {
            float sprintMultiply = CharacterManagerRef.SprintInProgress ? _kayakValues.MultiplyValueForceInSprint : 1;
            for (int i = 0; i <= _kayakValues.NumberOfForceAppliance; i++)
            {
                float x = 1f / _kayakValues.NumberOfForceAppliance * i;
                float force = (_kayakValues.ForceCurve.Evaluate(x) * _kayakValues.PaddleForce) * sprintMultiply;
                Vector3 forceToApply = _kayakController.transform.forward * force;
                _kayakRigidbody.AddForce(forceToApply);

                yield return new WaitForSeconds(_kayakValues.TimeBetweenEveryAppliance);
            }
        }

        /// <summary>
        /// Count the different cooldowns
        /// </summary>
        private void PaddleCooldownManagement()
        {
            _leftPaddleCooldown -= Time.deltaTime;
            _rightPaddleCooldown -= Time.deltaTime;

            _staticInputTimer -= Time.deltaTime;

            _timerLastInputTrigger += Time.deltaTime;
        }

        #endregion

        #region Rotate Movement

        /// <summary>
        ///detect static rotation input and apply static rotation by adding rotation force & setting animator booleans
        /// </summary>
        private void HandleStaticRotation()
        {
            bool isFast = Mathf.Abs(_kayakRigidbody.velocity.x + _kayakRigidbody.velocity.z) >= 0.1f;

            //left
            if (_inputs.Inputs.RotateLeft > _inputs.Inputs.Deadzone)
            {
                if (isFast)
                {
                    DecelerationAndRotate(Direction.Right);
                }

                RotationStaticForceY += _kayakValues.StaticRotationForce;

                CharacterManagerRef.PaddleAnimatorProperty.SetBool("BrakeLeft", true);
                CharacterManagerRef.CharacterAnimatorProperty.SetBool("BrakeLeft", true);
            }
            else
            {
                CharacterManagerRef.PaddleAnimatorProperty.SetBool("BrakeLeft", false);
                CharacterManagerRef.CharacterAnimatorProperty.SetBool("BrakeLeft", false);
            }

            //right
            if (_inputs.Inputs.RotateRight > _inputs.Inputs.Deadzone)
            {
                if (isFast)
                {
                    DecelerationAndRotate(Direction.Left);
                }

                RotationStaticForceY -= _kayakValues.StaticRotationForce;

                CharacterManagerRef.PaddleAnimatorProperty.SetBool("BrakeRight", true);
                CharacterManagerRef.CharacterAnimatorProperty.SetBool("BrakeRight", true);
            }
            else
            {
                CharacterManagerRef.PaddleAnimatorProperty.SetBool("BrakeRight", false);
                CharacterManagerRef.CharacterAnimatorProperty.SetBool("BrakeRight", false);
            }
        }

        /// <summary>
        /// Lerp the kayak velocity to 0 and make add rotation force
        /// </summary>
        private void DecelerationAndRotate(Direction direction)
        {
            Vector3 targetVelocity = new Vector3(0, _kayakRigidbody.velocity.y, 0);

            float lerp = _kayakValues.VelocityDecelerationLerp *
                         CharacterManagerRef.PlayerStats.BreakingDistanceMultiplier;
            _kayakRigidbody.velocity = Vector3.Lerp(_kayakRigidbody.velocity, targetVelocity, lerp);

            float force = _kayakValues.VelocityDecelerationRotationForce;

            RotationStaticForceY += direction == Direction.Left ? -force : force;
        }

        #endregion
    }
}