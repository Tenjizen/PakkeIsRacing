using GameTools;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBird : MonoBehaviour
{
    #region Variables
    [Header("Objcet to pool"), SerializeField] ObjectPool _birdObjectPool;
    //[SerializeField] Vector3 _offset;
    [Header("Target"), SerializeField] Transform _targetSpawn;
    [SerializeField] Transform[] _birdSpawns;

    [Tooltip("Time before the objects spawn")]
    [SerializeField] float _timerMin;
    [SerializeField] float _timerMax;

    [Header("Bird"),Tooltip("Number of objects to spawn")]
    [SerializeField] float _numberBirdMin;
    [SerializeField] float _numberBirdMax;
    [Tooltip("Time before the objects disappears"),SerializeField]float _lifeTimer;

    [Header("Radius"),Tooltip("Radius of the circle where the birds will appear"), SerializeField] float _radius;

    private float _randTimer;
    private float _randNumberBird;

    private bool _timerForSpawn = false;
    private float _timerLife;
    private float _timerSpawnBird;

    private List<GameObject> _tempBirdList = new List<GameObject>();
    #endregion

    void Start()
    {
        if (_numberBirdMax > _birdObjectPool.AmountToPool)
        {
            _numberBirdMax = _birdObjectPool.AmountToPool;
        }

        _randTimer = Random.Range(_timerMin, _timerMax);
        _randNumberBird = Random.Range(_numberBirdMin, _numberBirdMax);
        _timerForSpawn = true;

    }

    void Update()
    {
        if (_timerForSpawn == true)
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
                    for (int i = _tempBirdList.Count - 1; i >= 0; i--)
                    {
                        if (_tempBirdList[i].transform.localScale.x > 0.3f)
                        {
                            _tempBirdList[i].transform.localScale -= Vector3.one * 0.1f;
                        }
                        else
                        {
                            _tempBirdList[i].gameObject.SetActive(false);
                            _tempBirdList.Remove(_tempBirdList[i]);
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
            _timerForSpawn = true;
        }


        if (_timerSpawnBird >= _randTimer)
        {
            _timerForSpawn = false;
            _timerLife = _lifeTimer;
            _timerSpawnBird = 0;
            _tempBirdList.Clear();



            var randomPosInCircle = new Vector2(RandomNumberInCircleInterval(0.8f, 1), RandomNumberInCircleInterval(0.8f, 1));
            Vector3 targetPos = _targetSpawn.position;
            targetPos.x += randomPosInCircle.x * _radius;
            targetPos.y = Random.Range(20, 40);
            targetPos.z += randomPosInCircle.y * _radius;
            this.transform.position = targetPos;

            Vector3 lookAtPos = _targetSpawn.position - transform.position;
            lookAtPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookAtPos);


            //float count = 0;
            for (int i = 0; i < _randNumberBird; i++)
            {
                SpawnObject(_birdSpawns[i].position);
                //int signI = i % 2 == 0 ? 1 : -1;
                //SpawnObject(new Vector3((i * signI), 0, -(count * 2)));
                //if (i % 2 == 0)
                //{
                //    count++;
                //}
            }
            _randTimer = Random.Range(_timerMin, _timerMax);
            _randNumberBird = Random.Range(_numberBirdMin, _numberBirdMax);
        }
    }

    private void SpawnObject(Vector3 position)
    {
        GameObject bird = _birdObjectPool.GetPooledObject();
        _tempBirdList.Add(bird);

        if (bird == null) return;

        bird.gameObject.SetActive(true);
        bird.transform.position = position;
        bird.GetComponent<MovingBird>().ResetVariable();
        bird.transform.localScale = Vector3.zero;
        bird.transform.localEulerAngles = Vector3.zero;
    }
    private float RandomNumberInCircleInterval(float min, float max)
    {
        if (Random.Range(0, 100) < 50)
        {
            return Random.Range(-max, -min);
        }
        else
        {
            return Random.Range(min, max);
        }
    }
}
