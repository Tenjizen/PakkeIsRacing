using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Character.State.CharacterNavigationState;

public class UISprintManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _left;
    [SerializeField] private SpriteRenderer _right;

    private Direction _lastDirection;

    private float _currentScale;
    private void Start()
    {

    }
    public void ManageScaleUISprint(float timer)
    {
        if (timer > CharacterManager.Instance.KayakControllerProperty.Data.KayakValues.TimerMaxForSprint)
        {
            if (_left.enabled != false || _right.enabled != false)
            {
                _left.enabled = false;
                _right.enabled = false;
            }
            return;
        }

        float middle = (CharacterManager.Instance.KayakControllerProperty.Data.KayakValues.TimerMaxForSprint + CharacterManager.Instance.KayakControllerProperty.Data.KayakValues.TimerMinForSprint) / 2;
        Vector3 initScale = (Vector3.one / 2) / 10;
        Vector3 scale = Vector3.one * (initScale.x - timer / (middle / initScale.x));
        if (scale.x >= 0)
        {
            _right.gameObject.transform.localScale = scale;
            _left.gameObject.transform.localScale = scale;
        }
    }

    public void EnableDisable(Direction direction)
    {
        var _character = CharacterManager.Instance;

        if (_character.Abilities.SprintUnlock == false ||
            (_character.InputManagementProperty.Inputs.PaddleRight && _character.InputManagementProperty.Inputs.PaddleLeft) ||
            _character.KayakControllerProperty.Rigidbody.velocity.magnitude < 5f ||
            direction == _lastDirection)
        {
            if (_left.enabled != false || _right.enabled != false)
            {
                _left.enabled = false;
                _right.enabled = false;
            }
            return;
        }


        if (direction == Direction.Left)
        {
            _right.enabled = true;
            _left.enabled = false;
        }
        else if (direction == Direction.Right)
        {
            _left.enabled = true;
            _right.enabled = false;
        }
        Vector3 initScale = (Vector3.one / 2) / 10;
        _left.gameObject.transform.localScale = initScale;
        _right.gameObject.transform.localScale = initScale;

        _lastDirection = direction;
    }

}
