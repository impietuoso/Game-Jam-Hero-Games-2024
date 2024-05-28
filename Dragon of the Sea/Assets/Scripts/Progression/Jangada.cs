using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangada : MonoBehaviour {
    public Menu menu;
    public void Partir(int scene) {
        if (Player.instance.save.rescueComplete) {
            StartCoroutine(newScene(scene));
        } else {
            VsnController.instance.StartVSN("FaltouAmigos");
        }
    }

    public void Island(int scene) {
        StartCoroutine(newScene(scene));
    }

    IEnumerator newScene(int scene) {
        yield return new WaitForSeconds(0.1f);
        Player.instance.StopPlayer();
        yield return new WaitForSeconds(0.1f);
        menu.LoadScene(scene);
    }


    void StopPlayer() {
        Player.instance.StopPlayer();
    }
}