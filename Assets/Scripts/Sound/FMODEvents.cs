using FMODUnity;
using Tools.SingletonClassBase;
using UnityEngine;

namespace Sound
{
    public class FMODEvents : Singleton<FMODEvents>
    {
        [field: SerializeField, Header("Ambience")] public EventReference Ambience { get; private set; }
        [field: SerializeField, Header("Music")] public EventReference Music { get; private set; }
    }
}
