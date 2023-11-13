using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Character.Data.Experience
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ExperienceData", order = 1)]
    public class ExperienceData : ScriptableObject
    {
        [field:SerializeField, Header("Experience gained")]
        public float ExperienceGainedAtCheckpoint { get; private set; } 
        
        
        [field:SerializeField]
        public float ExperienceGainedAtCollectible { get; private set; } 
        
        
        [field:SerializeField]
        public float ExperienceGainedAtEnemySeal { get; private set; } 
        
        [field:SerializeField]
        public float ExperienceGainedAtEnemyShark { get; private set; } 

        
        [field:SerializeField, Header("Experience levels")] 
        public List<Level> Levels { get; private set; } 
    }

    [Serializable]
    public struct Level
    {
        public float ExperienceNeededToComplete;
    }
}