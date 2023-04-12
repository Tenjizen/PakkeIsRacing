using UnityEngine;

namespace Sound
{
    public class AmbienceChangeTrigger : MonoBehaviour
    {
        [Header("Parameter Change"), SerializeField] private string _parameterName;
        [SerializeField] private float _parameterValue;

        private void OnTriggerEnter(Collider collision)
        {
            if (GetComponent<BoxCollider>().tag.Equals("Player"))
            {
                AudioManager.Instance.SetAmbienceParameter(_parameterName, _parameterValue);
            }
        }
    }
}
