using UnityEngine;

namespace Character.Camera.State
{
    public class CameraNavigationState : CameraStateBase
    {
        private float _timerCameraReturnBehindBoat = 0;

        public override void EnterState(CameraManager camera)
        {
            Time.timeScale = 1;
            CamManager.LastInputX = 0;
            CamManager.LastInputY = 0;

            CamManager.ShakeCamera(0);
            CamManager.AnimatorRef.Play("FreeLook");
            CamManager.Brain.m_BlendUpdateMethod = Cinemachine.CinemachineBrain.BrainUpdateMethod.LateUpdate;

            CamManager.ResetNavigationValue();

        }
        public override void UpdateState(CameraManager camera)
        {
            if (Mathf.Abs(CamManager.RotationZ) > 0)
            {
                CamManager.SmoothResetRotateZ();
            }

            MoveCamera();

            ClampRotationCameraValue(CamManager.BottomClamp, CamManager.TopClamp);

            CamManager.ApplyRotationCamera();

            if (Input.GetKeyDown(KeyCode.K))
            {
                CameraTrackState cameraTrackState = new CameraTrackState("VCam TrackDolly");
                CamManager.SwitchState(cameraTrackState);
            }

            if (CamManager.Waves.CircularWavesDurationList.Count > 0)
            {
                CamManager.ShakeCamera(CamManager.AmplitudShakeWhenWaterWave);
            }
            else if (CamManager.WaterFlow)
            {
                CamManager.ShakeCamera(CamManager.AmplitudShakeWhenWaterFlow);
            }
            else
            {
                CamManager.ShakeCamera(0);
            }
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        public override void SwitchState(CameraManager camera)
        {

        }

        private void MoveCamera()
        {
            //rotate freely with inputs
            bool rotateInput = Mathf.Abs(CamManager.Input.Inputs.RotateCamera.x) + Mathf.Abs(CamManager.Input.Inputs.RotateCamera.y) >= CamManager.Input.Inputs.Deadzone; //0.5f;
            const float minimumVelocityToReplaceCamera = 0.2f;
            _timerCameraReturnBehindBoat += Time.deltaTime;
            if (rotateInput && CamManager.CanMoveCameraManually)
            {
                ManageFreeCameraMove(ref _timerCameraReturnBehindBoat, CameraMode.Navigation);
            }
            //manage rotate to stay behind boat
            else if (Mathf.Abs(CamManager.RigidbodyKayak.velocity.x + CamManager.RigidbodyKayak.velocity.z) > minimumVelocityToReplaceCamera && _timerCameraReturnBehindBoat > CamManager.TimerCameraReturnBehindBoat ||
                     (Mathf.Abs(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY) > minimumVelocityToReplaceCamera / 20) && _timerCameraReturnBehindBoat > CamManager.TimerCameraReturnBehindBoat)
            {
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
                Quaternion targetQuaternion = Quaternion.Euler(new Vector3(0,
                    -(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY + CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY) * CamManager.MultiplierValueRotation,
                    localRotation.z));
                //get camera local position
                Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

                const float rotationThreshold = 0.15f;
                float rotationStaticY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY;
                float rotationPaddleY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY;
                #endregion

                //calculate camera rotation & position
                if (Mathf.Abs(rotationStaticY) > rotationThreshold || // if kayak is rotating
                    Mathf.Abs(rotationPaddleY) > rotationThreshold) //if kayak moving
                {
                    //rotation
                    CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, targetQuaternion, CamManager.LerpLocalRotationMove);

                    //position
                    if (Mathf.Abs(rotationStaticY) > rotationThreshold / 2)// if kayak is rotating
                    {
                        cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, CamManager.LerpLocalPositionNotMoving);
                    }
                    else if (Mathf.Abs(rotationPaddleY) > rotationThreshold / 2)// if kayak is moving
                    {
                        cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x,
                            (rotationStaticY + rotationPaddleY) * CamManager.MultiplierValuePosition, //value
                            CamManager.LerpLocalPositionMove); //time lerp
                        cameraTargetLocalPosition.z = 0;
                    }
                }
                else //if kayak not moving or rotating
                {
                    CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(0, 0, localRotation.z)), CamManager.LerpLocalRotationNotMoving);
                    cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, CamManager.LerpLocalPositionNotMoving);
                }


                //apply camera rotation & position
                CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);

                //camera target to kayak rotation and position
                CamManager.MakeTargetFollowRotationWithKayak();
            }
            else
            {
                CamManager.LastInputValue();
            }
        }
        private void ResetCameraBehindBoat()
        {
            //Start
            CamManager.MakeTargetFollowRotationWithKayak();

            //Middle
            Quaternion localRotation = CamManager.CinemachineCameraTarget.transform.localRotation;
            Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

            CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(0, 0, localRotation.z)), 1f);
            cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, 1f);
            CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);

            //End
            CamManager.ApplyRotationCamera();
        }
    }
}
