using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowBtn : MonoBehaviour
{
    // Affichage du contenu
    [SerializeField] GameObject _content;
    [HideInInspector] public bool IsSelected = true;
    public GameObject ButtonGameObject;

    // Modif de l'image de fond
    public GameObject ImageBg;
    private int rotateDirection;
    private float rotateChanged = 0;
    [SerializeField] private float rotateChanger = 1f;
    [SerializeField] private float scaleChanger = 2;

    public void Update()
    {
        // Compare selected gameObject with referenced Button gameObject
        if (EventSystem.current.currentSelectedGameObject == ButtonGameObject)
        {
            if (IsSelected == true)
            {
                int seed = (int)System.DateTime.Now.Ticks;
                Random.InitState(seed);
                rotateDirection = Random.Range(0, 2);
            }
            IsSelected = false;
            _content.SetActive(true);
            // Modif de l'image de fond
            ImageBg.transform.localScale += new Vector3(scaleChanger, scaleChanger, 0) * Time.deltaTime;
            if (rotateDirection == 0) 
                rotateChanged -= rotateChanger * Time.deltaTime;
            else
                rotateChanged += rotateChanger * Time.deltaTime;
            ImageBg.transform.localRotation = Quaternion.Euler(0, 0, rotateChanged);
        }
        else
        {
            IsSelected = true;
            _content.SetActive(false);
            ImageBg.transform.localScale = new Vector3(100, 100, 100);
            ImageBg.transform.rotation = Quaternion.Euler(0, 0, 0);
            rotateChanged = 0;
        }
    }
}
