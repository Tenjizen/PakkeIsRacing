using System;
using System.Collections.Generic;
using System.Linq;
using Character;
using DG.Tweening;
using TMPro;
using Tools.HideIf;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class InfoBoxManager : MonoBehaviour
    {
        [SerializeField] private Transform _boxTransform;
        [SerializeField] private Transform _background;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TMP_Text _actionText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private float _timeScale;

        [SerializeField, Space(10)] private List<TutoInfo> _tutorialsList = new List<TutoInfo>();

        private Dictionary<Image, float> _backgroundDictionary = new Dictionary<Image, float>();
        private bool _isActive;
        private TutoInfo _currentTuto;
        private float _timeCooldown;

        #region Inputs

        public enum GameplayButtonsEnum
        {
            LeftTrigger,
            RightTrigger,
            Y,
            RightStick,
            LeftStick,
        }
        
        private void Start()
        {
            foreach (var image in _background.GetComponentsInChildren<Image>())
            {
                _backgroundDictionary.Add(image,image.color.a);
            }
            
            _currentTuto = new TutoInfo();
            _isActive = true;
            HideBox();
            _boxTransform.DOComplete();
            
            InputManagement manager = CharacterManager.Instance.InputManagementProperty;
            manager.GameplayInputs.Boat.PaddleLeft.started += LeftTrigger;
            manager.GameplayInputs.Boat.PaddleRight.started += RightTrigger;
            manager.GameplayInputs.Boat.OpenWheelMenu.started += Y;
            manager.GameplayInputs.Boat.RotateCamera.started += RightStick;
            manager.GameplayInputs.Boat.StaticRotateLeft.started += LeftStick;
            manager.GameplayInputs.Boat.StaticRotateRight.started += LeftStick;
        }

        private void LeftTrigger(InputAction.CallbackContext callback)
        {
            if (_currentTuto.Button == GameplayButtonsEnum.LeftTrigger)
            {
                HideBox();
            }
        }
        
        private void RightTrigger(InputAction.CallbackContext callback)
        {
            if (_currentTuto.Button == GameplayButtonsEnum.RightTrigger)
            {
                HideBox();
            }
        }
        
        private void Y(InputAction.CallbackContext callback)
        {
            if (_currentTuto.Button == GameplayButtonsEnum.Y)
            {
                HideBox();
            }
        }
        
        private void RightStick(InputAction.CallbackContext callback)
        {
            if (_currentTuto.Button == GameplayButtonsEnum.RightStick)
            {
                HideBox();
            }
        }
        
        private void LeftStick(InputAction.CallbackContext callback)
        {
            if (_currentTuto.Button == GameplayButtonsEnum.LeftStick)
            {
                HideBox();
            }
        }

        #endregion
        

        private void Update()
        {
            if (_timeCooldown > 0)
            {
                _timeCooldown -= Time.unscaledDeltaTime;
                return;
            }

            if (_currentTuto.LaunchAnotherTutorialAfter == false)
            {
                return;
            }

            SetBox(_currentTuto.IDToLaunch);
        }

        public void SetBox(string tutorialID)
        {
            if (_isActive)
            {
                return;
            }  
            
            for (int i = 0; i < _tutorialsList.Count; i++)
            {
                if (_tutorialsList[i].ID != tutorialID)
                {
                    continue;
                }

                _currentTuto = _tutorialsList[i];
                if (_currentTuto.HasBeenActivated)
                {
                    return;
                }

                _currentTuto.HasBeenActivated = true;
                _tutorialsList[i] = _currentTuto;
                break;
            }

            _isActive = true;
            Time.timeScale = _timeScale;

            _buttonImage.sprite = _currentTuto.ButtonImage;
            _actionText.text = CharacterManager.Instance.Parameters.Language ? _currentTuto.ActionText_EN : _currentTuto.ActionText_FR;
            _descriptionText.text = CharacterManager.Instance.Parameters.Language ? _currentTuto.DescriptionText_EN : _currentTuto.DescriptionText_FR;
            _boxTransform.localPosition = new Vector3(_currentTuto.Position.x, _currentTuto.Position.y, 0);

            _boxTransform.DOKill();
            _boxTransform.DOScale(Vector3.one, 0.3f).SetUpdate(true);
            _backgroundDictionary.ToList().ForEach(x => x.Key.DOKill());
            _backgroundDictionary.ToList().ForEach(x => x.Key.DOFade(x.Value,0.3f));
        }

        public void HideBox()
        {
            if (_isActive == false)
            {
                return;
            }
            _isActive = false;
            Time.timeScale = 1;

            _boxTransform.DOKill();
            _boxTransform.DOScale(Vector3.zero, 0.3f).SetUpdate(true);;
            _backgroundDictionary.ToList().ForEach(x => x.Key.DOKill());
            _backgroundDictionary.ToList().ForEach(x => x.Key.DOFade(0,0.3f));

            if (_currentTuto.LaunchAnotherTutorialAfter == false)
            {
                return;
            }

            _timeCooldown = _currentTuto.TimeToLaunch;
        }
    }

    [Serializable]
    public struct TutoInfo
    {
        [ReadOnly] public bool HasBeenActivated;
        public string ID;
        public InfoBoxManager.GameplayButtonsEnum Button;
        public string ActionText_EN;
        public string ActionText_FR;
        [TextArea] public string DescriptionText_EN;
        [TextArea] public string DescriptionText_FR;
        public Sprite ButtonImage;
        public Vector2 Position;
        
        public bool LaunchAnotherTutorialAfter;
        public float TimeToLaunch;
        public string IDToLaunch;
    }
}
