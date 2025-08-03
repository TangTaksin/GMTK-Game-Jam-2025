using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip musicClip;
    public AudioClip ambientClip;

    [Header("Sound Effects")]
    public AudioClip pickUp_SFX;
    public AudioClip correct_answer_SFX;
    public AudioClip wrong_answer_SFX;
    public AudioClip[] walkingClips;

    [Header("Step Timing Settings")]
    [SerializeField] public float minStepDelay = 0.3f;
    [SerializeField] public float maxStepDelay = 0.6f;

    [Header("Pitch Randomization")]
    [SerializeField] public float minPitch = 0.9f;
    [SerializeField] public float maxPitch = 1.1f;

    [Header("Fade Durations")]
    [SerializeField] private float fadeOutDuration = 1.0f;
    [SerializeField] private float fadeInDuration = 1.0f;

    private float lastStepTime;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (musicClip != null) PlayMusic(musicClip);
        if (ambientClip != null) PlayAmbient(ambientClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;

        musicSource.clip = clip;
        musicSource.volume = 0f;
        musicSource.Play();
        StartCoroutine(FadeInAudio(musicSource));
    }

    public void PlayAmbient(AudioClip clip)
    {
        if (clip == null) return;

        ambientSource.clip = clip;
        ambientSource.volume = 0f;
        ambientSource.Play();
        StartCoroutine(FadeInAudio(ambientSource));
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clip);
    }

    public void PlayWalkingSFX()
    {
        if (walkingClips == null || walkingClips.Length == 0) return;

        if (Time.time - lastStepTime >= Random.Range(minStepDelay, maxStepDelay))
        {
            AudioClip step = walkingClips[Random.Range(0, walkingClips.Length)];
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
            sfxSource.PlayOneShot(step);
            lastStepTime = Time.time;
        }
    }

    public void StopMusicFadeOut()
    {
        StartCoroutine(FadeOutAudio(musicSource));
    }

    public void StopAmbientFadeOut()
    {
        StartCoroutine(FadeOutAudio(ambientSource));
    }

    private IEnumerator FadeOutAudio(AudioSource source)
    {
        float startVolume = source.volume;

        while (source.volume > 0f)
        {
            source.volume -= startVolume * Time.deltaTime / fadeOutDuration;
            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

    private IEnumerator FadeInAudio(AudioSource source)
    {
        float targetVolume = 1f;

        while (source.volume < targetVolume)
        {
            source.volume += targetVolume * Time.deltaTime / fadeInDuration;
            yield return null;
        }

        source.volume = targetVolume;
    }
}