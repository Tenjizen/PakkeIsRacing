using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Character.State.CharacterNavigationState;
using DG.Tweening;

public class UISprintManager : MonoBehaviour
{

    [SerializeField] private CharacterMultiPlayerManager Character;

    [SerializeField] private SpriteRenderer _left;
    [SerializeField] private SpriteRenderer _right;

    [SerializeField, Header("Good timing")] private float _scaleMax = 0.05f;
    [SerializeField] private float _speed = 2;
    [SerializeField] private float _duration = 0.5f;

    [SerializeField] private GameObject[] _trails;
    //[SerializeField] private ParticleSystem _particle;

    private Direction _lastDirection;

    public void SprintEnded(float timer)
    {
        if (timer > Character.CharacterManager.KayakControllerProperty.Data.KayakValues.TimerMaxForSprint)
        {
            if (_left.enabled != false || _right.enabled != false)
            {
                _left.enabled = false;
                _right.enabled = false;
            }
            ParticleSpeedEmission(false);

            if (_trails.Length > 0 && _trails[_trails.Length - 1].GetComponent<TrailRenderer>().emitting == true)
                TrailEmitting(false);

            return;
        }

        float middle = (Character.CharacterManager.KayakControllerProperty.Data.KayakValues.TimerMaxForSprint + Character.CharacterManager.KayakControllerProperty.Data.KayakValues.TimerMinForSprint) / 2;
        Vector3 initScale = (Vector3.one / 2) / 10;
        Vector3 scale = Vector3.one * (initScale.x - timer / (middle / initScale.x));
        if (scale.x >= 0)
        {
            if (_lastDirection == Direction.Left)
            {
                _right.gameObject.transform.localScale = scale;
            }
            else if (_lastDirection == Direction.Right)
            {
                _left.gameObject.transform.localScale = scale;
            }
        }
    }

    public IEnumerator GoodTiming()
    {
        float timeElapsed = 0f;
        while (timeElapsed < _duration)
        {
            float t = timeElapsed / _duration;

            if (_lastDirection == Direction.Right)
            {
                _right.enabled = true;
                _right.gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                if (_right.gameObject.transform.localScale.x >= _scaleMax - 0.05f)
                {
                    _right.DOFade(0, 0.2f);

                    if (_right.color.a <= 0.01f)
                    {
                        _right.DOKill();
                        _right.enabled = false;
                        yield return null;
                        break;
                    }
                }
            }
            else if (_lastDirection == Direction.Left)
            {
                _left.enabled = true;
                _left.gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                if (_left.gameObject.transform.localScale.x >= _scaleMax - 0.05f)
                {
                    _left.DOFade(0, 0.2f);
                    if (_left.color.a <= 0.01f)
                    {
                        _left.DOKill();
                        _left.enabled = false;
                        yield return null;
                        break;
                    }
                }
            }
            timeElapsed += Time.deltaTime * _speed;
            yield return null;
        }
    }

    public void UIDirection()
    {
        if (_lastDirection == Direction.Right)
        {
            _right.enabled = false;
        }
        else if (_lastDirection == Direction.Left)
        {
            _left.enabled = false;
        }

    }

    public void DisableSprintUI()
    {
        _right.enabled = false;
        _left.enabled = false;
        TrailEmitting(false);
        ParticleSpeedEmission(false);
    }

    public void EnableFeedback(Direction direction)
    {
        var _character = Character.CharacterManager;
        var velocity = Mathf.Abs(_character.KayakControllerProperty.Rigidbody.velocity.x) + Mathf.Abs(_character.KayakControllerProperty.Rigidbody.velocity.z);
        if (_character.Abilities.SprintUnlock == false ||
            (_character.InputManagementProperty.Inputs.PaddleRight && _character.InputManagementProperty.Inputs.PaddleLeft) ||
            velocity < 13f ||
            direction == _lastDirection)
        {
            if (_left.enabled != false || _right.enabled != false)
            {
                _left.enabled = false;
                _right.enabled = false;
            }
            return;
        }

        TrailEmitting(true);
        ParticleSpeedEmission(true);

        Vector3 initScale = (Vector3.one / 2) / 10;
        if (direction == Direction.Left)
        {
            _right.gameObject.transform.localScale = initScale;
            _right.DOFade(1, 0);
            _right.enabled = true;
        }
        else if (direction == Direction.Right)
        {
            _left.gameObject.transform.localScale = initScale;
            _left.DOFade(1, 0);
            _left.enabled = true;
        }

        //_lastDirection = direction;
    }

    public void ParticleSpeedEmission(bool active)
    {
        //var ps = _particle.emission;
        //ps.enabled = active;
    }
    public void TrailEmitting(bool active)
    {
        foreach (var trail in _trails)
        {
            trail.GetComponent<TrailRenderer>().emitting = active;
        }
    }
}
