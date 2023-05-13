using Character.Camera;
using Character.Camera.State;
using DG.Tweening;
using Kayak.Data;
using UnityEngine;
using UnityEngine.Events;

namespace Character.State
{
    public abstract class CharacterStateBase 
    {
        protected CharacterManager CharacterManagerRef;
        public CameraManager CameraManagerRef;
        public MonoBehaviour MonoBehaviourRef;
        
        public bool CanBeMoved = true;
        public bool CanCharacterMove = true;
        public bool CanCharacterMakeActions = true;
        public bool CanCharacterOpenWeapons = true;
        public bool IsDead;

        public float RotationStaticForceY = 0f;
        public float RotationPaddleForceY = 0f;

        [ReadOnly] public Floaters Floaters;

        //events
        public UnityEvent OnPaddleLeft = new UnityEvent();
        public UnityEvent OnPaddleRight = new UnityEvent();
        
        protected Transform PlayerPosition;
        
        protected CharacterStateBase()
        {
            if (CharacterManager.Instance != null)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            CharacterManagerRef = CharacterManager.Instance;
            CameraManagerRef = CharacterManager.Instance.CameraManagerProperty;
            MonoBehaviourRef = CharacterManager.Instance.CharacterMonoBehaviour;

            Floaters = CharacterManagerRef.KayakControllerProperty.FloatersRef;
        }

        public abstract void EnterState(CharacterManager character);

        public abstract void UpdateState(CharacterManager character);

        public abstract void FixedUpdate(CharacterManager character);

        public abstract void SwitchState(CharacterManager character);
        
        public abstract void ExitState(CharacterManager character);

        /// <summary>
        /// Rotate the boat alongside the Balance value
        /// </summary>
        protected void MakeBoatRotationWithBalance(Transform kayakTransform, float multiplier)
        {
            Quaternion localRotation = kayakTransform.localRotation;
            Vector3 boatRotation = localRotation.eulerAngles;
            Quaternion targetBoatRotation = Quaternion.Euler(boatRotation.x,boatRotation.y, CharacterManagerRef.Balance * 3 * multiplier);
            
            localRotation = Quaternion.Lerp(localRotation, targetBoatRotation, 0.025f);
            kayakTransform.localRotation = localRotation;
        }

        /// <summary>
        /// Move the velocity toward the player's facing direction
        /// </summary>
        protected void VelocityToward()
        {
            Vector3 oldVelocity = CharacterManagerRef.KayakControllerProperty.Rigidbody.velocity;
            float oldVelocityMagnitude = new Vector2(oldVelocity.x, oldVelocity.z).magnitude;
            Vector3 forward = CharacterManagerRef.KayakControllerProperty.transform.forward;
            
            Vector2 newVelocity = oldVelocityMagnitude * new Vector2(forward.x,forward.z).normalized;

            CharacterManagerRef.KayakControllerProperty.Rigidbody.velocity = new Vector3(newVelocity.x, oldVelocity.y, newVelocity.y);
        }

        protected void CheckBalance()
        {
            //check balanced -> unbalanced
            if (Mathf.Abs(CharacterManagerRef.Balance) >= CharacterManagerRef.Data.BalanceLimit * CharacterManagerRef.ExperienceManagerProperty.BalanceLimitMultiplier)
            {
                CameraManagerRef.CanMoveCameraManually = false;
                CharacterManagerRef.KayakControllerProperty.CanReduceDrag = false;
                
                //switch states
                CharacterUnbalancedState characterUnbalancedState = new CharacterUnbalancedState();
                CharacterManagerRef.SwitchState(characterUnbalancedState);

                CameraUnbalancedState cameraUnbalancedState = new CameraUnbalancedState();
                CameraManagerRef.SwitchState(cameraUnbalancedState);
            }
        }
        
        public void LaunchNavigationState()
        {
            CharacterManager character = CharacterManager.Instance;
            character.WeaponChargedParticleSystem.Stop();

            CameraNavigationState cameraNavigationState = new CameraNavigationState();
            character.CameraManagerProperty.SwitchState(cameraNavigationState);
                
            CharacterNavigationState characterNavigationState = new CharacterNavigationState();
            character.SwitchState(characterNavigationState);

            character.WeaponUIManagerProperty.SetPaddleDownImage(false);
            character.WeaponUIManagerProperty.SetCursor(false);
            character.WeaponUIManagerProperty.SetCooldownUI(0);
            
            character.WeaponUIManagerProperty.AutoAimController.ShowAutoAimCircle(false);
            character.WeaponUIManagerProperty.AutoAimController.ShowAutoAimUI(false);
        }

        #region Wave/Floaters and Balance management

        /// <summary>
        /// Check the floater's level to see if the boat is unbalanced
        /// </summary>
        protected void CheckRigidbodyFloatersBalance()
        {
            float frontLeftY = Floaters.FrontLeft.transform.position.y;
            float frontRightY = Floaters.FrontRight.transform.position.y;
            float backLeftY = Floaters.BackLeft.transform.position.y;
            float backRightY = Floaters.BackRight.transform.position.y;

            float frontLevel = (frontLeftY + frontRightY) / 2;
            float backLevel = (backLeftY + backRightY) / 2;
            float leftLevel = (frontLeftY + backLeftY) / 2;
            float rightLevel = (frontRightY + backRightY) / 2;

            float multiplier = CharacterManagerRef.Data.FloatersLevelDifferenceToBalanceMultiplier;
            float frontBackDifference = Mathf.Abs(frontLevel - backLevel) * multiplier;
            float leftRightDifference = Mathf.Abs(leftLevel - rightLevel) * multiplier;

            CharacterManagerRef.AddBalanceValueToCurrentSide(frontBackDifference);
            CharacterManagerRef.Balance += leftRightDifference;
        }

        #endregion

    }
}