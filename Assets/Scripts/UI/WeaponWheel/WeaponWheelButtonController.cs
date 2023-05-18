using System;
using System.Collections.Generic;
using Character;
using Character.Camera.State;
using Character.State;
using DG.Tweening;
using Fight;
using Fight.Data;
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
        [SerializeField] private Sprite _weaponIcon;
        [SerializeField] private List<GameObject> _weaponObjectToSet;
        
        public WeaponType Type;
        public bool IsPaddle;
        
        public bool IsUnlocked;
        [ReadOnly] public bool IsSelected;
        
        [Header("Events")] public UnityEvent OnSelected = new UnityEvent();
        
        
        private CharacterManager _characterManager;

        private void Start()
        {
            _characterManager = CharacterManager.Instance;
            _weaponObjectToSet.ForEach(x => x.SetActive(IsUnlocked));
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsUnlocked == false)
            {
                return;
            }
            
            Hover();
        }

        public void SetWeapon(bool isUnlocked)
        {
            _weaponObjectToSet.ForEach(x => x.SetActive(isUnlocked));
            IsUnlocked = isUnlocked;
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

        public void Select(Image weaponIcon)
        {
            if (IsUnlocked == false)
            {
                CharacterManager.Instance.CurrentStateBaseProperty.LaunchNavigationState();
                return;
            }
  
            weaponIcon.sprite = _weaponIcon == null ? weaponIcon.sprite : _weaponIcon;
            weaponIcon.DOFade(_weaponIcon == null ? 0 : 1, 0.2f);

            if (_projectile == null)
            {
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
