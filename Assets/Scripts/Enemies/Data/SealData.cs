using UnityEngine;

namespace Enemies.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SealData", order = 1)]
    public class SealData : ScriptableObject
    {
        [field: SerializeField, Header("Speed"), Range(0, 0.1f)]
        public float MovingValueAtPlayerDetected { get; private set; } = 0.05f;
        
        [field: SerializeField, Range(0, 1f)]
        public float SealSpeedLerpToMovingValue { get; private set; } = 0.1f;
    }
}