using System;
using System.Collections.Generic;
using Character;
using TMPro;
using UI.SkillTree;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI.Menu
{
    [Serializable]
    public struct SkillTreeLine
    {
        public List<SkillTileUIObject> Tiles;
    }
    
    public class SubMenuSkillTreeController : MenuController
    {
        [SerializeField] private Color _navigationColor, _combatColor;
        [SerializeField] private List<SkillTreeLine> _navigationLines, _combatLines;
        [SerializeField] private TMP_Text _titleText, _descriptionText, _skillPointsText;

        private List<SkillTileUIObject> _objects = new List<SkillTileUIObject>();
        private SkillTileUIObject _currentTile;

        private void Awake()
        {
            foreach (SkillTreeLine line in _navigationLines)
            {
                foreach (SkillTileUIObject skillObject in line.Tiles)
                {
                    _objects.Add(skillObject);
                }
            }
            foreach (SkillTreeLine line in _combatLines)
            {
                foreach (SkillTileUIObject skillObject in line.Tiles)
                {
                    _objects.Add(skillObject);
                }
            }

            _currentTile = _navigationLines[0].Tiles[0];
            _currentTile.OnActivated.AddListener(ActivateSkill);
        }

        private void SetColors()
        {
            foreach (SkillTreeLine line in _navigationLines)
            {
                line.Tiles.ForEach(x => x.SetSkillTile(_navigationColor));
            }
            foreach (SkillTreeLine line in _combatLines)
            {
                line.Tiles.ForEach(x => x.SetSkillTile(_combatColor));
            }
        }
        
        public override void SetMenu(bool isActive, bool isUsable)
        {
            base.SetMenu(isActive, isUsable);

            SetColors();

            Height = _navigationLines.Count;

            if (isUsable == false)
            {
                return;
            }

            SetSkillsPoints();
            SetLine(Length,VerticalIndex);
            SetTile();
        }

        private void SetSkillsPoints()
        {
            _skillPointsText.text = CharacterManager.Instance.ExperienceManagerProperty.SkillPoints.ToString();
        }

        #region Direction

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex <= 0)
            {
                return;
            }

            base.Up(context);
            SetLine(Length,VerticalIndex);
            SetTile();
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex >= Height-1)
            {
                return;
            }

            base.Down(context);
            SetLine(Length,VerticalIndex);
            SetTile();
        }
        
        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false || HorizontalIndex <= 0)
            {
                return;
            }

            base.Left(context);
            SetTile();
        }

        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false || HorizontalIndex >= Length-1)
            {
                return;
            }
            
            base.Right(context);
            SetTile();
        }

        #endregion

        private void SetLine(int previousLenght, int newVerticalIndex)
        {
            VerticalIndex = newVerticalIndex < 0 ? 0 : newVerticalIndex;
            Length = _navigationLines[VerticalIndex].Tiles.Count + _combatLines[VerticalIndex].Tiles.Count;

            HorizontalIndex = IsLeftSide(previousLenght) ? 0 : Length/2;
        }

        private bool IsLeftSide(int lenght)
        {
            return ((float)HorizontalIndex + 1) / (float)lenght <= 0.5f;
        }

        private void SetTile()
        {
            List<SkillTileUIObject> line = IsLeftSide(Length) ? _navigationLines[VerticalIndex].Tiles : _combatLines[VerticalIndex].Tiles;

            _objects.ForEach(x => x.Set(false));

            _currentTile.OnActivated.RemoveListener(ActivateSkill);
            _currentTile = line[IsLeftSide(Length) ? HorizontalIndex : Math.Clamp(HorizontalIndex-1,0,100) / 2 ];
            _currentTile.Set(true);
            _currentTile.OnActivated.AddListener(ActivateSkill);


            if (_titleText == null || _descriptionText == null)
            {
                return;
            }

            _titleText.text = _currentTile == null ? String.Empty : _currentTile.GetTitle();
            _descriptionText.text = _currentTile == null ? String.Empty : _currentTile.GetDescription();
        }

        private void ActivateSkill()
        {
            if (CharacterManager.Instance.ExperienceManagerProperty.SkillPoints <= 0 ||
                _currentTile.CanBeActivated() == false)
            {
                return;
            }
            
            _currentTile.SetActivated(true);
            CharacterManager.Instance.ExperienceManagerProperty.SkillPoints--;
            SetSkillsPoints();

            //unlock line below
            bool isNavigation = IsLeftSide(Length);
            List<SkillTreeLine> skillTree = isNavigation ? _navigationLines : _combatLines;
            int index = VerticalIndex + 1 < skillTree.Count ? VerticalIndex + 1 : -1;
            if (index == -1)
            {
                return;
            }
            skillTree[index].Tiles.ForEach(x => x.SetLock(true));
        }
    }
}