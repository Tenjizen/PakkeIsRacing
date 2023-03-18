using System;
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
            CharacterManagerRef.weaponUIManagerRef.SetPaddleDownImage(true);
            
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
            if (_projectileInShoot == false)
            {
                _currentWeaponCooldown -= Time.deltaTime;
                float value = _currentWeaponCooldown / _weaponPrefab.Data.Cooldown;
                CharacterManagerRef.weaponUIManagerRef.SetCooldownUI(value);
            }
        }
        
        private void HandleAim()
        {
            bool aimingState = _isAiming;
            _isAiming = CharacterManagerRef.InputManagement.Inputs.Aim;
            
            //aim directly
            _isAiming = true;

            //cursor
            if (aimingState != _isAiming)
            {
                CharacterManagerRef.weaponUIManagerRef.SetCursor(_isAiming);
            }

            //kayak rotation
            Quaternion kayakTransformRotation = CharacterManagerRef.KayakController.transform.rotation;
            float targetAngle = CharacterManagerRef.CameraManagerRef.CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(kayakTransformRotation.eulerAngles.x, targetAngle, kayakTransformRotation.eulerAngles.z));
            Quaternion rotation = Quaternion.Lerp(kayakTransformRotation,targetRotation,CharacterManagerRef.BoatFollowAimLerp);
            CharacterManagerRef.KayakController.transform.rotation = rotation;
        }

        private void HandleShoot()
        {
            if (CharacterManagerRef.InputManagement.Inputs.Shoot && _currentWeaponCooldown <= 0 && _weaponPrefab != null)
            {
                _currentWeaponCooldown = _weaponPrefab.Data.Cooldown;
                _projectileInShoot = true;
                
                Projectile projectile = GameObject.Instantiate(_weaponPrefab, CharacterManagerRef.transform.position, Quaternion.identity);
                
                GameObject owner = CharacterManagerRef.gameObject;
                projectile.Owner = owner;
                
                projectile.Data.ForbiddenColliders.Add(owner);
                projectile.Data.ForbiddenColliders.Add(CharacterManagerRef.KayakController.gameObject);

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