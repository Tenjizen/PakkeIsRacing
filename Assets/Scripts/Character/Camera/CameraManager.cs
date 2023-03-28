using Character.Camera.State;
using Cinemachine;
using UnityEngine;
using WaterAndFloating;

namespace Character.Camera
{
    public class CameraManager : MonoBehaviour
    {
    
        [field:SerializeField] public CameraStateBase CurrentStateBase { get; private set; }
        [field:SerializeField, Header("Cinemachine"), Tooltip("Target set in Virtual Camera that the camera follow")] public GameObject CinemachineCameraTarget { get; private set; }
        [field:SerializeField] public GameObject CinemachineCameraTargetFollow { get; private set; }
        [field:SerializeField] public GameObject CinemachineCameraFollowCombat { get; private set; }
        [field:SerializeField] public Animator AnimatorRef { get; private set; }

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [field:SerializeField, Header("References")] public Rigidbody RigidbodyKayak { get; private set; }
        [field:SerializeField] public CharacterManager CharacterManager { get; private set; }
        [field:SerializeField] public InputManagement Input { get; private set; }

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
        [ReadOnly] 
        public bool CanMoveCameraManually = true;
        
        [field:SerializeField, Header("Virtual Camera")] public CinemachineBrain Brain { get; private set; }
        [field:SerializeField] public CinemachineVirtualCamera VirtualCameraFreeLook { get; private set; }
        [field:SerializeField] public CinemachineVirtualCamera VirtualCameraCombat { get; private set; }
        
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
        public float AmplitudShakeWhenUnbalanced = 0.5f;
        public float AmplitudShakeWhenWaterFlow = 0.2f;
        public float AmplitudShakeWhenWaterWave = 0.2f;
        [field:SerializeField] public Waves Waves { get; private set; }

        [Header("Combat")]
        public Vector3 CombatOffset = new Vector3(-1, -1, 0);
        public float CombatFov = 40f;
        public Vector2 HeightClamp = new Vector2(-30, 30);

        //camera
        [Space(5), Header("Infos"), ReadOnly] public float CameraAngleOverride = 0.0f;
        [HideInInspector] public float CameraBaseFov;
        [HideInInspector] public Vector3 CameraTargetBasePos;
        [HideInInspector] public float RotationZ = 0;
        //cinemachine yaw&pitch
        [HideInInspector] public float CinemachineTargetYaw;
        [HideInInspector] public float CinemachineTargetPitch;
        //inputs
        [HideInInspector] public float LastInputX;
        [HideInInspector] public float LastInputY;
        //other
        [HideInInspector] public bool StartDeath = false;
        [HideInInspector] public bool WaterFlow = false;
        //combat values
        [HideInInspector] public Cinemachine3rdPersonFollow CinemachineCombat3rdPersonFollow;
        [HideInInspector] public Vector3 CombatBaseShoulderOffset;

        private void Awake()
        {
            if (Waves == null)
            {
                Debug.LogError("Missing wave reference");
            }

            CinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            CameraTargetBasePos = CinemachineCameraTarget.transform.localPosition;

            CameraBaseFov = VirtualCameraFreeLook.m_Lens.FieldOfView;

            CameraNavigationState navigationState = new CameraNavigationState();
            CurrentStateBase = navigationState;

            CinemachineCombat3rdPersonFollow = VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            CombatBaseShoulderOffset = CinemachineCombat3rdPersonFollow.ShoulderOffset;
        }

        private void Start()
        {
            CurrentStateBase.EnterState(this);
        }
        private void Update()
        {
            CurrentStateBase.UpdateState(this);
            CurrentStateBase.ResetShoulderOffset();

            FieldOfView(VirtualCameraFreeLook);
        }
        private void FixedUpdate()
        {
            CurrentStateBase.FixedUpdate(this);
        }
        public void SwitchState(CameraStateBase stateBaseCharacter)
        {
            CurrentStateBase = stateBaseCharacter;
            stateBaseCharacter.EnterState(this);
        }

