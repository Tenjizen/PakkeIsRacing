using System.Collections.Generic;
using Fight;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CameraData", order = 1)]
    public class CameraData : ScriptableObject
    {
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;
        
        [Header("Rotation Values")]
        public float BalanceRotationMultiplier = 1f;
        [Range(0, 0.1f)] 
        public float BalanceRotationZLerp = 0.01f;

        [Header("Camera")]
        [Range(0, 10)] 
        public float MultiplierValueRotation = 20.0f;
        [Range(0, 0.1f), Tooltip("The lerp value applied to the rotation of the camera when the player moves")] 
        public float LerpLocalRotationMove = 0.005f;
        [Range(0, 10)] 
        public float MultiplierValuePosition = 2;
        [Range(0, 0.1f), Tooltip("The lerp value applied to the position of the camera when the player moves")] 
        public float LerpLocalPositionMove = .005f;
        
        [Range(0, 5)] 
        public float MultiplierFovCamera = 1;

        [Header("Input rotation smooth values")]
        [Range(0, 0.1f), Tooltip("The lerp value applied to the mouse/stick camera movement X input value when released")]
        public float LerpTimeX = 0.02f;
        [Range(0, 0.1f), Tooltip("The lerp value applied to the mouse/stick camera movement Y input value when released")]
        public float LerpTimeY = 0.06f;
        [Range(0, 10), Tooltip("The time it takes the camera to move back behind the boat after the last input")]
        public float TimerCameraReturnBehindBoat = 3.0f;
        [Tooltip("The curve of force in function of the joystick position X")]
        public AnimationCurve JoystickFreeRotationX;
        [Tooltip("The curve of force in function of the joystick position Y")]
        public AnimationCurve JoystickFreeRotationY;

        [Header("Lerp")]
        [Range(0, 0.1f), Tooltip("The lerp value applied to the field of view of camera depending on the speed of the player")]
        public float LerpFOV = .01f;
        [Range(0, 0.1f), Tooltip("The lerp value applied to the position of the camera when the player is not moving")]
        public float LerpLocalPositionNotMoving = 0.01f;
        [Range(0, 0.1f), Tooltip("The lerp value applied to the position of the camera when the player is not moving")]
        public float LerpLocalRotationNotMoving = 0.01f;

        [Header("Death")]
        [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float ValueAddForTopDownWhenDeath = 0.1f;
        [Tooltip("The value to add for the camera to move backwards")]
        public float ValueAddForDistanceWhenDeath = 0.05f;
        [Tooltip("The value that the camera distance should reach")]
        public float MaxValueDistanceToStartDeath = 10f;

        [Header("Respawn")]
        public float CameraDistanceRespawn = 25;
        public float MultiplyTimeForDistanceWhenRespawn = 5;
        public float CameraAngleTopDownRespawn = 28;
        public float MultiplyTimeForTopDownWhenRespawn = 6;

        [Header("Shake Camera")]
        public float AmplitudeShakeWhenUnbalanced = 0.5f;
        public float AmplitudeShakeWhenWaterFlow = 0.2f;
        public float AmplitudeShakeWhenWaterWave = 0.2f;
        
        [Header("Combat")]
        public Vector3 CombatOffset = new Vector3(-1, -1, 0);
        public float CombatFov = 40f;
        public float CombatZoomFov = 20f;
        [Range(0,1)] public float CombatZoomFovLerp = 0.1f;
        public Vector2 HeightClamp = new Vector2(-30, 30);
    }
}