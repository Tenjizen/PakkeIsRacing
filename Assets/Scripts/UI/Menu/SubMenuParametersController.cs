using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    public class SubMenuParametersController : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();

        private int _index;
        
        public override void SetMenu(bool isActive, bool isUsable)
        {
            base.SetMenu(isActive, isUsable);
            
            if (isUsable == false)
            {
                return;
            }

            if (IsUsable)
            {
                SetTile();
            }
        }

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex <= 0)
            {
                return;
            }
            
            base.Up(context);
            _index --;
            SetTile();
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex >= Height || _index+1 >= _objectsList.Count)
            {
                return;
            }
            
            base.Down(context);
            _index ++;
            SetTile();
        }
        
        private void SetTile()
        {
            if (_objectsList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < _objectsList.Count; i++)
            {
                _objectsList[i].Set(i == _index);
                _objectsList[i].IsSelected = i == _index;
            }
        }
    }
}