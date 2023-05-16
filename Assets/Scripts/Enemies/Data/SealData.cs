using UnityEngine;
using WaterAndFloating;

namespace Enemies.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SealData", order = 1)]
    public class SealData : ScriptableObject
    {
        [field: SerializeField, Range(0, 0.01f)]
        public float MovingSpeed { get; private set; } = 0.05f;
        
        [field: SerializeField, Range(0, 6)]
        public int Life { get; private set; } = 3;

        [field: SerializeField] 
        public float StopMovementRadius { get; private set; } = 3;
        
        [field: SerializeField] 
        public AnimationCurve UpAndDownMovements { get; private set; }
        
        [field: SerializeField] 
        public float UpAndDownMultiplier { get; private set; } = 3;
        
        [field: SerializeField] 
        public float WaveHeightOffset { get; private set; } = -2;
        
        [field: SerializeField] 
        public CircularWave Wave { get; private set; }
    }
}