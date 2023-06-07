using System;
using Character;
using Character.Data.Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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
            InversedControls = 1,
            Language = 2
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

        public UnityEvent OnOn;
        public UnityEvent OnOff;

        public override void Set(bool isActive)
        {
            base.Set(isActive);
            
            _onPosition = _onText.rectTransform.localPosition + _offset;
            _offPosition = _offText.rectTransform.localPosition + _offset;

            _isOn = _parameter switch
            {
                Parameter.AutoAim => CharacterManager.Instance.Parameters.AutoAim,
                Parameter.InversedControls => CharacterManager.Instance.Parameters.InversedControls,
                Parameter.Language => CharacterManager.Instance.Parameters.Language,
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
            switch (_isOn)
            {
                case true : OnOn.Invoke(); break;
                case false : OnOff.Invoke(); break;
            }
            
            SetParameters();
        }

        private void SetParameters()
        {
            PlayerParameters playerParameters = CharacterManager.Instance.Parameters;
            PlayerParameters parameters = new PlayerParameters()
            {
                AutoAim = _parameter == Parameter.AutoAim ? _isOn : playerParameters.AutoAim,
                InversedControls = _parameter == Parameter.InversedControls ? _isOn : playerParameters.InversedControls,
                Language = _parameter == Parameter.Language ? _isOn : playerParameters.Language
            };
            CharacterManager.Instance.Parameters = parameters;
            
            SetText();
        }

        private void SetText()
        {
            const float fadeTime = 0.2f;
            
            _onText.DOColor(_isOn ? _textOnColor : _textOffColor, fadeTime).SetUpdate(true);
            _offText.DOColor(_isOn ? _textOffColor : _textOnColor, fadeTime).SetUpdate(true);
            
            Vector3 position = _isOn ? _onPosition : _offPosition;
            _selectionBackground.DOLocalMove(position,fadeTime).SetUpdate(true);
        }
    }
}