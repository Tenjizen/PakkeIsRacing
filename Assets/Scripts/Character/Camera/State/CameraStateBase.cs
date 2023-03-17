using System;
using UnityEngine;

public abstract class CameraStateBase
{
    public enum CameraMode
    {
        Navigation = 0,
        Combat = 1,
    }
    
    public CameraStateBase(CameraManager cameraManagerRef, MonoBehaviour monoBehaviour)
    {
        CameraManagerRef = cameraManagerRef;
        MonoBehaviourRef = monoBehaviour;
    }

    protected CameraManager CameraManagerRef;
    protected MonoBehaviour MonoBehaviourRef;

    public abstract void EnterState(CameraManager camera);

    public abstract void UpdateState(CameraManager camera);

    public abstract void FixedUpdate(CameraManager camera);

    public abstract void SwitchState(CameraManager camera);
    
    protected void ClampRotationCameraValue()
    {
        CameraManagerRef.CinemachineTargetYaw = ClampAngle(CameraManagerRef.CinemachineTargetYaw, float.MinValue, float.MaxValue);
        CameraManagerRef.CinemachineTargetPitch = ClampAngle(CameraManagerRef.CinemachineTargetPitch, CameraManagerRef.BottomClamp, CameraManagerRef.TopClamp);
    }
    public float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    public void ManageFreeCameraMove(ref float timerCameraReturnBehindBoat, CameraMode cameraMode)
    {
        //rotate freely with inputs
        if (CameraManagerRef.CanMoveCameraManually)
        {
            Vector2 aim = Vector2.zero;
            switch (cameraMode)
            {
                case CameraMode.Navigation:
                    aim = CameraManagerRef.Input.Inputs.RotateCamera;
                    break;
                case CameraMode.Combat:
                    aim = CameraManagerRef.Input.Inputs.MovingAim;
                    aim = new Vector2(aim.x, aim.y * -1);
;                   break;
            }
            
            //Controller
            CameraManagerRef.CinemachineTargetYaw += CameraManagerRef.JoystickFreeRotationX.Evaluate(aim.x);
            CameraManagerRef.CinemachineTargetPitch += CameraManagerRef.JoystickFreeRotationY.Evaluate(aim.y);

            #region clavier souris
            //KBM
            //CameraManagerRef.CinemachineTargetYaw += CameraManagerRef.Input.Inputs.RotateCamera.x;
            //CameraManagerRef.CinemachineTargetPitch += CameraManagerRef.Input.Inputs.RotateCamera.y;
            #endregion

            //last inputs
            CameraManagerRef.LastInputX = aim.x != 0 ? aim.x : CameraManagerRef.LastInputX;
            CameraManagerRef.LastInputY = aim.y != 0 ? aim.y : CameraManagerRef.LastInputY;

            timerCameraReturnBehindBoat = 0;
        }
    }

    private float _uselessFloatForRef;
    public void ManageFreeCameraMove(CameraMode cameraMode)
    {
        ManageFreeCameraMove(ref _uselessFloatForRef, cameraMode);
    }

    public void SetFOV(float value)
    {
        CameraManagerRef.VirtualCamera.m_Lens.FieldOfView = value;
    }
}


