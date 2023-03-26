using System.Collections;
using System.Collections.Generic;
using Character.Camera.State;
using UnityEngine;

public class CameraDialogState : CameraStateBase
{
    public CameraDialogState(CameraManager cameraManagerRef, MonoBehaviour monoBehaviour) :
      base(cameraManagerRef, monoBehaviour)
    {
    }

    public override void EnterState(CameraManager camera)
    {
        //look-at
        CameraManagerRef.VirtualCameraCombat.LookAt = null;
    }
    public override void UpdateState(CameraManager camera)
    {

    }
    public override void FixedUpdate(CameraManager camera)
    {

    }
    public override void SwitchState(CameraManager camera)
    {

    }
}
