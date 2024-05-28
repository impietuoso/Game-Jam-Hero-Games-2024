using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangada : MonoBehaviour
{
    public void Partir() {
        if (Player.instance.save.rescueComplete) {
            Invoke("StopPlayer", 0.1f);
        }
    }

    void StopPlayer() {
        Player.instance.StopPlayer();
    }
}
