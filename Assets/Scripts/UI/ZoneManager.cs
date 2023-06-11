using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ZoneManager : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> _textList;
        [SerializeField] private TMP_Text _zoneNameText, _titleText;
        [SerializeField] private Image _separationLine;
        [SerializeField, Range(0,5)] private float _showTime, holdTime, _hideTime;

        private void Start()
        {
            _textList.ForEach(x => x.DOFade(0,0)); 
            _separationLine.DOFade(0, 0);
        }

        public void ShowZone(string zoneName)
        {
            _textList.ForEach(x => x.DOFade(1,_showTime).OnComplete(HideZone));
            _separationLine.DOFade(1, _showTime);
            _zoneNameText.text = zoneName;
            _titleText.text = CharacterManager.Instance.Parameters.Language ? "Checkpoint" : "Point de sauvegarde";
        }
    
        private void HideZone()
        {
            StartCoroutine(HideZoneTexts());
        }

        private IEnumerator HideZoneTexts()
        {
            yield return new WaitForSeconds(holdTime);
            _separationLine.DOFade(0, _hideTime);
            _textList.ForEach(x => x.DOFade(0,_hideTime));
        }
    }
}
