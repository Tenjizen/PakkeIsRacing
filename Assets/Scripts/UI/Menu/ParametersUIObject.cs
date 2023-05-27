using System;
using Character;
using Character.Data.Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;
using PlayerParameters = Character.PlayerParameters;

namespace UI.Menu
{
    public class ParametersUIObject : MenuUIObject
    {
        public enum Parameter
        {
            AutoAim = 0,
            InversedControls = 1
        }
        
        [SerializeField] private bool _isOn;
        [SerializeField] private Parameter _parameter;
        [SerializeField] TMP_Text _offText, _onText;
        [SerializeField] Transform _selectionBackground;
        [SerializeField] Vector3 _offset;
        [SerializeField] private Color _textOffColor, _textOnColor;
        [SerializeField] private Color _backgroundOffColor, _backgroundOnColor;
        [SerializeField] private Image _backgroundImage;

        [SerializeField] private Vector3 _onPosition, _offPosition;

        public override void Set(bool isActive)
        {
            base.Set(isActive);
            
            _onPosition = _onText.rectTransform.localPosition + _offset;
            _offPosition = _offText.rectTransform.localPosition + _offset;

            _isOn = _parameter switch
            {
                Parameter.AutoAim => CharacterManager.Instance.Parameters.AutoAim,
                Parameter.InversedControls => CharacterManager.Instance.Parameters.InversedControls,
            };

            _backgroundImage.color = isActive ? _backgroundOnColor : _backgroundOffColor;

            SetParameters();
        }

        protected override void Activate(InputAction.CallbackContext context)
        {
            if (IsSelected == false)
            {
                return;
            }

            base.Activate(context);
            
            _isOn = _isOn == false;
            
            SetParameters();
        }

        private void SetParameters()
        {
            PlayerParameters playerParameters = CharacterManager.Instance.Parameters;
            PlayerParameters parameters = new PlayerParameters()
            {
                AutoAim = _parameter == Parameter.AutoAim ? _isOn : playerParameters.AutoAim,
                InversedControls = _parameter == Parameter.InversedControls ? _isOn : playerParameters.InversedControls
            };
            CharacterManager.Instance.Parameters = parameters;
            
            SetText();
        }

        private void SetText()
        {
            const float fadeTime = 0.2f;
            
            _onText.DOColor(_isOn ? _textOnColor : _textOffColor, fadeTime);
            _offText.DOColor(_isOn ? _textOffColor : _textOnColor, fadeTime);
            
            Vector3 position = _isOn ? _onPosition : _offPosition;
            _selectionBackground.DOLocalMove(position,fadeTime);
        }
    }
}