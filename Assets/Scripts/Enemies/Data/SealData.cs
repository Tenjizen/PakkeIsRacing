using UnityEngine;

namespace Enemies.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SealData", order = 1)]
    public class SealData : ScriptableObject
    {
        [field: SerializeField, Range(0, 0.01f)]
        public float MovingSpeed { get; private set; } = 0.05f;
        
        [field: SerializeField, Range(0, 6)]
        public int Life { get; private set; } = 3;
    }
}