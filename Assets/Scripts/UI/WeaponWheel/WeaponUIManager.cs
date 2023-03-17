using Character;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WeaponWheel
{
    public class WeaponUIManager : MonoBehaviour
    {
        [SerializeField] private InputManagement _inputManagement;
        [SerializeField] private CharacterManager _characterManager;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private GameObject _weaponUI;
        [SerializeField] private Transform _vignette;
        
        [SerializeField] private WeaponWheelButtonController _harpoonButton;
        [SerializeField] private WeaponWheelButtonController _netButton;
        
        [SerializeField] private Image _paddleArrowDownImage;
        [SerializeField] private Image _cursor;

        private Vector3 _vignetteBaseScale;
        private bool _isMenuOpen;

        private void Start()
        {
            _vignetteBaseScale = _vignette.localScale;
            _weaponUI.SetActive(_isMenuOpen);
            
            SetCursor(false);
            SetPaddleDownImage(false);
        }

        private void Update()
        {
            if ((_inputManagement.Inputs.OpenWeaponMenu && _isMenuOpen == false) || 
                (_inputManagement.Inputs.OpenWeaponMenu == false && _isMenuOpen))
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
            _characterManager.CurrentStateBase.CanCharacterMove = _isMenuOpen;
            _characterManager.CurrentStateBase.CanCharacterMakeActions = _isMenuOpen;
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
            switch (_inputManagement.Inputs.SelectWeaponMenu)
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

        #endregion
    }
}
