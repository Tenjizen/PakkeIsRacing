using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class TopMenuSelection : MenuController
    {
        [Header("Top Menu Controller")] public List<MenuStruct> MenuList = new List<MenuStruct>();
        [SerializeField] private TMP_Text _text;

        protected override void Start()
        {
            base.Start();
            Height = 1;
        }

        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            int index = HorizontalIndex;
            base.Left(context);
            SetMenuActiveFromOldIndex(index);
        }
        
        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            int index = HorizontalIndex;
            base.Right(context);
            SetMenuActiveFromOldIndex(index);
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            base.Down(context);
            if (VerticalIndex >= 1)
            {
                IsUsable = false;
                MenuList[HorizontalIndex].Menu.Set(true,true);
            }
        }

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            base.Up(context);
            if (VerticalIndex <= 0)
            {
                IsUsable = true;
                MenuList[HorizontalIndex].Menu.Set(true,false);
            }
        }

        private void SetMenuActiveFromOldIndex(int oldMenuIndex)
        {
            MenuList[oldMenuIndex].Menu.Set(false, false);
            _text.text = MenuList[HorizontalIndex].Name;
            MenuList[HorizontalIndex].Menu.Set(true,false);
        }

        public override void Set(bool isActive, bool isUsable)
        {
            base.Set(isActive, isUsable);
            SetMenuActiveFromOldIndex(HorizontalIndex);
        }
    }

    
}