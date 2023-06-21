using System;
using System.Collections.Generic;
using Character.Data;
using Character.Data.Experience;
using Fight.Data;
using UI;
using UI.WeaponWheel;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class ExperienceManager : MonoBehaviour
    {
        [field:SerializeField] public ExperienceUIManager ExperienceUIManagerProperty { get; private set; }
        public ExperienceData Data;

        [Header("Levels"),ReadOnly, SerializeField] private Level _currentLevelData;
        [ReadOnly, SerializeField] private int _currentLevel;
        [ReadOnly, SerializeField] private float _currentExperience;

        [Header("Events")] 
        public List<UnityEvent> EventAtEachLevel = new List<UnityEvent>();

        [ReadOnly] public int SkillPoints;
        
        private void Start()
        {
            _currentLevel = 0;
            _currentExperience = 0f;
            _currentLevelData = Data.Levels[_currentLevel];

            ExperienceUIManagerProperty.SetMaxLevel(Data.Levels.Count);
            ExperienceUIManagerProperty.SetGauge(_currentExperience,_currentLevelData.ExperienceNeededToComplete, 0f);
        }

        public void AddExperience(float value)
        {
            _currentExperience += value * CharacterManager.Instance.PlayerStats.ExperienceGainMultiplier;
            ExperienceUIManagerProperty.SetGauge(_currentExperience, _currentLevelData.ExperienceNeededToComplete, 2f);
            
            while (_currentExperience >= _currentLevelData.ExperienceNeededToComplete && _currentLevel < Data.Levels.Count-1)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            _currentExperience -= _currentLevelData.ExperienceNeededToComplete;

            _currentLevel++;
            SkillPoints++;
            _currentLevelData = Data.Levels[_currentLevel];

            //events
            if (_currentLevel < EventAtEachLevel.Count)
            {
                EventAtEachLevel[_currentLevel].Invoke();
            }
            
            //weapons
            for (int i = 0; i < Data.WeaponLevels.Count; i++)
            {
                if (Data.WeaponLevels[i].Level > _currentLevel)
                {
                    continue;
                }

                WheelButton button = CharacterManager.Instance.WeaponUIManagerProperty.Buttons.Find(x => x.ButtonController.Type == Data.WeaponLevels[i].Type);
                button.ButtonController.SetCanBeUnlocked(true);
            }
        }
    }
}