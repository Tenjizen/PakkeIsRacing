using System;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class TopMenuSelection : MenuController
    {
        [Header("Top Menu Controller")] public List<MenuStruct> MenuList = new List<MenuStruct>();
        [Header("Sub Menu"), SerializeField] private List<Image> _pointsList = new List<Image>();
        [SerializeField] private TMP_Text _text;
        [Header("Events")] public UnityEvent OnMenuChanged;

        protected override void Start()
        {
            Debug.Log("comm input");
            //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuTopLeft.started += Left;
            //CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuTopRight.started += Right;

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
            OnMenuChanged.Invoke();
            
            MenuList[oldMenuIndex].Menu.SetMenu(false, false);
            MenuList[HorizontalIndex].Menu.SetMenu(true,true);
            
            Debug.Log("comm");
            //_text.text = CharacterManager.Instance.Parameters.Language ? MenuList[HorizontalIndex].Name_EN : MenuList[HorizontalIndex].Name_FR;

            _pointsList[oldMenuIndex].color = new Color(1f, 1f, 1f, 0f);
            _pointsList[HorizontalIndex].color = Color.white;
        }

        public override void SetMenu(bool isActive, bool isUsable)
        {
            base.SetMenu(isActive, isUsable);
            Debug.Log("comm");
            //_text.text = CharacterManager.Instance.Parameters.Language ? MenuList[HorizontalIndex].Name_EN : MenuList[HorizontalIndex].Name_FR;

            MenuList.ForEach(x => x.Menu.SetMenu(false,false));
            MenuList[HorizontalIndex].Menu.SetMenu(isActive,isUsable);

            _pointsList.ForEach(x => x.DOKill());
            _pointsList.ForEach(x => x.color = new Color(1f, 1f, 1f, 0f));
            _pointsList[HorizontalIndex].color = Color.white;
        }
    }
}