using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using _Project.Data;
using Common;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
    SFX,
    Ambient,
    Music
}

public static class AudioPersistentSettings
{
    private const string MUSIC_KEY = "music";
    private const string AMBIENT_KEY = "ambiet";
    private const string SFX_KEY = "sfx";
    
    public static float MusicVolume
    {
        get => PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        set => PlayerPrefs.SetFloat(MUSIC_KEY, Mathf.Clamp(value, 0f, 1f));
    }
    
    public static float AmbientVolume
    {
        get => PlayerPrefs.GetFloat(AMBIENT_KEY, 1f);
        set => PlayerPrefs.SetFloat(AMBIENT_KEY, Mathf.Clamp(value, 0f, 1f));
    }
        
    public static float SFXVolume
    {
        get => PlayerPrefs.GetFloat(SFX_KEY, 1f);
        set => PlayerPrefs.SetFloat(SFX_KEY, Mathf.Clamp(value, 0f, 1f));
    }
}

public class AudioSystem : MonoBehaviour
{
    [SerializeField]
    private AudioSource ambientSource;
    [SerializeField]
    private AudioSource musicSource;

    private Dictionary<string, float> lastPlayTime = new Dictionary<string, float>();
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private int initialPoolSize = 10;

    // Audio settings
    public bool MuteSFX { get; set; }
    public bool MuteAmbient { get; set; }
    public bool MuteMusic { get; set; }

    public float SFXVolume { get; set; } = 1.0f;
    public float AmbientVolume { get; set; } = 1.0f;
    public float MusicVolume { get; set; } = 1.0f;

    void Awake()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        ambientSource = gameObject.AddComponent<AudioSource>();
        
        AudioMixer mixer = CMS.Get<Mixer>().Get<TagMixer>().mixer;
        musicSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        ambientSource.outputAudioMixerGroup = mixer.FindMatchingGroups("Music")[0];
        
        SetVolume(AudioType.SFX, AudioPersistentSettings.SFXVolume);
        SetVolume(AudioType.Ambient, AudioPersistentSettings.AmbientVolume);
        SetVolume(AudioType.Music, AudioPersistentSettings.MusicVolume);

        // Initialize the audio source pool with a predefined size
        for (int i = 0; i < initialPoolSize; i++)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            audioSource.playOnAwake = false;
            audioSourcePool.Add(audioSource);
        }
    }

    public void Play<T>() where T : CMSEntity
    {
        Play(CMS.Get<T>(E.Id<T>()));
    }

    public void Play(CMSEntity definition)
    {
        if (definition.Is<SFXTag>(out var sfx))
            PlaySFX(definition);
        else if (definition.Is<AmbientTag>(out var ambient))
            PlayAmbient(ambient);
        else if (definition.Is<MusicTag>(out var music))
            PlayMusic(music);
    }

    public void PlaySFX(CMSEntity sfx)
    {
        if (!MuteSFX && CanPlaySFX(sfx.id))
        {
            if (sfx.Is<SFXArray>(out var sfxarr))
            {
                var clip = sfxarr.files.GetRandom(ignoreEmpty: true);
                var audioSource = GetAvailableAudioSource();

                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.volume = SFXVolume * sfxarr.volume;
                    audioSource.Play();
                    lastPlayTime[sfx.id] = Time.time;
                    ReturnAudioSourceToPool(audioSource, clip.length).Forget();
                }
            }
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var audioSource in audioSourcePool)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }

        // If no available AudioSource, create a new one and add it to the pool
        var newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        audioSourcePool.Add(newAudioSource);
        return newAudioSource;
    }

    private async UniTaskVoid ReturnAudioSourceToPool(AudioSource audioSource, float delay)
    {
        await UniTask.WaitForSeconds(delay);
        audioSource.Stop();
        audioSource.clip = null; // Clear the clip after playing
    }

    private bool CanPlaySFX(string sfxId)
    {
        if (!lastPlayTime.ContainsKey(sfxId))
            return true;

        float lastTimePlayed = lastPlayTime[sfxId];
        float cooldown = CMS.Get<CMSEntity>(sfxId).Get<SFXTag>().Cooldown;

        return (Time.time - lastTimePlayed >= cooldown);
    }

    public void PlayAmbient(AmbientTag ambient)
    {
        if (!MuteAmbient)
        {
            ambientSource.clip = ambient.clip;
            ambientSource.loop = true;
            ambientSource.volume = AmbientVolume;
            ambientSource.Play();
        }
    }

    public void PlayMusic(MusicTag music)
    {
        if (!MuteMusic)
        {
            musicSource.clip = music.clip;
            musicSource.loop = true;
            musicSource.volume = MusicVolume;
            musicSource.Play();
        }
    }

    public void SetVolume(AudioType type, float volume)
    {
        switch (type)
        {
            case AudioType.SFX:
                SFXVolume = volume;
                AudioPersistentSettings.SFXVolume = volume;
                break;
            case AudioType.Ambient:
                AmbientVolume = volume;
                ambientSource.volume = AmbientVolume;
                AudioPersistentSettings.AmbientVolume = volume;
                break;
            case AudioType.Music:
                MusicVolume = volume;
                musicSource.volume = volume;
                AudioPersistentSettings.MusicVolume = volume;
                break;
        }
    }

    public void Mute(AudioType type, bool mute)
    {
        switch (type)
        {
            case AudioType.SFX:
                MuteSFX = mute;
                break;
            case AudioType.Ambient:
                MuteAmbient = mute;
                ambientSource.enabled = !mute;
                break;
            case AudioType.Music:
                MuteMusic = mute;
                musicSource.enabled = !mute;
                break;
        }
    }

    public void OnAdded()
    {
    }

    public void OnGameStarted()
    {
    }

    public void Stop(AudioType tp)
    {
        if (tp == AudioType.Ambient)
            ambientSource.Stop();
        if (tp == AudioType.Music)
            musicSource.Stop();
    }
}
