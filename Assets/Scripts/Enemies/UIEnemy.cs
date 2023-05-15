using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _uIGameObject;
    [SerializeField] private GameObject _lifeGauge;




    // Start is called before the first frame update
    void Start()
    {
        _uIGameObject.SetActive(false);
    }

    public void SetGauge(float life, float maxLife)
    {
        float percent = life / maxLife;
        var scale = _lifeGauge.transform.localScale;
        scale.x = percent;
        _lifeGauge.transform.localScale = scale;
    }

    public void ActiveGameObject()
    {
        _uIGameObject.SetActive(true);
    }
    public void DisableGameObject()
    {
        _uIGameObject.SetActive(false);
    }
}