        private void FieldOfView(CinemachineVirtualCamera virtualCamera)
        {
            float velocityXZ = Mathf.Abs(RigidbodyKayak.velocity.x) + Mathf.Abs(RigidbodyKayak.velocity.z);
            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView,
                CameraBaseFov + (velocityXZ * MultiplierFovCamera),
                LerpFOV);
        }
        #region Methods
        public void ApplyRotationCamera()
        {
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                CinemachineTargetPitch, //pitch
                CinemachineTargetYaw, //yaw
                RotationZ); //z rotation
        }
        public void ApplyRotationCameraInCombat()
        {
            //CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
            //    0,
            //    //CinemachineTargetPitch, //pitch
            //    //CinemachineCameraTarget.transform.rotation.x, //pitch
            //    CinemachineTargetYaw, //yaw
            //    RotationZ); //z rotation

            CinemachineCameraFollowCombat.transform.rotation = Quaternion.Euler(
                CinemachineTargetPitch, //pitch
                CinemachineTargetYaw, //yaw
                RotationZ); //z rotation
        }
        public void ApplyRotationCameraWhenCharacterDeath()
        {
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                CinemachineTargetPitch + CameraAngleOverride, //pitch
                CinemachineTargetYaw, //yaw
                RotationZ); //z rotation
        }

        public void SmoothResetRotateZ()
        {
            RotationZ = Mathf.Lerp(RotationZ, 0, 0.01f);
        }
        public void ResetNavigationValue()
        {
            const float cameraDistance = 7;
            VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = cameraDistance;
            StartDeath = false;
        }

        public void ResetCameraLocalPos()
        {
            Vector3 localPos = CinemachineCameraTarget.transform.localPosition;
            localPos.x = CameraTargetBasePos.x;
            localPos.y = CameraTargetBasePos.y;
            localPos.z = CameraTargetBasePos.z;
            CinemachineCameraTarget.transform.localPosition = localPos;
        }


        public void LastInputValue()
        {
            //last input value
            LastInputX = CurrentStateBase.ClampAngle(LastInputX, -5.0f, 5.0f);
            LastInputY = CurrentStateBase.ClampAngle(LastInputY, -1.0f, 1.0f);
            LastInputX = Mathf.Lerp(LastInputX, 0, LerpTimeX);
            LastInputY = Mathf.Lerp(LastInputY, 0, LerpTimeY);

            //apply value to camera
            CinemachineTargetYaw += LastInputX;
            CinemachineTargetPitch += LastInputY;

        }

        public void MakeTargetFollowRotationWithKayak()
        {
            Vector3 rotation = CinemachineCameraTargetFollow.transform.rotation.eulerAngles;
            Vector3 kayakRotation = RigidbodyKayak.gameObject.transform.eulerAngles;
            CinemachineCameraTargetFollow.transform.rotation = Quaternion.Euler(new Vector3(rotation.x, kayakRotation.y, rotation.z));
        }

        public void MakeSmoothCameraBehindBoat()
        {
            Quaternion localRotation = CinemachineCameraTarget.transform.localRotation;
            Vector3 cameraTargetLocalPosition = CinemachineCameraTarget.transform.localPosition;

            CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(0, 0, localRotation.z)), LerpLocalRotationNotMoving);
            cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, LerpLocalPositionNotMoving);
            CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);
        }
        public void ResetCameraBehindBoat()
        {
            Quaternion localRotation = CinemachineCameraTarget.transform.localRotation;
            Vector3 cameraTargetLocalPosition = CinemachineCameraTarget.transform.localPosition;

            CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(0, 0, localRotation.z)), 1f);
            cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, LerpLocalPositionNotMoving);
            CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);
        }
        public void CinemachineTargetEulerAnglesToRotation(Vector3 targetLocalPosition)
        {
            CinemachineCameraTarget.transform.localPosition = targetLocalPosition;
            CinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            if (CinemachineCameraTarget.transform.rotation.eulerAngles.x > 180)
            {
                CinemachineTargetPitch = CinemachineCameraTarget.transform.rotation.eulerAngles.x - 360;
            }
            else
            {
                CinemachineTargetPitch = CinemachineCameraTarget.transform.rotation.eulerAngles.x;
            }
        }
        public void ShakeCamera(float intensity)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = VirtualCameraFreeLook.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        }
        #endregion
    }
}
