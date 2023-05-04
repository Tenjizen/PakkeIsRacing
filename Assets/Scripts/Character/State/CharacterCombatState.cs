using Character.Camera.State;
using Fight;
using UnityEngine;
using Debug = FMOD.Debug;

namespace Character.State
{
    public class CharacterCombatState : CharacterStateBase
    {
        private Projectile _weaponPrefab;

        private bool _isHoldingShoot;
        private float _holdingTime;

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
            CheckBalance();
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
            // Quaternion kayakTransformRotation = CharacterManagerRef.KayakControllerProperty.transform.rotation;
            // float targetAngle = CharacterManagerRef.CameraManagerProperty.CinemachineCameraFollowCombat.transform.rotation.eulerAngles.y;
            // Quaternion targetRotation = Quaternion.Euler(new Vector3(kayakTransformRotation.eulerAngles.x, targetAngle, kayakTransformRotation.eulerAngles.z));
            // Quaternion rotation = Quaternion.Lerp(kayakTransformRotation,targetRotation,CharacterManagerRef.Data.BoatFollowAimLerp);
            // CharacterManagerRef.KayakControllerProperty.transform.rotation = rotation;
        }

        private void HandleShoot()
        {
            CharacterManager character = CharacterManagerRef;
            
            if (character.InputManagementProperty.Inputs.Shoot && character.WeaponCooldown <= 0)
            {
                _isHoldingShoot = true;
                
                //holding vfx
                _holdingTime += Time.deltaTime;
                const float startTime = 0.3f;
                if (_holdingTime > startTime && character.WeaponChargedParticleSystem.isPlaying == false)
                {
                    character.WeaponChargedParticleSystem.Play();
                }
            }

            if (_isHoldingShoot && character.InputManagementProperty.Inputs.Shoot == false)
            {
                if (character.WeaponCooldown <= 0 && _weaponPrefab != null)
                {
                    Vector3 playerPosition = character.transform.position;
                    Projectile projectile = Object.Instantiate(_weaponPrefab, playerPosition, Quaternion.identity);
                   
                    GameObject owner = character.gameObject;
                    projectile.SetOwner(owner);
                    
                    projectile.Data.ForbiddenColliders.Add(owner);
                    projectile.Data.ForbiddenColliders.Add(character.KayakControllerProperty.gameObject);

                    float power = Mathf.Clamp(_holdingTime, 0.5f, 1f);
                    
                    Vector3 direction = MathTools.GetDirectionFromTransformToPointCameraLooking(character.transform, 30f);
                    if (Physics.Raycast(playerPosition, direction, out var hit)) //TODO from cam direction cam.transform.forward
                    {
                        direction = hit.point - character.transform.position;
                    }
                    
                    projectile.Launch(direction.normalized, power);
                    projectile.OnProjectileDie.AddListener(ProjectileHit);
                    
                    character.WeaponCooldown = _weaponPrefab.Data.Cooldown;
                    character.ProjectileIsInAir = true;
                    
                    LaunchNavigationState();
                }

                _isHoldingShoot = false;
            }

            if (character.InputManagementProperty.Inputs.DeselectWeapon)
            {
                LaunchNavigationState();
            }
        }

        private void ProjectileHit()
        {
            CharacterManagerRef.WeaponCooldownBase = _weaponPrefab.Data.Cooldown;
            CharacterManagerRef.ProjectileIsInAir = false;
        }

        private void LaunchNavigationState()
        {
            CharacterManagerRef.WeaponChargedParticleSystem.Stop();

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