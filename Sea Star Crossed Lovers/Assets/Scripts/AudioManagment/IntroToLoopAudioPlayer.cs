using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class IntroToLoopAudioPlayer : MonoBehaviour
{
    [Tooltip("AudioSource used for intro;")]
    [SerializeField] private AudioSource introTrack;

    [Tooltip("AudioSource used for loop after intro; Set to Loop")]
    [SerializeField] private AudioSource loopTrack;

    private void Start()
    {
        StartCoroutine(StartIntroToLoop());
    }

    private IEnumerator StartIntroToLoop()
    {
        introTrack.Play();
        float introLength = introTrack.clip.length;
        yield return new WaitForSeconds(introLength);

        loopTrack.Play();
    }
}
