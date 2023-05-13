using System;
using Character;
using Character.Camera.State;
using Character.State;
using Fight;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WeaponWheel
{
    public class WeaponWheelButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Button _button;
        [SerializeField] private Projectile _projectile;

        public bool IsPaddle;
        [ReadOnly] public bool IsSelected;
        public UnityEvent OnSelected = new UnityEvent();
        
        private CharacterManager _characterManager;

        private void Start()
        {
            _characterManager = CharacterManager.Instance;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hover();
        }

        public void Hover()
        {
            IsSelected = true;
            _animator.SetBool("Hover",true);
            _button.Select();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit();
        }

        public void Exit()
        {
            if (IsSelected == false)
            {
                return;
            }
            IsSelected = false;
            _animator.SetBool("Hover",false);
        }

        public void Select()
        {
            if (_projectile == null)
            {
                Debug.Log($"null ref projectile : {transform.gameObject.name}");
                return;
            }

            OnSelected.Invoke();
            _characterManager.CurrentProjectile = _projectile;

            CharacterCombatState characterCombatState = new CharacterCombatState();
            _characterManager.SwitchState(characterCombatState);
        
            CameraCombatState cameraCombatState = new CameraCombatState();
            _characterManager.CameraManagerProperty.SwitchState(cameraCombatState);
        }
    }
}
