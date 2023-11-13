using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptForDebug : MonoBehaviour
{

    [SerializeField] CanvasGroup _fadeDead;

    private float _timerDebug;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_fadeDead.alpha > 0.9f)
        {
            _timerDebug += Time.deltaTime;

            if(_timerDebug > 10f)
            {
                //Reset value
                CharacterManager.Instance.KayakControllerProperty.CanReduceDrag = true;
                CharacterManager.Instance.SetBalanceValueToCurrentSide(0);
                
                CharacterManager.Instance.TransitionManagerProperty.LaunchTransitionOut(SceneTransition.TransitionType.Fade);
              
            }
        }

    }

    public void ResetTimerDebug()
    {
        _timerDebug = 0;
    }

}
