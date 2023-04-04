using System;
using Character.Data;
using UI;
using UnityEngine;

namespace Character
{
    public class ExperienceManager : MonoBehaviour
    {
        [field:SerializeField] public ExperienceUIManager ExperienceUIManagerProperty { get; private set; }
        public ExperienceData Data;

        [ReadOnly, SerializeField] private Level _currentLevelData;
        [ReadOnly, SerializeField] private int _currentLevel;
        [ReadOnly, SerializeField] private float _currentExperience;
        [ReadOnly, SerializeField] private float _currentCombatExperience;
        [ReadOnly, SerializeField] private float _currentNavigationExperience;

        private void Start()
        {
            _currentLevel = 0;
            _currentExperience = 0f;
            _currentLevelData = Data.Levels[_currentLevel];

            ExperienceUIManagerProperty.SetMaxLevel(Data.MaxLevel);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Combat,_currentExperience,_currentLevelData.ExperienceNeededToComplete);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Navigation,_currentExperience,_currentLevelData.ExperienceNeededToComplete);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Experience,_currentExperience,_currentLevelData.ExperienceNeededToComplete);
        }

        public void AddExperience(float value)
        {
            _currentExperience += value;
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Experience, _currentExperience, _currentLevelData.ExperienceNeededToComplete);
            
            if (_currentExperience >= _currentLevelData.ExperienceNeededToComplete && _currentLevel <= Data.MaxLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            _currentExperience -= _currentLevelData.ExperienceNeededToComplete;

            _currentCombatExperience += _currentLevelData.CombatExperienceGained;
            _currentNavigationExperience += _currentLevelData.NavigationExperienceGained;
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Combat, _currentCombatExperience, Data.CombatGaugeMax);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Navigation, _currentNavigationExperience, Data.NavigationGaugeMax);
                
            _currentLevel++;
            _currentLevelData = Data.Levels[_currentLevel];
            
        }
    }
}