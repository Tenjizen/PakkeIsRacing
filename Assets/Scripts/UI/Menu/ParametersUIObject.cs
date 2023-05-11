using System;
using Character;
using Character.Data.Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

        private void Awake()
        {
            Color color = Color.white;
            _offText.color = color;
            _onText.color = color;
        }

        public override void Set(bool isActive)
        {
            base.Set(isActive);

            _isOn = _parameter switch
            {
                Parameter.AutoAim => CharacterManager.Instance.Parameters.AutoAim,
                Parameter.InversedControls => CharacterManager.Instance.Parameters.InversedControls,
            };
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
            _offText.DOFade(_isOn ? 0.25f : 1f, 0.3f);
            _onText.DOFade(_isOn ? 1f : 0.25f, 0.3f);
        }
    }
}