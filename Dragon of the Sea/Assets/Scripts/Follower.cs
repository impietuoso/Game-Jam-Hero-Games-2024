using Unity.VisualScripting;
using UnityEngine;
public class Follower : MonoBehaviour {
    public Transform entity;
    public Transform target;
    private Vector3 currentVelocity;
    public float followTime;
    public bool followOnlyX;

    private void LateUpdate() {
        if (followOnlyX) FollowX();
        else Follow();
    }

    public void Follow() {
        if(target) entity.position = Vector3.SmoothDamp(entity.position, target.position, ref currentVelocity, followTime);
    }

    public void FollowX() {
        Vector2 targetx = target.position;
        targetx.y = entity.position.y;
        if (target) entity.position = Vector3.SmoothDamp(entity.position, targetx, ref currentVelocity, followTime);
    }
}
