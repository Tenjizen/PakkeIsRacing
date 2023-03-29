using System;
using UnityEngine;
using UnityEngine.Serialization;
using WaterAndFloating;

namespace Kayak.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/KayakData", order = 1)]
    public class KayakData : ScriptableObject
    {
        [Header("Drag"), Range(50,51f), Tooltip("The multiplier of the current velocity to reduce drag -> velocity * DragReducingMultiplier * deltaTime")] 
        public float DragReducingMultiplier = 50.5f;
        
        [Header("Parameters"), Tooltip("The different values related to the kayak")] 
        public KayakParameters KayakValues;
        
        [field:SerializeField, Tooltip("The audio clip of the paddling"), Header("Audio")] 
        public AudioClip PaddlingAudioClip { get; private set; }
        [field:SerializeField, Tooltip("The audio clip of the kayak colliding")] 
        public AudioClip CollisionAudioClip { get; private set; }

        [Header("VFX"), SerializeField]
        public float TimeToPlayParticlesAfterPaddle;
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