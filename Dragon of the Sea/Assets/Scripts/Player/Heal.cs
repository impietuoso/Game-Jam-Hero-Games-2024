using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour {
    public int cura;

    public void Cura() {
        Player.instance.Cura(cura);
    }
}
