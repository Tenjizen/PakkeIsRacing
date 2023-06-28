using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Menu
{
    public class StartParametersMenu : MenuController
    {
        [Header("Sub Menu"), SerializeField] private List<MenuUIObject> _objectsList = new List<MenuUIObject>();

        private List<Image> _imagesDictionary = new List<Image>();
        private List<TMP_Text> _textsDictionary = new List<TMP_Text>();

        [SerializeField, ReadOnly] public bool CanBeOpened = true;

        [SerializeField] VideoPlayer _video;
        [SerializeField] RawImage _videoRender;
        [SerializeField] Image _imageSkip;
        [SerializeField] TMP_Text _textSkip;
        [SerializeField] bool _videoIsPlaying;

        private float _timeToSkip;

        private int _index;

        private void Awake()
        {
            _videoIsPlaying = true;
            _imageSkip.fillAmount = 0f;
        }

        protected override void Start()
        {
            _index = 0;
            foreach (Image image in MenuGameObject.transform.GetComponentsInChildren<Image>())
            {
                _imagesDictionary.Add(image);
            }
            foreach (TMP_Text text in MenuGameObject.transform.GetComponentsInChildren<TMP_Text>())
            {
                _textsDictionary.Add(text);
            }

            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuDown.started += Down;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuUp.started += Up;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuLeft.started += Left;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.MenuRight.started += Right;
            CharacterManager.Instance.InputManagementProperty.GameplayInputs.Boat.ShowLeaveMenu.started += CloseMenu;

            _video.loopPointReached += CheckVideoOver;

            IsActive = true;
            IsUsable = true;

            SetTile();
        }

        private void Update()
        {
            CheckSkip();
        }

        private void CheckSkip()
        {
            if (_videoIsPlaying == false)
            {
                return;
            }

            const float timeToSkip = 2f;
            if (CharacterManager.Instance.InputManagementProperty.Inputs.AnyButton)
            {
                _timeToSkip += Time.deltaTime;

                if (_timeToSkip < 0.25f)
                {
                    return;
                }

                _imageSkip.DOKill();
                _imageSkip.DOFade(1, 0.2f);
                _imageSkip.fillAmount = _timeToSkip / timeToSkip;

                _textSkip.DOKill();
                _textSkip.DOFade(1, 0.2f);
            }
            else
            {
                _timeToSkip = 0;

                _imageSkip.DOKill();
                _imageSkip.DOFade(0, 0.2f);
                _textSkip.DOKill();
                _textSkip.DOFade(0, 0.2f);
            }

            if (_timeToSkip < timeToSkip)
            {
                return;
            }

            CheckVideoOver(_video);
        }

        private void CheckVideoOver(VideoPlayer vp)
        {
            _videoRender.DOFade(0, 1).SetUpdate(true);
            _videoIsPlaying = false;
            StartCoroutine(SetGameLaunched(1));
        }
        
        private void CloseMenu(InputAction.CallbackContext context)
        {
            IsActive = false;


            foreach (var item in _objectsList)
            {
                item.IsSelected = false;
            }

            const float fadeTime = 1f;
            foreach (Image image in _imagesDictionary)
            {
                image.DOKill();
                image.DOFade(0, fadeTime).SetUpdate(true);
            }
            foreach (TMP_Text txt in _textsDictionary)
            {
                txt.DOKill();
                txt.DOFade(0, fadeTime).SetUpdate(true);
            }

            IsUsable = IsActive;

            //StartCoroutine(SetGameLaunched(fadeTime));
            _video.Play();
            StartCoroutine(WaitShowVideo(1));
        }

        protected override void Up(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex <= 0)
            {
                return;
            }

            base.Up(context);
            _index--;
            SetTile();
        }

        protected override void Down(InputAction.CallbackContext context)
        {
            if (IsUsable == false || VerticalIndex >= Height || _index + 1 >= _objectsList.Count)
            {
                return;
            }

            base.Down(context);
            _index++;
            SetTile();
        }

        protected override void Left(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }

            _objectsList[_index].Activate(new InputAction.CallbackContext());
        }

        protected override void Right(InputAction.CallbackContext context)
        {
            if (IsUsable == false)
            {
                return;
            }

            _objectsList[_index].Activate(new InputAction.CallbackContext());
        }

        private void SetTile()
        {
            if (_objectsList.Count <= 0)
            {
                return;
            }

            for (int i = 0; i < _objectsList.Count; i++)
            {
                _objectsList[i].Set(i == _index);
                _objectsList[i].IsSelected = i == _index;
            }
        }

        private IEnumerator SetGameLaunched(float time)
        {
            yield return new WaitForSeconds(time);
            _video.gameObject.SetActive(false);
            MenuGameObject.SetActive(IsActive);
            CharacterManager.Instance.IsGameLaunched = true;
        }

        private IEnumerator WaitShowVideo(float time)
        {
            yield return new WaitForSeconds(time);
            _videoRender.DOColor(Color.white, 1f).SetUpdate(true);
        }
    }
}
