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
        if(target) entity.position = Vector3.SmoothDamp(entity.position, target.position, ref currentVelocity, followTime);
    }
}
