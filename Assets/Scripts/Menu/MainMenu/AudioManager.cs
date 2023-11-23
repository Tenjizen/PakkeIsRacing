using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class AudioManager : MonoBehaviour
{
    public AudioMixer Mixer;
    public AudioSource AudioSourceMusic;
    public AudioSource AudioSourceSFX;
    public AudioSource AudioSourceSFXRepeat;

    public AudioClip[] AudioClips;

    [HideInInspector] public bool Coroutine = false;

    public IEnumerator Musique;
    public IEnumerator SFX;

    //public float step = 1f;

    public float MusicSliderValue;
    public float SfxSliderValue;

    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        SetMusicLevel(MusicSliderValue);
        SetSFXLevel(SfxSliderValue);
    }

    public void SetMusicLevel(float sliderValue)
    {
        MusicSliderValue = sliderValue;
        Mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetSFXLevel(float sliderValue)
    {
        SfxSliderValue = sliderValue;
        Mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void PlayMusicSound(string name)
    {
        AudioClip clip = GetClip(name);
        AudioSourceMusic.PlayOneShot(clip);
    }
    //public void PlaySFXSoundRepeat(AudioClip clip)
    //{
    //    AudioSourceMusic.PlayOneShot(clip);
    //}
    public void PlaySFXSound(string name)
    {
        AudioClip clip = GetClip(name);
        AudioSourceSFX.PlayOneShot(clip);
    }

    public void PlaySFXSound(AudioClip clip)
    {
        //AudioClip clip = GetClip(name);
        AudioSourceSFXRepeat.PlayOneShot(clip);
    }

    public IEnumerator IEPlayMusicSound(string name)
    {
        AudioClip clip = GetClip(name);
        AudioSourceMusic.PlayOneShot(clip);
        yield return new WaitForSeconds(clip.length);
        Musique = IEPlayMusicSound(name);
        StartCoroutine(Musique);
    }
    
    
    public IEnumerator IEPlaySound(AudioClip clip)
    {
        AudioSourceSFXRepeat.PlayOneShot(clip);
        Debug.Log("test");
        yield return new WaitForSeconds(clip.length);
        SFX = IEPlaySound(clip);
        StartCoroutine(SFX);
    }

    public void StopMusicCoroutine()
    {
        if (Musique == null) return;
        AudioSourceMusic.Stop();
        StopCoroutine(Musique);
        Musique = null;
    }



    public void StopSoundCoroutine()
    {
        if (SFX == null) return;
        AudioSourceSFXRepeat.Stop();
        StopCoroutine(SFX);
        SFX = null;
    }


    //public IEnumerator Step(string name)
    //{
    //    AudioClip clip = GetClip(name);
    //    AudioSourceSFX.PlayOneShot(clip);
    //    yield return new WaitForSeconds(clip.length);
    //    StartCoroutine(Step(name));
    //}

    AudioClip GetClip(string name)
    {
        foreach (var item in AudioClips)
        {
            if (item.name == name)
                return item;
        }
        return null;
    }
}
