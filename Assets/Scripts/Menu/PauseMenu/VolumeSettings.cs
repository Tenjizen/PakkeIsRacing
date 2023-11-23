using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public Slider SliderVolume;
    public Slider SliderSFX;

    private void Awake()
    {
        AudioManager volume = FindObjectOfType<AudioManager>();
        SliderVolume.value = volume.MusicSliderValue;
        SliderSFX.value = volume.SfxSliderValue;
    }

    public void OnVolumeMusicChanged(float value)
    {
        AudioManager volume = FindObjectOfType<AudioManager>();
        if (volume != null)
            volume.SetMusicLevel(value);
    }
    public void OnVolumeSfxChanged(float value)
    {
        AudioManager volume = FindObjectOfType<AudioManager>();
        if (volume != null)
            volume.SetSFXLevel(value);
    }

}
