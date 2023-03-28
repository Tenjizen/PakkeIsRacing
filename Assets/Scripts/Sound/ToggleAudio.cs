using System;
using System.Collections;
using System.Collections.Generic;
using Sound;
using UnityEngine;

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
                SoundManager.Instance.ToggleMusic();
                break;
            case AudioType.Effects:
                SoundManager.Instance.ToggleEffects();
                break;
        }
    }
}
