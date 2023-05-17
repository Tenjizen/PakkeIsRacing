using System;
using Art.Script;
using Character.Data.Character;
using Fight;
using Fight.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Character.State
{
    public class CharacterCombatState : CharacterStateBase
    {
        private Projectile _weaponPrefab;

        private bool _isHoldingShoot;
        private float _holdingTime;

        private IHittable _hittable;
        private float _currentHittableAimTime;

        #region Base methods

        public override void EnterState(CharacterManager character)
        {
            CharacterManagerRef.WeaponUIManagerProperty.SetCombatWeaponUI(true);
            
            _weaponPrefab = CharacterManagerRef.CurrentProjectile;
            
            CharacterManagerRef.WeaponUIManagerProperty.AutoAimController.ShowAutoAimCircle(true);

            switch (_weaponPrefab.Data.Type)
            {
                case WeaponType.Harpoon:
                    CharacterManagerRef.IKPlayerControl.CurrentType = IKType.Harpoon;
                    CharacterManagerRef.IKPlayerControl.SetHarpoon();
                    CharacterManagerRef.HarpoonAnimator.SetBool("IdleHarpoon",true);
                    break;
                case WeaponType.Net:
                    CharacterManagerRef.IKPlayerControl.CurrentType = IKType.Net;
                    CharacterManagerRef.IKPlayerControl.SetNet();
                    CharacterManagerRef.HarpoonAnimator.SetBool("IdleNet",true);
                    break;
            }

        }

        public override void UpdateState(CharacterManager character)
        {
            CheckAutoAim();
            HandleShoot();
            CheckBalance();
        }

        public override void FixedUpdate(CharacterManager character)
        {
            CheckRigidbodyFloatersBalance();
        }

        public override void SwitchState(CharacterManager character)
        {
        }

        public override void ExitState(CharacterManager character)
        {
            CharacterManagerRef.WeaponChargedParticleSystem.Stop();

            CharacterManagerRef.WeaponUIManagerProperty.SetCombatWeaponUI(false);
            CharacterManagerRef.WeaponUIManagerProperty.SetCooldownUI(0);

            CharacterManagerRef.WeaponUIManagerProperty.AutoAimController.ShowAutoAimCircle(false);
            CharacterManagerRef.WeaponUIManagerProperty.AutoAimController.ShowAutoAimUI(false);
            
            //CharacterManagerRef.IKPlayerControl.SetPaddle();
            CharacterManagerRef.IKPlayerControl.CurrentType = IKType.Paddle;
        }

        #endregion

        #region Methods

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
                    
                    //projectile creation
                    Projectile projectile = Object.Instantiate(_weaponPrefab, CharacterManagerRef.WeaponSpawnPosition.position, Quaternion.identity);
                    GameObject owner = character.gameObject;
                    projectile.SetOwner(owner);
                    projectile.Data.ForbiddenColliders.Add(owner);
                    projectile.Data.ForbiddenColliders.Add(character.KayakControllerProperty.gameObject);

                    //add power and direction
                    float power = Mathf.Clamp(_holdingTime, 0.5f, 1f);
                    Vector3 direction = MathTools.GetDirectionFromTransformToPointCameraLooking(CharacterManagerRef.transform, 30f);

                    //raycast to center
                    UnityEngine.Camera mainCamera = UnityEngine.Camera.main;
                    Vector3 screenCenter = new Vector3(0.5f, 0.5f, 0f);
                    Vector3 rayDirection = mainCamera.ViewportPointToRay(screenCenter).direction;
                    Ray ray = new Ray(mainCamera.transform.position, rayDirection);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        direction = hit.point - CharacterManagerRef.transform.position;
                    }
                    
                    //auto aim
                    IHittable hittable = GetAutoAimHittable();
                    if (hittable != null)
                    {
                        direction = hittable.Transform.position - CharacterManagerRef.transform.position;
                    }

                    projectile.Launch(direction.normalized, power);
                    projectile.OnProjectileDie.AddListener(ProjectileHit);
                    
                    character.WeaponCooldown = _weaponPrefab.Data.Cooldown;
                    character.ProjectileIsInAir = true;

                    if (_hittable != null)
                    {
                        projectile.SetHittableAutoAim(_hittable);
                    }
                    
                    //anim
                    switch (_weaponPrefab.Data.Type)
                    {
                        case WeaponType.Harpoon:
                            CharacterManagerRef.IKPlayerControl.SetHarpoon();
                            CharacterManagerRef.HarpoonAnimator.SetTrigger("FireHarpoon");
                            break;
                        case WeaponType.Net:
                            CharacterManagerRef.IKPlayerControl.SetNet();
                            CharacterManagerRef.HarpoonAnimator.SetTrigger("FireNet");
                            break;
                    }
                    
                    LaunchNavigationState();
                }

                _isHoldingShoot = false;
            }

            if (character.InputManagementProperty.Inputs.DeselectWeapon)
            {
                LaunchNavigationState();
            }
        }

        private void CheckAutoAim()
        {
            //raycast
            RaycastHit[] hits = new RaycastHit[100];
            Transform camera = UnityEngine.Camera.main.transform;
            CharacterData data = CharacterManagerRef.Data;
            bool hasFoundAHittable = false;

            for (int i = 0; i < data.AutoAimNumberOfCastStep; i++)
            {
                float positionMultiplier = Mathf.Clamp( (data.AutoAimDistanceBetweenEachStep * i), 1, 10000);
                Vector3 newPosition = camera.position + camera.forward * positionMultiplier;
                
                float radiusMultiplier = Mathf.Clamp( Vector3.Distance(camera.position, newPosition)/5,1,10000);
                float radius = data.AutoAimSize * radiusMultiplier;
                
                int hitCount = Physics.SphereCastNonAlloc(newPosition, radius, camera.forward, hits, data.AutoAimDistanceToSweepEachStep);
                
                for (int j = 0; j < hitCount; j++)
                {
                    if (hits[i].collider == null)
                    {
                        continue;
                    }
                    
                    IHittable hittable = hits[i].collider.GetComponent<IHittable>();
                    if (hittable == null)
                    {
                        continue;
                    }
                    
                    hasFoundAHittable = true;
                    _hittable = hittable;
                    goto LoopEnd;
                }
            }
            LoopEnd:
            if (hasFoundAHittable == false)
            {
                _hittable = null;
            }
            
            //if null
            if (_hittable == null)
            {
                if (_currentHittableAimTime != 0)
                {
                    CharacterManagerRef.WeaponUIManagerProperty.AutoAimController.ShowAutoAimUI(false);
                }
                _currentHittableAimTime = 0;
                return;
            }
            
            //manage 
            if (_currentHittableAimTime == 0)
            {
                CharacterManagerRef.WeaponUIManagerProperty.AutoAimController.ShowAutoAimUI(true);
            }
            _currentHittableAimTime += Time.deltaTime;
            float percentage = _currentHittableAimTime / CharacterManagerRef.Data.TimeToAutoAim;
            CharacterManagerRef.WeaponUIManagerProperty.AutoAimController.SetAutoAimUI(percentage, _hittable.Transform.position);
        }

        private IHittable GetAutoAimHittable()
        {
            if (_currentHittableAimTime >= CharacterManagerRef.Data.TimeToAutoAim && _hittable != null && CharacterManagerRef.Parameters.AutoAim)
            {
                return _hittable;
            }

            return null;
        }

        private void ProjectileHit()
        {
            CharacterManagerRef.WeaponCooldownBase = _weaponPrefab.Data.Cooldown;
            CharacterManagerRef.ProjectileIsInAir = false;
        }

        #endregion
        
    }
}