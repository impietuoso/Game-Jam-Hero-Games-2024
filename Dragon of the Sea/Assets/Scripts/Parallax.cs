using UnityEngine;

public class Parallax : MonoBehaviour {
    private float length;
    private Vector2 StartPos;
    private Transform cam;
    public float ParallaxEffect;

    void Start() {
        StartPos.x = transform.position.x;
        StartPos.y = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
    }

    void LateUpdate() {
        float RePos = cam.position.x * (1 - ParallaxEffect);
        float Distance = cam.position.x * ParallaxEffect;

        transform.position = new Vector3(StartPos.x + Distance, StartPos.y, transform.position.z);

        if (RePos > StartPos.x + length) {
            StartPos.x += length;
        } else if (RePos < StartPos.x - length) {
            StartPos.x -= length;
        }
    }
}