using Character;
using UnityEngine;
using UnityEngine.UI;

namespace Sound
{
    public class VolumeSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private void Start()
        {
            CharacterManager.Instance.SoundManagerProperty.ChangeMasterVolume(_slider.value);
            _slider.onValueChanged.AddListener(value => CharacterManager.Instance.SoundManagerProperty.ChangeMasterVolume(value));
        }
    }
}
