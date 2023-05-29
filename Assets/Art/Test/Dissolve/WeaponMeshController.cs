
using System.Collections.Generic;
using System.Linq;
using UI.WeaponWheel;
using UnityEngine;

namespace Art.Test.Dissolve
{
    public class DissolveScript : MonoBehaviour
    {
        [SerializeField] private float _noiseStrength = 0.25f;
        [SerializeField] private float _invisibleValue = 0.5f;
        [SerializeField] private float _visibleValue = 2.5f;
        [SerializeField] private float _timeToShow = 1.5f;
        [SerializeField] private List<GameObject> _gameObjectsToActivateAfterDissolve = new List<GameObject>();
        [Range(0,1) ,SerializeField] private float _percentageToShowObjects = 0.7f;
        
        private float _time;
        private bool _launcheffect;
        private List<Material> _materials = new List<Material>();

        private void Awake()
        {
            List<Renderer> rendererList = GetComponentsInChildren<Renderer>().ToList();
            foreach (Renderer r in rendererList)
            {
                _materials.Add(r.material);
            }
        }

        public void Launch()
        {
            _time = 0;
            _launcheffect = true;
            SetDissolve(0);
            _gameObjectsToActivateAfterDissolve.ForEach(x=>x.SetActive(false));
        }

        private void Update()
        {
            if(_launcheffect == false || _time > _timeToShow)
            {
                return;
            }
            
            _time += Time.deltaTime;
            
            float value = (_time / _timeToShow) * (_visibleValue - _invisibleValue) + _invisibleValue;
            SetDissolve(value);

            if ((_time / _timeToShow) > _percentageToShowObjects)
            {
                _gameObjectsToActivateAfterDissolve.ForEach(x=>x.SetActive(true));
            }
        }

        private void SetDissolve(float height)
        {
            _materials.ForEach( x => x.SetFloat("_CutoffHeight", height));
            _materials.ForEach( x => x.SetFloat("_NoiseStrength", _noiseStrength));
        }
    }
}