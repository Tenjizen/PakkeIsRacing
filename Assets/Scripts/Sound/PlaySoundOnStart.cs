using Character;
using UnityEngine;

namespace Sound
{
    public class PlaySoundOnStart : MonoBehaviour
    {
        [SerializeField] private AudioClip _clip;

        private void Start()
        {
            CharacterManager.Instance.SoundManagerProperty.PlaySound(_clip);
        }
    }
}
