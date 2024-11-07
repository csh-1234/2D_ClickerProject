using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
public class SoundManager : MonoBehaviour
{
    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount
    }

    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];
    private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    private GameObject _soundRoot = null;

    public void Init()
    {
        if (_soundRoot == null)
        {
            _soundRoot = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(_soundRoot);

            string[] soundNames = System.Enum.GetNames(typeof(Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = _soundRoot.transform;
            }

            _audioSources[(int)Sound.Bgm].loop = true;
        }
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    public void Play(string path, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    public void Play(AudioClip audioClip, Sound type = Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Sound.Bgm];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.pitch = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            AudioSource audioSource = _audioSources[(int)Sound.Effect];
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    private AudioClip GetOrAddAudioClip(string path, Sound type = Sound.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        Debug.Log($"Attempting to load audio clip: {path}");

        AudioClip audioClip = null;

        if (type == Sound.Bgm)
        {
            audioClip = Managers.Instance.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Instance.Resource.Load<AudioClip>(path);
                if (audioClip != null)
                {
                    _audioClips.Add(path, audioClip);
                }
            }
        }

        if (audioClip == null)
        {
            Debug.LogError($"AudioClip Missing! Path: {path}");
            // 사용 가능한 모든 클립 출력
            AudioClip[] allClips = Resources.LoadAll<AudioClip>("Sounds");
            Debug.Log($"Available clips in Sounds folder:");
            foreach (var clip in allClips)
            {
                Debug.Log($"- {clip.name}");
            }
        }

        return audioClip;
    }

    public void Stop(Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.Stop();
    }

    public void SetVolume(Sound type, float volume)
    {
        AudioSource audioSource = _audioSources[(int)type];
        audioSource.volume = volume;
    }

    public float GetVolume(Sound type)
    {
        AudioSource audioSource = _audioSources[(int)type];
        return audioSource.volume;
    }
}

