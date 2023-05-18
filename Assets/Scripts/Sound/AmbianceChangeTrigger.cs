using UnityEngine;

namespace Sound
{
    public class AmbianceChangeTrigger : MonoBehaviour
    {
        [Header("Area")]
        [SerializeField] private AmbienceArea area;
        public void SetAmbience()
        {
            AudioManager.Instance.SetAbmienceArea(area);
            Debug.Log("Type de l'ambiance mise à jour");
        }
    }
}
