using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playlist : MonoBehaviour
{
    public List<Music> playlist = new();
    new public AudioSource audio;
    public bool destructableAudio;

    private void Awake() {
        audio = GetComponent<AudioSource>();
    }

    public void PlaySFX(string musicName) {
        foreach (Music music in playlist) {
            if (music.alias == musicName) {
                if (destructableAudio) Player.instance.playlist.audio.PlayOneShot(music.clip);
                else audio.PlayOneShot(music.clip);
            }
        }
    }
}

[Serializable]
public class Music {
    public string alias;
    public AudioClip clip;
}