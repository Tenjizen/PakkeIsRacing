using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Tools.SingletonClassBase;
using UnityEngine;

namespace Sound
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Volume"), Range(0, 1)] public float MasterVolume = 1;
        [Range(0, 10)] public float MusicVolume = 5;
        [Range(0, 1)] public float AmbienceVolume = 1;
        [Range(0, 1)] public float SfxVolume = 1;

        private Bus _masterBus;
        private Bus _musicBus;
        private Bus _ambienceBus;
        private Bus _sfxBus;
        private List<EventInstance> _eventInstances;
        private List<StudioEventEmitter> _eventEmitters;
        private EventInstance _ambienceEventInstance;
        private EventInstance _musicEventInstance;
        //public static AudioManager Instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();

            _eventInstances = new List<EventInstance>();
            _eventEmitters = new List<StudioEventEmitter>();
            _masterBus = RuntimeManager.GetBus("bus:/");
            _musicBus = RuntimeManager.GetBus("bus:/Music");
            _ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
            _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }
        private void Start()
        {
            InitializeAmbience(FMODEvents.Instance.Ambience);
            InitializeMusic(FMODEvents.Instance.Music);
        }
        private void Update()
        {
            _masterBus.setVolume(MasterVolume);
            _musicBus.setVolume(MusicVolume);
            _ambienceBus.setVolume(AmbienceVolume);
            _sfxBus.setVolume(SfxVolume);
        }
        private void InitializeAmbience(EventReference ambienceEventReference)
        {
            _ambienceEventInstance = CreateInstance(ambienceEventReference);
            _ambienceEventInstance.start();
        }
        private void InitializeMusic(EventReference musicEventReference)
        {
            _musicEventInstance = CreateInstance(musicEventReference);
            _musicEventInstance.start();
        }
        public void SetAmbienceParameter(string parameterName, float parameterValue)
        {
            _ambienceEventInstance.setParameterByName(parameterName, parameterValue);
        }
        public void PlayOneShot(EventReference sound, Vector3 worldPos)
        {
            RuntimeManager.PlayOneShot(sound, worldPos);
        }
        public EventInstance CreateInstance(EventReference eventReference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }
        public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
        {
            StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
            emitter.EventReference = eventReference;
            _eventEmitters.Add(emitter);
            return emitter;
        }
        private void CleanUp()
        {
            foreach (EventInstance eventInstance in _eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
            foreach (StudioEventEmitter emitter in _eventEmitters)
            {
                emitter.Stop();
            }
        }
        protected override void OnDestroy()
        {
            CleanUp();
        }
    }
}
