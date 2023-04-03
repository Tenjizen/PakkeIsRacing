using System;
using System.Collections.Generic;
using UnityEngine;

namespace Character.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExperienceData", order = 1)]
    public class ExperienceData : ScriptableObject
    {
        [Header("Experience gained")]
        public float ExperienceGainedAtCheckpoint;
        public float ExperienceGainedAtColletible;
        public float ExperienceGainedAtEnemy;

        [Header("Experience levels")] 
        public int MaxLevel;
        public List<Level> Levels;
    }

    [Serializable]
    public struct Level
    {
        public float ExperienceNeededToComplete;
        public int CombatExperienceGained;
        public int NavigationExperienceGained;
    }
}