using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    public static CameraShake instance;
    void Awake()     {
        instance = this;
    }

    public void Shake() {
        
    }
}
