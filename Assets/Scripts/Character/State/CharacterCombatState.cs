using Character.Camera.State;
using Fight;
using UnityEngine;

namespace Character.State
{
    public class CharacterCombatState : CharacterStateBase
    {
        private Projectile _weaponPrefab;

        private bool _isHoldingShoot;

        #region Base methods

        public override void EnterState(CharacterManager character)
        {
            CharacterManagerRef.WeaponUIManagerProperty.SetPaddleDownImage(true);
            
            _weaponPrefab = CharacterManagerRef.CurrentProjectile;
            
            CharacterManagerRef.WeaponUIManagerProperty.SetCursor(true);
        }

        public override void UpdateState(CharacterManager character)
        {
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

        private void HandleAim()
        {
            //kayak rotation
            Quaternion kayakTransformRotation = CharacterManagerRef.KayakControllerProperty.transform.rotation;
            float targetAngle = CharacterManagerRef.CameraManagerProperty.CinemachineCameraFollowCombat.transform.rotation.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(kayakTransformRotation.eulerAngles.x, targetAngle, kayakTransformRotation.eulerAngles.z));
            Quaternion rotation = Quaternion.Lerp(kayakTransformRotation,targetRotation,CharacterManagerRef.Data.BoatFollowAimLerp);
            CharacterManagerRef.KayakControllerProperty.transform.rotation = rotation;
        }

        private void HandleShoot()
        {
            if (CharacterManagerRef.InputManagementProperty.Inputs.Shoot && CharacterManagerRef.WeaponCooldown <= 0)
            {
                _isHoldingShoot = true;
                
                //zoom
                CameraManagerRef.VirtualCameraCombat.m_Lens.FieldOfView -= 1f;
            }

            if (_isHoldingShoot && CharacterManagerRef.InputManagementProperty.Inputs.Shoot == false)
            {
                if (CharacterManagerRef.WeaponCooldown <= 0 && _weaponPrefab != null)
                {
                    CharacterManagerRef.WeaponCooldown = _weaponPrefab.Data.Cooldown;
                    CharacterManagerRef.ProjectileIsInAir = true;
                
                    Projectile projectile = Object.Instantiate(_weaponPrefab, CharacterManagerRef.transform.position, Quaternion.identity);
                
                    GameObject owner = CharacterManagerRef.gameObject;
                    projectile.SetOwner(owner);
                
                    projectile.Data.ForbiddenColliders.Add(owner);
                    projectile.Data.ForbiddenColliders.Add(CharacterManagerRef.KayakControllerProperty.gameObject);

                    const float pointDistance = 30f;
                    projectile.Launch(MathTools.GetDirectionToPointCameraLooking(CharacterManagerRef.transform, pointDistance));
                
                    projectile.OnProjectileDie.AddListener(ProjectileHit);
                
                    LaunchNavigationState();
                }

                _isHoldingShoot = false;
            }
            
        }

        private void ProjectileHit()
        {
            CharacterManagerRef.WeaponCooldownBase = _weaponPrefab.Data.Cooldown;
            CharacterManagerRef.ProjectileIsInAir = false;
        }

        private void LaunchNavigationState()
        {
            CameraNavigationState cameraNavigationState = new CameraNavigationState();
            CameraManagerRef.SwitchState(cameraNavigationState);
                
            CharacterNavigationState characterNavigationState = new CharacterNavigationState();
            CharacterManagerRef.SwitchState(characterNavigationState);

            CharacterManagerRef.WeaponUIManagerProperty.SetPaddleDownImage(false);
            CharacterManagerRef.WeaponUIManagerProperty.SetCursor(false);
            CharacterManagerRef.WeaponUIManagerProperty.SetCooldownUI(0);
        }

        #endregion
        
    }
}