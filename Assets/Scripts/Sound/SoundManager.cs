using Tools.SingletonClassBase;
using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {

        [Header("Sources")]
        [SerializeField] private AudioSource _musicSource, _effectSource, _dialogSource;

        public void PlaySound(AudioClip clip)
        {
            _effectSource.PlayOneShot(clip);
        }
    
        public void PlayDialog(AudioClip clip)
        {
            _dialogSource.PlayOneShot(clip);
        }

        public void ChangeMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        public void ToggleEffects()
        {
            _effectSource.mute = _effectSource.mute == false;
        }
    
        public void ToggleMusic()
        {
            _musicSource.mute = _musicSource.mute == false;
        }
    
        public void ToggleSource()
        {
            _dialogSource.mute = _dialogSource.mute == false;
        }
    }
}
