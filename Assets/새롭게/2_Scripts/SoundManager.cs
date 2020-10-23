using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTypeBGM
{
    STAGE1,
    STAGE2,
}

public enum eTypeEffect
{
    BUTTON,
    ZOMBIE_DIE,
    GUN_RELOAD,
    GUN_SHOT,
    BARREL_EXP,
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [SerializeField] int audioCnt;
    [SerializeField] AudioClip[] bgmClips;
    [SerializeField] AudioClip[] effectClips;
    

    AudioSource bgmAudioSource;
    AudioSource[] effectAudioSource;

    public bool bgmOn;
    public bool effectOn;

    public static SoundManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            bgmAudioSource = GetComponent<AudioSource>();
            bgmOn = true;
            effectOn = true;
            effectAudioSource = new AudioSource[audioCnt];
            for (int i = 0; i < audioCnt; i++)
            {
                 effectAudioSource[i] = gameObject.AddComponent<AudioSource>();
                effectAudioSource[i].playOnAwake = false;
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(eTypeBGM _eTypeBGM, float _volume = 1.0f, bool _loop = true)
    {
        bgmAudioSource.clip = bgmClips[(int)_eTypeBGM];
        bgmAudioSource.volume = _volume;
        bgmAudioSource.loop = _loop;
        
        bgmAudioSource.UnPause();
    }

    public void PauseBGM()
    {
        bgmAudioSource.Pause();
    }

    public void PlayEffect(eTypeEffect _eTypeEffect, float _volume = 1.0f, bool _loop = false)
    {
        if (effectOn)
        {
            for (int i = 0; i < audioCnt; i++)
            {
                if(!effectAudioSource[i].isPlaying)
                {
                    effectAudioSource[i].clip = effectClips[(int)_eTypeEffect];
                    effectAudioSource[i].volume = _volume;
                    effectAudioSource[i].loop = _loop;
                    effectAudioSource[i].Play();
                    break;
                }
            }
        }
    }
}
