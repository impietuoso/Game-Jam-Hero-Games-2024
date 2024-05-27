using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatAlpha : MonoBehaviour
{
    public SpriteRenderer copyTarget;
    SpriteRenderer r;

    private void Awake() {
        r = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if(r.color != copyTarget.color) r.color = copyTarget.color;
    }
}
