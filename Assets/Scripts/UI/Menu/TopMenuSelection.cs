using System;
using System.Collections.Generic;
using Character;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class TopMenuSelection : MenuController
    {
        [Header("Top Menu Controller")] public List<MenuStruct> MenuList = new List<MenuStruct>();
        [Header("Sub Menu"), SerializeField] private List<Image> _pointsList = new List<Image>();
        [SerializeField] private TMP_Text _text;

        protected override void Start()
        {
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuTopLeft.started += Left;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuTopRight.started += Right;
            
            Height = 1;
        }

        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false ||
                HorizontalIndex <= 0)
            {
                return;
            }
            
            int index = HorizontalIndex;
            base.Left(context);
            SetMenuActiveFromOldIndex(index);
        }
        
        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false ||
                HorizontalIndex >= Length-1)
            {
                return;
            }
            
            int index = HorizontalIndex;
            base.Right(context);
            SetMenuActiveFromOldIndex(index);
        }

        private void SetMenuActiveFromOldIndex(int oldMenuIndex)
        {
            MenuList[oldMenuIndex].Menu.Set(false, false);
            MenuList[HorizontalIndex].Menu.Set(true,true);
            
            _text.text = MenuList[HorizontalIndex].Name;
            
            _pointsList[oldMenuIndex].color = new Color(0.48f, 0.48f, 0.48f);
            _pointsList[HorizontalIndex].color = Color.white;
        }

        public override void Set(bool isActive, bool isUsable)
        {
            base.Set(isActive, isUsable);
            _text.text = MenuList[HorizontalIndex].Name;
            MenuList[HorizontalIndex].Menu.Set(isActive,isUsable);
        }
    }
}