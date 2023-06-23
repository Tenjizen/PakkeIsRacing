using Cinemachine;
using UnityEngine;

namespace Character.Camera.State
{
    public class CameraNavigationState : CameraStateBase
    {
        private float _timerCameraReturnBehindBoat = 0;
        private bool _startMoving = false;

        public override void EnterState(CameraManager camera)
        {
            _timerCameraReturnBehindBoat = CamManager.Data.TimerCameraReturnBehindBoat + 1;
            Time.timeScale = 1;
            CamManager.LastInputX = 0;
            CamManager.LastInputY = 0;


            CamManager.CameraAnimator.Play("FreeLook");
            CamManager.Brain.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.LateUpdate;

            CamManager.ShakeCameraNavigating(0);
            CamManager.ResetNavigationValue();
        }
        public override void UpdateState(CameraManager camera)
        {
            if (Mathf.Abs(CamManager.RotationZ) > 0)
            {
                CamManager.SmoothResetRotateZ();
            }

            if (CamManager.Input.Inputs.ResetCamera)
            {
                _timerCameraReturnBehindBoat += CamManager.Data.TimerCameraReturnBehindBoat + 1;
                _startMoving = true;
            }

            MoveCamera(camera);

            camera.MakeTargetFollowRotationWithKayak();

            ClampRotationCameraValue(CamManager.Data.BottomClamp, CamManager.Data.TopClamp, float.MinValue, float.MaxValue);

            if (CamManager.Waves.CircularWavesDurationList.Count > 0)
            {
                CamManager.ShakeCameraWarning(CamManager.Data.AmplitudeShakeWhenWaterWave);
            }
            else if (CamManager.WaterFlow == true)
            {
                CamManager.ShakeCameraWarning(CamManager.Data.AmplitudeShakeWhenWaterFlow);
            }
            else
            {
                var velocity = Mathf.Abs(CamManager.RigidbodyKayak.velocity.x) + Mathf.Abs(CamManager.RigidbodyKayak.velocity.z);
                if (velocity > 1)
                {
                    CamManager.ShakeCameraNavigating(CamManager.Data.AmplitudeShakeMinimumWhenNavigating + Mathf.Clamp(velocity, 0, 20) / CamManager.Data.DivideVelocityPlayer);
                }
                else
                {
                    CamManager.ShakeCameraNavigating(CamManager.Data.AmplitudeShakeMinimumWhenNavigating);
                }
            }
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void LateUpdate(CameraManager camera)
        {

            //rotate freely with inputs
            bool rotateInput = Mathf.Abs(CamManager.Input.Inputs.RotateCamera.x) + Mathf.Abs(CamManager.Input.Inputs.RotateCamera.y) >= CamManager.Input.Inputs.Deadzone; //0.5f;
            const float minimumVelocityToReplaceCamera = 0.05f;
            bool rotateCamClick = CamManager.Input.Inputs.ResetCamera;

            _timerCameraReturnBehindBoat += Time.deltaTime;

       

            if (rotateInput && CamManager.CanMoveCameraManually /*&& rotateCamClick*/)
            {
                _startMoving = false;
                camera.MoveTargetInFreeLook();
                camera.CameraDistanceFreeLook(camera.VirtualCameraFreeLook);
            }

            //manage rotate to stay behind boat
            else if (Mathf.Abs(CamManager.RigidbodyKayak.velocity.x) + Mathf.Abs(CamManager.RigidbodyKayak.velocity.z) > minimumVelocityToReplaceCamera && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat ||
                     (Mathf.Abs(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY) > minimumVelocityToReplaceCamera) && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat ||
                    _startMoving == true && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat)
            {
                camera.CameraDistance(camera.VirtualCameraFreeLook);
                camera.ResetTargetPos();
                _startMoving = true;
                #region clavier souris
                //avoid last input to be 0
                if (CamManager.LastInputX != 0 || CamManager.LastInputY != 0)
                {
                    CamManager.LastInputX = 0;
                    CamManager.LastInputY = 0;
                }
                #endregion
                #region variable

                //get target rotation
                Quaternion localRotation = CamManager.CinemachineCameraTarget.transform.localRotation;

                Quaternion targetQuaternion = Quaternion.Euler(new Vector3(CamManager.Data.BaseRotation.x,
                    (-(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY + CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY) * CamManager.Data.MultiplierValueRotation) * 20,
                    localRotation.z));

                Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

                //const float rotationThreshold = 0.15f;
                const float rotationThreshold = 0.01f;
                float rotationStaticY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY;
                float rotationPaddleY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY;

                #endregion
                if (CharacterManager.Instance.SprintInProgress == true
                    && Mathf.Abs(CamManager.RigidbodyKayak.velocity.x) + Mathf.Abs(CamManager.RigidbodyKayak.velocity.z) > CharacterManager.Instance.KayakControllerProperty.Data.KayakValues.MaximumFrontVelocity + 1)
                {
                    //CamManager.Data.SprintPosition 
                    cameraTargetLocalPosition = Vector3.Lerp(cameraTargetLocalPosition, CamManager.Data.SprintPosition, Time.deltaTime * CamManager.Data.LerpSprint);

                    //CamManager.Data.SprintRotation 
                    targetQuaternion = Quaternion.Euler(CamManager.Data.SprintRotation);

                    CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, targetQuaternion, Time.deltaTime * CamManager.Data.LerpSprint);

                    //CamManager.Data.SprintDistance 
                    var Dist = camera.VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance;

                    camera.VirtualCameraFreeLook.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = Mathf.Lerp(Dist, CamManager.Data.SprintDistance, Time.deltaTime * CamManager.Data.LerpSprint);

                    //CamManager.Data.SprintFOV
                    camera.VirtualCameraFreeLook.m_Lens.FieldOfView = Mathf.Lerp(camera.VirtualCameraFreeLook.m_Lens.FieldOfView, camera.Data.SprintFOV, Time.deltaTime * CamManager.Data.LerpSprint);

                }
                //calculate camera rotation & position
                else if (Mathf.Abs(rotationStaticY) > rotationThreshold || // if kayak is rotating
                    Mathf.Abs(rotationPaddleY) > rotationThreshold) //if kayak moving
                {
                    if (Mathf.Abs(rotationPaddleY) > rotationThreshold / 2)// if kayak is moving
                    {
                        //rotation
                        targetQuaternion = Quaternion.Euler(
                            new Vector3(CamManager.Data.BaseRotation.x - CamManager.RigidbodyKayak.velocity.magnitude * 0.2f,
                    (-(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY + CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY) * CamManager.Data.MultiplierValueRotation) * 20,
                    localRotation.z));

                        CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, targetQuaternion, CamManager.Data.LerpRotationWhenPlayerMoving * Time.deltaTime * 100);

                        //position
                        cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x,
                            (rotationStaticY + rotationPaddleY) * CamManager.Data.MultiplierValuePosition, //value
                            CamManager.Data.LerpPositionWhenMoving * Time.deltaTime * 100); //time lerp

                        cameraTargetLocalPosition.y = Mathf.Lerp(cameraTargetLocalPosition.y,
                                                       CamManager.Data.BasePosition.y, //value
                                                        CamManager.Data.LerpPositionWhenMoving * Time.deltaTime * 100); //time lerp
                        cameraTargetLocalPosition.z = 0;
                    }
                    else if (Mathf.Abs(rotationStaticY) > rotationThreshold / 2)// if kayak is rotating
                    {
                        //position
                        cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, CamManager.Data.LerpPositionWhenRotating * Time.deltaTime * 100);

                        //rotation
                        CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, targetQuaternion, CamManager.Data.LerpRotationWhenPlayerRotating * Time.deltaTime * 100);
                    }
                }
                else if (_startMoving == true)//if kayak not moving or rotating & cam start moving
                {
                    CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(CamManager.Data.BaseRotation.x, CamManager.Data.BaseRotation.y, localRotation.z)), CamManager.Data.LerpRotationNotMoving * Time.deltaTime * 100);
                    cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, CamManager.Data.LerpPositionNotMoving * Time.deltaTime * 100);
                }

                //apply camera rotation & position
                CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);

            }
            CamManager.ApplyRotationCamera();
        }
        public override void SwitchState(CameraManager camera)
        {

        }
        private void MoveCamera(CameraManager camera)
        {
            //rotate freely with inputs
            bool rotateInput = Mathf.Abs(CamManager.Input.Inputs.RotateCamera.x) + Mathf.Abs(CamManager.Input.Inputs.RotateCamera.y) >= CamManager.Input.Inputs.Deadzone; //0.5f;
            const float minimumVelocityToReplaceCamera = 0.05f;
            _timerCameraReturnBehindBoat += Time.deltaTime;

            bool rotateCamClick = CamManager.Input.Inputs.ResetCamera;

            if (rotateInput && CamManager.CanMoveCameraManually /*&& rotateCamClick*/)
            {
                _startMoving = false;
                ManageFreeCameraMove(ref _timerCameraReturnBehindBoat, CameraMode.Navigation);
            }
            //manage rotate to stay behind boat
            else if (Mathf.Abs(CamManager.RigidbodyKayak.velocity.x) + Mathf.Abs(CamManager.RigidbodyKayak.velocity.z) > minimumVelocityToReplaceCamera && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat ||
                     (Mathf.Abs(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY) > minimumVelocityToReplaceCamera) && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat ||
                     _startMoving == true && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat)
            {
                _startMoving = true;

                #region clavier souris
                //avoid last input to be 0
                if (CamManager.LastInputX != 0 || CamManager.LastInputY != 0)
                {
                    CamManager.LastInputX = 0;
                    CamManager.LastInputY = 0;
                }
                #endregion

                #region variable

                //get camera local position
                Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

                const float rotationThreshold = 0.15f;
                float rotationStaticY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY;
                float rotationPaddleY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY;

                #endregion

                //apply camera rotation & position
                CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);
            }
            else
            {
                CamManager.LastInputValue();
            }





        }
    }
}
