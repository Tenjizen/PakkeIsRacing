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

        private void Start()
        {
            _currentLevel = 0;
            _currentExperience = 0f;
            _currentLevelData = Data.Levels[_currentLevel];
            
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Combat,_currentExperience,_currentLevelData.ExperienceNeededToComplete);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Navigation,_currentExperience,_currentLevelData.ExperienceNeededToComplete);
            ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Experience,_currentExperience,_currentLevelData.ExperienceNeededToComplete);
        }

        public void AddExperience(float value)
        {
            _currentExperience += value;
            if (_currentExperience >= _currentLevelData.ExperienceNeededToComplete)
            {
                LevelUp();
            }
            else
            {
                ExperienceUIManagerProperty.SetGauge(ExperienceUIManager.Gauge.Experience, _currentExperience, _currentLevelData.ExperienceNeededToComplete);
            }
        }

        private void LevelUp()
        {
            _currentExperience -= _currentLevelData.ExperienceNeededToComplete;

            _currentLevel++;
            _currentLevelData = Data.Levels[_currentLevel];
            
            ExperienceUIManagerProperty.SetLevelUp();
        }
    }
}