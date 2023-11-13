
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Art.Test.Dissolve
{
    public class WeaponMeshController : MonoBehaviour
    {
        [SerializeField] private float _noiseStrength = 0.25f;
        [SerializeField] private float _invisibleValue = 0.5f;
        [SerializeField] private float _visibleValue = 2.5f;
        [SerializeField] private float _timeToShow = 1.5f;
        [SerializeField] private float _cutoffHeightLaunch = 1000f;
        [SerializeField] private List<GameObject> _gameObjectsToActivateAfterDissolve = new List<GameObject>();
        [Range(0,1) ,SerializeField] private float _percentageToShowObjects = 0.7f;
        
        private float _time;
        private bool _launcheffect;
        private List<Material> _materials = new List<Material>();
        private List<Renderer> _rendererList = new List<Renderer>();

        private void Awake()
        {
            _rendererList = GetComponentsInChildren<Renderer>().ToList();
            foreach (Renderer r in _rendererList)
            {
                _materials.Add(r.material);
            }
        }

        public void LaunchDissolveUp()
        {
            _time = 0;
            _launcheffect = true;
            SetDissolve(0);
            
            SetMeshes(true);
            _gameObjectsToActivateAfterDissolve.ForEach(x=>x.SetActive(false));
        }

        public void SetMeshes(bool set)
        {
            _rendererList.ForEach(x=> x.gameObject.SetActive(set));
            _gameObjectsToActivateAfterDissolve.ForEach(x=>x.SetActive(set));
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

        public void SetDissolveMax()
        {
            SetDissolve(_cutoffHeightLaunch);
        }
    }
}