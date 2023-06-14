using Character.Camera.State;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using WaterAndFloating;
using CameraData = Character.Data.CameraData;

namespace Character.Camera
{
    public class CameraManager : MonoBehaviour
    {
        [Header("Start Menu Game")] public float TimerBeforeCanMovingAtStart = 3f;
        [field: SerializeField] public CameraStateBase CurrentStateBase { get; private set; }
        [field: SerializeField, Header("Properties"), Space(5)] public GameObject CinemachineCameraTarget { get; private set; }
        [field: SerializeField] public GameObject CinemachineCameraTargetFollow { get; private set; }
        [field: SerializeField] public GameObject CinemachineCameraFollowCombat { get; private set; }
        [field: SerializeField] public Animator CameraAnimator { get; private set; }
        [field: SerializeField] public Rigidbody RigidbodyKayak { get; private set; }
        [field: SerializeField, Header("Virtual Camera")] public CinemachineBrain Brain { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera VirtualCameraFreeLook { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera VirtualCameraCombat { get; private set; }
        [field: SerializeField] public Waves Waves { get; private set; }
        [field: SerializeField] public NoiseSettings MyNoiseProfileWhenNavigating { get; private set; }
        [field: SerializeField] public NoiseSettings MyNoiseProfileWarning { get; private set; }


        [Space(5), Header("Camera Data")] public CameraData Data;

        public CharacterManager CharacterManager { get; private set; }
        public InputManagement Input { get; private set; }
        public Cinemachine3rdPersonFollow CinemachineCombat3RdPersonFollow { get; private set; }

        [ReadOnly] public bool CanRotateCamera = true;

        //camera
        [HideInInspector] public float CameraAngleOverride = 0.0f;
        [HideInInspector] public float CameraBaseFov;
        [HideInInspector] public Vector3 CameraTargetBasePos;
        [HideInInspector] public float RotationZ = 0;
        [HideInInspector] public bool CanMoveCameraManually;
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
        [HideInInspector] public Vector3 CombatBaseShoulderOffset;


        private void Awake()
        {
            CameraStartGameState startGameState = new CameraStartGameState();
            CurrentStateBase = startGameState;

            CinemachineCombat3RdPersonFollow = VirtualCameraCombat.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            CombatBaseShoulderOffset = CinemachineCombat3RdPersonFollow.ShoulderOffset;
        }

        private void Start()
        {
            CharacterManager = CharacterManager.Instance;
            Input = CharacterManager.InputManagementProperty;



            CurrentStateBase.Initialize();
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

        private void LateUpdate()
        {
            CurrentStateBase.LateUpdate(this);
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
                CameraBaseFov + (velocityXZ * Data.MultiplierFovCamera),
                Data.LerpFOV);
        }

        public void CameraDistance(CinemachineVirtualCamera virtualCamera)
        {
            float velocityXZ = Mathf.Abs(RigidbodyKayak.velocity.x) + Mathf.Abs(RigidbodyKayak.velocity.z);

            var Dist = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;
            virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = Mathf.Lerp(Dist, Data.NavigationCamDistance - (velocityXZ * Data.MultiplierCameraGettingCloser), Data.LerpCameraGettingCloser);
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
            VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = Data.NavigationCamDistance;
            StartDeath = false;
        }
        public void SmoothResetDistanceValue()
        {
            Cinemachine3rdPersonFollow camFollow = VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            camFollow.CameraDistance = Mathf.Lerp(camFollow.CameraDistance, Data.NavigationCamDistance, Time.deltaTime * 2);
        }


        public void LastInputValue()
        {
            //last input value
            LastInputX = CurrentStateBase.ClampAngle(LastInputX, -5.0f, 5.0f);
            LastInputY = CurrentStateBase.ClampAngle(LastInputY, -1.0f, 1.0f);
            LastInputX = Mathf.Lerp(LastInputX, 0, Data.LerpTimeX);
            LastInputY = Mathf.Lerp(LastInputY, 0, Data.LerpTimeY);

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

            CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(Data.NavigationRotation.x, Data.NavigationRotation.y, localRotation.z)), Data.LerpLocalRotationNotMoving);
            cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, Data.LerpLocalPositionNotMoving);
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

        public void ShakeCameraWarning(float intensity)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = VirtualCameraFreeLook.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            cinemachineBasicMultiChannelPerlin.m_NoiseProfile = MyNoiseProfileWarning;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        }
        public void ShakeCameraNavigating(float intensity)
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = VirtualCameraFreeLook.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (cinemachineBasicMultiChannelPerlin.m_NoiseProfile != MyNoiseProfileWhenNavigating)
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
                cinemachineBasicMultiChannelPerlin.m_NoiseProfile = MyNoiseProfileWhenNavigating;
            }

            if (cinemachineBasicMultiChannelPerlin.m_AmplitudeGain < intensity)
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain += Time.deltaTime;
            }
            else
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
            }
        }

        public void ResetCameraLocalPos()
        {
            Vector3 localPos = CinemachineCameraTarget.transform.localPosition;
            localPos.x = CameraTargetBasePos.x;
            localPos.y = CameraTargetBasePos.y;
            localPos.z = CameraTargetBasePos.z;
            CinemachineCameraTarget.transform.localPosition = localPos;
        }

        public void ResetCameraBehindBoat()
        {
            Quaternion localRotation = CinemachineCameraTarget.transform.localRotation;
            Vector3 cameraTargetLocalPosition = CinemachineCameraTarget.transform.localPosition;

            CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(Data.NavigationRotation.x, 0, localRotation.z)), 1f);
            cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, Data.LerpLocalPositionNotMoving);
            CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);
        }
        #endregion
        public void InitializeCams()
        {
            Transform kayakTransform = CharacterManager.KayakControllerProperty.transform;
            Transform stateDrivenCam = CameraAnimator.gameObject.transform;
            Cinemachine3rdPersonFollow cinemachine3rdPerson = VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

            stateDrivenCam.position = kayakTransform.position;
            stateDrivenCam.eulerAngles = kayakTransform.eulerAngles;

            CinemachineCameraFollowCombat.transform.localPosition = Data.CombatPosition;
            CinemachineCameraTarget.transform.localPosition = Data.NavigationPosition;

            Vector3 targetAngles = CinemachineCameraTarget.transform.localEulerAngles;
            targetAngles.x = Data.NavigationRotation.x;
            targetAngles.y = Data.NavigationRotation.y + kayakTransform.eulerAngles.y;
            targetAngles.z = Data.NavigationRotation.z;
            CinemachineCameraTarget.transform.localEulerAngles = targetAngles;

            cinemachine3rdPerson.CameraDistance = Data.NavigationCamDistance;
            cinemachine3rdPerson.ShoulderOffset = Data.NavigationCamShoulderOffset;

            CinemachineTargetPitch = Data.NavigationRotation.x;
            CinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            CameraTargetBasePos = CinemachineCameraTarget.transform.localPosition;
            CameraBaseFov = VirtualCameraFreeLook.m_Lens.FieldOfView;

        }
    }
}
