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
        [field:SerializeField, Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp { get; private set; } = 70.0f;
        
        
        [field:SerializeField, Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp { get; private set; } = -30.0f;
        
        
        [field:SerializeField, Header("Rotation Values")]
        public float BalanceRotationMultiplier { get; private set; } = 1f;
        
        
        [field:SerializeField, Range(0, 0.1f)] 
        public float BalanceRotationZLerp { get; private set; } = 0.01f;

        
        [field:SerializeField, Header("Camera"), Range(0, 10)] 
        public float MultiplierValueRotation { get; private set; } = 20.0f;
        
        
        [field:SerializeField, Range(0, 0.1f), Tooltip("The lerp value applied to the rotation of the camera when the player moves")] 
        public float LerpLocalRotationMove { get; private set; } = 0.005f;
        
        
        [field:SerializeField, Range(0, 10)] 
        public float MultiplierValuePosition { get; private set; } = 2;
        
        
        [field:SerializeField, Range(0, 0.1f), Tooltip("The lerp value applied to the position of the camera when the player moves")] 
        public float LerpLocalPositionMove { get; private set; } = .005f;
        
        
        [field:SerializeField, Range(0, 5)] 
        public float MultiplierFovCamera { get; private set; } = 1;
        
        
        [field:SerializeField, Header("Input rotation smooth values"), Range(0, 0.1f), Tooltip("The lerp value applied to the mouse/stick camera movement X input value when released")]
        public float LerpTimeX { get; private set; } = 0.02f;
        
        
        [field:SerializeField, Range(0, 0.1f), Tooltip("The lerp value applied to the mouse/stick camera movement Y input value when released")]
        public float LerpTimeY { get; private set; } = 0.06f;
        
        
        [field:SerializeField, Range(0, 10), Tooltip("The time it takes the camera to move back behind the boat after the last input")]
        public float TimerCameraReturnBehindBoat { get; private set; } = 3.0f;
        
        
        [field:SerializeField, Tooltip("The curve of force in function of the joystick position X")]
        public AnimationCurve JoystickFreeRotationX { get; private set; }
        
        
        [field:SerializeField, Tooltip("The curve of force in function of the joystick position Y")]
        public AnimationCurve JoystickFreeRotationY { get; private set; } 
        

        [field:SerializeField, Header("Lerp"), Range(0, 0.1f), Tooltip("The lerp value applied to the field of view of camera depending on the speed of the player")]
        public float LerpFOV { get; private set; } = .01f;
        
        
        [field:SerializeField, Range(0, 0.1f), Tooltip("The lerp value applied to the position of the camera when the player is not moving")]
        public float LerpLocalPositionNotMoving { get; private set; } = 0.01f;
        
        
        [field:SerializeField, Range(0, 0.1f), Tooltip("The lerp value applied to the position of the camera when the player is not moving")]
        public float LerpLocalRotationNotMoving { get; private set; } = 0.01f;
        

        [field:SerializeField, Header("Death"), Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
        public float ValueAddForTopDownWhenDeath { get; private set; } = 0.1f;
        
        
        [field:SerializeField, Tooltip("The value to add for the camera to move backwards")]
        public float ValueAddForDistanceWhenDeath { get; private set; } = 0.05f;
        
        
        [field:SerializeField, Tooltip("The value that the camera distance should reach")]
        public float MaxValueDistanceToStartDeath { get; private set; } = 10f;

        
        [field:SerializeField, Header("Respawn")]
        public float CameraDistanceRespawn { get; private set; } = 25;
        
        
        [field:SerializeField]
        public float MultiplyTimeForDistanceWhenRespawn { get; private set; } = 5;
        
        
        [field:SerializeField]
        public float CameraAngleTopDownRespawn { get; private set; } = 28;
        
        
        [field:SerializeField]
        public float MultiplyTimeForTopDownWhenRespawn { get; private set; } = 6;
        

        [field:SerializeField, Header("Shake Camera")]
        public float AmplitudeShakeWhenUnbalanced { get; private set; } = 0.5f;
        
        
        [field:SerializeField]
        public float AmplitudeShakeWhenWaterFlow { get; private set; }  = 0.2f;
        
        
        [field:SerializeField]
        public float AmplitudeShakeWhenWaterWave { get; private set; } = 0.2f;
        
        
        [field:SerializeField, Header("Combat")]
        public Vector3 CombatOffset { get; private set; } = new Vector3(-1, -1, 0);
        
        
        [field:SerializeField]
        public float CombatFov { get; private set; } = 40f;
        
        
        [field:SerializeField]
        public float CombatZoomFov { get; private set; } = 20f;
        
        
        [field:SerializeField, Range(0,1)] 
        public float CombatZoomFovLerp { get; private set; } = 0.1f;
        
        
        [field:SerializeField, Range(0,1)] 
        public float CombatZoomAimSpeedMultiplier { get; private set; } = 0.5f;
        
        
        [field:SerializeField]
        public Vector2 CombatHeightClamp { get; private set; } = new Vector2(-30, 30);
        
        
        [field:SerializeField]
        public Vector2 CombatLengthClamp { get; private set; } = new Vector2(-60, 60);

        [field: SerializeField] public float CameraCombatSensibility { get; private set; } = 1f;
    }
}