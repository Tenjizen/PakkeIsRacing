﻿using System;
using Fight;
using UI.WeaponWheel;
using UnityEngine;

namespace Character.State
{
    public class CharacterWeaponState : CharacterStateBase
    {
        private Weapon _weapon;
        private bool _isAiming;
        
        private float _currentFov;
        private const float AimFOV = 40f;
        private const float BaseFov = 55f;

        private Projectile _weaponPrefab;
        private float _currentWeaponCooldown;
        
        #region Constructor

        public CharacterWeaponState(CharacterManager characterManagerRef, MonoBehaviour monoBehaviour, CameraManager cameraManagerRef) : base(characterManagerRef, monoBehaviour, cameraManagerRef)
        {
        }

        #endregion

        #region Base methods

        public override void EnterState(CharacterManager character)
        {
            CharacterManagerRef.weaponUIManagerRef.SetPaddleDownImage(true);
            _currentFov = BaseFov;
            
            _weapon = CharacterManagerRef.CurrentWeapon;
            switch (_weapon)
            {
                case Weapon.Harpoon:
                    _weaponPrefab = CharacterManagerRef.HarpoonPrefab;
                    break;
                case Weapon.Net:
                    _weaponPrefab = CharacterManagerRef.NetPrefab;
                    break;
            }
            
            _currentWeaponCooldown = _weaponPrefab.Data.Cooldown;
        }

        public override void UpdateState(CharacterManager character)
        {
            if (CharacterManagerRef.InputManagement.Inputs.DeselectWeapon)
            {
                CharacterNavigationState characterNavigationState = new CharacterNavigationState(CharacterManagerRef.KayakController, CharacterManagerRef.InputManagement, CharacterManagerRef, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterNavigationState);

                CameraNavigationState cameraNavigationState = new CameraNavigationState(CameraManagerRef, MonoBehaviourRef);
                CameraManagerRef.SwitchState(cameraNavigationState);
                
                CharacterManagerRef.weaponUIManagerRef.SetPaddleDownImage(false);
                CharacterManagerRef.weaponUIManagerRef.SetCursor(false);
                CharacterManagerRef.weaponUIManagerRef.SetCooldownUI(0);
            }
            
            ManageCooldown();
            HandleAim();
            HandleShoot();
        }

        public override void FixedUpdate(CharacterManager character)
        {
        }

        public override void SwitchState(CharacterManager character)
        {
        }

        #endregion

        #region Methods

        private void ManageCooldown()
        {
            _currentWeaponCooldown -= Time.deltaTime;
            float value = _currentWeaponCooldown / _weaponPrefab.Data.Cooldown;
            CharacterManagerRef.weaponUIManagerRef.SetCooldownUI(value);
        }
        
        private void HandleAim()
        {
            bool aimingState = _isAiming;
            _isAiming = CharacterManagerRef.InputManagement.Inputs.Aim;
            
            //aim directly
            _isAiming = true;

            float fovLerp = 0.1f;
            _currentFov = Mathf.Lerp(_currentFov, _isAiming ? AimFOV : BaseFov, fovLerp);
            CharacterManagerRef.CameraManagerRef.CurrentStateBase.SetFOV(_currentFov);

            if (aimingState != _isAiming)
            {
                CharacterManagerRef.weaponUIManagerRef.SetCursor(_isAiming);
            }
            
            if (_isAiming)
            {
                Quaternion kayakTransformRotation = CharacterManagerRef.KayakController.transform.rotation;
                float targetAngle = CharacterManagerRef.CameraManagerRef.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(kayakTransformRotation.eulerAngles.x, targetAngle, kayakTransformRotation.eulerAngles.z));
                Quaternion rotation = Quaternion.Lerp(kayakTransformRotation,targetRotation,CharacterManagerRef.BoatFollowAimLerp);
                CharacterManagerRef.KayakController.transform.rotation = rotation;
            }
        }

        private void HandleShoot()
        {
            if (CharacterManagerRef.InputManagement.Inputs.Shoot && _currentWeaponCooldown <= 0 && _weaponPrefab != null)
            {
                _currentWeaponCooldown = _weaponPrefab.Data.Cooldown;
                Projectile projectile = GameObject.Instantiate(_weaponPrefab, CharacterManagerRef.transform.position, Quaternion.identity);
                projectile.Owner = CharacterManagerRef.gameObject;
            }
        }

        #endregion
        
    }
}