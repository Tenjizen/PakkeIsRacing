using Kayak;
using UnityEngine;

public class EnemyCombatState : EnemyBaseState
{
    private float timer;
    public override void EnterState(EnemyManager enemyManager)
    {
        Debug.Log("combat");
        circle = enemyManager.GetComponentInChildren<SpriteRenderer>().gameObject;
    }

    public override void UpdateState(EnemyManager enemyManager)
    {
        timer += Time.deltaTime;
        if (enemyManager.Target != null)
        {
            if (timer > enemyManager.TimerToJump && jump == false)
            {
                circle.SetActive(true);
                CircleUI(enemyManager);
                //Jump(enemyManager);
            }
            else
            {
                RotateAround(enemyManager);
                circle.SetActive(false);

            }


            if (jump == true)
                Jump(enemyManager);

        }
    }
    private bool jump = false;
    public override void FixedUpdate(EnemyManager enemyManager)
    {

    }

    public override void SwitchState(EnemyManager enemyManager)
    {

    }

    private void RotateAround(EnemyManager enemyManager)
    {
        Vector3 position = enemyManager.Target != null ? enemyManager.Target.position : Vector3.zero;

        enemyManager.PositionOffset.Set(Mathf.Cos(enemyManager.Angle) * enemyManager.CircleRadius, enemyManager.ElevationOffset, Mathf.Sin(enemyManager.Angle) * enemyManager.CircleRadius);
        enemyManager.transform.position = new Vector3(enemyManager.Target.position.x + enemyManager.PositionOffset.x, 0, enemyManager.Target.position.z + enemyManager.PositionOffset.z);
        enemyManager.Angle += Time.deltaTime * enemyManager.RotationSpeed;

        Quaternion rotation = enemyManager.transform.rotation;
        Vector3 enemyPos = enemyManager.transform.position;

        rotation = Quaternion.LookRotation(new Vector3(position.x - enemyPos.x, 0, position.z - enemyPos.z), enemyManager.Target.up);
        rotation.x = 0;
        rotation.z = 0;
        enemyManager.transform.rotation = rotation;

        enemyManager.Angle += Time.deltaTime * enemyManager.RotationSpeed;
    }

    float time = 0;
    private Keyframe lastKey;

    private void Jump(EnemyManager enemyManager)
    {
        time += Time.deltaTime;

        if (enemyManager.Local == true)
        {
            Vector3 posTarget = circle.transform.localPosition;
            enemyManager.transform.localPosition = posTarget;
        }
        else
        {
            Vector3 posTarget = circle.transform.position;
            enemyManager.transform.position = posTarget;
        }

        Vector3 pos = enemyManager.transform.position;
        pos.y = enemyManager.jumpCurve.Evaluate(time);
        enemyManager.transform.position = pos;

        
        
        lastKey = enemyManager.jumpCurve[enemyManager.jumpCurve.length - 1];

        #region rotation y
        Vector3 rotation = enemyManager.transform.eulerAngles;
        if (time > lastKey.time / 2)
        {
            rotation.z = -90;
            enemyManager.transform.eulerAngles = rotation;
        }
        else
        {
            rotation.z = 90;
            enemyManager.transform.eulerAngles = rotation;
        }
        #endregion

        if (time >= lastKey.time)
        {
            RotateAround(enemyManager);
            timer = 0;
            time = 0;
            jump = false;
        }

    }



    private GameObject circle;
    float timerFollow = 0;
    private void CircleUI(EnemyManager enemyManager)
    {
        if (timerFollow < enemyManager.TimerCircleFollow)
        {
            circle.transform.position = enemyManager.Target.position;

        }

        circle.transform.localScale += (Vector3.one / 2) * Time.deltaTime;
        timerFollow += Time.deltaTime;

        if (circle.transform.localScale.x >= 2.5f && timerFollow > 3)
        {
            jump = true;
            //Jump(enemyManager);
            circle.transform.localScale = Vector3.one;
            timerFollow = 0;
        }

    }
}
