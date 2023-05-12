using FMODUnity;
using Tools.SingletonClassBase;
using UnityEngine;

namespace Sound
{
    public class FMODEvents : Singleton<FMODEvents>
    {
        [field: SerializeField, Header("Ambience")] public EventReference Ambience { get; private set; }
        [field: SerializeField, Header("Music")] public EventReference Music { get; private set; }
        [field: Header("SFX"), SerializeField] public EventReference checkpointActivated { get; set; }
        [field: SerializeField, Header("SFX")] public EventReference harpoonThrow { get; set; }
        [field: SerializeField, Header("SFX")] public EventReference harpoonHit { get; set; }
        [field: SerializeField, Header("SFX")] public EventReference harpoonEquiped { get; set; }
        [field: SerializeField, Header("SFX")] public EventReference netThrow { get; set; }
        [field: SerializeField, Header("SFX")] public EventReference netHit { get; set; }
        [field: SerializeField, Header("SFX")] public EventReference netEquiped { get; set; }

        //public static FMODEvents Instance { get; private set; }
    }
}
