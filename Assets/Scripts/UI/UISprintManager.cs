using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISprintManager : MonoBehaviour
{
    //[SerializeField] private Image _left;
    //[SerializeField] private Image _right;
    [SerializeField] private Transform _waterRipplePaggaieLeft;
    [SerializeField] private Transform _waterRipplePaggaieRight;

    [SerializeField] private SpriteRenderer _left;
    [SerializeField] private SpriteRenderer _right;


    void Update()
    {
        var _character = CharacterManager.Instance;
        //if (_character.Parameters.SprintUnlock == false && (
        //_character.InputManagementProperty.Inputs.PaddleRight && _character.InputManagementProperty.Inputs.PaddleLeft))
        //{
        //    _left.enabled = false;
        //    _right.enabled = false;
        //    return;
        //}
        
        if (_character.Parameters.SprintUnlock == false || 
            (_character.InputManagementProperty.Inputs.PaddleRight && _character.InputManagementProperty.Inputs.PaddleLeft)||
            _character.KayakControllerProperty.Rigidbody.velocity.magnitude < 5f)
        {
            _left.enabled = false;
            _right.enabled = false;
            return;
        }



        Trigger(0);
    }

    public void Trigger(float timer /*direction*/)
    {
        //timer
        //max
        //min
        _left.enabled = true;
        _right.enabled = true;

        //show image autre direction

        //timer qui monte scale qui descend

        //scale = -timer
        //(min + max) / 2 = scale 0 ?
        //0.05f
        //trigger = scale au dessus de 0 sur l'autre direction

        //max = scale = 0;
        //min = scale = 1 ou au dessus de 0


    }
}
