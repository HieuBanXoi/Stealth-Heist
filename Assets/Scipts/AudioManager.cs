using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource audioSource;
    public AudioSource vfxAudioSource;
    public AudioClip musicClip;
    public AudioClip winClip;
    public AudioClip loseClip;
    public AudioClip correctClip;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Xoá bản thừa
        }
    }
    void Start()
    {
        audioSource.clip = musicClip;
        audioSource.Play();
    }
    public void PlaySFX(AudioClip sfxClip)
    {
        vfxAudioSource.clip = sfxClip;
        vfxAudioSource.PlayOneShot(sfxClip);
    }
    public static AudioManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindAnyObjectByType<AudioManager>();
        }
        return instance;
    }
}
