using UnityEngine;
using System.Collections;
public class MovingBird : MonoBehaviour
{
    public float HorizontalSpeed;
    public float VerticalSpeed;
    public float Amplitude;

    [SerializeField] private Animator _animator;

    private Vector3 _tempPosition;
    private Vector3 _startPosition;
    private float _startTime;


    void Start()
    {
        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        _animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f) * state.length);

        _startTime = Random.value;
        _tempPosition = transform.position;
        _startPosition = transform.position;
    }
    void FixedUpdate()
    {
        _tempPosition += transform.forward * Time.fixedDeltaTime * HorizontalSpeed;

        _tempPosition.y = Mathf.Sin(_startTime + Time.realtimeSinceStartup * VerticalSpeed * Time.fixedDeltaTime) * Amplitude + _startPosition.y;
        transform.position = _tempPosition;
    }


    public void ResetVariable()
    {
        _tempPosition = transform.position;
        _startPosition = transform.position;

        AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
        _animator.Play(state.fullPathHash, -1, Random.Range(0f, 1f) * state.length);
    }
}