using System;
using System.Collections.Generic;
using Character;
using Character.State;
using Sound;
using UnityEngine;
using UnityEngine.Serialization;
using WaterAndFloating;

namespace Kayak
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class KayakController : MonoBehaviour
    {
        //TODO to scriptable object
        [Header("Drag")]
        [SerializeField, Range(50,51f), Tooltip("The multiplier of the current velocity to reduce drag -> velocity * DragReducingMultiplier * deltaTime")] 
        private float _dragReducingMultiplier = 50.5f;
        [ReadOnly, Tooltip("If this value is <= 0, the drag reducing will be activated")] 
        public float DragReducingTimer;
        
        [Header("Parameters")]
        [Tooltip("The different values related to the kayak")]
        public KayakParameters KayakValues;
        [ReadOnly, Tooltip("= is the drag reducing method activated ?")] 
        public bool CanReduceDrag = true;
        
        [field:SerializeField, Header("References"), Tooltip("Reference of the character manager in the scene")] 
        public CharacterManager CharacterManagerProperty { get; private set; }
        
        [field:SerializeField, Tooltip("Reference of the kayak rigidbody")]
        public Rigidbody Rigidbody { get; private set; }
        
        [SerializeField, Tooltip("The floaters associated to the kayak's rigidbody")] 
        public Floaters FloatersRef;
        
        [Header("Audio")] 
        [Tooltip("The audio clip of the paddling")]
        public AudioClip PaddlingAudioClip;
        [SerializeField, Tooltip("The audio clip of the kayak colliding")] 
        private AudioClip CollisionAudioClip;

        [Header("VFX")] 
        [SerializeField] private float _timeToPlayParticlesAfterPaddle;
        [SerializeField] private ParticleSystem _leftPaddleParticle, _rightPaddleParticle;
        private float _particleTimer = -1;
        private CharacterNavigationState.Direction _particleSide;

        private void Start()
        {
            Rigidbody = GetComponent<Rigidbody>();

        }
        private void Update()
        {
            ClampVelocity();
            ManageParticlePaddle();
        }

        private void FixedUpdate()
        {
            DragReducing();
        }

        private void OnCollisionEnter(Collision collision)
        {
            float value = collision.relativeVelocity.magnitude / KayakValues.CollisionToBalanceMagnitudeDivider;
            Debug.Log($"collision V.M. :{Math.Round(collision.relativeVelocity.magnitude)} -> {Math.Round(value,2)}");
            CharacterManagerProperty.AddBalanceValueToCurrentSide(value);
            SoundManager.Instance.PlaySound(CollisionAudioClip);
        }

        /// <summary>
        /// Clamp the kayak velocity x & z between -maximumFrontVelocity & maximumFrontVelocity
        /// </summary>
        private void ClampVelocity()
        {
            Vector3 velocity = Rigidbody.velocity;
            
            float velocityX = velocity.x;
            velocityX = Mathf.Clamp(velocityX, -KayakValues.MaximumFrontVelocity, KayakValues.MaximumFrontVelocity);
            
            float velocityZ = velocity.z;
            velocityZ = Mathf.Clamp(velocityZ, -KayakValues.MaximumFrontVelocity, KayakValues.MaximumFrontVelocity);
            
            Rigidbody.velocity = new Vector3(velocityX, velocity.y, velocityZ);
        }

        /// <summary>
        /// Artificially reduce the kayak drag to let it slide longer on water
        /// </summary>
        private void DragReducing()
        {
            if (DragReducingTimer > 0 || CanReduceDrag == false)
            {
                DragReducingTimer -= Time.deltaTime;
                return;
            }
            
            Vector3 velocity = Rigidbody.velocity;
            float absX = Mathf.Abs(velocity.x);
            float absZ = Mathf.Abs(velocity.z);

            if (absX + absZ > 1)
            {
                Rigidbody.velocity = new Vector3(
                    velocity.x * _dragReducingMultiplier * Time.deltaTime, 
                      velocity.y, 
                    velocity.z * _dragReducingMultiplier * Time.deltaTime);
            }
        }

        public void PlayPaddleParticle(CharacterNavigationState.Direction side)
        {
            _particleTimer = _timeToPlayParticlesAfterPaddle;
            _particleSide = side;
        }

        private void ManageParticlePaddle()
        {
            if (_particleTimer > 0)
            {
                _particleTimer -= Time.deltaTime;
                if (_particleTimer <= 0)
                {
                    _particleTimer = -1;
                    switch (_particleSide)
                    {
                        case CharacterNavigationState.Direction.Left:
                            if (_leftPaddleParticle != null)
                            {
                                _leftPaddleParticle.Play();
                            }
                            break;
                        case CharacterNavigationState.Direction.Right:
                            if (_rightPaddleParticle != null)
                            {
                                _rightPaddleParticle.Play();
                            }
                            break;
                    }
                }
            }
        }
    }

    [Serializable]
    public struct KayakParameters
    {
        [Header("Velocity & Collisions")]
        [Range(0,40), Tooltip("the maximum velocity that the kayak can go")] 
        public float MaximumFrontVelocity;
        [Range(0,40), Tooltip("the divider of the collision magnitude value applied to the balance")] 
        public float CollisionToBalanceMagnitudeDivider;

        [Header("Paddle")]
        [Range(0,2), Tooltip("The rotation force that each paddle will apply to the kayak rotation")] 
        public float PaddleSideRotationForce;
        [Range(0,3), Tooltip("The cooldown to use each paddle")] 
        public float PaddleCooldown;
        [Range(0, 0.25f), Tooltip("The lerp value applied to the rotation force of the kayak, the higher it is the faster the kayak will stop rotating")] 
        public float PaddleRotationDeceleration;
        [Range(0,200), Tooltip("The raw front force applied to the kayak when paddling once")] 
        public float PaddleForce;
        [Range(0,30), Tooltip("The number of times the kayak will \"paddle\" when pressing the input once")] 
        public int NumberOfForceAppliance;
        [Range(0,0.2f), Tooltip("The time between each of those paddling, the lower it is the seamless the kayak paddling is")] 
        public float TimeBetweenEveryAppliance;
        [Tooltip("The curve of force applied to the different appliance of paddling")]
        public AnimationCurve ForceCurve;

        [Header("Static Rotation")] 
        [Range(0, 0.1f), Tooltip("The force applied on rotation when static rotating")] 
        public float StaticRotationForce;
        [Range(0, 0.25f), Tooltip("The lerp deceleration value resetting the static rotation to 0 over time")] 
        public float StaticRotationDeceleration;
        [Range(0, 5f), Tooltip("The cooldown after paddling allowing the player to static rotate ")] 
        public float StaticRotationCooldownAfterPaddle;
        
        [Header("Deceleration")]
        [Range(0,0.1f), Tooltip("The lerp value of the velocity deceleration over time")] 
        public float VelocityDecelerationLerp;
        [Range(0, 0.1f), Tooltip("The lerp value of the rotation velocity deceleration over time")] 
        public float VelocityDecelerationRotationForce;

        [Header("Unbalanced")] 
        [Range(0, 1), Tooltip("The cooldown of the paddle when unbalanced")] 
        public float UnbalancePaddleCooldown;
        [Range(0, 10), Tooltip("The paddle force on balance when unbalanced")] 
        public float UnbalancePaddleForce;
    }

    [Serializable]
    public struct Floaters
    {
        public Floater FrontLeft;
        public Floater FrontRight;
        public Floater BackLeft;
        public Floater BackRight;
    }
}