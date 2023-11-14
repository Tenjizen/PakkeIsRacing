using System;
using Character;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class MenuController : MonoBehaviour
    {
        [ReadOnly] public bool IsActive = false;
        [ReadOnly] public bool IsUsable = false;
        [SerializeField] protected int Height, Length;
        [SerializeField] protected GameObject MenuGameObject;
        [Header("Events")] public UnityEvent OnSelected;
        
        protected int VerticalIndex, HorizontalIndex;

        protected virtual void Start()
        {
            Debug.Log("comm input");
            //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuDown.started += Down;
            //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuUp.started += Up;
            //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuLeft.started += Left;
            //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuRight.started += Right;
        }

        #region Directions

        protected virtual void Up(InputAction.CallbackContext context)
        {
            if (VerticalIndex <= 0 || IsUsable == false)
            {
                return;
            }

            VerticalIndex--;
        }
        
        protected virtual void Down(InputAction.CallbackContext context)
        {
            if (VerticalIndex > Height-1 || IsUsable == false)
            {
                return;
            }
            
            VerticalIndex++;
        }

        protected virtual void Left(InputAction.CallbackContext context)
        {
            if (HorizontalIndex <= 0 || IsUsable == false)
            {
                return;
            }
            
            HorizontalIndex--;
        }
        
        protected virtual void Right(InputAction.CallbackContext context)
        {
            if (HorizontalIndex >= Length - 1 || IsUsable == false)
            {
                return;
            }
            
            HorizontalIndex++;
        }
        
        #endregion

        public virtual void SetMenu(bool isActive, bool isUsable)
        {
            IsActive = isActive;
            IsUsable = isUsable;
            MenuGameObject.gameObject.SetActive(isActive);
            OnSelected.Invoke();
        }
    }
    
    [Serializable]
    public struct MenuStruct
    {
        public string Name_FR;
        public string Name_EN;
        public MenuController Menu;
    }
}
