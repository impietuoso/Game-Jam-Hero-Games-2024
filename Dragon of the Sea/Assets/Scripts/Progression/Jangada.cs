using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jangada : MonoBehaviour
{
    public void Partir() {
        if (Player.instance.save.rescueComplete) {
            Player.instance.StopPlayer();

        }
    }
}
