using System;
using Character;
using UnityEngine;

namespace Sound
{
    public class ToggleAudio : MonoBehaviour
    {
        [Serializable]
        private enum AudioType
        {
            Music,
            Effects,
            Dialog
        }

        [SerializeField] private AudioType _toggleType;

        public void Toggle()
        {
            switch (_toggleType)
            {
                case AudioType.Music:
                    CharacterManager.Instance.SoundManagerProperty.ToggleMusic();
                    break;
                case AudioType.Effects:
                    CharacterManager.Instance.SoundManagerProperty.ToggleEffects();
                    break;
            }
        }
    }
}
