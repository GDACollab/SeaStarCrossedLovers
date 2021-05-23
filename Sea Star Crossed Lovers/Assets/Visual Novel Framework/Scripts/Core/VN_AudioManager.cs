using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VN_AudioManager : MonoBehaviour
{
    public AudioSource buttonClick;

    private VN_Manager manager;

    public List<AudioClip> audioList = new List<AudioClip>();
    public AudioMixerGroup masterMixer;

    private Dictionary<string, AudioSource> audioDict = new Dictionary<string, AudioSource>();

    public void Construct(VN_Manager manager)
    {
        this.manager = manager;

        foreach(AudioClip sound in audioList)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            newSource.outputAudioMixerGroup = masterMixer;
            newSource.clip = sound;

            audioDict.Add(sound.name.ToString(), newSource);
            print(sound.name.ToString());
        }
    }

    public bool PlayAudio(string audioName)
    {
        if (audioDict.ContainsKey(audioName))
        {
            audioDict[audioName].Play();
            return true;
        }
        else
        {
            return false;
        }
    }
}
