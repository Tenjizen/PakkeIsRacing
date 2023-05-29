
using System.Collections.Generic;
using System.Linq;
using UI.WeaponWheel;
using UnityEngine;

namespace Art.Test.Dissolve
{
    public class DissolveScript : MonoBehaviour
    {
        [SerializeField] private float _noiseStrength = 0.25f;
        [SerializeField] private float _objectHeight = 1.0f;
        [SerializeField] private float _invisibleValue = 0.5f;
        [SerializeField] private float _visibleValue = 2.5f;
        [SerializeField] private float _timeToShow = 1.5f;
        
        private float _time;
        private bool _launcheffect;
        private List<Material> _materials = new List<Material>();

        private void Awake()
        {
            List<Renderer> _renderers = GetComponentsInChildren<Renderer>().ToList();
            foreach (Renderer renderer in _renderers)
            {
                _materials.Add(renderer.material);
            }
        }

        public void Launch()
        {
            _time = 0;
            _launcheffect = true;
        }

        private void Update()
        {
            if(_launcheffect == false || _time > _timeToShow)
            {
                return;
            }
            
            Debug.Log("launch");
            _time += Time.deltaTime;
            float value = (_time / _timeToShow) * (_visibleValue - _invisibleValue) + _invisibleValue;
            SetDissolve(value);
            
            //var time = Time.time * Mathf.PI * 0.25f;
            //float height = transform.position.y;
            //height += Mathf.Sin(time) * (objectHeight / 2.0f);
            //SetHeight(height);
        }

        private void SetDissolve(float height)
        {
            _materials.ForEach( x => x.SetFloat("_CutoffHeight", height));
            _materials.ForEach( x => x.SetFloat("_NoiseStrength", _noiseStrength));
        }
    }
}