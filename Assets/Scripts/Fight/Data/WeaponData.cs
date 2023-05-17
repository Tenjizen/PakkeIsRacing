using System.Collections.Generic;
using UnityEngine;

namespace Fight.Data
{
    public enum WeaponType
    {
        Harpoon = 0,
        Net = 1
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public WeaponType Type;
        public float LaunchForce;
        public float Cooldown;
        public float Lifetime = 4f;
        public List<GameObject> ForbiddenColliders;

        public WeaponData()
        {
            ForbiddenColliders = new List<GameObject>();
        }
    }
}