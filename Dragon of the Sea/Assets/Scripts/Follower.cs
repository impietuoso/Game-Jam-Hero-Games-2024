using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Follower : MonoBehaviour {
    public Transform entity;
    public Transform target;
    private Vector3 currentVelocity;
    public float followTime;

    private void LateUpdate() {
        Follow();
    }

    public void Follow() {
        entity.position = Vector3.SmoothDamp(entity.position, target.position, ref currentVelocity, followTime);
    }
}
