using System;
using System.Collections.Generic;
using Character;
using Collectible;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class SubMenuCollectibleController : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();
        [SerializeField] private MenuController _topSelectionMenu;
        [SerializeField] private TMP_Text _nameText, _descriptionText;
        [SerializeField] private GameObject _defaultCollectibleMesh;

        private int _index;
        private GameObject _currentDisplayedMesh;

        protected override void Start()
        {
            base.Start();
            CollectibleJsonFileManager.Instance.OnNewCollectibleGet.AddListener(SetTilesData);
            if (_defaultCollectibleMesh != null)
            {
                _defaultCollectibleMesh.SetActive(false);
            }
        }

        private void Update()
        {
            if (IsUsable == false || _currentDisplayedMesh == null)
            {
                return;
            }
            
            RotateGameObjectFromSticks(_currentDisplayedMesh.transform);
        }

        public override void Set(bool isActive, bool isUsable)
        {
            base.Set(isActive, isUsable);
            
            if (isUsable == false)
            {
                return;
            }

            if (IsUsable)
            {
                SetTile();
            }
        }

        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false ||
                HorizontalIndex <= 0)
            {
                return;
            }

            base.Left(context);
            _index--;
            SetTile();
        }

        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false ||
                HorizontalIndex >= Length-1 ||
                _index >= _objectsList.Count-1)
            {
                return;
            }
            
            base.Right(context);
            _index++;
            SetTile();
        }

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false ||
                VerticalIndex <= 0)
            {
                return;
            }
            
            base.Up(context);
            _index -= Length;
            SetTile();
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false ||
                VerticalIndex >= Height ||
                _index+Length >= _objectsList.Count)
            {
                return;
            }
            
            base.Down(context);
            _index += Length;
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
            }
            
            _objectsList[_index].Set(true);

            if (_nameText == null || _descriptionText == null)
            {
                return;
            }

            CollectibleUIObject collectibleUIObject = _objectsList[_index].GetComponent<CollectibleUIObject>();
            _nameText.text = collectibleUIObject == null ? _objectsList[_index].GetName() : collectibleUIObject.GetName();
            _descriptionText.text = collectibleUIObject == null ? _objectsList[_index].GetDescription() : collectibleUIObject.GetDescription();
            
            //mesh
            if (_currentDisplayedMesh != null)
            {
                _currentDisplayedMesh.SetActive(false);
            }
            GameObject mesh = _defaultCollectibleMesh;
            if (collectibleUIObject != null && collectibleUIObject.CollectibleMesh != null && collectibleUIObject.Data != null)
            {
                mesh = collectibleUIObject.CollectibleMesh;
            }
            mesh.SetActive(true);
            _currentDisplayedMesh = mesh;
        }

        private void SetTilesData()
        {
            for (int i = 0; i < _objectsList.Count; i++)
            {
                CollectibleUIObject collectibleUIObject = _objectsList[i].GetComponent<CollectibleUIObject>();
                
                if (collectibleUIObject == null ||
                    i >= CollectibleJsonFileManager.Instance.CollectedItems.Count || 
                    CollectibleJsonFileManager.Instance.CollectedItems[i] == null)
                {
                    continue;
                }
                
                collectibleUIObject.Data = CollectibleJsonFileManager.Instance.CollectedItems[i].CollectibleGameObject.Data;
            }
        }

        private void RotateGameObjectFromSticks(Transform transform)
        {
            Vector3 sticksInputs = CharacterManager.Instance.InputManagementProperty.Inputs.RotateCamera;

            const float rotationSpeed = 3f;
            transform.Rotate(sticksInputs.x * -rotationSpeed, sticksInputs.y * -rotationSpeed, 0);
        }
    }
}