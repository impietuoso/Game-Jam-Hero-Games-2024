using UnityEngine;
using UnityEngine.Events;

public class TriggerOnCollision2D : MonoBehaviour {
    public string[] targetTag;
    public UnityEvent<Collision2D> onCollisionEnter2D, onCollisionStay2D, onCollisionExit2D;

    private void OnCollisionEnter2D(Collision2D col) {
        for (int i = 0; i < targetTag.Length; i++) {
            if (string.IsNullOrEmpty(targetTag[i]) || col.gameObject.CompareTag(targetTag[i])) onCollisionEnter2D.Invoke(col);
        }
    }

    private void OnCollisionStay2D(Collision2D col) {
        for (int i = 0; i < targetTag.Length; i++) {
            if (string.IsNullOrEmpty(targetTag[i]) || col.gameObject.CompareTag(targetTag[i])) onCollisionStay2D.Invoke(col);
        }
    }

    private void OnCollisionExit2D(Collision2D col) {
        for (int i = 0; i < targetTag.Length; i++) {
            if (string.IsNullOrEmpty(targetTag[i]) || col.gameObject.CompareTag(targetTag[i])) onCollisionExit2D.Invoke(col);
        }
    }
}