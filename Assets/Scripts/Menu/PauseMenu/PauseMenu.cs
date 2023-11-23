using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //[SerializeField] MainGame _mainGame;

    public GameObject AccueilPause;
    public GameObject Parameter;
    public GameObject Controls;
    public GameObject Credits;

    [Header("Controller"), Space(5)]
    public GameObject parameterFirst;
    public GameObject controlsFirst, creditsFirst, resumeFirst;
    public GameObject parameterReturn, controlsReturn, creditsReturn;


    private GameObject _current;
    private GameObject _currentSelected;


    [SerializeField] InputActionReference InputOpenClosedMenu, InputReturn;

    void Awake()
    {
        AccueilPause.SetActive(false);
        _current = AccueilPause;
        Parameter.SetActive(false);
        Controls.SetActive(false);
        Credits.SetActive(false);


        InputOpenClosedMenu.action.started += OpenClosedMenu;
        InputReturn.action.started += Return;
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    if (_current == AccueilPause)
        //    {
        //        AccueilPause.SetActive(!AccueilPause.activeSelf);
        //        EventSystem.current.SetSelectedGameObject(resumeFirst);

        //        //_mainGame.Pause = AccueilPause.activeSelf;
        //    }
        //    else
        //    {
        //        _current.SetActive(false);
        //        AccueilPause.SetActive(true);
        //        _current = AccueilPause;
        //        EventSystem.current.SetSelectedGameObject(_currentSelected);
        //    }
        //}
    }

    public void OpenClosedMenu(InputAction.CallbackContext obj)
    {
        AccueilPause.SetActive(!AccueilPause.activeSelf);
        EventSystem.current.SetSelectedGameObject(resumeFirst);
        //GameCore.Instance.MenuOpen = !GameCore.Instance.MenuOpen;
    }
    public void Return(InputAction.CallbackContext obj)
    {
        if (_current == AccueilPause)
        {
            AccueilPause.SetActive(false);
            EventSystem.current.SetSelectedGameObject(resumeFirst);
            //if (GameCore.Instance.MenuOpen) GameCore.Instance.MenuOpen = false;
            //_mainGame.Pause = AccueilPause.activeSelf;
        }
        else
        {
            _current.SetActive(false);
            AccueilPause.SetActive(true);
            _current = AccueilPause;
            EventSystem.current.SetSelectedGameObject(_currentSelected);
        }

    }

    public void OnClickResume()
    {
        AccueilPause.SetActive(false);
        //_mainGame.Pause = false;
    }
    public void OnClickRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void OnClickParameter()
    {
        _current.SetActive(false);
        Parameter.SetActive(true);
        _current = Parameter;
        EventSystem.current.SetSelectedGameObject(parameterFirst);
        _currentSelected = parameterReturn;
    }
    public void OnClickCredits()
    {
        _current.SetActive(false);
        Credits.SetActive(true);
        _current = Credits;
        EventSystem.current.SetSelectedGameObject(creditsFirst);
        _currentSelected = creditsReturn;
    }
    public void OnClickControls()
    {
        _current.SetActive(false);
        Controls.SetActive(true);
        _current = Controls;
        EventSystem.current.SetSelectedGameObject(controlsFirst);
        _currentSelected = controlsReturn;
    }
    public void OnClickReturn()
    {
        _current.SetActive(false);

        AccueilPause.SetActive(true);
        _current = AccueilPause;

        EventSystem.current.SetSelectedGameObject(_currentSelected);
        _currentSelected = null;
    }
    public void OnClickLeave()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
