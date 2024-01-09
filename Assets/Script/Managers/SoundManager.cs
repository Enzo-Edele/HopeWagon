using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    public Sound[] soundsEffects;
    public Sound[] musics;

    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;

        foreach (Sound s in soundsEffects)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;

            s.source.volume = s.volume;

            s.source.loop = s.loop;
        }
        foreach (Sound s in musics)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.outputAudioMixerGroup = s.mixerGroup;

            s.source.volume = s.volume;

            s.source.loop = s.loop;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(soundsEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("sound name not find : " + name);
            return;
        }
        s.source.Play();
    }
    public void PauseSound(string name)
    {
        Sound s = Array.Find(soundsEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.Pause();
    }
    public void UnpauseSound(string name)
    {
        Sound s = Array.Find(soundsEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.UnPause();
    }
    public void StopSound(string name)
    {
        Sound s = Array.Find(soundsEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.Stop();
    }
    public float PlayTime(string name)
    {
        Sound s = Array.Find(soundsEffects, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("sound name not find : " + name);
            return 0;
        }
        s.source.Play();
        return s.clip.length;
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musics, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.Play();
    }
    public void PauseMusic(string name)
    {
        Sound s = Array.Find(musics, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.Pause();
    }
    public void UnpauseMusic(string name)
    {
        Sound s = Array.Find(musics, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.UnPause();
    }
    public void StopMusic(string name)
    {
        Sound s = Array.Find(musics, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("musics name not find : " + name);
            return;
        }
        s.source.Stop();
    }

    public void StopAllSoud()
    {
        foreach (Sound s in soundsEffects)
        {
            s.source.Stop();
        }
        foreach (Sound s in musics)
        {
            s.source.Stop();
        }
    }

    public void SliderMasterVolume(float volume)
    {
        if (volume <= -25)
            volume = -80;
        audioMixer.SetFloat("Master", volume);
    }
    public void SliderMusicVolume(float volume)
    {
        if (volume <= -10)
            volume = -80;
        audioMixer.SetFloat("Music", volume);
    }
    public void SliderEffectVolume(float volume)
    {
        if (volume <= -10)
            volume = -80;
        audioMixer.SetFloat("SoundEffect", volume);
    }
}
