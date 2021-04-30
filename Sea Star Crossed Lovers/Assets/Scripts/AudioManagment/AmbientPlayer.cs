using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AmbientPlayer : MonoBehaviour
{
    [Tooltip("Audio mixer to use for sounds")]
    public AudioMixerGroup outputAudioMixer;

    [Tooltip("Pool of sound clips used for ambient sounds")]
    [SerializeField] private List<AudioClip> ambientSounds;

    // Dictionary for getting AudioSource of AudioClips
    // Bool is flag for checking if the PlaySource coroutine is running
    private Dictionary<AudioClip, (AudioSource, bool)> AudioDict =
        new Dictionary<AudioClip, (AudioSource, bool)>();

    [Tooltip("Max volume sounds reach after fading in")]
    public float baseVolume = 1;

    [Tooltip("Minimum time until a new sound is tried to be played")]
    public float minPlayCooldown = 0;
    [Tooltip("Maximum time until a new sound is tried to be played")]
    public float maxPlayCooldown = 1;

    [Tooltip("Minimum duration of played clips including fade in and out time")]
    public float minClipDuration = 5;
    [Tooltip("Maximum duration of played clips including fade in and out time")]
    public float maxClipDuration = 10;

    [Tooltip("Duration of clip fade in and out during beginning and end of chosen clip duration")]
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        ambientSounds.ForEach(clip =>
        {
            var thisSource = gameObject.AddComponent<AudioSource>();
            thisSource.clip = clip;
            thisSource.volume = baseVolume;
            thisSource.outputAudioMixerGroup = outputAudioMixer;
            AudioDict.Add(clip, (thisSource, false));
        });
    }

    private void Start()
    {
        StartCoroutine(LoopPlayAmbience());
    }

    private IEnumerator LoopPlayAmbience()
    {
        while(true)
        {
            // Pick random audio clip
            AudioClip thisClip = ambientSounds[Random.Range(0, ambientSounds.Count)];
            AudioSource thisSource = AudioDict[thisClip].Item1;
            bool thisPlaying = AudioDict[thisClip].Item2;

            // Check if clip's source is playing or if it's coroutine is running
            if (!thisSource.isPlaying && !thisPlaying)
            {
                // Random start and duration of clip playing
                float thisStart = Random.Range(0, thisClip.length - thisClip.length / 10);
                float thisDuration = Random.Range(minClipDuration, maxClipDuration);
                thisDuration = Mathf.Clamp(thisDuration, 0, thisClip.length - thisStart);

                AudioDict[thisClip] = (thisSource, true);
                StartCoroutine(PlaySource(thisSource, thisStart, thisDuration));
            }

            float thisWait = Random.Range(minPlayCooldown, maxPlayCooldown);
            yield return new WaitForSeconds(thisWait);
        }
    }

    private IEnumerator PlaySource(AudioSource thisSource, float start, float duration)
    {
        print("play start: " + thisSource.clip.name);
        thisSource.time = start;
        // Fade audio in
        thisSource.volume = 0;
        thisSource.Play();
        yield return StartCoroutine(FadeAudio(thisSource, fadeDuration, baseVolume));
        // Maintain baseVolume for middle of duration
        yield return new WaitForSeconds(duration - fadeDuration*2);
        // Fade audio out
        yield return StartCoroutine(FadeAudio(thisSource, fadeDuration, 0));
        thisSource.Stop();
        AudioDict[thisSource.clip] = (thisSource, false);
        print("play end: " + thisSource.clip.name);
        yield break;
    }

    private IEnumerator FadeAudio(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            float t = currentTime / duration;
            t = t * t * (3f - 2f * t);
            audioSource.volume = Mathf.Lerp(start, targetVolume, t);
            currentTime += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = targetVolume;
        yield break;
    }
}
