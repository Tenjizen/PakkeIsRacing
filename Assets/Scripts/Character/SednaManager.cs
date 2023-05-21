using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterAndFloating;

public class SednaManager : MonoBehaviour
{
    public Waves Waves; 

    #region respawn
    [field: SerializeField, Header("Respawn")] public GameObject SednaGameObject { get; set; }
    [field: SerializeField] public GameObject SednaTargetRespawnPlayer { get; set; }
    public bool SednaIsMoving = false;
    float TimerSednaIsMoving;
    public AnimationCurve Curve;
    private Keyframe _lastKey;
    public GameObject LookAt;
    #endregion

    void Start()
    {
        #region respawn
        _lastKey = Curve[Curve.length - 1];
        #endregion
    }
    private void Update()
    {
        #region respawn
        if (SednaIsMoving == true)
            PlayerRespawn();
        #endregion
    }

    #region respawn function

    public void SednaRespawn()
    {
        if (CharacterManager.Instance.CheckpointManagerProperty.CurrentCheckpoint.NextCheckPoint != null)
        {
            TimerSednaIsMoving = 0;
            SednaIsMoving = true;
            SednaGameObject.transform.position = SednaTargetRespawnPlayer.transform.position;
            SednaGameObject.SetActive(true);
            LookAt.transform.position = CharacterManager.Instance.CheckpointManagerProperty.CurrentCheckpoint.NextCheckPoint.transform.position;
        }
    }

    public void PlayerRespawn()
    {
        if (CharacterManager.Instance.CheckpointManagerProperty.CurrentCheckpoint.NextCheckPoint == null)
        {
            return;
        }
        TimerSednaIsMoving += Time.deltaTime;
        if (TimerSednaIsMoving <= _lastKey.time)
        {
            SednaGameObject.transform.Translate(Vector3.forward * (5 * Time.deltaTime), Space.Self);
            //move sedna
            if (TimerSednaIsMoving >= _lastKey.time - 2)
            {
                var targetPos = LookAt.transform.position;
                targetPos.y -= 0.2f;
                LookAt.transform.position = targetPos;
                SednaGameObject.transform.LookAt(LookAt.transform.position);
            }
            else
            {
                SednaGameObject.transform.LookAt(CharacterManager.Instance.CheckpointManagerProperty.CurrentCheckpoint.NextCheckPoint.transform);
            }
        }
        else
        {
            SednaIsMoving = false;
            SednaGameObject.SetActive(false);
        }

        Vector3 sednaPosition = SednaGameObject.transform.position;
        sednaPosition.y = Curve.Evaluate(TimerSednaIsMoving);
        SednaGameObject.transform.position = sednaPosition;
    }
    #endregion
}
