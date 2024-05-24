using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScale : MonoBehaviour
{
    TrailRenderer trail;

    private void Awake() {
        trail = GetComponent<TrailRenderer>();
    }

    private void Update() {
        trail.widthMultiplier = transform.localScale.x;
    }
}
