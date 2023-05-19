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
            CamManager.CameraAnimator.Play("FreeLook");
            CamManager.Brain.m_BlendUpdateMethod = Cinemachine.CinemachineBrain.BrainUpdateMethod.LateUpdate;

            CamManager.ResetNavigationValue();

        }
        public override void UpdateState(CameraManager camera)
        {

            camera.CameraDistance(camera.VirtualCameraFreeLook);


            if (Mathf.Abs(CamManager.RotationZ) > 0)
            {
                CamManager.SmoothResetRotateZ();
            }

            MoveCamera();

            camera.MakeTargetFollowRotationWithKayak();

            ClampRotationCameraValue(CamManager.Data.BottomClamp, CamManager.Data.TopClamp, float.MinValue, float.MaxValue);

            //CamManager.ApplyRotationCamera();

            //if (Input.GetKeyDown(KeyCode.K))
            //{
            //    CameraTrackState cameraTrackState = new CameraTrackState("VCam TrackDolly");
            //    CamManager.SwitchState(cameraTrackState);
            //}

            if (CamManager.Waves.CircularWavesDurationList.Count > 0)
            {
                CamManager.ShakeCamera(CamManager.Data.AmplitudeShakeWhenWaterWave);
            }
            else if (CamManager.WaterFlow)
            {
                CamManager.ShakeCamera(CamManager.Data.AmplitudeShakeWhenWaterFlow);
            }
            else
            {
                CamManager.ShakeCamera(0);
            }
        }
        public override void FixedUpdate(CameraManager camera)
        {

        }
        private bool _startMoving = false;
        public override void LateUpdate(CameraManager camera)
        {

            //rotate freely with inputs
            bool rotateInput = Mathf.Abs(CamManager.Input.Inputs.RotateCamera.x) + Mathf.Abs(CamManager.Input.Inputs.RotateCamera.y) >= CamManager.Input.Inputs.Deadzone; //0.5f;
            const float minimumVelocityToReplaceCamera = 0.05f;
            //bool rotateCamClick = CamManager.Input.Inputs.RotateCameraClick;

            _timerCameraReturnBehindBoat += Time.deltaTime;
            if (rotateInput && CamManager.CanMoveCameraManually /*&& rotateCamClick*/)
            {
                _startMoving = false;
                //ManageFreeCameraMove(ref _timerCameraReturnBehindBoat, CameraMode.Navigation);
            }

            //manage rotate to stay behind boat
            else if (Mathf.Abs(CamManager.RigidbodyKayak.velocity.x + CamManager.RigidbodyKayak.velocity.z) > minimumVelocityToReplaceCamera && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat ||
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

                //get target rotation
                Quaternion localRotation = CamManager.CinemachineCameraTarget.transform.localRotation;

                Quaternion targetQuaternion = Quaternion.Euler(new Vector3(CamManager.Data.NavigationRotation.x,
                    (-(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY + CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY) * CamManager.Data.MultiplierValueRotation) * 20,
                    localRotation.z));

                Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

                //const float rotationThreshold = 0.15f;
                const float rotationThreshold = 0.01f;
                float rotationStaticY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY;
                float rotationPaddleY = CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY;

                #endregion

                //calculate camera rotation & position
                if (Mathf.Abs(rotationStaticY) > rotationThreshold || // if kayak is rotating
                    Mathf.Abs(rotationPaddleY) > rotationThreshold) //if kayak moving
                {

                    if (Mathf.Abs(rotationStaticY) > rotationThreshold / 2)// if kayak is rotating
                    {
                        //position
                        cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, CamManager.Data.LerpLocalPositionWhenRotating * Time.deltaTime * 100);

                        //rotation
                        CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, targetQuaternion, CamManager.Data.LerpLocalRotationWhenRotating * Time.deltaTime * 100);
                        //CamManager.CinemachineCameraTarget.transform.localRotation = targetQuaternion;
                    }
                    else if (Mathf.Abs(rotationPaddleY) > rotationThreshold / 2)// if kayak is moving
                    {

                        //rotation
                        targetQuaternion = Quaternion.Euler(
                            new Vector3(CamManager.RigidbodyKayak.velocity.magnitude * 0.2f < CamManager.Data.NavigationRotation.x - CamManager.Data.ClampValueRotationX ?
                            CamManager.Data.NavigationRotation.x - CamManager.RigidbodyKayak.velocity.magnitude * 0.2f
                            : CamManager.Data.NavigationRotation.x - (CamManager.Data.NavigationRotation.x - CamManager.Data.ClampValueRotationX),
                    (-(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY + CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY) * CamManager.Data.MultiplierValueRotation) * 20,
                    localRotation.z));

                        CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, targetQuaternion, CamManager.Data.LerpLocalRotationMove * Time.deltaTime * 100);

                        //position
                        cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x,
                            (rotationStaticY + rotationPaddleY) * CamManager.Data.MultiplierValuePosition, //value
                            CamManager.Data.LerpLocalPositionMove * Time.deltaTime * 100); //time lerp

                        if (CamManager.RigidbodyKayak.velocity.magnitude * 0.1f < CamManager.Data.NavigationPosition.y - CamManager.Data.ClampValuePositionY)
                        {
                            cameraTargetLocalPosition.y = Mathf.Lerp(cameraTargetLocalPosition.y,
                               CamManager.Data.NavigationPosition.y - CamManager.RigidbodyKayak.velocity.magnitude * 0.1f, //value
                                CamManager.Data.LerpLocalPositionMove * Time.deltaTime * 100); //time lerp
                        }
                        else
                        {
                            cameraTargetLocalPosition.y = Mathf.Lerp(cameraTargetLocalPosition.y,
                                                           CamManager.Data.NavigationPosition.y - (CamManager.Data.NavigationPosition.y - CamManager.Data.ClampValuePositionY), //value
                                                            CamManager.Data.LerpLocalPositionMove * Time.deltaTime * 100); //time lerp
                        }
                        cameraTargetLocalPosition.z = 0;
                    }
                }
                else if (_startMoving==true)//if kayak not moving or rotating & cam start moving
                {
                    CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(CamManager.Data.NavigationRotation.x, CamManager.Data.NavigationRotation.y, localRotation.z)), CamManager.Data.LerpLocalRotationNotMoving * Time.deltaTime * 100);
                    cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, CamManager.Data.LerpLocalPositionNotMoving * Time.deltaTime * 100);
                }

                //apply camera rotation & position
                CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);

            }
            CamManager.ApplyRotationCamera();
        }



        public override void SwitchState(CameraManager camera)
        {

        }

        private void MoveCamera()
        {
            //rotate freely with inputs
            bool rotateInput = Mathf.Abs(CamManager.Input.Inputs.RotateCamera.x) + Mathf.Abs(CamManager.Input.Inputs.RotateCamera.y) >= CamManager.Input.Inputs.Deadzone; //0.5f;
            const float minimumVelocityToReplaceCamera = 0.05f;
            _timerCameraReturnBehindBoat += Time.deltaTime;

            //bool rotateCamClick = CamManager.Input.Inputs.RotateCameraClick;

            if (rotateInput && CamManager.CanMoveCameraManually /*&& rotateCamClick*/)
            {
                _startMoving = false;
                ManageFreeCameraMove(ref _timerCameraReturnBehindBoat, CameraMode.Navigation);
            }
            //manage rotate to stay behind boat
            else if (Mathf.Abs(CamManager.RigidbodyKayak.velocity.x + CamManager.RigidbodyKayak.velocity.z) > minimumVelocityToReplaceCamera && _timerCameraReturnBehindBoat > CamManager.Data.TimerCameraReturnBehindBoat ||
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

                //get target rotation
                //Quaternion localRotation = CamManager.CinemachineCameraTarget.transform.localRotation;
                //Quaternion targetQuaternion = Quaternion.Euler(new Vector3(0,
                //    -(CamManager.CharacterManager.CurrentStateBaseProperty.RotationStaticForceY + CamManager.CharacterManager.CurrentStateBaseProperty.RotationPaddleForceY) * CamManager.Data.MultiplierValueRotation,
                //    localRotation.z));
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
        //private void ResetCameraBehindBoat()
        //{
        //    //Start
        //    //CamManager.MakeTargetFollowRotationWithKayak();

        //    //Middle
        //    Quaternion localRotation = CamManager.CinemachineCameraTarget.transform.localRotation;
        //    Vector3 cameraTargetLocalPosition = CamManager.CinemachineCameraTarget.transform.localPosition;

        //    CamManager.CinemachineCameraTarget.transform.localRotation = Quaternion.Slerp(localRotation, Quaternion.Euler(new Vector3(0, 0, localRotation.z)), 1f);
        //    cameraTargetLocalPosition.x = Mathf.Lerp(cameraTargetLocalPosition.x, 0, 1f);
        //    CamManager.CinemachineTargetEulerAnglesToRotation(cameraTargetLocalPosition);

        //    //End
        //    CamManager.ApplyRotationCamera();
        //}
    }
}
