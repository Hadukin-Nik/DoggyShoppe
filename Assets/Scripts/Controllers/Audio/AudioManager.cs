using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public SoundTrack[] tracks;


    private void Awake()
    {
        foreach(SoundTrack track in tracks)
        {
            track.source = gameObject.AddComponent<AudioSource>();
            track.source.clip = track.clip;
            track.source.volume = track.volume;
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
