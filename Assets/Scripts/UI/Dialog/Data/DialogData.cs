using System;
using System.Collections.Generic;
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
        public List<DialogStruct> DialogList = new List<DialogStruct>();
        [Space(10), Header("Dialog Summary")] public string Title;
        [TextArea] public string Summary;
    }
    
    [Serializable]
    public struct DialogStruct
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
