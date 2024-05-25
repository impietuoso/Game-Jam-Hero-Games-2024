using UnityEngine;
using UnityEngine.Events;

public class TriggerOnTrigger2D : MonoBehaviour {
    public string[] targetTag;
    public UnityEvent<Collider2D> onTriggerEnter2D, onTriggerStay2D, onTriggerExit2D;

    private void OnTriggerEnter2D(Collider2D col) {
        for (int i = 0; i < targetTag.Length; i++) {
            if (string.IsNullOrEmpty(targetTag[i]) || col.CompareTag(targetTag[i])) onTriggerEnter2D.Invoke(col);
        }
    }

    private void OnTriggerStay2D(Collider2D col) {
        for (int i = 0; i < targetTag.Length; i++) {
            if (string.IsNullOrEmpty(targetTag[i]) || col.CompareTag(targetTag[i])) onTriggerStay2D.Invoke(col);
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        for (int i = 0; i < targetTag.Length; i++) {
            if (string.IsNullOrEmpty(targetTag[i]) || col.CompareTag(targetTag[i])) onTriggerExit2D.Invoke(col);
        }
    }
}