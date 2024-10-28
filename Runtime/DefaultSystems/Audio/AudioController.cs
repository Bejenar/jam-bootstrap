using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Data
{
    public class AudioController : MonoBehaviour
    {
        [SerializeField]
        private Slider musicVolumeSlider;
        
        [SerializeField]
        private Slider sfxVolumeSlider;
        
        [SerializeField]
        private Slider ambientVolumeSlider;
        
        private const float VolumeToSlider = 20f;
        private const float SliderToVolume = 0.05f;

        private void Awake()
        {
            InitSlider(musicVolumeSlider, AudioPersistentSettings.MusicVolume);
            InitSlider(sfxVolumeSlider, AudioPersistentSettings.SFXVolume);
            InitSlider(ambientVolumeSlider, AudioPersistentSettings.AmbientVolume);
        }

        private void InitSlider(Slider slider, float value)
        {
            if (slider == null) return;

            slider.minValue = 0;
            slider.maxValue = 20;
            slider.value = value * VolumeToSlider;
        }

        private void OnEnable()
        {
            musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
            sfxVolumeSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);
            ambientVolumeSlider?.onValueChanged.AddListener(OnAmbientVolumeChanged);
        }
        
        private void OnDisable()
        {
            musicVolumeSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            sfxVolumeSlider?.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            ambientVolumeSlider?.onValueChanged.RemoveListener(OnAmbientVolumeChanged);
        }
        
        private void OnMusicVolumeChanged(float value)
        {
            Core.audio.SetVolume(AudioType.Music, value*SliderToVolume);
        }
        
        private void OnSFXVolumeChanged(float value)
        {
            Core.audio.SetVolume(AudioType.SFX, value*SliderToVolume);
        }
        
        private void OnAmbientVolumeChanged(float value)
        {
            Core.audio.SetVolume(AudioType.Ambient, value*SliderToVolume);
        }
    }
}