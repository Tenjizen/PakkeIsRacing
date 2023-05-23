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
        [SerializeField] private GameObject _lock;
        [SerializeField] private List<GameObject> _weaponObjectToSet;
        
        public WeaponType Type;
        public bool IsPaddle;
        
        public bool CanBeUnlocked;
        public bool IsUnlocked;
        public bool IsLocked;
        [ReadOnly] public bool IsSelected;
        
        [Header("Events")] public UnityEvent OnSelected = new UnityEvent();
        public UnityEvent OnWeaponTryUnlockButLocked = new UnityEvent();
        public UnityEvent OnWeaponUnlocked = new UnityEvent();
        
        private CharacterManager _characterManager;

        private void Start()
        {
            _characterManager = CharacterManager.Instance;
            _weaponObjectToSet.ForEach(x => x.SetActive(IsUnlocked));
           
            _lock.SetActive(false);
            IsLocked = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsUnlocked == false )
            {
                return;
            }
            
            Hover();
        }

        public void SetCanBeUnlocked(bool canBeUnlocked)
        {
            CanBeUnlocked = true;
        }
        
        public void SetWeapon(bool isUnlocked)
        {
            if (CanBeUnlocked == false)
            {
                OnWeaponTryUnlockButLocked.Invoke();
                return;
            }
            
            OnWeaponUnlocked.Invoke();
            _weaponObjectToSet.ForEach(x => x.SetActive(isUnlocked));
            IsUnlocked = isUnlocked;
        }

        public void Hover()
        {
            if (IsLocked)
            {
                return;
            }
            
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
            _characterManager = CharacterManager.Instance;

            if (IsUnlocked == false || _projectile == null || IsLocked)
            {
                _characterManager.CurrentStateBaseProperty.LaunchNavigationState();
                return;
            }

            weaponIcon.sprite = _weaponIcon == null ? weaponIcon.sprite : _weaponIcon;
            weaponIcon.DOFade(_weaponIcon == null ? 0 : 1, 0.2f);

            OnSelected.Invoke();
            _characterManager.CurrentProjectile = _projectile;

            CharacterCombatState characterCombatState = new CharacterCombatState();
            _characterManager.SwitchState(characterCombatState);
        
            CameraCombatState cameraCombatState = new CameraCombatState();
            _characterManager.CameraManagerProperty.SwitchState(cameraCombatState);
        }

        #region Lock

        public void Lock(bool isLock)
        {
            if (IsUnlocked == false)
            {
                return;
            }
            
            IsLocked = isLock;
            _lock.SetActive(IsLocked);
        }

        #endregion
    }
}
