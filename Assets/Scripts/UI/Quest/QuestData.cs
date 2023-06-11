using UnityEngine;

namespace UI.Quest
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuestData", order = 1)]
    public class QuestData : ScriptableObject
    {
        public Sprite QuestLogo;
        public string QuestTitle_EN;
        public string QuestTitle_FR;
        [TextArea] public string QuestDescription_EN;
        [TextArea] public string QuestDescription_FR;
    }
}