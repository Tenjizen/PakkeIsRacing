using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class SubMenuController : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [Header("Sub Menu"), SerializeField] private MenuController _topSelectionMenu;

        private int _index;
        
        public override void Set(bool isActive, bool isUsable)
        {
            base.Set(isActive, isUsable);
            if (isUsable == true)
            {
                _index = -Length;
                SetTile();
            }
        }

        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            base.Left(context);
            _index--;
            SetTile();
            Debug.Log("left");
        }

        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            base.Right(context);
            _index++;
            SetTile();
            Debug.Log("right");
        }

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            base.Up(context);
            _index -= Length;
            SetTile();

            if (_index <= 0)
            {
                IsUsable = false;
                _topSelectionMenu.Set(true,true);
            }
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }
            
            base.Down(context);
            _index += Length;
            SetTile();
            Debug.Log("down");
        }

        private void SetTile()
        {
            for (int i = 0; i < _objectsList.Count; i++)
            {
                _objectsList[i].Set(i == _index);
            }
        }
    }
}