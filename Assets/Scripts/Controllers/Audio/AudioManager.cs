using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public SoundTrack[] tracks;
    public AudioMixerGroup mixer;

    private void Awake()
    {
        foreach(SoundTrack track in tracks)
        {
            track.source = gameObject.AddComponent<AudioSource>();
            track.source.clip = track.clip;
            track.source.volume = track.volume;
            track.source.outputAudioMixerGroup = mixer;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Play("BGM");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(string name)
    {
        SoundTrack t = Array.Find(tracks, track => track.name == name);
        t.source.loop = true;
    }
}
