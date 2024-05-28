using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangada : MonoBehaviour {
    public Menu menu;
    public void Partir() {
        if (Player.instance.save.rescueComplete) {
            Invoke("StopPlayer", 0.1f);
        }
    }

    public void Island(int scene) {
        menu.LoadScene(scene);
    }

    void StopPlayer() {
        Player.instance.StopPlayer();
    }
}