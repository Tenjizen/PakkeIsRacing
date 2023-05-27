using Character;
using Character.Camera.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptForDebug : MonoBehaviour
{

    [SerializeField] CanvasGroup _fadeDead;

    private float _timerDebug;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_fadeDead.alpha > 0.9f)
        {
            _timerDebug += Time.deltaTime;

            if(_timerDebug > 10f)
            {
                Transform checkpoint = CharacterManager.Instance.CheckpointManagerProperty.GetRespawnPoint();
                //put kayak in checkpoint position & rotation
                CharacterManager.Instance.KayakControllerProperty.transform.position = checkpoint.position;
                CharacterManager.Instance.KayakControllerProperty.transform.rotation = checkpoint.rotation;

                //Reset value
                CharacterManager.Instance.KayakControllerProperty.CanReduceDrag = true;
                CharacterManager.Instance.CameraManagerProperty.CanMoveCameraManually = true;
                CharacterManager.Instance.SetBalanceValueToCurrentSide(0);
                
                CharacterManager.Instance.TransitionManagerProperty.LaunchTransitionOut(SceneTransition.TransitionType.Fade);
                CameraNavigationState cameraNavigationState = new CameraNavigationState();
                CharacterManager.Instance.CameraManagerProperty.SwitchState(cameraNavigationState);
            }
        }

    }

    public void ResetTimerDebug()
    {
        _timerDebug = 0;
    }

}
