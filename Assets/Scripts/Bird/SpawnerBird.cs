using Character;
using GameTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBird : MonoBehaviour
{
    [SerializeField] ObjectPool _birdObjectPool;

    [SerializeField] Transform[] _pointsSpawns;

    [SerializeField] float _timerMin;
    [SerializeField] float _timerMax;

    [SerializeField] float _numberBirdMin;
    [SerializeField] float _numberBirdMax;

    float _randTimer;
    float _timerSpawnBird;
    float _randNumberBird;

    public float Radius;

    public bool timerRun = false;
    public float LifeTimer;
    private float _timerLife;


    private List<GameObject> _tempBirdList = new List<GameObject>();

    void Start()
    {
        _randTimer = Random.Range(_timerMin, _timerMax);
        _randNumberBird = Random.Range(_numberBirdMin, _numberBirdMax);
        timerRun = true;

    }

    void Update()
    {
        if (timerRun == true)
        {
            _timerSpawnBird += Time.deltaTime;
        }
        else
        {
            _timerLife -= Time.deltaTime;
            if (_timerLife <= 0)
            {
                if (_tempBirdList.Count > 0)
                {
                    foreach (var item in _tempBirdList)
                    {
                        if (item.transform.localScale.x > 0.3f)
                        {
                            item.transform.localScale -= Vector3.one * 0.1f;
                        }
                        else
                        {
                            item.gameObject.SetActive(false);
                            _tempBirdList.Remove(item);
                        }
                    }
                }
            }
            else
            {
                if (_tempBirdList.Count > 0)
                {
                    foreach (var item in _tempBirdList)
                    {
                        if (item.transform.localScale.x < 3)
                        {
                            item.transform.localScale += Vector3.one * 0.1f;
                        }
                    }
                }
            }
        }

        if (_tempBirdList.Count <= 0)
        {
            timerRun = true;
        }


        if (_timerSpawnBird >= _randTimer)
        {
            timerRun = false;
            _timerLife = LifeTimer;
            _timerSpawnBird = 0;
            _tempBirdList.Clear();

            var randomPosInCircle = new Vector2(Random.Range(0.5f, 1), Random.Range(0.5f, 1)) * Radius;
            Vector3 targetPos = CharacterManager.Instance.KayakControllerProperty.gameObject.transform.position;
            targetPos.x += randomPosInCircle.x;
            targetPos.y = 15;
            targetPos.z += randomPosInCircle.y;
            this.transform.position = targetPos;

            Vector3 lookAtPos = CharacterManager.Instance.KayakControllerProperty.gameObject.transform.position - transform.position;
            lookAtPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookAtPos);

            for (int i = 0; i < _randNumberBird; i++)
            {
                SpawnObject(_pointsSpawns[i].transform.position);
            }
            _randTimer = Random.Range(_timerMin, _timerMax);
            _randNumberBird = Random.Range(_numberBirdMin, _numberBirdMax);
        }
    }


    private void SpawnObject(Vector3 position)
    {
        GameObject bird = _birdObjectPool.GetPooledObject();
        _tempBirdList.Add(bird);
        if (bird == null)
        {
            return;
        }

        bird.gameObject.SetActive(true);
        bird.GetComponent<MovingBird>().ResetVariable(position);
        //bird.transform.position = position;
        bird.transform.localScale = Vector3.zero;
        bird.transform.localEulerAngles = Vector3.zero;
    }
}
