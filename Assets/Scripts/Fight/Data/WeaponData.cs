using System.Collections.Generic;
using UnityEngine;

namespace Fight.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponData", order = 1)]
    public class WeaponData : ScriptableObject
    {
        public string WeaponName;
        public float LaunchForce;
        public float Cooldown;
        public float Lifetime = 4f;
        public List<GameObject> ForbiddenColliders;
    }
}