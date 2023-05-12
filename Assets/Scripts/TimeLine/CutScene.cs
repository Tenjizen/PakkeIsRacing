using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutScene : MonoBehaviour
{
    [SerializeField] PlayableDirector _director;
    [SerializeField] GameObject _trigger;
    [SerializeField] CharacterManager _characterManager;

    private void Awake()
    {
        _director = GetComponent<PlayableDirector>();
        _director.played += Director_Played;
        _director.stopped += Director_Stopped;
    }

    private void Director_Played(PlayableDirector obj)
    {
        _trigger.SetActive(false);
        _characterManager.CurrentStateBaseProperty.CanCharacterMove = false;
        _characterManager.CameraManagerProperty.CanMoveCameraManually = false;
    }
    private void Director_Stopped(PlayableDirector obj)
    {
        //Trigger.SetActive(true);
        _characterManager.CurrentStateBaseProperty.CanCharacterMove = true;
        _characterManager.CameraManagerProperty.CanMoveCameraManually = true;
    }
    public void StartTimeLine()
    {
        _director.Play();
    }

}
