using System;
using Character;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UI
{
    public class StartVideoPlayerSkip : MonoBehaviour
    {
        [SerializeField] private InputManagement _inputManagement;
        [SerializeField] private string _sceneToGoToName;
        
        private void Update()
        {
            if (_inputManagement.Inputs.Start)
            {
                SceneManager.LoadScene(_sceneToGoToName, LoadSceneMode.Single);
            }
        }
    }
}
