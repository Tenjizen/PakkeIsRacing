using UnityEngine;

namespace Sound
{
    public class ParametersChangeTrigger : MonoBehaviour
    {
        [Header("Parameter Change Ambience"), SerializeField] private string _parameterName;
        [SerializeField] private float _parameterValue;
        [Header("Parameter Change Music"), SerializeField] private string _parameterNameMusic;
        [SerializeField] private float _parameterValueMusic;

        public void SetTension()
        {
            AudioManager.Instance.SetAmbienceParameter(_parameterName, _parameterValue);
            AudioManager.Instance.SetMusicParameter(_parameterNameMusic, _parameterValueMusic);
            Debug.Log("Tension de la musique mise à jour");
        }
    }
}
