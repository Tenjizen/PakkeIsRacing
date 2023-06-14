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
            if (IsUsable == false || VerticalIndex >= Height)
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
            print(newVerticalIndex);
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

            SkillTileUIObject skillTile = line[IsLeftSide(Length) ? HorizontalIndex : Math.Clamp(HorizontalIndex-1,0,100) / 2 ];
            skillTile.Set(true);

            if (_titleText == null || _descriptionText == null)
            {
                return;
            }

            _titleText.text = skillTile == null ? String.Empty : skillTile.GetTitle();
            _descriptionText.text = skillTile == null ? String.Empty : skillTile.GetDescription();
        }
    }
}