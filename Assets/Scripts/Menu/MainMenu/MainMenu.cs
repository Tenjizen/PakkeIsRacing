using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    //public Image imageFade;

    //public GameObject Title;

    public GameObject accueil;
    public GameObject parameter;
    public GameObject controls;
    public GameObject credits;


    [Header("Controller"), Space(5)]
    public GameObject parameterFirst;
    public GameObject controlsFirst, creditsFirst;
    public GameObject parameterReturn, controlsReturn, creditsReturn;

    private GameObject _current;
    //public List<Interface> buttons;
    private GameObject _currentSelected;

    private void Update()
    {
        //if (accueil.gameObject.activeInHierarchy == true) Title.SetActive(true);
        //else Title.SetActive(false);
    }
    private void Awake()
    {
        accueil.SetActive(true);
        _current = accueil;
        parameter.SetActive(false);
        controls.SetActive(false);
        credits.SetActive(false);
    }
    private void Start()
    {
        //AudioManager.Instance.StopMusicCoroutine();
        //AudioManager.Instance.Musique = AudioManager.Instance.IEPlayMusicSound("snd_music_menu");
        //StartCoroutine(AudioManager.Instance.Musique);
    }
    public void OnClickPlay()
    {

        //imageFade.DOFade(1, 0.8f).OnComplete(FadePlayComplete);
        //for (int i = 1; i < buttons.Count; i++)
        //{
        //    buttons[i].Hide(0.1f);
        //}//Polish pas la
        FadePlayComplete();//a tej si tu remet au dessus
    }

    void FadePlayComplete()
    {
        //AudioManager.Instance.StopMusicCoroutine();
        //AudioManager.Instance.Musique = AudioManager.Instance.IEPlayMusicSound("main_musique");
        //StartCoroutine(AudioManager.Instance.Musique);

        //AudioManager.Instance.PlaySFXSound("snd_interface");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnClickParameter()
    {
        _current.SetActive(false);
        parameter.SetActive(true);
        _current = parameter;


        EventSystem.current.SetSelectedGameObject(parameterFirst);
        _currentSelected = parameterReturn;

        //AudioManager.Instance.PlaySFXSound("snd_interface");
    }
    public void OnClickCredits()
    {
        _current.SetActive(false);
        credits.SetActive(true);
        _current = credits;

        EventSystem.current.SetSelectedGameObject(creditsFirst);
        _currentSelected = creditsReturn;

        //AudioManager.Instance.PlaySFXSound("snd_interface");
    }
    public void OnClickControls()
    {
        _current.SetActive(false);
        controls.SetActive(true);
        _current = controls;

        EventSystem.current.SetSelectedGameObject(controlsFirst);
        _currentSelected = controlsReturn;

        //AudioManager.Instance.PlaySFXSound("snd_interface");
    }
    public void OnClickReturn()
    {
        _current.SetActive(false);
        accueil.SetActive(true);
        _current = accueil;

        EventSystem.current.SetSelectedGameObject(_currentSelected);
        _currentSelected = null;

        //AudioManager.Instance.PlaySFXSound("snd_interface");
    }

    public void OnClickLeave()
    {
        //AudioManager.Instance.PlaySFXSound("snd_interface");
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
