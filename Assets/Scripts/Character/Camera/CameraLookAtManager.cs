using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAtManager : MonoBehaviour
{
    [SerializeField] private float _startLookAt, _endLookAt;
    [SerializeField] bool CanBeReplay;
    private bool _hasBeenUsed;

    public void CameraLookAt(Transform transform)
    {
        if (_hasBeenUsed == true && CanBeReplay == false)
        {
            return;
        }
        CharacterManager.Instance.CameraManagerProperty.VirtualCameraLookAt.LookAt = transform;
        StartCoroutine(StartLookAt(_startLookAt, _endLookAt));
    }


    private IEnumerator StartLookAt(float time, float endTime)
    {
        yield return new WaitForSeconds(time);
        CharacterManager.Instance.CameraManagerProperty.CameraAnimator.Play("LookAt");
        StartCoroutine(StopLookAt(endTime));
    }
    IEnumerator StopLookAt(float time)
    {
        yield return new WaitForSeconds(time);
        CharacterManager.Instance.CameraManagerProperty.CameraAnimator.Play("FreeLook");
        _hasBeenUsed = true;
    }
}
