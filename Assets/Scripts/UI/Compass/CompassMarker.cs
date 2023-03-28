using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Compass
{
    public class CompassMarker : MonoBehaviour
    {
        [field:SerializeField] public Sprite Icon { get; private set; }
        [ReadOnly] public Image Image;

        public Vector2 Position
        {
            get
            {
                Vector3 position = transform.position;
                return new Vector2(position.x, position.z);
            }
        }
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position,1f);
        }

#endif
    }
}
