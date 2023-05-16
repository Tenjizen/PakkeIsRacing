using Character;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuStart : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
        {
            DisableMenuStartUI();
        }
    }


    public void DisableMenuStartUI()
    {
        _text.DOFade(0, 0.15f);
    }
}
