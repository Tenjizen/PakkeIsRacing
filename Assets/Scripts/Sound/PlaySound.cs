using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

namespace Sound
{
    //[RequireComponent(typeof(StudioEventEmitter))]
    public class PlaySound : MonoBehaviour
    {
        [SerializeField] private EventReference sound;
        // Start is called before the first frame update
        public void PlaySoundTrigger()
        {
            AudioManager.Instance.PlayOneShot(sound, this.transform.position);
        }
    }
}
