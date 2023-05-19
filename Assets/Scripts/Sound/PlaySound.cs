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
        
        public void PlaySoundTrigger()
        {
            /*if (sound.Path == null || AudioManager.Instance == null)
            {
                return;
            }*/
            AudioManager.Instance.PlayOneShot(sound, this.transform.position);
        }
    }
}
