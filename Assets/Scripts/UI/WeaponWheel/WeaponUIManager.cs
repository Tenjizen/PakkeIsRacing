using Character;
using Character.Camera;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WeaponWheel
{
    public class WeaponUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _weaponUI;
        [SerializeField] private Transform _vignette;
        
        [SerializeField] private WeaponWheelButtonController _harpoonButton;
        [SerializeField] private WeaponWheelButtonController _netButton;
        
        [SerializeField] private Image _paddleArrowDownImage;
        [SerializeField] private Image _cursor;
        [SerializeField] private Transform _cursorPivot;
        [SerializeField] private Image _cooldown;

        [field:SerializeField, Header("AutoAim")] public AutoAimUIController AutoAimController { get; private set; }

        private Vector3 _vignetteBaseScale;
        private bool _isMenuOpen;
        
        private InputManagement _inputManagement;
        private CharacterManager _characterManager;
        private CameraManager _cameraManager;

        private void Start()
        {
            _characterManager = CharacterManager.Instance;
            _inputManagement = _characterManager.InputManagementProperty;
            _cameraManager = _characterManager.CameraManagerProperty;
            
            _vignetteBaseScale = _vignette.localScale;
            _weaponUI.SetActive(_isMenuOpen);
            
            SetCursor(false);
            SetCooldownUI(0);
            SetPaddleDownImage(false);
        }

        private void Update()
        {
            if (((_inputManagement.Inputs.OpenWeaponMenu && _isMenuOpen == false) || 
                (_inputManagement.Inputs.OpenWeaponMenu == false && _isMenuOpen)) &&
                _characterManager.CurrentStateBaseProperty.CanCharacterOpenWeapons)
            {
                PressMenu();
            }

            if (_isMenuOpen)
            {
                WeaponChoice();
            }
        }

        private void PressMenu()
        {
            _characterManager.CurrentStateBaseProperty.CanCharacterMove = _isMenuOpen;
            _characterManager.CurrentStateBaseProperty.CanCharacterMakeActions = _isMenuOpen;
            _cameraManager.CanMoveCameraManually = _isMenuOpen;
            
            if (_isMenuOpen && EventSystem.current.currentSelectedGameObject != null)
            {
                WeaponWheelButtonController button = EventSystem.current.currentSelectedGameObject.GetComponent<WeaponWheelButtonController>();
                if (button != null)
                {
                    button.Select();
                }
            }
            _isMenuOpen = _isMenuOpen == false;
            ResetSelection();
            
            _weaponUI.SetActive(_isMenuOpen);
            Time.timeScale = _isMenuOpen ? 1f : 1f;

            if (_isMenuOpen)
            {
                VignetteZoom();
            }
        }

        private void WeaponChoice()
        {
            const float DEADZONE = 0.5f;
            switch (_inputManagement.Inputs.SelectWeaponMenuX)
            {
                case < -DEADZONE:
                    _harpoonButton.Hover();
                    _netButton.Exit();
                    break;
                case > DEADZONE:
                    _netButton.Hover();
                    _harpoonButton.Exit();
                    break;
                default:
                    _netButton.Exit();
                    _harpoonButton.Exit();
                    break;
            }
            
            //cursor
            Vector3 rotation = _cursorPivot.rotation.eulerAngles;
            Vector2 joystickInput = new Vector2(_inputManagement.Inputs.SelectWeaponMenuX, _inputManagement.Inputs.SelectWeaponMenuY);
            float angle = Mathf.Atan2(joystickInput.y, joystickInput.x);
            float angleDegrees = angle * Mathf.Rad2Deg - 90;
            if (angleDegrees < 0f)
            {
                angleDegrees += 360f;
            }
            _cursorPivot.rotation = Quaternion.Euler(new Vector3(rotation.x, rotation.y, angleDegrees));
        }

        public void ResetSelection()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void VignetteZoom()
        {
            _vignette.localScale = _vignetteBaseScale * 2;
            _vignette.DOScale(_vignetteBaseScale, 0.3f);
        }

        #region Set images

        public void SetPaddleDownImage(bool show)
        {
            _paddleArrowDownImage.DOFade(show ? 1 : 0, 0.4f);
        }

        public void SetCursor(bool show)
        {
            _cursor.DOFade(show ? 1 : 0, 0.2f);
        }

        public void SetCooldownUI(float value)
        {
            value = Mathf.Clamp01(value);
            _cooldown.fillAmount = value;

            if (value is 0 or 1)
            {
                _cooldown.DOFade(0, 0.5f);
            }
            else
            {
                _cooldown.DOFade(1, 0.2f);
            }
        }

        #endregion
    }
}
