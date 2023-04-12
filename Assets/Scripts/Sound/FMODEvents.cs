using FMODUnity;
using Tools.SingletonClassBase;
using UnityEngine;

namespace Sound
{
    public class FmodEvents : Singleton<FmodEvents>
    {
        [field: SerializeField, Header("Ambience")] public EventReference Ambience { get; private set; }
        [field: SerializeField, Header("Music")] public EventReference Music { get; private set; }
    }
}
