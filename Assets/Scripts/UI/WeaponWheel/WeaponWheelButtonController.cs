using Character;
using Character.Camera.State;
using Character.State;
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
        [SerializeField] private CharacterManager _characterManager;
        [SerializeField] private Weapon _weapon;
    
        public UnityEvent OnSelected = new UnityEvent();

        public void OnPointerEnter(PointerEventData eventData)
        {
            Hover();
        }

        public void Hover()
        {
            _animator.SetBool("Hover",true);
            _button.Select();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Exit();
        }

        public void Exit()
        {
            _animator.SetBool("Hover",false);
        }

        public void Select()
        {
            Debug.Log($"select {_weapon}");
        
            OnSelected.Invoke();
            _characterManager.CurrentWeapon = _weapon;

            CharacterCombatState characterCombatState = 
                new CharacterCombatState(_characterManager,_characterManager.CurrentStateBaseProperty.MonoBehaviourRef,_characterManager.CurrentStateBaseProperty.CameraManagerRef);
            _characterManager.SwitchState(characterCombatState);
        
            CameraCombatState cameraCombatState = new CameraCombatState(_characterManager.CameraManagerProperty, _characterManager.CurrentStateBaseProperty.MonoBehaviourRef);
            _characterManager.CameraManagerProperty.SwitchState(cameraCombatState);
        }
    }
}
