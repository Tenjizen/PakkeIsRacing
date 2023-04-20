using System;
using UnityEngine;

namespace Dialog
{
    [Serializable]
    public enum SequencingType
    {
        Automatic = 0,
        PressButton = 1
    }

    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/DialogData", order = 1)]
    public class DialogData : ScriptableObject
    {
        [TextArea] public string Text;
        public float TextShowTime;
        public bool ShowLetterByLetter;
        public float TextHoldTime;
        public Color32 TextColor;
        [Range(0,2)] public float SizeEffect;
        public SequencingType SequencingTypeNext;
        public AudioClip Clip;
    }
}
