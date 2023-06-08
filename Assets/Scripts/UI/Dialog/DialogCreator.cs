using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Character.Camera;
using DG.Tweening;
using Dialog;
using GPEs;
using Json;
using Sound;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Dialog
{
    public class DialogCreator : PlayerTriggerManager
    {
        #region Enums

        [Serializable]
        private enum LaunchType
        {
            TriggerZone = 0,
            Method = 1
        }

        [Serializable]
        private enum DialogState
        {
            NotLaunched = 0,
            Showing = 1,
            Holding = 2,
            WaitingForInput = 3,
        }

        #endregion

        [Header("Parameters")]
        public DialogData Dialog_FR;
        public DialogData Dialog_EN;
        [SerializeField]
        private LaunchType _launchType;
        [SerializeField] 
        private bool _canBeReplayed;
        [SerializeField, ReadOnly] 
        private bool _hasEnded;
        [SerializeField] 
        private bool _blockPlayerMovement, _blockCameraMovement;
        [SerializeField]
        private float _timeUntilLaunch;

        [Space(20), Header("Events")] 
        public UnityEvent OnDialogLaunch = new UnityEvent();
        public UnityEvent OnDialogEnd = new UnityEvent();

        [Header("Debug"), SerializeField, ReadOnly] private DialogState _currentDialogState = DialogState.NotLaunched;
       
        private int _dialogIndex;
        private float _currentDialogCooldown;
        private bool _launchTimer;
        private GameplayInputs _gameplayInputs;
        private CharacterManager _characterManager;
        private CameraManager _cameraManager;
        private DialogData _currentDialogData;

        private void Start()
        {
            OnPlayerEntered.AddListener(StartTrigger);

            _characterManager = CharacterManager.Instance;
            _cameraManager = CharacterManager.Instance.CameraManagerProperty;
            
            _gameplayInputs = new GameplayInputs();
            _gameplayInputs.Enable();
            DialogManager.Instance.ToggleDialog(false);
        }

        private void OnDestroy()
        {
            OnPlayerEntered.RemoveListener(StartTrigger);
        }

        public override void Update()
        {
            base.Update();
            
            if (_launchTimer && _timeUntilLaunch > 0)
            {
                _timeUntilLaunch -= Time.deltaTime;
                if (_timeUntilLaunch <= 0)
                {
                    StartTrigger();
                }
            }
            
            if (_currentDialogState == DialogState.NotLaunched)
            {
                return;
            }

            CoolDownManagement();
        }

        private void StartTrigger()
        {
            if (_launchType != LaunchType.TriggerZone || _currentDialogState != DialogState.NotLaunched || 
                (_hasEnded && _canBeReplayed == false))
            {
                return;
            }
            
            if ( ( (_hasEnded && _canBeReplayed) || (_hasEnded == false && _currentDialogState == DialogState.NotLaunched) ) && 
                 _timeUntilLaunch <= 0)
            {
                DialogManager.Instance.DialogQueue.Enqueue(this);
                
                if (DialogManager.Instance.DialogQueue.Count > 1)
                {
                    return;
                }
                
                LaunchDialog();
            }
            else
            {
                _launchTimer = true;
            }
        }

        private void CoolDownManagement()
        {
            _currentDialogCooldown -= Time.deltaTime;

            if (_currentDialogCooldown > 0)
            {
                return;
            }

            switch (_currentDialogState)
            {
                case DialogState.Showing:
                    _currentDialogState = DialogState.Holding;
                    _currentDialogCooldown = _currentDialogData.DialogList[_dialogIndex].TextHoldTime;
                    break;
            
                case DialogState.Holding:
                    if (_currentDialogData.DialogList[_dialogIndex].SequencingTypeNext == SequencingType.Automatic)
                    {
                        _dialogIndex++;
                        CheckForDialogEnd();
                        if (_dialogIndex < _currentDialogData.DialogList.Count)
                        {
                            ShowDialog(_dialogIndex);
                        }
                    }
                    else
                    {
                        _currentDialogState = DialogState.WaitingForInput;
                        DialogManager.Instance.PressButtonImage.DOFade(1, 0.2f);
                    }
                    break;
            
                case DialogState.WaitingForInput:
                    if (_gameplayInputs.Boat.DialogSkip.triggered)
                    {
                        _dialogIndex++;
                        CheckForDialogEnd();

                        if (_dialogIndex < _currentDialogData.DialogList.Count)
                        {
                            ShowDialog(_dialogIndex);
                        }
                    }
                    break;
            }
        }

        public void LaunchDialog()
        {
            DialogManager.Instance.ToggleDialog(true);
            
            OnDialogLaunch.Invoke();

            _dialogIndex = 0;
            _currentDialogData = CharacterManager.Instance.Parameters.Language ? Dialog_EN : Dialog_FR;
            ShowDialog(_dialogIndex);

            if (_blockPlayerMovement)
            {
                _characterManager.CurrentStateBaseProperty.CanCharacterMove = false;
            }

            if (_blockCameraMovement)
            {
                _cameraManager.CanMoveCameraManually = false;
            }

            //visual
            DialogManager.Instance.PressButtonImage.DOFade(0f, 0f);
        }

        private void ShowDialog(int index)
        {
            if (_currentDialogData == null)
            {
                return;
            }
            
            _currentDialogState = DialogState.Showing;
            _currentDialogCooldown = _currentDialogData.DialogList[index].TextShowTime;

            DialogManager.Instance.TypeWriterText.DisplayText.color = _currentDialogData.DialogList[index].TextColor;
            if (_currentDialogData.DialogList[index].ShowLetterByLetter)
            {
                DialogManager.Instance.TypeWriterText.FullText = _currentDialogData.DialogList[index].Text;
                DialogManager.Instance.TypeWriterText.Delay =  _currentDialogData.DialogList[index].TextShowTime / _currentDialogData.DialogList[index].Text.Length;
                StartCoroutine(DialogManager.Instance.TypeWriterText.ShowText());
            }
            else
            {
                DialogManager.Instance.TypeWriterText.DisplayText.text = _currentDialogData.DialogList[index].Text;
            }

            //visual
            DialogManager.Instance.TypeWriterText.transform.DOPunchScale(Vector3.one * _currentDialogData.DialogList[index].SizeEffect, 0.3f, 10, 0);
            DialogManager.Instance.PressButtonImage.DOFade(0, 0.1f);
        }

        private void EndDialog()
        {
            OnDialogEnd.Invoke();
            
            _hasEnded = true;
            _currentDialogState = DialogState.NotLaunched;
            
            //booleans
            _characterManager.CurrentStateBaseProperty.CanCharacterMove = true;
            _cameraManager.CanMoveCameraManually = true;
            
            //json
            JsonFilesManagerSingleton.Instance.DialogsJsonFileManagerProperty.SetDialogCollected(this);
            
            //check for queue
            if (DialogManager.Instance.DialogQueue.Count > 0)
            {
                DialogManager.Instance.DialogQueue.Dequeue();
            }
            if (DialogManager.Instance.DialogQueue.Count > 0)
            {
                DialogManager.Instance.DialogQueue.Peek().LaunchDialog();
            }
            else
            {
                Transform dialog = DialogManager.Instance.DialogUIGameObject.transform;
                StartCoroutine(DeactivateDialogObject(0.25f));
            }
        }

        private void CheckForDialogEnd()
        {
            if (_dialogIndex >= _currentDialogData.DialogList.Count)
            {
                EndDialog();
            }
        }

        private IEnumerator DeactivateDialogObject(float time)
        {
            yield return new WaitForSeconds(time);
            DialogManager.Instance.ToggleDialog(false);
        }
    }
}