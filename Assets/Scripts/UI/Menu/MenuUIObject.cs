using System;
using Character;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class MenuUIObject : MonoBehaviour
    {
        [ReadOnly] public bool IsSelected;
        [SerializeField] protected Image OverlayImage;
        [SerializeField] protected Image IconImage;
        public UnityEvent OnActivated;

        private void Start()
        {
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ParameterMenuPressButton.started += Activate;
        }

        public void Initialize(Sprite image)
        {
            IconImage.sprite = image;
        }
        
        public virtual void Set(bool isActive)
        {
            OverlayImage.DOKill();
            OverlayImage.DOFade(isActive ? 1f : 0f, 0.1f);
        }

        public virtual string GetName()
        {
            return string.Empty;
        }

        public virtual string GetDescription()
        {
            return string.Empty;
        }

        protected virtual void Activate(InputAction.CallbackContext context)
        {
            OnActivated.Invoke();
        }
    }
}