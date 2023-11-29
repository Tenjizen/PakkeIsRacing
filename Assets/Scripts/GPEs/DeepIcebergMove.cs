using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepIcebergMove : MonoBehaviour
{
    [SerializeField] private float LerpMove;
    private List<Transform> _targets = new List<Transform>();
    private int _indexTarget;
    [SerializeField] private float timeStop;
    [SerializeField] Transform _targetRoot;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (_move == false) return;
        MoveIce();
    }
    public void Initialize()
    {

        _indexTarget = 0;


        for (int i = 0; i < _targetRoot.childCount; i++)
        {
            _targets.Add(_targetRoot.GetChild(i));
        }

        if (_targets.Count <= 0) return;

        Vector3 position = _targets[0].position;
        transform.position = new Vector3(position.x, transform.position.y, position.z);
    }
    private void MoveIce()
    {
        Transform t = transform;

        var target = _targets[_indexTarget].position;
        var distTarget = Vector3.Distance(t.position, target);

        if (distTarget <= 0.5f) {
            _indexTarget = (_indexTarget + 1) % _targets.Count;
            _move = false;
            StartCoroutine(StopMove(timeStop)); 
            
        }

        var speed = LerpMove;
        var position = Vector3.MoveTowards(t.position, target, speed * Time.deltaTime);
        t.position = position;
    }
    bool _move = true;
    IEnumerator StopMove(float time)
    {
        yield return new WaitForSeconds(time);
        _move = true;
    }
    //Vector3 velocity;
    //IEnumerator MoveR(float _posX)
    //{
    //    if (_posX <= posX + posXMax && moveR == true)
    //    {

    //        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(transform.localPosition.x + moveX, 0, transform.localPosition.z), ref velocity, LerpMove);
    //    }
    //    if (_posX >= posX + posXMax)
    //    {
    //        yield return new WaitForSeconds(timeStop);
    //        moveR = false;
    //    }
    //}
    //IEnumerator MoveL(float _posX)
    //{
    //    if (_posX >= posX - posXMin && moveR == false)
    //    {
    //        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(transform.localPosition.x - moveX, 0, transform.localPosition.z), ref velocity, LerpMove);
    //    }
    //    if (_posX <= posX - posXMin)
    //    {
    //        yield return new WaitForSeconds(timeStop);
    //        moveR = true;
    //    }
    //}
}
