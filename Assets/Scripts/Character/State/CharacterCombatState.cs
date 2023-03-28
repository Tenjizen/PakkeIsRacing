using System;
using Character.Camera;
using Character.Camera.State;
using Fight;
using UI.WeaponWheel;
using UnityEngine;

namespace Character.State
{
    public class CharacterCombatState : CharacterStateBase
    {
        private Weapon _weapon;
        private bool _isAiming;

        private Projectile _weaponPrefab;
        private float _currentWeaponCooldown;

        private bool _projectileInShoot;
        
        #region Constructor

        public CharacterCombatState(CharacterManager characterManagerRef, MonoBehaviour monoBehaviour, CameraManager cameraManagerRef) : base(characterManagerRef, monoBehaviour, cameraManagerRef)
        {
        }

        #endregion

        #region Base methods

        public override void EnterState(CharacterManager character)
        {
            CharacterManagerRef.WeaponUIManagerProperty.SetPaddleDownImage(true);
            
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
            
            //_currentWeaponCooldown = _weaponPrefab.Data.Cooldown;
        }

        public override void UpdateState(CharacterManager character)
        {
            if (CharacterManagerRef.InputManagementProperty.Inputs.DeselectWeapon)
            {
                CameraNavigationState cameraNavigationState = new CameraNavigationState(CameraManagerRef, MonoBehaviourRef);
                CameraManagerRef.SwitchState(cameraNavigationState);
                
                CharacterNavigationState characterNavigationState = new CharacterNavigationState(CharacterManagerRef.KayakControllerProperty, CharacterManagerRef.InputManagementProperty, CharacterManagerRef, MonoBehaviourRef, CameraManagerRef);
                CharacterManagerRef.SwitchState(characterNavigationState);

                CharacterManagerRef.WeaponUIManagerProperty.SetPaddleDownImage(false);
                CharacterManagerRef.WeaponUIManagerProperty.SetCursor(false);
                CharacterManagerRef.WeaponUIManagerProperty.SetCooldownUI(0);
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

        public override void ExitState(CharacterManager character)
        {
            
        }

        #endregion

        #region Methods

        private void ManageCooldown()
        {
            if (_projectileInShoot == false)
            {
                _currentWeaponCooldown -= Time.deltaTime;
                float value = _currentWeaponCooldown / _weaponPrefab.Data.Cooldown;
                CharacterManagerRef.WeaponUIManagerProperty.SetCooldownUI(value);
            }
        }
        
        private void HandleAim()
        {
            bool aimingState = _isAiming;
            _isAiming = CharacterManagerRef.InputManagementProperty.Inputs.Aim;
            
            //aim directly
            _isAiming = true;

            //cursor
            if (aimingState != _isAiming)
            {
                CharacterManagerRef.WeaponUIManagerProperty.SetCursor(_isAiming);
            }

            //kayak rotation
            Quaternion kayakTransformRotation = CharacterManagerRef.KayakControllerProperty.transform.rotation;
            float targetAngle = CharacterManagerRef.CameraManagerProperty.CinemachineCameraFollowCombat.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(kayakTransformRotation.eulerAngles.x, targetAngle, kayakTransformRotation.eulerAngles.z));
            Quaternion rotation = Quaternion.Lerp(kayakTransformRotation,targetRotation,CharacterManagerRef.BoatFollowAimLerp);
            CharacterManagerRef.KayakControllerProperty.transform.rotation = rotation;
        }

        private void HandleShoot()
        {
            if (CharacterManagerRef.InputManagementProperty.Inputs.Shoot && _currentWeaponCooldown <= 0 && _weaponPrefab != null)
            {
                _currentWeaponCooldown = _weaponPrefab.Data.Cooldown;
                _projectileInShoot = true;
                
                Projectile projectile = GameObject.Instantiate(_weaponPrefab, CharacterManagerRef.transform.position, Quaternion.identity);
                
                GameObject owner = CharacterManagerRef.gameObject;
                projectile.SetOwner(owner);
                
                projectile.Data.ForbiddenColliders.Add(owner);
                projectile.Data.ForbiddenColliders.Add(CharacterManagerRef.KayakControllerProperty.gameObject);

                const float pointDistance = 30f;
                projectile.Launch(MathTools.GetDirectionToPointCameraLooking(CharacterManagerRef.transform, pointDistance));
                
                projectile.OnProjectileDie.AddListener(ProjectileHit);
            }
        }

        private void ProjectileHit()
        {
            _projectileInShoot = false;
        }

        #endregion
        
    }
}